using System.Windows;
using AnimationExtensions;

namespace Trials.GTC.AnimationExtensions
{
    public static class Extensions
    {
        public static Prototype SlideIn(this FrameworkElement element, double duration = 350)
        {
            return element.Move(0, -20)
                .Move(0, 0, duration, Eq.OutSine)
                .Fade(1, duration);
        }
    }

}
