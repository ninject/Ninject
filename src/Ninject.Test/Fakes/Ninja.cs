namespace Ninject.Tests.Fakes
{
    using System;

#if !SILVERLIGHT
    internal class Ninja : IWarrior
#else
    public class Ninja : IWarrior
#endif
    {
        public Ninja(IWeapon weapon)
        {
            this.Weapon = weapon;
        }

        [Inject]
        public virtual IWeapon OffHandWeapon { get; set; }

        [Inject]
        internal virtual IWeapon SecondaryWeapon { get; set; }

        [Inject]
        protected virtual IWeapon SecretWeapon { get; set; }

        [Inject]
        private IWeapon VerySecretWeapon { get; set; }

        public IWeapon SecretWeaponAccessor
        {
            get { return this.SecretWeapon; }
            set { this.SecretWeapon = value; }
        }
        
        public IWeapon VerySecretWeaponAccessor
        {
            get { return this.VerySecretWeapon; }
            set { this.VerySecretWeapon = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// Added to have properties with the same name.
        /// </summary>
        /// <value>Allways null.</value>
        public object this[int index]
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// Added to have properties with the same name.
        /// </summary>
        /// <value>Allways null.</value>
        public object this[string index]
        {
            get { return null; }
            set { }
        }
        
        #region IWarrior Members

        public IWeapon Weapon { get; set; }

        #endregion
    }
}