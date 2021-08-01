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
using Trials.GTC.Framework;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.ViewModel;

namespace Trials.GTC.Commands
{
    public class TrackDetailsCommand: ActionCommand<Track>
    {
        public TrackDetailsCommand()
            : base(null)
        {

        }

        public TrackDetailsCommand(Action<Track> spotAction)
            : base(spotAction)
        {

        }

        public override void Execute(object track)
        {
            //if (track != null && track is Track)
            //    ViewModelLocator.TrackVM.LoadData(track as Track);
        }
    }

    public class SearchAuthorCommand : ActionCommand<Track>
    {
        public SearchAuthorCommand()
            : base(null)
        {

        }

        public SearchAuthorCommand(Action<Track> spotAction)
            : base(spotAction)
        {

        }

        public override void Execute(object track)
        {
            ViewModelLocator.TracksVM.Search.Creator = (string)track;
            ViewModelLocator.TracksVM.ShowSearch();
        }
    }
}
