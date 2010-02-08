#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Activation.Providers
{
	/// <summary>
	/// A provider that delegates to a callback method to create instances.
	/// </summary>
	/// <typeparam name="T">The type of instances the provider creates.</typeparam>
	public class CallbackProvider<T> : Provider<T>
	{
		/// <summary>
		/// Gets the callback method used by the provider.
		/// </summary>
		public Func<IContext, T> Method { get; private set; }

#if MONO
		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="method">The callback method that will be called to create instances.</param>
#else
		/// <summary>
		/// Initializes a new instance of the <see cref="Ninject.Activation.Providers.CallbackProvider{T}"/> class.
		/// </summary>
		/// <param name="method">The callback method that will be called to create instances.</param>
#endif
		public CallbackProvider(Func<IContext, T> method)
		{
			Ensure.ArgumentNotNull(method, "method");
			Method = method;
		}

		/// <summary>
		/// Invokes the callback method to create an instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override T CreateInstance(IContext context)
		{
			return Method(context);
		}
	}
}