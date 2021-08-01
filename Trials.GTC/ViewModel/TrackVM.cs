using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using Trials.GTC.GlobalTrackCentral;

namespace Trials.GTC.ViewModel
{
    public class TrackVM : INotifyPropertyChanged
    {
        static TrackCentralClient client = new TrackCentralClient();

        private ObservableCollection<GetMessagesResult> messages = new ObservableCollection<GetMessagesResult>();
        public ObservableCollection<GetMessagesResult> Messages
        {

            get
            {
                return messages;
            }
            set
            {
                messages = value;
                this.RaisePropertyChanged("Messages");
            }
        }

        private ObservableCollection<Time> times = new ObservableCollection<Time>();
        public ObservableCollection<Time> Times
        {
            get
            {
                return times;
            }
        }

        public event EventHandler LoadStarted;
        public event EventHandler LoadCompleted;

        private Track track;
        public Track Track
        {
            get
            {
                return this.track;
            }
            set
            {
                this.track = value;

                this.RaisePropertyChanged("Track");
                this.RaisePropertyChanged("ThumbnailUri");
                this.RaisePropertyChanged("HashTag");

            }
        }

        public string ThumbnailUri
        {
            get
            {
                if (this.Track == null)
                    return null;

                if (this.Track.HasThumb)
                    return this.GetThumbUrl();
                else if (!string.IsNullOrEmpty(this.Track.Hyperlink))
                    return this.GetYoutubeThumb();
                else
                    return null;

            }
        }

        private string GetYoutubeThumb()
        {
            var url = this.Track.Hyperlink;
            if (!string.IsNullOrEmpty(url))
            {
                string ytId;

                try
                {
                    if (url.StartsWith("http://youtu.be/"))
                    {
                        ytId = url.Replace("http://youtu.be/", "");
                    }
                    else
                    {
                        var splitedUrl = url.Split(new[] { "v=" }, StringSplitOptions.None);
                        var splitedUrl2 = splitedUrl[1].Split('&');
                        ytId = splitedUrl2[0];
                    }
                }
                catch
                {
                    return null;
                }

                return string.Format("http://img.youtube.com/vi/{0}/0.jpg", ytId);
            }

            return null;
        }

        private string GetThumbUrl()
        {
            string src = Application.Current.Host.Source.ToString();

            //Get the application root, where 'ClientBin' is the known dir where the XAP is 
            string appRoot = src.Substring(0, src.IndexOf("ClientBin")).TrimEnd('/');

            return string.Format("{1}/Services/Thumbnails/{0}.png?r={2}", this.track.LinkId, appRoot, DateTime.Now.TimeOfDay.Ticks);
        }

        public TrackVM()
        {
            client.GetTrackCompleted += new EventHandler<GetTrackCompletedEventArgs>(client_GetTrackCompleted);
            client.GetTimesCompleted += new EventHandler<GetTimesCompletedEventArgs>(client_GetTimesCompleted);
            client.GetMessagesCompleted += new EventHandler<GetMessagesCompletedEventArgs>(client_GetMessagesCompleted);
            client.DeleteTrackCompleted += new EventHandler<DeleteTrackCompletedEventArgs>(client_DeleteTrackCompleted);

        }

        public void LoadData(string linkId)
        {
            client.GetTrackAsync(linkId);
        }

        internal void Reload()
        {
            if (this.track.LinkId != null)
                this.LoadData(this.track.LinkId);
        }

        public void LoadTimes()
        {
            client.GetTimesAsync(this.track.Id);
        }

        public void LoadMessages()
        {
            if (this.track == null)
                return;

            client.GetMessagesAsync(this.track.Id);

            //WebClient client = new WebClient();
            ////tweets = new ObservableCollection<Tweet>();

            //client.DownloadStringCompleted += (s, ea) =>
            //{
            //    try
            //    {
            //        XDocument doc = XDocument.Parse(ea.Result, LoadOptions.PreserveWhitespace);
            //        XNamespace ns = "http://www.w3.org/2005/Atom";


            //        var items = from item in doc.Descendants(ns + "entry")
            //                    select new Tweet()
            //                    {
            //                        Author = item.Element(ns + "author").Element(ns + "name").Value,
            //                        Title = item.Element(ns + "title").Value.Replace("#gtc" + this.track.LinkId, string.Empty).Trim(),
            //                        Image = new Uri((from XElement xe in item.Descendants(ns + "link")
            //                                         where xe.Attribute("type").Value == "image/png"
            //                                         select xe.Attribute("href").Value).First<string>()),

            //                        Link = new Uri((from XElement xe in item.Descendants(ns + "link")
            //                                        where xe.Attribute("type").Value == "text/html"
            //                                        select xe.Attribute("href").Value).First<string>()),
            //                    };

            //        foreach (Tweet t in items)
            //        {
            //            tweets.Add(t);
            //        }

            //        this.RaisePropertyChanged("Tweets");
            //    }
            //    catch
            //    {

            //    }
            //};


            //client.DownloadStringAsync(new Uri("http://search.twitter.com/search.atom?q=%23gtc" + this.track.LinkId));
        }

        internal void DeleteTrack()
        {
            var result = MessageBox.Show("Are you sure you want to delete this track?", "Delete this track?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                client.DeleteTrackAsync(this.track.Id);
        }

        void client_GetTrackCompleted(object sender, GetTrackCompletedEventArgs e)
        {
            this.Track = e.Result;

            this.RaiseLoadCompleted();
            this.LoadMessages();
            this.LoadTimes();
        }

        void client_GetTimesCompleted(object sender, GetTimesCompletedEventArgs e)
        {
            if (this.track.TimeUltimate != null && this.track.TimeUltimate.Value.Ticks > 0 &&
                this.track.TimeGold != null && this.track.TimeGold.Value.Ticks > 0 &&
                this.track.TimePlatinum != null && this.track.TimePlatinum.Value.Ticks > 0 &&
                this.track.TimeSilver != null && this.track.TimeSilver.Value.Ticks > 0 &&
                this.track.ScoreCoefficient.HasValue && this.track.ScoreCoefficient.Value > 0)
            {
                foreach (var t in e.Result)
                    t.CalculateTime(this.Track);
            }

            var times1 = from t in e.Result
                         orderby t.Points descending
                         orderby t.Time1 ascending
                         select t;

            this.times.Clear();
            foreach (var t in times1)
                this.times.Add(t);

            this.RaisePropertyChanged("Times");
        }

        void client_GetMessagesCompleted(object sender, GetMessagesCompletedEventArgs e)
        {
            this.Messages = new ObservableCollection<GetMessagesResult>(e.Result);
        }

        void client_DeleteTrackCompleted(object sender, DeleteTrackCompletedEventArgs e)
        {
            var uri = new Uri("/All", UriKind.RelativeOrAbsolute);
            App.ContentFrame.Navigate(uri);
        }

        private void RaiseLoadCompleted()
        {
            if (this.LoadCompleted != null)
                this.LoadCompleted(this, null);
        }

        private void RaiseLoadStarted()
        {
            if (this.LoadStarted != null)
                this.LoadStarted(this, null);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
