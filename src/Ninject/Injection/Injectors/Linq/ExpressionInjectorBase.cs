using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public abstract class ExpressionInjectorBase<TMember, TDelegate> : IInjector<TMember>
		where TMember : MemberInfo
	{
		private TDelegate _callback;

		public TMember Member { get; private set; }

		public TDelegate Callback
		{
			get
			{
				if (_callback == null) _callback = BuildExpression(Member).Compile();
				return _callback;
			}
		}

		protected ExpressionInjectorBase(TMember member)
		{
			Member = member;
		}

		protected abstract Expression<TDelegate> BuildExpression(TMember member);
	}
}