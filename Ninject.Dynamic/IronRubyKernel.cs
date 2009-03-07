#region Usings

using Ninject.Modules;

#endregion

namespace Ninject.Dynamic
{
    public class IronRubyKernel : StandardKernel
    {
        public IronRubyKernel(params IModule[] modules) : base(modules)
        {
        }


        public IronRubyKernel(INinjectSettings settings, params IModule[] modules) : base(settings, modules)
        {
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            Components.Add<IRubyEngine, RubyEngine>();
            var loader = Components.Get<IModuleLoader>();
            Components.RemoveAll<IModuleLoader>();
            Components.Add<IModuleLoader, RubyModuleLoader>();
            var rubyLoader = Components.Get<IModuleLoader>();
            ((RubyModuleLoader) rubyLoader).Initialize(loader);
        }
    }
}