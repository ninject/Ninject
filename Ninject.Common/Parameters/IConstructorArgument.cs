namespace Ninject.Parameters
{
    using Ninject.Activation;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Defines the interface for constructor arguments.
    /// </summary>
    public interface IConstructorArgument : IParameter
    {
        /// <summary>
        /// Determines if the parameter applies to the given target.
        /// </summary>
        /// <remarks>
        /// Only one parameter may return true.
        /// </remarks>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Tre if the parameter applies in the specified context to the specified target.</returns>
        bool AppliesToTarget(IContext context, ITarget target);
    }
}