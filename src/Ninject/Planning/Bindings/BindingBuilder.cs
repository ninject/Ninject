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
    using System;
#if !NETCF
    using System.Linq.Expressions;
#endif
    using Ninject.Activation;
    using Ninject.Activation.Providers;
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
        /// <param name="serviceNames">The names of the services.</param>
        public BindingBuilder(IBindingConfiguration bindingConfiguration, IKernel kernel, string serviceNames)
        {
            Ensure.ArgumentNotNull(bindingConfiguration, "binding");
            Ensure.ArgumentNotNull(kernel, "kernel");
            this.BindingConfiguration = bindingConfiguration;
            this.Kernel = kernel;
            this.ServiceNames = serviceNames;
            this.BindingConfiguration.ScopeCallback = kernel.Settings.DefaultScopeCallback;
        }

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Gets the names of the services.
        /// </summary>
        /// <value>The names of the services.</value>
        protected string ServiceNames { get; private set; }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalTo<TImplementation>()
        {
            return this.InternalTo<TImplementation>(typeof(TImplementation));
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="T">The type of the returned syntax.</typeparam>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<T> InternalTo<T>(Type implementation)
        {
            this.BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
            this.BindingConfiguration.Target = BindingTarget.Type;

            return new BindingConfigurationBuilder<T>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }
        
        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConfiguration<TImplementation>(TImplementation value) 
        {
            this.BindingConfiguration.ProviderCallback = ctx => new ConstantProvider<TImplementation>(value);
            this.BindingConfiguration.Target = BindingTarget.Constant;
            this.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;

            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToMethod<TImplementation>(Func<IContext, TImplementation> method)
        {
            this.BindingConfiguration.ProviderCallback = ctx => new CallbackProvider<TImplementation>(method);
            this.BindingConfiguration.Target = BindingTarget.Method;

            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToProvider<TImplementation>(IProvider<TImplementation> provider)
        {
            this.BindingConfiguration.ProviderCallback = ctx => provider;
            this.BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProviderInternal<TProvider, TImplementation>()
            where TProvider : IProvider
        {
            this.BindingConfiguration.ProviderCallback = ctx => ctx.Kernel.Get<TProvider>();
            this.BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="T">The type of the returned fleunt syntax</typeparam>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<T> ToProviderInternal<T>(Type providerType)
        {
            this.BindingConfiguration.ProviderCallback = ctx => ctx.Kernel.Get(providerType) as IProvider;
            this.BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<T>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

#if !NETCF
        /// <summary>
        /// Indicates that the service should be bound to the speecified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
        {
            var ctorExpression = newExpression.Body as NewExpression;
            if (ctorExpression == null)
            {
                throw new ArgumentException("The expression must be a constructor call.", "newExpression");
            }

            this.BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(ctorExpression.Type, ctorExpression.Constructor);
            this.BindingConfiguration.Target = BindingTarget.Type;
            this.AddConstructorArguments(ctorExpression, newExpression.Parameters[0]);

            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, this.ServiceNames, this.Kernel);
        }

        /// <summary>
        /// Adds the constructor arguments for the specified constructor expression.
        /// </summary>
        /// <param name="ctorExpression">The ctor expression.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        private void AddConstructorArguments(NewExpression ctorExpression, ParameterExpression constructorArgumentSyntaxParameterExpression)
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
                !methodCall.Method.IsGenericMethod ||
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