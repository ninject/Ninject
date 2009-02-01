using System;
using System.Collections.Generic;
using Ninject.Bindings;
using Ninject.Infrastructure;
using Ninject.Syntax;

namespace Ninject.Modules
{
	public abstract class ModuleBase : IBindingRoot, IModule
	{
		public IKernel Kernel { get; set; }
		public string Name { get; set; }
		public ICollection<IBinding> Bindings { get; set; }

		protected ModuleBase()
		{
			Bindings = new List<IBinding>();
			Name = GetType().FullName;
		}

		protected ModuleBase(string name)
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

		public IBindingToSyntax Bind(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder(binding);
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
	}
}