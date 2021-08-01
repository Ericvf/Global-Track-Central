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

namespace Trials.GTC.ViewModel
{
    public class ViewModelLocator
    {
        private static TracksVM tracksVM;
        public static TracksVM TracksVM
        {
            get
            {
                if (tracksVM == null)
                    tracksVM = new TracksVM();

                return tracksVM;
            }
        }

        private static TrackVM trackVM;
        public static TrackVM TrackVM
        {
            get
            {
                if (trackVM == null)
                    trackVM = new TrackVM();

                return trackVM;
            }
        }

        private static NewTrackVM newTrackVM;
        public static NewTrackVM NewTrackVM
        {
            get
            {
                if (newTrackVM == null)
                    newTrackVM = new NewTrackVM();

                return newTrackVM;
            }
        }

        private static NewTimeVM newTimeVM;
        public static NewTimeVM NewTimeVM
        {
            get
            {
                if (newTimeVM == null)
                    newTimeVM = new NewTimeVM();

                return newTimeVM;
            }
        }

        private static UserVM userVM;
        public static UserVM UserVM
        {
            get
            {
                if (userVM == null)
                    userVM = new UserVM();

                return userVM;
            }
        }
    }
}
