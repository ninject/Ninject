using System;
using Ninject.Planning.Bindings;

namespace Ninject
{
	/// <summary>
	/// Indicates that the decorated member should only be injected using binding(s) registered
	/// with the specified name.
	/// </summary>
	public class NamedAttribute : ConstraintAttribute
	{
		/// <summary>
		/// Gets or sets the binding name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedAttribute"/> class.
		/// </summary>
		/// <param name="name">The name of the binding(s) to use.</param>
		public NamedAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Determines whether the specified binding metadata matches the constraint.
		/// </summary>
		/// <param name="metadata">The metadata in question.</param>
		/// <returns><c>True</c> if the metadata matches; otherwise <c>false</c>.</returns>
		public override bool Matches(IBindingMetadata metadata)
		{
			return metadata.Name == Name;
		}
	}
}
