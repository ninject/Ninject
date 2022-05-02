using System;

namespace Ninject.Tests.Fakes
{
    public class Samurai : IWarrior
    {
        public IWeapon _weapon;
        private string _name = null;

        public IWeapon Weapon
        {
            get { return this._weapon; }
            set { this._weapon = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public bool IsBattleHardened { get; set; }

        public Samurai(IWeapon weapon)
        {
            this.Weapon = weapon;
        }

        public void SetName(string name)
        {
            this._name = name;
        }

        public void DoNothing()
        {
        }

        public string Attack(string enemy)
        {
            this.IsBattleHardened = true;
            return String.Format("Attacked {0} with a {1}", enemy, this.Weapon.Name);
        }
    }
}