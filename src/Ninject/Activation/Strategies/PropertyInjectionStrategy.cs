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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Injects properties on an instance during activation.
	/// </summary>
	public class PropertyInjectionStrategy : ActivationStrategy
	{
		private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

		private BindingFlags Flags
		{
			get
			{
				#if !NO_LCG && !SILVERLIGHT
				return Settings.InjectNonPublic ? (DefaultFlags | BindingFlags.NonPublic) : DefaultFlags;
				#else
				return DefaultFlags;
				#endif
			}
		}

		/// <summary>
		/// Gets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
		/// </summary>
		/// <param name="injectorFactory">The injector factory component.</param>
		public PropertyInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		/// <summary>
		/// Injects values into the properties as described by <see cref="PropertyInjectionDirective"/>s
		/// contained in the plan.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="reference">A reference to the instance being activated.</param>
		public override void Activate(IContext context, InstanceReference reference)
		{
			Ensure.ArgumentNotNull(context, "context");
			Ensure.ArgumentNotNull(reference, "reference");

			var propertyValues = context.Parameters.Where(parameter => parameter is PropertyValue);
			IEnumerable<string> parameterNames = propertyValues.Select(parameter => parameter.Name);

			foreach (var directive in context.Plan.GetAll<PropertyInjectionDirective>())
			{
				PropertyInjectionDirective propertyInjectionDirective = directive;
				if (parameterNames.Any(name => string.Equals(name, propertyInjectionDirective)))
					continue;

				object value = GetValue(context, directive.Target);
				directive.Injector(reference.Instance, value);
			}

			AssignProperyOverrides( context, reference, propertyValues );
		}

		/// <summary>
		/// Applies user supplied override values to instance properties.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="reference">A reference to the instance being activated.</param>
		/// <param name="propertyValues">The parameter ovverride value accessors.</param>
		private void AssignProperyOverrides( IContext context, InstanceReference reference, IEnumerable<IParameter> propertyValues )
		{
			var properties = reference.Instance.GetType().GetProperties( Flags );
			foreach (var propertyValue in propertyValues)
			{
				string propertyName = propertyValue.Name;
				var propertyInfo = properties
					.Where(property => string.Equals(property.Name, propertyName, StringComparison.Ordinal))
					.FirstOrDefault();

				if(propertyInfo == null)
					throw new ActivationException(ExceptionFormatter.CouldNotResolveProperyForValueInjection(context.Request, propertyName));
				
				var target = new PropertyInjectionDirective( propertyInfo, InjectorFactory.Create( propertyInfo ) );
				object value = GetValue(context, target.Target);
				target.Injector(reference.Instance, value);
			}
		}

		/// <summary>
		/// Gets the value to inject into the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <returns>The value to inject into the specified target.</returns>
		public object GetValue(IContext context, ITarget target)
		{
			Ensure.ArgumentNotNull(context, "context");
			Ensure.ArgumentNotNull(target, "target");

			var parameter = context.Parameters.OfType<PropertyValue>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}
	}
}