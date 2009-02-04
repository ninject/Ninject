using System;
using System.Collections.Generic;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Modules
{
	public abstract class StandardModule : IModule
	{
		public IKernel Kernel { get; set; }
		public string Name { get; set; }
		public ICollection<IBinding> Bindings { get; set; }

		protected StandardModule()
		{
			Bindings = new List<IBinding>();
			Name = GetType().FullName;
		}

		protected StandardModule(string name)
			: this()
		{
			Name = name;
		}

		public void OnLoad(IKernel kernel)
		{
			Kernel = kernel;
			Load();
		}

		public void OnUnload(IKernel kernel)
		{
			Unload();
			Bindings.Map(Kernel.RemoveBinding);
			Kernel = null;
		}

		public abstract void Load();

		public virtual void Unload()
		{
		}

		public IBindingToSyntax<T> Bind<T>()
		{
			return RegisterBindingAndCreateBuilder<T>(typeof(T));
		}

		public IBindingToSyntax<object> Bind(Type service)
		{
			return RegisterBindingAndCreateBuilder<object>(service);
		}

		public void AddBinding(IBinding binding)
		{
			Bindings.Add(binding);
			Kernel.AddBinding(binding);
		}

		public void RemoveBinding(IBinding binding)
		{
			Kernel.RemoveBinding(binding);
		}

		protected virtual BindingBuilder<T> RegisterBindingAndCreateBuilder<T>(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder<T>(binding);
		}
	}
}