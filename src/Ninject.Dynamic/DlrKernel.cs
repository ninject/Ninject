#region Usings
using System;
using Ninject.Dynamic.Modules;
using Ninject.Modules;
#endregion

namespace Ninject.Dynamic
{
    public class DlrKernel : StandardKernel
    {
        public DlrKernel(params INinjectModule[] modules) : base(modules)
        {
        }


        public DlrKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
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