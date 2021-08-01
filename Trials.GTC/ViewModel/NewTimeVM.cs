using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Trials.GTC.Framework;
using Trials.GTC.GlobalTrackCentral;
using System.Security.Cryptography;
using System;
using System.Globalization;

namespace Trials.GTC.ViewModel
{
    public class NewTimeVM : INotifyPropertyChanged
    {
        private static TrackCentralClient client;

        public event EventHandler Completed;

        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }
        public string LinkId { get; set; }
        private string rider;
        public string Rider
        {
            get
            {
                return this.rider;
            }
            set
            {
                this.rider = value;
                this.RaisePropertyChanged("Rider");
            }
        }

        private string time;
        public string Time
        {
            get
            {
                return this.time;
            }
            set
            {
                this.time = value;
                this.RaisePropertyChanged("Time");
            }
        }

        private int faults;
        public int Faults
        {
            get
            {
                return this.faults;
            }
            set
            {
                this.faults = value;
                this.RaisePropertyChanged("Faults");
            }
        }

        private string link;
        public string Link
        {
            get
            {
                return this.link;
            }
            set
            {
                this.link = value;
                this.RaisePropertyChanged("Link");
            }
        }

        public NewTimeVM()
        {
            client = new TrackCentralClient();
            client.SubmitTimeCompleted += new EventHandler<SubmitTimeCompletedEventArgs>(client_SubmitTimeCompleted);
        }

        void client_SubmitTimeCompleted(object sender, SubmitTimeCompletedEventArgs e)
        {
            if (this.Completed != null)
                this.Completed(sender, e);
        }

        internal void Reset(Track track)
        {
            this.TrackId = track.Id;
            
            this.LinkId = track.LinkId;
            this.Rider = string.Empty;
            this.Time = new TimeSpan().ToString();
            this.Faults = 0;
            this.Link = string.Empty;

            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                this.UserId = (Guid)ViewModelLocator.UserVM.Id;
                this.Rider = ViewModelLocator.UserVM.UserName;
            }
        }

        internal void Submit()
        {
            client.SubmitTimeAsync(this.UserId, this.TrackId, this.LinkId, this.Rider.Trim(), TimeSpan.Parse(this.Time), this.Faults, this.Link.Trim());
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;



    }
}
