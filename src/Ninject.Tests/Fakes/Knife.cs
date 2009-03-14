namespace Ninject.Tests.Fakes
{
    public class Knife : IWeapon

    {
        private readonly string _name;

        public Knife(string name)
        {
            _name = name;
        }

        #region Implementation of IWeapon

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}