using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Trials.GTC.Framework;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.UserControls;
using System.Windows.Data;
using System.Diagnostics;

namespace Trials.GTC.ViewModel
{
    public class TracksVM : INotifyPropertyChanged
    {
        public event EventHandler LoadStarted;
        public event EventHandler LoadCompleted;
        //public event EventHandler ShowSearchEvent;
        //public event EventHandler HideSearchEvent;

        private static TrackCentralClient client;

        private bool showingSearch;
        public bool ShowingSearch
        {
            get
            {
                return this.showingSearch;
            }
            set
            {
                this.showingSearch = value;
                this.RaisePropertyChanged("ShowingSearch");
            }
        }

        private SearchVM searchVM = new SearchVM();
        public SearchVM Search
        {
            get
            {
                return this.searchVM;
            }
            set
            {
                this.searchVM = value;
                this.RaisePropertyChanged("Search");
            }
        }

        private ObservableCollection<Track> tracks = new ObservableCollection<Track>();
        public ObservableCollection<Track> Tracks
        {
            get
            {
                return this.tracks;
            }
            set
            {
                this.tracksView = null;
                this.tracks = value;
                this.RaisePropertyChanged("Tracks");
                this.RaisePropertyChanged("TracksView");
            }
        }

        private CollectionViewSource tracksView;
        public ICollectionView TracksView
        {
            get
            {
                if (tracksView == null)
                {
                    var sortName = this.SortName;
                    switch (sortName)
                    {
                        case "Version":
                            sortName = "TrialsVersion";
                            break;
                        case "Type":
                            sortName = "TrialsType";
                            break;
                        case "Submitted":
                            sortName = "SubmittedOn";
                            break;
                        case "SubmittedBy":
                            sortName = "SubmittedBy";
                            break;
                    }

                    tracksView = new CollectionViewSource();
                    tracksView.SortDescriptions.Clear();
                    tracksView.SortDescriptions.Add(new SortDescription(sortName, this.SortDirection ? ListSortDirection.Ascending : ListSortDirection.Descending));
                    tracksView.Source = new ObservableCollection<Track>(tracks);
                }

                return tracksView.View;
            }
        }

        private int resultsPerPage = 16;
        private int page = 1;
        public int Page
        {
            get
            {
                return this.page;
            }
            set
            {
                this.page = value;
                this.RaisePropertyChanged(@"Page");
                this.RaisePropertyChanged(@"HasPages");

            }
        }

        private int totalPages = 0;
        public int TotalPages
        {
            get
            {
                return this.totalPages;
            }
            set
            {
                if (value != this.totalPages)
                {
                    this.totalPages = value;
                    this.RaisePropertyChanged(@"HasPages");
                    this.RaisePropertyChanged(@"TotalPages");
                }
            }
        }

        private int totalItems = 0;
        public int TotalItems
        {
            get
            {
                return this.totalItems;
            }
            set
            {
                if (this.totalItems != value)
                {
                    this.totalItems = value;
                    this.RaisePropertyChanged(@"TotalItems");
                    this.RaisePropertyChanged(@"HasItems");
                }
            }
        }

        public bool HasPages
        {
            get
            {
                return this.totalPages > 1;
            }
        }
        public bool HasItems
        {
            get
            {
                return this.totalItems > 0;
            }
        }

        private bool isLoading = true;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }

        private bool isFilter;
        public bool IsFilter
        {
            get
            {
                return this.isFilter;
            }
            set
            {
                this.isFilter = value;
                this.RaisePropertyChanged("IsFilter");
            }
        }
        
        public string SortName { get; set; }
        public bool SortDirection { get; set; }

        public TracksVM()
        {
            this.InitCommands();

            client = new GlobalTrackCentral.TrackCentralClient();
            client.GetTracksCompleted += new EventHandler<GetTracksCompletedEventArgs>(client_GetTracksCompleted);

            this.Search.Tags = App.tags;
            
            this.SortName = "Submitted";
            

            //client.GetTagsCompleted += new EventHandler<GetTagsCompletedEventArgs>(client_GetTagsCompleted);
        }

        #region Commands

        private ActionCommand<string> _creatorCommand;
        public ActionCommand<string> CreatorCommand
        {
            get
            {
                return this._creatorCommand;
            }
            set
            {
                this._creatorCommand = value;
            }
        }

        public ActionCommand _firstPageCommand;
        public ActionCommand FirstPageCommand
        {
            get
            {
                return this._firstPageCommand;
            }
        }

        public ActionCommand _prevPageCommand;
        public ActionCommand PrevPageCommand
        {
            get
            {
                return this._prevPageCommand;
            }
        }

        public ActionCommand _nextPageCommand;
        public ActionCommand NextPageCommand
        {
            get
            {
                return this._nextPageCommand;
            }
        }

        public ActionCommand _lastPageCommand;
        public ActionCommand LastPageCommand
        {
            get
            {
                return this._lastPageCommand;
            }
        }

        public ActionCommand _refreshPageCommand;
        public ActionCommand RefreshPageCommand
        {
            get
            {
                return this._refreshPageCommand;
            }
        }

