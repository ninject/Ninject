#if !SILVERLIGHT
namespace Ninject.Tests.Fakes
{
    using System.Runtime.InteropServices;

    public enum ShieldColor
    {
        Red,
        Green,
        Blue, 
        Orange,
    }

    public class Shield
    {
        public Shield([DefaultParameterValue(ShieldColor.Red)] ShieldColor color)
        {
            this.Color = color;
        }

        public ShieldColor Color { get; set; }
    }
}
#endif