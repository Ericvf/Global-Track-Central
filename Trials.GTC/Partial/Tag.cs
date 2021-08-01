using System;
using System.ComponentModel;

namespace Trials.GTC.GlobalTrackCentral
{
    public partial class Time : INotifyPropertyChanged
    {
        public double Points { get; set; }

        internal void CalculateTime(Track track)
        {
            var time = this.Time1.TotalMilliseconds;
            double scorepct;
            double score;

            var TIMESILVER = track.TimeSilver.Value.TotalMilliseconds;
            var TIMEGOLD = track.TimeGold.Value.TotalMilliseconds;
            var TIMEPLAT = track.TimePlatinum.Value.TotalMilliseconds;
            var TIME100 = track.TimeUltimate.Value.TotalMilliseconds;
            var SCORECOEFF = track.ScoreCoefficient.Value;

            if (time > TIMESILVER)
            {
                // a bronze time
                scorepct = 0.25 * Math.Exp((TIMESILVER - time) / (TIMESILVER - TIMEPLAT));
            }
            else if (time > TIMEGOLD)
            {
                // a silver time
                scorepct = 0.25 + 0.25 * (TIMESILVER - time) / (TIMESILVER - TIMEGOLD);
            }
            else if (time > TIMEPLAT)
            {
                // a gold time
                scorepct = 0.5 + 0.25 * (TIMEGOLD - time) / (TIMEGOLD - TIMEPLAT);
            }
            else if (time > TIME100)
            {
                // a platinum time
                scorepct = 0.75 + 0.25 * (TIMEPLAT - time) / (TIMEPLAT - TIME100);
            }
            else
            {
                // a >100% time
                scorepct = 1.0 + 0.25 * (TIMEPLAT - time) / TIMEPLAT;
            }

            if (this.Faults > 6)
            {
                score = Math.Max(60000 - (this.Faults - 6) * 100, 10000) + 100 * scorepct;
            }
            else
            {
                score = 60000 + 300000 * Math.Pow(SCORECOEFF,this.Faults) * ((1 - SCORECOEFF) * scorepct + SCORECOEFF);
            }

            this.Points = Math.Ceiling(score);
        }
    }

    public partial class Tag : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool Checked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.RaisePropertyChanged("Checked");
            }
        }
    }
}
