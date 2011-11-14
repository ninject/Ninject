//-------------------------------------------------------------------------------
// <copyright file="GlobalKernelRegistrationModule.cs" company="Ninject Project Contributors">
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

#if !SILVERLIGHT && !NETCF
namespace Ninject
{
    using Ninject.Modules;

    /// <summary>
    /// Registers the kernel into which the module is loaded on the GlobalKernelRegistry using the
    /// type specified by TGlobalKernelRegistry.
    /// </summary>
    /// <typeparam name="TGlobalKernelRegistry">The type that is used to register the kernel.</typeparam>
    public abstract class GlobalKernelRegistrationModule<TGlobalKernelRegistry> : NinjectModule
        where TGlobalKernelRegistry : GlobalKernelRegistration
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            GlobalKernelRegistration.RegisterKernelForType(this.Kernel, typeof(TGlobalKernelRegistry));
        }

        /// <summary>
        /// Unloads the module from the kernel.
        /// </summary>
        public override void Unload()
        {
            GlobalKernelRegistration.UnregisterKernelForType(this.Kernel, typeof(TGlobalKernelRegistry));
        }
    }
}
#endif