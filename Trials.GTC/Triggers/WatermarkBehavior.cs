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
using System.Windows.Interactivity;

namespace Trials.GTC.Triggers
{
    public class WatermarkBehavior : Behavior<TextBox>
    {
        private bool _hasWatermark; 
        private Brush _textBoxForeground;
        public String Text { get; set; }
        public Brush Foreground { get; set; }
        protected override void OnAttached()
        {
            _textBoxForeground = AssociatedObject.Foreground;
            base.OnAttached();
            if (Text != null)
                SetWatermarkText();
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
            AssociatedObject.Loaded += Loaded;
        }

        void Loaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Text.Length == 0)
                if (Text != null)
                    SetWatermarkText();
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Text.Length == 0)
                if (Text != null)
                    SetWatermarkText();
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (_hasWatermark)
                RemoveWatermarkText();
        }

        private void RemoveWatermarkText()
        {
            AssociatedObject.Foreground = _textBoxForeground;
            AssociatedObject.Text = "";
            AssociatedObject.FontStyle = FontStyles.Normal;
            _hasWatermark = false;
        }

        private void SetWatermarkText()
        {
            AssociatedObject.FontStyle = FontStyles.Italic;
            AssociatedObject.Foreground = Foreground;
            AssociatedObject.Text = Text;
            _hasWatermark = true;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
        }
    }
}
