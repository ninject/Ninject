using System;
using IronRuby.Builtins;
using Ninject.Dynamic.Extensions;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Dynamic
{
    public class RubyModule : Module
    {
        private readonly IRubyEngine _engine;
        private readonly string _scriptPath;



        public RubyModule(IRubyEngine engine, string scriptPath)
        {
            _engine = engine;
            _scriptPath = scriptPath;
        }

        #region Overrides of Module

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            var bindings = ((RubyEngine)_engine).ExecuteFile<RubyArray>(_scriptPath);

            bindings.ForEach(item => AddBinding((IBinding)item));
        }

        #endregion
    }
}