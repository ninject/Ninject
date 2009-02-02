using System;
using System.Reflection;

namespace Ninject.Planning.Targets
{
	public class PropertyTarget : Target<PropertyInfo>
	{
		public override Type Type
		{
			get { return Site.PropertyType; }
		}

		public override string Name
		{
			get { return Site.Name; }
		}

		public PropertyTarget(PropertyInfo site) : base(site) { }
	}
}