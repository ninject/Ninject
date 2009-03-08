using System.IO;
using System.Reflection;
using Microsoft.Scripting;

namespace Ninject.Dynamic
{
    public class AssemblyStreamContentProvider : StreamContentProvider
    {
        private readonly string _fileName;
        private readonly Assembly _assembly;

        public AssemblyStreamContentProvider(string fileName, Assembly assembly)
        {
            _fileName = fileName;
            _assembly = assembly;
        }

        #region Overrides of StreamContentProvider

        public override Stream GetStream()
        {
            return _assembly.GetManifestResourceStream(_fileName);
        }

        #endregion
    }
}