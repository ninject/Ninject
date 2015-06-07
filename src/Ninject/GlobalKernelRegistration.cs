//-------------------------------------------------------------------------------
// <copyright file="GlobalKernelRegistration.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------

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
#if !WINRT && !PCL
        private static readonly ReaderWriterLock kernelRegistrationsLock = new ReaderWriterLock();
#elif !PCL
        private static readonly ReaderWriterLockSlim kernelRegistrationsLock = new ReaderWriterLockSlim();
#endif
        private static readonly IDictionary<Type, Registration> kernelRegistrations = new Dictionary<Type, Registration>(); 

        internal static void RegisterKernelForType(IReadonlyKernel kernel, Type type)
        {
 #if PCL
            throw new NotImplementedException();
#else
            var registration = GetRegistrationForType(type);
#if !WINRT
            registration.KernelLock.AcquireReaderLock(Timeout.Infinite);
#else
            registration.KernelLock.EnterReadLock();
#endif
            try
            {
                registration.Kernels.Add(new WeakReference(kernel));
            }
            finally
            {
#if !WINRT
                registration.KernelLock.ReleaseReaderLock();
#else
                registration.KernelLock.ExitReadLock();
#endif
            }
#endif
        }

        internal static void UnregisterKernelForType(IReadonlyKernel kernel, Type type)
        {
#if PCL
            throw new NotImplementedException();
#else
            var registration = GetRegistrationForType(type);
            RemoveKernels(registration, registration.Kernels.Where(reference => reference.Target == kernel || !reference.IsAlive));
#endif
        }

        /// <summary>
        /// Performs an action on all registered kernels.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void MapKernels(Action<IReadonlyKernel> action)
        {
#if PCL
            throw new NotImplementedException();
#else
            bool requiresCleanup = false;
            var registration = GetRegistrationForType(this.GetType());
#if !WINRT
            registration.KernelLock.AcquireReaderLock(Timeout.Infinite);
#else
            registration.KernelLock.EnterReadLock();
#endif


            try
            {
                foreach (var weakReference in registration.Kernels)
                {
                    var kernel = weakReference.Target as IReadonlyKernel;
                    if (kernel != null)
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
#if !WINRT
                registration.KernelLock.ReleaseReaderLock();
#else
                registration.KernelLock.ExitReadLock();
#endif
            }

            if (requiresCleanup)
            {
                RemoveKernels(registration, registration.Kernels.Where(reference => !reference.IsAlive));
            }
#endif
        }
        
        private static void RemoveKernels(Registration registration, IEnumerable<WeakReference> references)
        {
#if PCL
            throw new NotImplementedException();
#else
#if !WINRT
            registration.KernelLock.ReleaseReaderLock();
#else
            registration.KernelLock.ExitReadLock();
#endif
            try
            {
                foreach (var reference in references.ToArray())
                {
                    registration.Kernels.Remove(reference);
                }
            }
            finally
            {
#if !WINRT
                registration.KernelLock.ReleaseReaderLock();
#else
                registration.KernelLock.ExitReadLock();
#endif
            }
#endif
        }

        private static Registration GetRegistrationForType(Type type)
        {
#if PCL
            throw new NotImplementedException();
#else
#if !WINRT
            kernelRegistrationsLock.AcquireReaderLock(Timeout.Infinite);
#else
            kernelRegistrationsLock.EnterUpgradeableReadLock();
#endif
            try
            {
                Registration registration;
                if (kernelRegistrations.TryGetValue(type, out registration))
                {
                    return registration;
                }
                
                return CreateNewRegistration(type);
            }
            finally
            {
#if !WINRT
                kernelRegistrationsLock.ReleaseReaderLock();
#else
                kernelRegistrationsLock.ExitUpgradeableReadLock();
#endif
            }
#endif
        }

        private static Registration CreateNewRegistration(Type type)
        {
#if PCL
            throw new NotImplementedException();
#else
#if !WINRT
            var lockCookie = kernelRegistrationsLock.UpgradeToWriterLock(Timeout.Infinite);
#else
            kernelRegistrationsLock.EnterWriteLock();
#endif
            try
            {
                Registration registration;
                if (kernelRegistrations.TryGetValue(type, out registration))
                {
                    return registration;
                }

                registration = new Registration();
                kernelRegistrations.Add(type, registration);
                return registration;
            }
            finally
            {
#if !WINRT
                kernelRegistrationsLock.DowngradeFromWriterLock(ref lockCookie);
#else
                kernelRegistrationsLock.ExitWriteLock();
#endif
            }
#endif
        }

        private class Registration
        {
#if !PCL
            public Registration()
            {
#if !WINRT
                this.KernelLock = new ReaderWriterLock();
#else
                this.KernelLock = new ReaderWriterLockSlim();
#endif
                this.Kernels = new List<WeakReference>();
            }

#if !WINRT
            public ReaderWriterLock KernelLock { get; private set; }
#else
            public ReaderWriterLockSlim KernelLock { get; private set; }
#endif

            public IList<WeakReference> Kernels { get; private set; }
#endif
        }
    }
}