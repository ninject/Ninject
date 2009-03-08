using System;

namespace Ninject.Tests.Fakes
{
	public class Sword : IWeapon
	{
		public string Name
		{
			get { return "sword"; }
		}
	}
}