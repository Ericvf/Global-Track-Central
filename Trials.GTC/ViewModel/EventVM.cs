using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.Framework;
using System;

namespace Trials.GTC.ViewModel
{
    public class EventVM : INotifyPropertyChanged
    {
        private static TrackCentralClient client= new TrackCentralClient();

        public struct Rider
        {
            public string Name {get;set;}
            public double Points { get; set; }
            public TimeSpan Time { get; set; }
        }

        private string title;
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                this.RaisePropertyChanged("Title");
            }
        }

        private ObservableCollection<EventSummaryResult> summary;
        public ObservableCollection<EventSummaryResult> Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                this.summary = value;
                this.RaisePropertyChanged("Summary");
            }
        }

        private ObservableCollection<Rider> riders;
        public ObservableCollection<Rider> Riders
        {
            get
            {
                return this.riders;
            }
            set
            {
                this.riders = value;
                this.RaisePropertyChanged("Riders");
            }
        }

        public EventVM()
        {
            client.GetEventSummaryCompleted += new System.EventHandler<GetEventSummaryCompletedEventArgs>(client_GetEventSummaryCompleted);
        }

        void client_GetEventSummaryCompleted(object sender, GetEventSummaryCompletedEventArgs e)
        {
            this.Summary = new ObservableCollection<EventSummaryResult>(e.Result);
            var riders = (from r in e.Result
                          group r by r.Rider into g
                          select new Rider()
                          {
                              Name = g.Key,
                              Time = new TimeSpan(g.Sum((es) => ((TimeSpan)es.Time).Ticks)),
                              Points = (double)g.Sum((es) => es.Points)
                          }
                          )
                          .OrderBy(r => r.Time)
                          .OrderByDescending(r => r.Points)
                          .Where(r => !string.IsNullOrEmpty(r.Name));

            this.Riders = new ObservableCollection<Rider>(riders);
        }
       
        internal void LoadData(Tag tag)
        {
            this.Title = tag.Name;
            client.GetEventSummaryAsync(tag.Id);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
