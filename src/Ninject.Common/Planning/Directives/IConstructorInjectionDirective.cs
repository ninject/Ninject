using System.Reflection;
using Ninject.Injection;

namespace Ninject.Planning.Directives
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstructorInjectionDirective : IMethodInjectionDirectiveBase<ConstructorInjector>
    {
        /// <summary>
        /// The base .ctor definition.
        /// </summary>
        ConstructorInfo Constructor { get; set; }
    }
}