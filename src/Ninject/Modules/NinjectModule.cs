//-------------------------------------------------------------------------------
// <copyright file="NinjectModule.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
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

namespace Ninject.Modules
{
    using System;
    using System.Collections.Generic;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// A loadable unit that defines bindings for your application.
    /// </summary>
    public abstract class NinjectModule : BindingRoot, INinjectModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectModule"/> class.
        /// </summary>
        protected NinjectModule()
        {
            this.Bindings = new List<IBinding>();
        }

        /// <summary>
        /// Gets the kernel that the module is loaded into.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Gets the module's name. Only a single module with a given name can be loaded at one time.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        /// Gets the bindings that were registered by the module.
        /// </summary>
        public ICollection<IBinding> Bindings { get; private set; }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected override IKernel KernelInstance
        {
            get
            {
                return this.Kernel;
            }
        }
        
        /// <summary>
        /// Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        public void OnLoad(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            this.Kernel = kernel;
            this.Load();
        }

        /// <summary>
        /// Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is unloading the module.</param>
        public void OnUnload(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            this.Unload();
            this.Bindings.Map(this.Kernel.RemoveBinding);
            this.Kernel = null;
        }

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public void OnVerifyRequiredModules()
        {
            this.VerifyRequiredModulesAreLoaded();
        }

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unloads the module from the kernel.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public virtual void VerifyRequiredModulesAreLoaded()
        {
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public override void Unbind(Type service)
        {
            this.Kernel.Unbind(service);
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.Kernel.AddBinding(binding);
            this.Bindings.Add(binding);
        }

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.Kernel.RemoveBinding(binding);
            this.Bindings.Remove(binding);
        }
    }
}