using System;

namespace Ninject
{
    /// <summary>
    /// Utility method to determine runtime platform.
    /// </summary>
    public static class RuntimeEnvironment
    {
        /// <summary>
        /// Is the code currently running on a Mono runtime?
        /// </summary>
        public static bool IsMonoRuntime
        {
            get
            {
                var t = Type.GetType("Mono.Runtime");
                return t != null;
            }
        }

        /// <summary>
        /// Is the code currently running on Mono-4.0 or later?
        /// </summary>
        /// <returns></returns>
        public static bool IsMonoOnFramework40OrGreater()
        {
            // If we are running on a 4.x runtime, we assume that we're running on a .NET 4+ compatible framework
            return IsMonoRuntime && Environment.Version.Major >= 4;
        }
    }
}