#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Ninject.Dynamic.Extensions;
using Ninject.Dynamic.Modules;
using Ninject.Modules;

#endregion

namespace Ninject.Dynamic.Modules
{
    public class RubyModuleLoader : IModuleLoader
    {
        private readonly IDecoratableModuleLoader _defaultModuleLoader;
        private readonly IRubyEngine _engine;

        public RubyModuleLoader(IKernel kernel, IRubyEngine engine, IDecoratableModuleLoader decoratableModuleLoader)
        {
            _engine = engine;
            Kernel = kernel;
            _defaultModuleLoader = decoratableModuleLoader;
        }

        public IKernel Kernel { get; set; }

        /// <summary>
        /// Normalizes the provided path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The normalized path.</returns>
        protected virtual string NormalizePath(string path)
        {
            path.EnsureArgumentNotNullOrBlank("path");

            if (path.StartsWith("~"))
                path = GetBaseDirectory() + path.Substring(1);

            return new DirectoryInfo(path).FullName;
        }

        private static string GetBaseDirectory()
        {
            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : AppDomain.CurrentDomain.BaseDirectory;
        }

        #region Implementation of IDisposable

        /// <summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _defaultModuleLoader.Dispose();
        }

        #endregion

        #region Implementation of INinjectComponent

        /// <summary>
        /// Gets or sets the settings that are being used.
        /// </summary>
        public INinjectSettings Settings
        {
            get { return _defaultModuleLoader.Settings; }
            set { _defaultModuleLoader.Settings = value; }
        }

        #endregion

        #region Implementation of IModuleLoader

        /// <summary>
        /// Loads all loadable modules defined in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void LoadModules(Assembly assembly)
        {
            _defaultModuleLoader.LoadModules(assembly);
        }

        /// <summary>
        /// Loads all loadable modules defined in the assembly with the specified assembly name or filename.
        /// </summary>
        /// <param name="assemblyOrFileName">Name of the assembly or file.</param>
        public void LoadModules(string assemblyOrFileName)
        {
            assemblyOrFileName.EnsureArgumentNotNullOrBlank("assemblyOrFileName");

            if (!File.Exists(assemblyOrFileName) || Path.GetExtension(assemblyOrFileName) != ".rb")
            {
                _defaultModuleLoader.LoadModules(assemblyOrFileName);
            }

            var rubyModule = new RubyModule(_engine, assemblyOrFileName);
            Kernel.LoadModule(rubyModule);
        }

        /// <summary>
        /// Scans specified path for assemblies that match the specified pattern(s),
        /// and loads any modules defined therein into the kernel.
        /// </summary>
        /// <param name="path">The path to scan.</param>
        /// <param name="patterns">The patterns to match.</param>
        /// <param name="recursive">If <c>true</c>, scan all subdirectories of the path as well.</param>
        public void ScanAndLoadModules(string path, IEnumerable<string> patterns, bool recursive)
        {
            path.EnsureArgumentNotNullOrBlank("path");
            patterns.EnsureArgumentNotNull("patterns");

            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var normalizedPath = NormalizePath(path);
            var files = patterns.SelectMany(pattern => Directory.GetFiles(normalizedPath, "*.rb", searchOption));

            if (files.IsEmpty())
            {
                _defaultModuleLoader.ScanAndLoadModules(path, patterns, recursive);
                return;
            }

            files.ForEach(LoadModules);
        }

        #endregion
    }
}