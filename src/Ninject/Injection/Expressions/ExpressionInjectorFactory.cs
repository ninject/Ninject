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
using System.Collections.Generic;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;
using Ninject.Injection.Expressions;
#endregion

namespace Ninject.Injection.Expressions
{
	/// <summary>
	/// Creates expression-based injectors from members.
	/// </summary>
	public class ExpressionInjectorFactory : NinjectComponent, IInjectorFactory
	{
		private readonly Dictionary<ConstructorInfo, IConstructorInjector> _constructorInjectors = new Dictionary<ConstructorInfo, IConstructorInjector>();
		private readonly Dictionary<MethodInfo, IMethodInjector> _methodInjectors = new Dictionary<MethodInfo, IMethodInjector>();
		private readonly Dictionary<PropertyInfo, IPropertyInjector> _propertyInjectors = new Dictionary<PropertyInfo, IPropertyInjector>();

		/// <summary>
		/// Gets or creates an injector for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The created injector.</returns>
		public IConstructorInjector GetConstructorInjector(ConstructorInfo constructor)
		{
			return _constructorInjectors.GetOrAddNew(constructor, c => new ExpressionConstructorInjector(c));
		}

		/// <summary>
		/// Gets or creates an injector for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The created injector.</returns>
		public IMethodInjector GetMethodInjector(MethodInfo method)
		{
			return _methodInjectors.GetOrAddNew(method, m => method.ReturnType == typeof(void) ? (IMethodInjector)new ExpressionVoidMethodInjector(m) : (IMethodInjector)new ExpressionMethodInjector(m));
		}

		/// <summary>
		/// Gets or creates an injector for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The created injector.</returns>
		public IPropertyInjector GetPropertyInjector(PropertyInfo property)
		{
			return _propertyInjectors.GetOrAddNew(property, p => new ExpressionPropertyInjector(p));
		}
	}
}