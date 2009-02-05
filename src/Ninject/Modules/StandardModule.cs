using System;
using System.Collections.Generic;
using Ninject.Messaging.Messages;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Modules
{
	public abstract class StandardModule : IModule
	{
		public IKernel Kernel { get; set; }
		public string Name { get; set; }
		public ICollection<IBinding> Bindings { get; set; }

		public event EventHandler<BindingMessage> BindingAdded;
		public event EventHandler<BindingMessage> BindingRemoved;

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
			Kernel.AddBinding(binding);
			Bindings.Add(binding);
			BindingAdded.Raise(this, new BindingMessage(binding));
		}

		public void RemoveBinding(IBinding binding)
		{
			Kernel.RemoveBinding(binding);
			Bindings.Remove(binding);
			BindingRemoved.Raise(this, new BindingMessage(binding));
		}

		protected virtual BindingBuilder<T> RegisterBindingAndCreateBuilder<T>(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder<T>(binding);
		}
	}
}