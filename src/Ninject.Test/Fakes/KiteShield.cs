#if !SILVERLIGHT
namespace Ninject.Tests.Integration
{
    using System.Runtime.InteropServices;
    using Ninject.Tests.Fakes;

    public class KiteShield
    {
        public KiteShield([DefaultParameterValue(ShieldColor.Orange)] ShieldColor color)
        {
            this.Color = color;
        }

        public ShieldColor Color { get; set; }
    }
}
#endif