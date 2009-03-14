using IronRuby.Builtins;
using Ninject.Activation;
using Ninject.Dynamic.Extensions;

namespace Ninject.Dynamic.Activation.Providers
{
    public class RubyProcProvider : Provider<object>
    {

        /// <summary>
		/// Gets the callback method used by the provider.
		/// </summary>
		public Proc Method { get; private set; }

		/// <summary>
        /// Initializes a new instance of the <see cref="RubyProcProvider"/> class.
		/// </summary>
		/// <param name="method">The callback method that will be called to create instances.</param>
        public RubyProcProvider(Proc method)
		{
			method.EnsureArgumentNotNull("method");
			Method = method;
		}

        #region Overrides of Provider<object>

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        protected override object CreateInstance(IContext context)
        {
            return Method.Call(context);
        }

        #endregion
    }
}