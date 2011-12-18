//-------------------------------------------------------------------------------
// <copyright file="BindingBuilder.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
#if !NETCF
    using System;
    using System.Linq.Expressions;
#endif
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="BindingConfiguration"/>.
    /// </summary>
    public class BindingBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder"/> class.
        /// </summary>
        /// <param name="bindingConfiguration">The binding to build.</param>
        /// <param name="kernel">The kernel.</param>
        public BindingBuilder(IBindingConfiguration bindingConfiguration, IKernel kernel)
        {
            Ensure.ArgumentNotNull(bindingConfiguration, "binding");
            Ensure.ArgumentNotNull(kernel, "kernel");
            this.BindingConfiguration = bindingConfiguration;
            this.Kernel = kernel;
        }

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }
#if !NETCF
        /// <summary>
        /// Adds the constructor arguments for the specified constructor expression.
        /// </summary>
        /// <param name="ctorExpression">The ctor expression.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        protected void AddConstructorArguments(NewExpression ctorExpression, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            var parameters = ctorExpression.Constructor.GetParameters();

            for (var i = 0; i < ctorExpression.Arguments.Count; i++)
            {
                var argument = ctorExpression.Arguments[i];
                var argumentName = parameters[i].Name;

                this.AddConstructorArgument(argument, argumentName, constructorArgumentSyntaxParameterExpression);
            }
        }

        /// <summary>
        /// Adds a constructor argument for the specified argument expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        private void AddConstructorArgument(Expression argument, string argumentName, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            var methodCall = argument as MethodCallExpression;
            if (methodCall == null ||
                methodCall.Method.GetGenericMethodDefinition().DeclaringType != typeof(IConstructorArgumentSyntax))
            {
                var compiledExpression = Expression.Lambda(argument, constructorArgumentSyntaxParameterExpression).Compile();
                this.BindingConfiguration.Parameters.Add(new ConstructorArgument(
                    argumentName, 
                    ctx => compiledExpression.DynamicInvoke(new ConstructorArgumentSyntax(ctx))));
            }
        }

        /// <summary>
        /// Passed to ToConstructor to specify that a constructor value is Injected.
        /// </summary>
        private class ConstructorArgumentSyntax : IConstructorArgumentSyntax
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorArgumentSyntax"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public ConstructorArgumentSyntax(IContext context)
            {
                this.Context = context;
            }

            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>The context.</value>
            public IContext Context
            {
                get;
                private set;
            }

            /// <summary>
            /// Specifies that the argument is injected.
            /// </summary>
            /// <typeparam name="T1">The type of the parameter</typeparam>
            /// <returns>Not used. This interface has no implementation.</returns>
            public T1 Inject<T1>()
            {
                throw new InvalidOperationException("This method is for declaration that a parameter shall be injected only! Never call it directly.");
            }
        }
#endif
    }
}