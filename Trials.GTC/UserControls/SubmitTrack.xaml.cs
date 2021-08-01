using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Trials.GTC.ViewModel;

namespace Trials.GTC.UserControls
{
    public partial class SubmitTrack : UserControl
    {
        public SubmitTrack()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(SubmitTrack_Loaded);
        }

        void SubmitTrack_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new SubmitTrackVM();
        }
    }
}
