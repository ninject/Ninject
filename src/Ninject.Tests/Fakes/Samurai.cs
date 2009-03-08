using System;

namespace Ninject.Tests.Fakes
{
	public class Samurai : IWarrior
	{
		public IWeapon _weapon;
		private string _name = null;

		public IWeapon Weapon
		{
			get { return _weapon; }
			set { _weapon = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool IsBattleHardened { get; set; }

		public Samurai(IWeapon weapon)
		{
			Weapon = weapon;
		}

		public void SetName(string name)
		{
			_name = name;
		}

		public void DoNothing()
		{
		}

		public string Attack(string enemy)
		{
			IsBattleHardened = true;
			return String.Format("Attacked {0} with a {1}", enemy, Weapon.Name);
		}
	}
}