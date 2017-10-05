namespace Ninject.Tests.Fakes
{
    public class Barracks
    {
        public Barracks()
        {
        }

        public Barracks( IWarrior warrior )
        {
            this.Warrior = warrior;
        }

        public Barracks( IWeapon weapon )
        {
            this.Weapon = weapon;
        }

        public Barracks( IWarrior warrior, IWeapon weapon )
        {
            this.Warrior = warrior;
            this.Weapon = weapon;
        }

        public IWeapon Weapon { get; set; }
        public IWarrior Warrior { get; set; }
    }
}