namespace Ninject
{
    using System.Collections.Generic;

    using Ninject.Infrastructure.Disposal;

    /// <summary>
    /// All kernels register on the static list of this class. This allows derived types to do things for the kernels.
    /// e.g. used by One per request module to release the objects in request scope right after the request ended.
    /// </summary>
    public class GlobalKernelRegistry : DisposableObject
    {
        private static readonly List<IKernel> kernels = new List<IKernel>();
        
        /// <summary>
        /// Start managing instances for the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void StartManaging(IKernel kernel)
        {
            kernels.Add(kernel);
        }

        /// <summary>
        /// Stops managing instances for the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void StopManaging(IKernel kernel)
        {
            kernels.Remove(kernel);
        }

        /// <summary>
        /// Gets all kernels of the application.
        /// </summary>
        /// <value>The kernels of the application.</value>
        public static IEnumerable<IKernel> Kernels
        {
            get
            {
                return kernels;
            }
        }
    }
}