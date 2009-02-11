using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject.Syntax;

namespace Ninject.Web.Mvc
{
	public class ControllerRegistry : IControllerRegistry
	{
		private readonly Dictionary<string, Type> _controllers = new Dictionary<string, Type>();

		public IControllerNamer Namer { get; set; }

		public ControllerRegistry(IControllerNamer namer)
		{
			Namer = namer;
		}

		public void RegisterAllControllersIn(string assemblyName)
		{
			RegisterAllControllersIn(Assembly.Load(assemblyName));
		}

		public void RegisterAllControllersIn(Assembly assembly)
		{
			foreach (ControllerDefinition definition in FindControllersIn(assembly))
				_controllers[definition.Name] = definition.Type;
		}

		public void Register(string controllerName, Type controllerType)
		{
			string normalizedName = Namer.NormalizeControllerName(controllerName);
			_controllers[normalizedName] = controllerType;
		}

		public Type GetController(RequestContext requestContext, string controllerName)
		{
			string normalizedName = Namer.NormalizeControllerName(controllerName);
			return _controllers.ContainsKey(normalizedName) ? _controllers[normalizedName] : null;
		}

		protected virtual bool ShouldRegister(Type type)
		{
			return typeof(IController).IsAssignableFrom(type) && type.IsPublic && !type.IsAbstract && !type.IsInterface;
		}

		private IEnumerable<ControllerDefinition> FindControllersIn(Assembly assembly)
		{
			return assembly.GetExportedTypes()
				.Where(ShouldRegister)
				.Select(type => new ControllerDefinition { Name = Namer.GetNameForController(type), Type = type });
		}

		private class ControllerDefinition
		{
			public string Name { get; set; }
			public Type Type { get; set; }
		}
	}
}