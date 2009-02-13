using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An injector that uses a delegate generated from an expression tree to inject values.
	/// </summary>
	/// <typeparam name="TMember">The type of member that this injector injects.</typeparam>
	/// <typeparam name="TDelegate">The type of delegate resulting from the expression tree.</typeparam>
	public abstract class ExpressionBasedInjector<TMember, TDelegate>
		where TMember : MemberInfo
	{
		private Expression<TDelegate> _expression;
		private TDelegate _delegate;

		/// <summary>
		/// Gets the callback that can be used to inject values.
		/// </summary>
		public TDelegate Callback
		{
			get
			{
				if (_delegate == null)
				{
					_delegate = _expression.Compile();
					_expression = null;
				}

				return _delegate;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpressionBasedInjector&lt;TMember, TDelegate&gt;"/> class.
		/// </summary>
		/// <param name="member">The member that will be injected.</param>
		protected ExpressionBasedInjector(TMember member)
		{
			_expression = BuildExpression(member);
		}

		/// <summary>
		/// Builds the expression tree that can be compiled into a delegate, which in turn
		/// can be used to inject values into the member.
		/// </summary>
		/// <param name="member">The member that will be injected.</param>
		/// <returns>The constructed expression tree.</returns>
		protected abstract Expression<TDelegate> BuildExpression(TMember member);
	}
}