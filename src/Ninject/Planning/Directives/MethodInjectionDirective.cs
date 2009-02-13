using System;
using System.Reflection;

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method.
	/// </summary>
	public class MethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionDirective"/> class.
		/// </summary>
		/// <param name="member">The method described by the directive.</param>
		public MethodInjectionDirective(MethodInfo member) : base(member) { }
	}
}