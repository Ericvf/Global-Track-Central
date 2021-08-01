using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using AnimationExtensions;

namespace Trials.GTC.Framework
{
    /// <summary>
    /// Provides attached properties for adding a 'tilt' effect to all controls within a container
    /// </summary>
    public class MenuButtonEffect : DependencyObject
    {

        /// <summary>
        /// Whether the tilt effect is enabled on a container (and all its children)
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabledProperty", typeof(bool), typeof(MenuButtonEffect), new PropertyMetadata(OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject source)
        {
            return (bool)source.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject source, bool value)
        {
            source.SetValue(IsEnabledProperty, value);
        }

        //static MenuButtonEffect()
        //{

        //}

        /// <summary>
        /// Add / Remove event handlers from the element that has (un)registered for tilting
        /// </summary>
        /// <param name="target">The element that the property is atteched to</param>
        /// <param name="args">Event args</param>
        static void OnIsEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (target is UIElement)
            {
                var t = target as UIElement;

                if ((bool)args.NewValue)
                {
                    t.MouseEnter += t_MouseEnter;
                    t.MouseLeave += t_MouseLeave;
                    t.MouseLeftButtonUp += t_MouseLeftButtonUp;
                }
                else
                {
                    t.MouseEnter -= t_MouseEnter;
                    t.MouseLeave -= t_MouseLeave;
                    t.MouseLeftButtonUp -= t_MouseLeftButtonUp;
                }
            }
        }

        static void t_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.Move(y: -10)
                .Move(0, 0, 750, Eq.OutElastic)
                .Play();
        }

        static void t_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.Move(y: 5, duration: 500, eq: Eq.OutSine)
                .Play();
        }

        static void t_MouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.Move(y: -10)
                .Move(0, 0, 750, Eq.OutElastic)
                .Play();

        }
    }
}
