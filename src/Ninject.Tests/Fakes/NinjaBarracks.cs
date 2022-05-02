namespace Ninject.Tests.Fakes
{
    public class NinjaBarracks : Barracks
    {
        public NinjaBarracks()
        {
        }

        public NinjaBarracks( IWarrior warrior )
        {
            this.Warrior = warrior;
        }

        public NinjaBarracks( IWeapon weapon )
        {
            this.Weapon = weapon;
        }

        [Inject]
        public NinjaBarracks( IWarrior warrior, IWeapon weapon )
        {
            this.Warrior = warrior;
            this.Weapon = weapon;
        }
    }
}