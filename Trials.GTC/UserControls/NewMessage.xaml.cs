using System;
using System.Windows;
using System.Windows.Controls;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.ViewModel;

namespace Trials.GTC.UserControls
{
    public partial class NewMessage : ChildWindow
    {
        private Guid trackId;

        public NewMessage(Guid trackId)
        {
            InitializeComponent();
            this.trackId = trackId;
        }
    
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbBody.Text))
                return;

            var userId = (Guid)ViewModelLocator.UserVM.Id;
            var client = new TrackCentralClient();

            client.NewMessageCompleted += (s, re) =>
                {
                    ViewModelLocator.TrackVM.LoadMessages();
                    this.Close();
                };
            client.NewMessageAsync(userId, trackId, this.tbBody.Text);
        }
    }
}

