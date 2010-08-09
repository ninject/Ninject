#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.IO;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Infrastructure.Introspection
{
	internal static class ExceptionFormatter
	{
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
				sw.WriteLine("	 that may be found by the module loader.");
				#endif

				return sw.ToString();
			}
		}

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

		public static string CouldNotUniquelyResolveBinding(IRequest request)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error activating {0}", request.Service.Format());
				sw.WriteLine("More than one matching bindings are available.");

				sw.WriteLine("Activation path:");
				sw.WriteLine(request.FormatActivationPath());

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that you have defined a binding for {0} only once.", request.Service.Format());

				return sw.ToString();
			}
		}

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
				#if !SILVERLIGHT
				sw.WriteLine("  4) If you are using automatic module loading, ensure the search path and filters are correct.");
				#endif

				return sw.ToString();
			}
		}

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
				sw.WriteLine("	 if you need initialization logic to be run after property values have been injected.");

				return sw.ToString();
			}
		}

		public static string InvalidAttributeTypeUsedInBindingCondition(IBinding binding, string methodName, Type type)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error registering binding for {0}", binding.Service.Format());
				sw.WriteLine("The type {0} used in a call to {1}() is not a valid attribute.", type.Format(), methodName);
				sw.WriteLine();

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that you have passed the correct type.");
				sw.WriteLine("  2) If you have defined your own attribute type, ensure that it extends System.Attribute.");
				sw.WriteLine("  3) To avoid problems with type-safety, use the generic version of the the method instead,");
				sw.WriteLine("	 such as {0}<SomeAttribute>().", methodName);

				return sw.ToString();
			}
		}

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

		public static string NoSuchComponentRegistered(Type component)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error loading Ninject component {0}", component.Format());
				sw.WriteLine("No such component has been registered in the kernel's component container.");
				sw.WriteLine();

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) If you have created a custom subclass for KernelBase, ensure that you have properly");
				sw.WriteLine("	 implemented the AddComponents() method.");
				sw.WriteLine("  2) Ensure that you have not removed the component from the container via a call to RemoveAll().");
				sw.WriteLine("  3) Ensure you have not accidentally created more than one kernel.");

				return sw.ToString();
			}
		}

		public static string CouldNotResolveProperyForValueInjection(IRequest request, string propertyName)
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
	}
}