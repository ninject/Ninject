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
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to resolve instances.
	/// </summary>
	public interface IResolutionRoot
	{
		/// <summary>
		/// Determines whether the specified request can be resolved.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
		bool CanResolve(IRequest request);

		/// <summary>
		/// Resolves instances for the specified request. The instances are not actually resolved
		/// until a consumer iterates over the enumerator.
		/// </summary>
		/// <param name="request">The request to resolve.</param>
		/// <returns>An enumerator of instances that match the request.</returns>
		IEnumerable<object> Resolve(IRequest request);

		/// <summary>
		/// Creates a request for the specified service.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
		/// <returns>The created request.</returns>
		IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional);
	}
}