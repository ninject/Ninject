namespace Ninject.Tests.Fakes
{
	internal class Ninja : IWarrior
	{
		public Ninja(IWeapon weapon)
		{
			Weapon = weapon;
		}

		[Inject]
		internal IWeapon SecondaryWeapon { get; set; }

		[Inject]
		private IWeapon SecretWeapon { get; set; }

		public IWeapon SecretWeaponAccessor
		{
			get { return SecretWeapon; }
			set { SecretWeapon = value; }
		}

		#region IWarrior Members

		public IWeapon Weapon { get; set; }

		#endregion
	}
}