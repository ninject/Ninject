using System;
using System.Reflection;

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents an injection target for a <see cref="PropertyInfo"/>.
	/// </summary>
	public class PropertyTarget : Target<PropertyInfo>
	{
		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public override string Name
		{
			get { return Site.Name; }
		}

		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		public override Type Type
		{
			get { return Site.PropertyType; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyTarget"/> class.
		/// </summary>
		/// <param name="site">The property that this target represents.</param>
		public PropertyTarget(PropertyInfo site) : base(site, site) { }
	}
}