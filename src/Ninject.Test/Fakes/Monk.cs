namespace Ninject.Tests.Fakes
{
    public class Monk : IWarrior, ICleric
    {
        public IWeapon Weapon
        {
            get
            {
                return null;
            }
        }
    }

    public interface ICleric
    {
    }
}