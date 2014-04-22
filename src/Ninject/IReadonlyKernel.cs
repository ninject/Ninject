using System;
using System.Collections.Generic;
using Ninject.Infrastructure.Disposal;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject
{
    /// <summary>
    /// A kernel that is used to resolve instances and has a configuration that can't be changed anymore
    /// </summary>
    /// ToDo: Remove IHaveNinjectComponents
    public interface IReadonlyKernel :
        IResolutionRoot, IHaveNinjectSettings, IHaveNinjectComponents, IServiceProvider, IDisposableObject
    {
        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        IEnumerable<IBinding> GetBindings(Type service);
    }
}