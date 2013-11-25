﻿namespace Ninject.Parameters
{
    using System;
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Overrides the injected value of a constructor argument.
    /// </summary>
    public class TypeMatchingConstructorArgument : IConstructorArgument
    {
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatchingConstructorArgument"/> class.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        public TypeMatchingConstructorArgument(Type type, Func<IContext, ITarget, object> valueCallback)
            : this(type, valueCallback, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatchingConstructorArgument"/> class.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public TypeMatchingConstructorArgument(Type type, Func<IContext, ITarget, object> valueCallback, bool shouldInherit)
        {
            Ensure.ArgumentNotNull(type, "type");
            Ensure.ArgumentNotNull(valueCallback, "valueCallback");

            this.ValueCallback = valueCallback;
            this.ShouldInherit = shouldInherit;
            this.type = type;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        public bool ShouldInherit { get; private set; }

        /// <summary>
        /// Gets or sets the callback that will be triggered to get the parameter's value.
        /// </summary>
        private Func<IContext, ITarget, object> ValueCallback { get; set; }

        /// <summary>
        /// Determines if the parameter applies to the given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// True if the parameter applies in the specified context to the specified target.
        /// </returns>
        /// <remarks>
        /// Only one parameter may return true.
        /// </remarks>
        public bool AppliesToTarget(IContext context, ITarget target)
        {
            return target.Type == this.type;
        }

        /// <summary>
        /// Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            Ensure.ArgumentNotNull(context, "context");
            return this.ValueCallback(context, target);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>True</c> if the objects are equal; otherwise <c>false</c></returns>
        public bool Equals(IParameter other)
        {
            var argument = other as TypeMatchingConstructorArgument;
            return argument != null && other.GetType() == this.type;
        }

        /// <summary>
        /// Determines whether the object equals the specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><c>True</c> if the objects are equal; otherwise <c>false</c></returns>
        public override bool Equals(object obj)
        {
            var parameter = obj as IParameter;
            return parameter != null ? this.Equals(parameter) : ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the object.</returns>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ this.type.GetHashCode();
        }
    }
}