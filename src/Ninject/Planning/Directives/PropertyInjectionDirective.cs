using System;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a property.
	/// </summary>
	public class PropertyInjectionDirective : IDirective
	{
		/// <summary>
		/// Gets or sets the member the directive describes.
		/// </summary>
		public PropertyInfo Member { get; private set; }

		/// <summary>
		/// Gets or sets the injection target for the directive.
		/// </summary>
		public ITarget Target { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionDirective"/> class.
		/// </summary>
		/// <param name="member">The member the directive describes.</param>
		public PropertyInjectionDirective(PropertyInfo member)
		{
			Member = member;
			Target = new PropertyTarget(member);
		}
	}
}