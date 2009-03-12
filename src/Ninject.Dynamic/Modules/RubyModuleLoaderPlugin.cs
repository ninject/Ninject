#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Ninject.Components;
using Ninject.Dynamic.Extensions;
using Ninject.Dynamic.Modules;
using Ninject.Modules;

#endregion

namespace Ninject.Dynamic.Modules
{
    public class RubyModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
    {
        private static readonly string[] Patterns = new[] { "*.rb", "*.erb" };
        
        //FIXME: Do not forget to move me to the tests project
        private ICollection<string> _patterns = new string[1];
        private readonly IRubyEngine _engine;

        public RubyModuleLoaderPlugin(IKernel kernel, IRubyEngine engine)
        {
            _engine = engine;
            Kernel = kernel;
        }

        /// <summary>
        /// Gets or sets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        public IKernel Kernel { get; set; }


        #region Implementation of IModuleLoaderPlugin

        /// <summary>
        /// Gets the file patterns (*.rb, etc.) supported by the plugin.
        /// </summary>
        public ICollection<string> SupportedPatterns
        {
            get
            {
                //FIXME: do not forget to move to the tests project
                if (_patterns.IsNotNull())
                    return _patterns;
                return Patterns;
            }
            internal set { _patterns = value; }
        }

        /// <summary>
        /// Loads modules from the specified files.
        /// </summary>
        /// <param name="files">The names of the files to load modules from.</param>
        public void LoadModules(IEnumerable<string> files)
        {
            files.ForEach(file =>
                              {
                                  var rubyModule = new RubyModule(_engine, file);
                                  Kernel.LoadModule(rubyModule);
                              });
        }

        #endregion
    }
}