using System;
using System.Reflection;

namespace Ninject.Planning.Targets
{
	public class ParameterTarget : Target<ParameterInfo>
	{
		public override Type Type
		{
			get { return Site.ParameterType; }
		}

		public override string Name
		{
			get { return Site.Name; }
		}

		public ParameterTarget(ParameterInfo site) : base(site) { }
	}
}