namespace Ninject.Tests.Fakes
{
	public class NinjaBarracks : Barracks
	{
		public NinjaBarracks()
		{
		}

		public NinjaBarracks( IWarrior warrior )
		{
			Warrior = warrior;
		}

		public NinjaBarracks( IWeapon weapon )
		{
			Weapon = weapon;
		}

		[Inject]
		public NinjaBarracks( IWarrior warrior, IWeapon weapon )
		{
			Warrior = warrior;
			Weapon = weapon;
		}
	}
}