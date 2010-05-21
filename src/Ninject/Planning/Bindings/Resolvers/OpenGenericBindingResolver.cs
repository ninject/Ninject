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
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;

#endregion

namespace Ninject.Planning.Bindings.Resolvers
{
	/// <summary>
	/// Resolves bindings for open generic types.
	/// </summary>
	public class OpenGenericBindingResolver : NinjectComponent, IBindingResolver
	{
		/// <summary>
		/// Returns any bindings from the specified collection that match the specified service.
		/// </summary>
		/// <param name="bindings">The multimap of all registered bindings.</param>
		/// <param name="service">The service in question.</param>
		/// <returns>The series of matching bindings.</returns>
		public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
		{
			if (!service.IsGenericType || !bindings.ContainsKey(service.GetGenericTypeDefinition()))
				return Enumerable.Empty<IBinding>();

			return bindings[service.GetGenericTypeDefinition()].ToEnumerable();
		}
	}
}