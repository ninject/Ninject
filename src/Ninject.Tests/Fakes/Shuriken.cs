namespace Ninject.Tests.Fakes
{
    using System;

    public class Shuriken : IWeapon
    {
        public string Name
        {
            get { return "shuriken"; }
        }
    }
}