//-------------------------------------------------------------------------------
// <copyright file="ExceptionFormatter.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Introspection
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Ninject.Activation;
    using Ninject.Modules;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Provides meaningful exception messages.
    /// </summary>
    public static class ExceptionFormatter
    {
        /// <summary>
        /// Generates a message saying that modules without names are not supported.
        /// </summary>
        /// <returns>The exception message.</returns>
        public static string ModulesWithNullOrEmptyNamesAreNotSupported()
        {
            return "Modules with null or empty names are not supported";
        }

        /// <summary>
        /// Generates a message saying that modules without names are not supported.
        /// </summary>
        /// <returns>The exception message.</returns>
        public static string TargetDoesNotHaveADefaultValue(ITarget target)
        {
            return string.Format("Target '{0}' at site '{1}' does not have a default value.", target.Member, target.Name);
        }

        /// <summary>
        /// Generates a message saying that a module with the same name is already loaded.
        /// </summary>
        /// <param name="newModule">The new module.</param>
        /// <param name="existingModule">The existing module.</param>
        /// <returns>The exception message.</returns>
        public static string ModuleWithSameNameIsAlreadyLoaded(INinjectModule newModule, INinjectModule existingModule)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error loading module '{0}' of type {1}", newModule.Name, newModule.GetType().Format());
                sw.WriteLine("Another module (of type {0}) with the same name has already been loaded", existingModule.GetType().Format());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have not accidentally loaded the same module twice.");
                #if !SILVERLIGHT
                sw.WriteLine("  2) If you are using automatic module loading, ensure you have not manually loaded a module");
                sw.WriteLine("     that may be found by the module loader.");
                #endif

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that no module has been loaded with the specified name.
        /// </summary>
        /// <param name="name">The module name.</param>
        /// <returns>The exception message.</returns>
        public static string NoModuleLoadedWithTheSpecifiedName(string name)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error unloading module '{0}': no such module has been loaded", name);

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure you have previously loaded the module and the name is spelled correctly.");
                sw.WriteLine("  2) Ensure you have not accidentally created more than one kernel.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the binding could not be uniquely resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="formattedMatchingBindings">The matching bindings, already formatted as strings</param>
        /// <returns>The exception message.</returns>
        public static string CouldNotUniquelyResolveBinding(IRequest request, string[] formattedMatchingBindings)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0}", request.Service.Format());
                sw.WriteLine("More than one matching bindings are available.");

                sw.WriteLine("Matching bindings:");
                for (int i = 0; i < formattedMatchingBindings.Length; i++)
                {
                    sw.WriteLine("  {0}) {1}", i + 1, formattedMatchingBindings[i]);
                }
                sw.WriteLine("Activation path:");
                sw.WriteLine(request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have defined a binding for {0} only once.", request.Service.Format());

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the binding could not be resolved on the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The exception message.</returns>
        public static string CouldNotResolveBinding(IRequest request)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0}", request.Service.Format());
                sw.WriteLine("No matching bindings are available, and the type is not self-bindable.");

                sw.WriteLine("Activation path:");
                sw.WriteLine(request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have defined a binding for {0}.", request.Service.Format());
                sw.WriteLine("  2) If the binding was defined in a module, ensure that the module has been loaded into the kernel.");
                sw.WriteLine("  3) Ensure you have not accidentally created more than one kernel.");
                sw.WriteLine("  4) If you are using constructor arguments, ensure that the parameter name matches the constructors parameter name.");
                #if !SILVERLIGHT
                sw.WriteLine("  5) If you are using automatic module loading, ensure the search path and filters are correct.");
                #endif

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the specified context has cyclic dependencies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The exception message.</returns>
        public static string CyclicalDependenciesDetected(IContext context)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
                sw.WriteLine("A cyclical dependency was detected between the constructors of two services.");
                sw.WriteLine();

                sw.WriteLine("Activation path:");
                sw.WriteLine(context.Request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have not declared a dependency for {0} on any implementations of the service.", context.Request.Service.Format());
                sw.WriteLine("  2) Consider combining the services into a single one to remove the cycle.");
                sw.WriteLine("  3) Use property injection instead of constructor injection, and implement IInitializable");
                sw.WriteLine("     if you need initialization logic to be run after property values have been injected.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that an invalid attribute type is used in the binding condition.
        /// </summary>
        /// <param name="serviceNames">The names of the services.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="type">The type.</param>
        /// <returns>The exception message.</returns>
        public static string InvalidAttributeTypeUsedInBindingCondition(string serviceNames, string methodName, Type type)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error registering binding(s) for {0}", serviceNames);
                sw.WriteLine("The type {0} used in a call to {1}() is not a valid attribute.", type.Format(), methodName);
                sw.WriteLine();

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have passed the correct type.");
                sw.WriteLine("  2) If you have defined your own attribute type, ensure that it extends System.Attribute.");
                sw.WriteLine("  3) To avoid problems with type-safety, use the generic version of the the method instead,");
                sw.WriteLine("     such as {0}<SomeAttribute>().", methodName);

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that no constructors are available on the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The exception message.</returns>
        public static string NoConstructorsAvailable(IContext context)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
                sw.WriteLine("No constructor was available to create an instance of the implementation type.");
                sw.WriteLine();

                sw.WriteLine("Activation path:");
                sw.WriteLine(context.Request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that the implementation type has a public constructor.");
                sw.WriteLine("  2) If you have implemented the Singleton pattern, use a binding with InSingletonScope() instead.");

                return sw.ToString();
            }
        }
        
        /// <summary>
        /// Generates a message saying that no constructors are available for the given component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The exception message.</returns>
        public static string NoConstructorsAvailableForComponent(Type component, Type implementation)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error loading Ninject component {0}", component.Format());
                sw.WriteLine("No constructor was available to create an instance of the registered implementation type {0}.", implementation.Format());
                sw.WriteLine();

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that the implementation type has a public constructor.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the specified component is not registered.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>The exception message.</returns>
        public static string NoSuchComponentRegistered(Type component)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error loading Ninject component {0}", component.Format());
                sw.WriteLine("No such component has been registered in the kernel's component container.");
                sw.WriteLine();

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) If you have created a custom subclass for KernelBase, ensure that you have properly");
                sw.WriteLine("     implemented the AddComponents() method.");
                sw.WriteLine("  2) Ensure that you have not removed the component from the container via a call to RemoveAll().");
                sw.WriteLine("  3) Ensure you have not accidentally created more than one kernel.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the specified property could not be resolved on the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The exception message.</returns>
        public static string CouldNotResolvePropertyForValueInjection(IRequest request, string propertyName)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0}", request.Service.Format());
                sw.WriteLine("No matching property {0}.", propertyName);

                sw.WriteLine("Activation path:");
                sw.WriteLine(request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that you have the correct property name.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the provider on the specified context returned null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The exception message.</returns>
        public static string ProviderReturnedNull(IContext context)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
                sw.WriteLine("Provider returned null.");
                
                sw.WriteLine("Activation path:");
                sw.WriteLine(context.Request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that the provider handles creation requests properly.");
                
                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates a message saying that the constructor is ambiguous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bestDirectives">The best constructor directives.</param>
        /// <returns>The exception message.</returns>
        public static string ConstructorsAmbiguous(IContext context, IGrouping<int, ConstructorInjectionDirective> bestDirectives)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
                sw.WriteLine("Several constructors have the same priority. Please specify the constructor using ToConstructor syntax or add an Inject attribute.");
                sw.WriteLine();
                
                sw.WriteLine("Constructors:");
                foreach (var constructorInjectionDirective in bestDirectives)
                {
                    FormatConstructor(constructorInjectionDirective.Constructor, sw);
                }

                sw.WriteLine();
                
                sw.WriteLine("Activation path:");
                sw.WriteLine(context.Request.FormatActivationPath());

                sw.WriteLine("Suggestions:");
                sw.WriteLine("  1) Ensure that the implementation type has a public constructor.");
                sw.WriteLine("  2) If you have implemented the Singleton pattern, use a binding with InSingletonScope() instead.");

                return sw.ToString();
            }
        }

        /// <summary>
        /// Formats the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="sw">The string writer.</param>
        private static void FormatConstructor(ConstructorInfo constructor, StringWriter sw)
        {
            foreach (Attribute attribute in constructor.GetCustomAttributes(false))
            {
                FormatAttribute(sw, attribute);
            }

            sw.Write(constructor.DeclaringType.Name);
            sw.Write("(");
            foreach (var parameterInfo in constructor.GetParameters())
            {
                foreach (Attribute attribute in parameterInfo.GetCustomAttributes(false))
                {
                    FormatAttribute(sw, attribute);
                }

                sw.Write(parameterInfo.ParameterType.Format());
                sw.Write(" ");
                sw.Write(parameterInfo.Name);
            }

            sw.WriteLine(")");
        }

        /// <summary>
        /// Formats the attribute.
        /// </summary>
        /// <param name="sw">The string writer.</param>
        /// <param name="attribute">The attribute.</param>
        private static void FormatAttribute(StringWriter sw, Attribute attribute)
        {
            sw.Write("[");
            var name = attribute.GetType().Format();
            name = name.EndsWith("Attribute") ? name.Substring(0, name.Length - 9) : name;
            sw.Write(name);
            sw.Write("]");
        }
    }
}