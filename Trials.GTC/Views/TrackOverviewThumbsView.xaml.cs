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
using Trials.GTC.GlobalTrackCentral;

namespace Trials.GTC.Views
{
    public partial class TrackOverviewThumbsView : UserControl
    {
        private TrackOverviewVM VM = ViewModelLocator.TracksViewModel;
        public event EventHandler Navigate;

        public TrackOverviewThumbsView()
        {
            InitializeComponent();

            this.DataContext = VM;

            Loaded += new RoutedEventHandler(TrackOverviewView_Loaded);
        }

        void TrackOverviewView_Loaded(object sender, RoutedEventArgs e)
        {
           // this.VM.LoadData(null);
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border.DataContext != null && border.Tag is Track)
            {
                ViewModelLocator.TrackDetailsViewModel.LoadData(border.Tag as Track);
            }

            if (this.Navigate != null)
                this.Navigate(this, null);
        }

    }
}
