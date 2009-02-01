using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Creation;
using Ninject.Infrastructure;
using Ninject.Bindings;
using Ninject.Infrastructure.Components;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Resolution;
using Ninject.Syntax;

namespace Ninject
{
	public abstract class KernelBase : IKernel
	{
		private readonly Multimap<Type, IBinding> _bindings = new Multimap<Type, IBinding>();
		private readonly Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();

		public KernelOptions Options { get; private set; }
		public IComponentContainer Components { get; private set; }

		protected KernelBase(params IModule[] modules)
			: this(new ComponentContainer(), new KernelOptions(), modules) { }

		protected KernelBase(KernelOptions options, params IModule[] modules)
			: this(new ComponentContainer(), options, modules) { }

		protected KernelBase(IComponentContainer components, KernelOptions options, params IModule[] modules)
		{
			Options = options;
			Components = components;
			modules.Map(Load);
		}

		public virtual void Load(IModule module)
		{
			_modules.Add(module.Name, module);
		}

		public virtual void Unload(string moduleName)
		{
			_modules[moduleName].Unload();
			_modules.Remove(moduleName);
		}

		public virtual void Unload(IModule module)
		{
			Unload(module.Name);
		}

		public virtual IEnumerable<IContext> Resolve(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		public virtual IEnumerable<IContext> Resolve(IRequest request)
		{
			if (!_bindings.ContainsKey(request.Service) && (!request.Service.IsGenericType || !_bindings.ContainsKey(request.Service.GetGenericTypeDefinition())))
				RegisterImplicitSelfBinding(request.Service);

			return GetBindings(request).Where(binding => request.Matches(binding)).Select(binding => CreateContext(request, binding));
		}

		public IBindingToSyntax Bind(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder(binding);
		}

		public void AddBinding(IBinding binding)
		{
			_bindings.Add(binding.Service, binding);
		}

		public void RemoveBinding(IBinding binding)
		{
			_bindings.Remove(binding.Service, binding);
		}

		public virtual IEnumerable<IBinding> GetBindings(IRequest request)
		{
			foreach (IBinding binding in _bindings[request.Service])
				yield return binding;

			if (request.Service.IsGenericType)
			{
				Type gtd = request.Service.GetGenericTypeDefinition();

				foreach (IBinding binding in _bindings[gtd])
					yield return binding;
			}
		}

		public IResolutionRoot BeginScope()
		{
			return new ResolutionScope(this);
		}

		protected virtual void RegisterImplicitSelfBinding(Type service)
		{
			if (service.IsInterface || service.IsAbstract || service.ContainsGenericParameters)
				throw new NotSupportedException();

			var binding = new Binding(service) { ProviderCallback = StandardProvider.GetCreationCallback(service) };
			AddBinding(binding);
		}

		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, null);
		}

		protected virtual IContext CreateContext(IRequest request, IBinding binding)
		{
			return new Context(this, request, binding, Components.Get<IResolver>());
		}
	}
}