        private ActionCommand<string> _searchPageCommand;
        public ActionCommand<string> SearchPageCommand
        {
            get
            {
                return this._searchPageCommand;
            }
            set
            {
                this._searchPageCommand = value;
            }
        }

        private ActionCommand<string> _updatePageCommand;
        public ActionCommand<string> UpdatePageCommand
        {
            get
            {
                return this._updatePageCommand;
            }
            set
            {
                this._updatePageCommand = value;
            }
        }

        private ActionCommand _searchCommand;
        public ActionCommand SearchCommand
        {
            get
            {
                return this._searchCommand;
            }
            set
            {
                this._searchCommand = value;
            }
        }

        private ActionCommand _searchResetCommand;
        public ActionCommand SearchResetCommand
        {
            get
            {
                return this._searchResetCommand;
            }
            set
            {
                this._searchResetCommand = value;
            }
        }

        public void CommandRefreshPage_Execute()
        {
            this.LoadData();
        }
        public void CommandFirstPage_Execute()
        {
            this.Page = 1;
            this.LoadData();
        }
        public void CommandPrevPage_Execute()
        {
            this.Page = Math.Max(1, this.Page - 1);
            this.LoadData();
        }
        public void CommandNextPage_Execute()
        {
            this.Page = Math.Min(this.totalPages, this.Page + 1);
            this.LoadData();
        }
        public void CommandLastPage_Execute()
        {
            this.Page = this.totalPages;
            this.LoadData();
        }

        public void CommandSearchReset_Execute()
        {
           // this.HideSearch();
            this.Reset();
            
            this.LoadData();


           
        }
        public void CommandSearch_Execute()
        {
          //  this.HideSearch();
            this.Page = 1;
            this.LoadData();
        }

        public void CommandSearchPage_Execute(string searchQuery)
        {
            this.Search.Keyword = searchQuery;
            this.Page = 1;
            this.LoadData();
        }
        public void CommandUpdatePage_Execute(string page)
        {
            int pageId = this.Page;
            if (int.TryParse(page, out pageId))
            {
                this.Page = Math.Max(1, Math.Min(this.totalPages, pageId));
                this.LoadData();
            }
        }
        public void CommandCreator_Execute(string creator)
        {
            this.Search.Creator = creator;
        }

        private void InitCommands()
        {
            this._creatorCommand = new ActionCommand<string>(this.CommandCreator_Execute);
            this._searchCommand = new ActionCommand(this.CommandSearch_Execute);

            this._firstPageCommand = new ActionCommand(this.CommandFirstPage_Execute);
            this._prevPageCommand = new ActionCommand(this.CommandPrevPage_Execute);
            this._nextPageCommand = new ActionCommand(this.CommandNextPage_Execute);
            this._lastPageCommand = new ActionCommand(this.CommandLastPage_Execute);
            this._refreshPageCommand = new ActionCommand(this.CommandRefreshPage_Execute);
            this._searchPageCommand = new ActionCommand<string>(this.CommandSearchPage_Execute);
            this._searchResetCommand = new ActionCommand(this.CommandSearchReset_Execute);
            this._updatePageCommand = new ActionCommand<string>(this.CommandUpdatePage_Execute);
        }

        #endregion

        public void CommandTrackDetails_Execute(Track track)
        {
            //ViewModelLocator.TrackVM.LoadData(track);
        }

        void client_GetTracksCompleted(object sender, GetTracksCompletedEventArgs e)
        {
            this.Tracks = new ObservableCollection<Track>(e.Result.Tracks);
            this.TotalPages = e.Result.TotalPages;
            this.TotalItems = e.Result.TotalItems;
            this.IsFilter = e.Result.IsFilter;

            if (this.LoadCompleted != null)
                this.LoadCompleted(this, null);

            this.IsLoading = false;

        }
      
        public void LoadData()
        {

            if (this.LoadStarted != null)
                this.LoadStarted(this, null);


            var guids = new ObservableCollection<Guid>();
            if (this.Search.Tags != null)
            {
                foreach (var t in this.Search.Tags)
                    if (t.Checked)
                        guids.Add(t.Id);
            }


            client.GetTracksAsync(this.Page, resultsPerPage,
                this.Search.TrialsVersion.Id,
                this.Search.TrialsType.Id,
                this.Search.Difficulty.Id,
                this.Search.Creator,
                this.Search.Keyword,
                this.SortName,
                this.SortDirection,
                guids);


            this.IsLoading = true;
        }

        #region INotifyPropertyChanged
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        internal void ShowSearch()
        {
            var window = new SearchWindow();
            window.Show();
        }

        internal void Reset()
        {
            this.Search.Reset();
            this.Page = 1;
        }

        internal void UpdateSort(string sortName)
        {
            if (sortName == "Submitted By")
                sortName = "SubmittedBy";

            if (this.SortName != sortName)
            {
                this.SortName = sortName;
                this.SortDirection = true;
            }
            else
            {
                this.SortDirection = !this.SortDirection;
            }
           
            this.LoadData();
        }
    }
}
