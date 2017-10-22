// -------------------------------------------------------------------------------------------------
// <copyright file="GlobalKernelRegistration.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Allows to register kernel globally to perform some tasks on all kernels.
    /// The registration is done by loading the GlobalKernelRegistrationModule to the kernel.
    /// </summary>
    public abstract class GlobalKernelRegistration
    {
        private static readonly ReaderWriterLockSlim KernelRegistrationsLock = new ReaderWriterLockSlim();

        private static readonly IDictionary<Type, Registration> KernelRegistrations = new Dictionary<Type, Registration>();

        /// <summary>
        /// Registers the kernel for the specified type.
        /// </summary>
        /// <param name="kernel">The <see cref="IKernel"/>.</param>
        /// <param name="type">The service type.</param>
        internal static void RegisterKernelForType(IKernel kernel, Type type)
        {
            var registration = GetRegistrationForType(type);

            registration.KernelLock.EnterWriteLock();

            try
            {
                registration.Kernels.Add(new WeakReference(kernel));
            }
            finally
            {
                registration.KernelLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Un-registers the kernel for the specified type.
        /// </summary>
        /// <param name="kernel">The <see cref="IKernel"/>.</param>
        /// <param name="type">The service type.</param>
        internal static void UnregisterKernelForType(IKernel kernel, Type type)
        {
            var registration = GetRegistrationForType(type);
            RemoveKernels(registration, registration.Kernels.Where(reference => reference.Target == kernel || !reference.IsAlive));
        }

        /// <summary>
        /// Performs an action on all registered kernels.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void MapKernels(Action<IKernel> action)
        {
            var requiresCleanup = false;
            var registration = GetRegistrationForType(this.GetType());

            registration.KernelLock.EnterReadLock();

            try
            {
                foreach (var weakReference in registration.Kernels)
                {
                    if (weakReference.Target is IKernel kernel)
                    {
                        action(kernel);
                    }
                    else
                    {
                        requiresCleanup = true;
                    }
                }
            }
            finally
            {
                registration.KernelLock.ExitReadLock();
            }

            if (requiresCleanup)
            {
                RemoveKernels(registration, registration.Kernels.Where(reference => !reference.IsAlive));
            }
        }

        private static void RemoveKernels(Registration registration, IEnumerable<WeakReference> references)
        {
            registration.KernelLock.EnterWriteLock();

            try
            {
                foreach (var reference in references.ToArray())
                {
                    registration.Kernels.Remove(reference);
                }
            }
            finally
            {
                registration.KernelLock.ExitWriteLock();
            }
        }

        private static Registration GetRegistrationForType(Type type)
        {
            KernelRegistrationsLock.EnterUpgradeableReadLock();
            try
            {
                if (KernelRegistrations.TryGetValue(type, out Registration registration))
                {
                    return registration;
                }

                return CreateNewRegistration(type);
            }
            finally
            {
                KernelRegistrationsLock.ExitUpgradeableReadLock();
            }
        }

        private static Registration CreateNewRegistration(Type type)
        {
            KernelRegistrationsLock.EnterWriteLock();

            try
            {
                if (KernelRegistrations.TryGetValue(type, out Registration registration))
                {
                    return registration;
                }

                registration = new Registration();
                KernelRegistrations.Add(type, registration);
                return registration;
            }
            finally
            {
                KernelRegistrationsLock.ExitWriteLock();
            }
        }

        private class Registration
        {
            public Registration()
            {
                this.KernelLock = new ReaderWriterLockSlim();

                this.Kernels = new List<WeakReference>();
            }

            public ReaderWriterLockSlim KernelLock { get; private set; }

            public IList<WeakReference> Kernels { get; private set; }
        }
    }
}