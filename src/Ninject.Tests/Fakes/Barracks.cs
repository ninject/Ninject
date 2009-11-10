namespace Ninject.Tests.Fakes
{
	public class Barracks
	{
		public Barracks()
		{
		}

		public Barracks( IWarrior warrior )
		{
			Warrior = warrior;
		}

		public Barracks( IWeapon weapon )
		{
			Weapon = weapon;
		}

		public Barracks( IWarrior warrior, IWeapon weapon )
		{
			Warrior = warrior;
			Weapon = weapon;
		}

		public IWeapon Weapon { get; set; }
		public IWarrior Warrior { get; set; }
	}
}