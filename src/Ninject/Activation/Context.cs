using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Ninject.Activation
{
	/// <summary>
	/// Contains information about the activation of a single instance.
	/// </summary>
	public class Context : TraceInfoProvider, IContext
	{
		/// <summary>
		/// Gets the kernel that is driving the activation.
		/// </summary>
		public IKernel Kernel { get; set; }

		/// <summary>
		/// Gets the request.
		/// </summary>
		public IRequest Request { get; set; }

		/// <summary>
		/// Gets the binding.
		/// </summary>
		public IBinding Binding { get; set; }

		/// <summary>
		/// Gets or sets the activation plan.
		/// </summary>
		public IPlan Plan { get; set; }

		/// <summary>
		/// Gets the parameters that were passed to manipulate the activation process.
		/// </summary>
		public ICollection<IParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the activated instance.
		/// </summary>
		public object Instance { get; set; }

		/// <summary>
		/// Gets the generic arguments for the request, if any.
		/// </summary>
		public Type[] GenericArguments { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the request involves inferred generic arguments.
		/// </summary>
		public bool HasInferredGenericArguments { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Context"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="request">The request.</param>
		/// <param name="binding">The binding.</param>
		public Context(IKernel kernel, IRequest request, IBinding binding)
		{
			Kernel = kernel;
			Request = request;
			Binding = binding;
			Parameters = request.Parameters.Union(binding.Parameters).ToList();

			if (binding.Service.IsGenericTypeDefinition)
			{
				HasInferredGenericArguments = true;
				GenericArguments = request.Service.GetGenericArguments();
			}
		}

		/// <summary>
		/// Gets the scope for the context that "owns" the instance activated therein.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		public object GetScope()
		{
			return Request.GetScope() ?? Binding.GetScope(this);
		}

		/// <summary>
		/// Gets the provider that should be used to create the instance for this context.
		/// </summary>
		/// <returns>The provider that should be used.</returns>
		public IProvider GetProvider()
		{
			return Binding.GetProvider(this);
		}
	}
}