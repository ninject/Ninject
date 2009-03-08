#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Linq.Expressions;
using System.Reflection;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Injection.Expressions
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
			Ensure.ArgumentNotNull(member, "member");
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