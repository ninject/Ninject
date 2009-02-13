using System;
using System.Linq;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method or constructor.
	/// </summary>
	/// <typeparam name="T">The type of member that the directive describes.</typeparam>
	public abstract class MethodInjectionDirectiveBase<T> : IDirective
		where T : MethodBase
	{
		/// <summary>
		/// Gets the member associated with the directive.
		/// </summary>
		public T Member { get; private set; }

		/// <summary>
		/// Gets the targets for the directive.
		/// </summary>
		public ITarget[] Targets { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionDirectiveBase{T}"/> class.
		/// </summary>
		/// <param name="member">The method described by the directive.</param>
		protected MethodInjectionDirectiveBase(T member)
		{
			Member = member;
			Targets = GetParameterTargets(member);
		}

		/// <summary>
		/// Creates targets for the parameters of the method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The targets for the method's parameters.</returns>
		protected ITarget[] GetParameterTargets(T method)
		{
			return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
		}
	}
}