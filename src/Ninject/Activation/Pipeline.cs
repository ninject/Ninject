using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Activation
{
	/// <summary>
	/// Drives the activation (injection, etc.) of an instance.
	/// </summary>
	public class Pipeline : NinjectComponent, IPipeline
	{
		/// <summary>
		/// Gets the strategies that contribute to the activation and deactivation processes.
		/// </summary>
		public IList<IActivationStrategy> Strategies { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipeline"/> class.
		/// </summary>
		/// <param name="strategies">The strategies to execute during activation and deactivation.</param>
		public Pipeline(IEnumerable<IActivationStrategy> strategies)
		{
			Strategies = strategies.ToList();
		}

		/// <summary>
		/// Activates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Activate(IContext context)
		{
			Strategies.Map(s => s.Activate(context));
		}

		/// <summary>
		/// Deactivates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Deactivate(IContext context)
		{
			Strategies.Map(s => s.Deactivate(context));
		}
	}
}