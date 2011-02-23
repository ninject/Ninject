#if NET_40
namespace Ninject.Tests.Fakes
{
    public enum ShieldColor
    {
        Red,
        Green,
        Blue, 
        Orange,
    }

    public class Shield
    {
        public Shield(ShieldColor color = ShieldColor.Red)
        {
            Color = color;
        }

        public ShieldColor Color { get; set; }
    }
}
#endif