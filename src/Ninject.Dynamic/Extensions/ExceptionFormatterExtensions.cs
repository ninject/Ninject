using System.IO;
using Ninject.Dynamic.Modules;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;

namespace Ninject.Dynamic.Extensions
{
    internal static class ExceptionFormatter
    {
        public static string ModuleWithSameNameIsAlreadyLoaded(RubyModule newModule, IModule existingModule)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error loading module '{0}' of type {1}", newModule.Name, newModule.ScriptPath);
                sw.WriteLine("Another module (of type {0}) with the same name has already been loaded", newModule.ScriptPath);

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have not accidentally loaded the same module twice.");
#if !SILVERLIGHT
                sw.WriteLine("  2) If you are using automatic module loading, ensure you have not manually loaded a module");
                sw.WriteLine("     that may be found by the module loader.");
#endif

                return sw.ToString();
            }
        }
    }
}