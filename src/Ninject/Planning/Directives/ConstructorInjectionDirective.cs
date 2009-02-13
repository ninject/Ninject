using System;
using System.Reflection;

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a constructor.
	/// </summary>
	public class ConstructorInjectionDirective : MethodInjectionDirectiveBase<ConstructorInfo>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorInjectionDirective"/> class.
		/// </summary>
		/// <param name="member">The constructor described by the directive.</param>
		public ConstructorInjectionDirective(ConstructorInfo member) : base(member) { }
	}
}