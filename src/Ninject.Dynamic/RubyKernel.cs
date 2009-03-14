#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Dynamic.Extensions;
using Ninject.Dynamic.Modules;
using Ninject.Events;
using Ninject.Infrastructure.Language;
using Ninject.Modules;

#endregion

namespace Ninject.Dynamic
{
    public class RubyKernel : StandardKernel
    {
        public RubyKernel(params IModule[] modules) : base(modules)
        {
        }


        public RubyKernel(INinjectSettings settings, params IModule[] modules) : base(settings, modules)
        {
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            Components.Add<IRubyEngine, RubyEngine>();
            Components.Add<IModuleLoaderPlugin, RubyModuleLoaderPlugin>();
        }

        /// <summary>
        /// Loads the assemblies the provide types are defined in into the ruby engines.
        /// </summary>
        /// <param name="types">The types.</param>
        public void LoadAssemblies(params Type[] types)
        {
            var engine = Components.Get<IRubyEngine>();
            engine.LoadAssemblies(types);
        }




    }
}