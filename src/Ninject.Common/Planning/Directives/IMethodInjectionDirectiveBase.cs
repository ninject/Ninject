using System.Linq;
using System.Collections.Generic;
using System;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInjector"></typeparam>
    public interface IMethodInjectionDirectiveBase<TInjector> : IDirective
    {
        /// <summary>
        /// Gets or sets the injector that will be triggered.
        /// </summary>
        TInjector Injector { get; }

        /// <summary>
        /// Gets or sets the targets for the directive.
        /// </summary>
        ITarget[] Targets { get; }
    }
}