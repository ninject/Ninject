namespace Ninject.Tests.Fakes
{
    using System;

    public class Sword : IWeapon
    {
        public string Name
        {
            get { return "sword"; }
        }
    }
}