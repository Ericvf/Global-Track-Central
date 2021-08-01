using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.Framework;

namespace Trials.GTC.ViewModel
{
    public class SearchVM : INotifyPropertyChanged
    {
        private List<App.TrialsVersion> trialsVersions;
        public List<App.TrialsVersion> TrialsVersions
        {
            get
            {
                if (this.trialsVersions == null)
                {
                    this.trialsVersions = new List<App.TrialsVersion>();
                    this.trialsVersions.Add(new App.TrialsVersion() { Id = -1, Name = @"All" });
                    this.trialsVersions.AddRange(App.trialsVersions);
                }

                return this.trialsVersions;
            }
        }

        private List<App.TrialsVersion> trialsTypes;
        public List<App.TrialsVersion> TrialsTypes
        {
            get
            {
                if (this.trialsTypes == null)
                {
                    this.trialsTypes = new List<App.TrialsVersion>();
                    this.trialsTypes.Add(new App.TrialsVersion() { Id = -1, Name = @"All" });
                    this.trialsTypes.AddRange(App.trialsTypes);
                }

                return this.trialsTypes;
            }
        }

        private List<App.TrialsVersion> difficulties;
        public List<App.TrialsVersion> Difficulties
        {
            get
            {
                if (this.difficulties == null)
                {
                    this.difficulties = new List<App.TrialsVersion>();
                    this.difficulties.Add(new App.TrialsVersion() { Id = -1, Name = @"All" });
                    this.difficulties.AddRange(App.difficulties);
                }

                return this.difficulties;
            }
        }

        public SearchVM()
        {
            this.TrialsVersion = TrialsVersions.FirstOrDefault();
            this.TrialsType = TrialsTypes.FirstOrDefault();
            this.Difficulty = Difficulties.FirstOrDefault();
        }

        private App.TrialsVersion trialsVersion;
        public App.TrialsVersion TrialsVersion
        {
            get
            {
                return this.trialsVersion;
            }
            set
            {
                this.trialsVersion = value;
                this.RaisePropertyChanged("TrialsVersion");
            }
        }

        private App.TrialsVersion trialsType;
        public App.TrialsVersion TrialsType
        {
            get
            {
                return this.trialsType;
            }
            set
            {
                this.trialsType = value;
                this.RaisePropertyChanged("TrialsType");
            }
        }

        private App.TrialsVersion difficulty;
        public App.TrialsVersion Difficulty
        {
            get
            {
                return this.difficulty;
            }
            set
            {
                this.difficulty = value;
                this.RaisePropertyChanged("Difficulty");
            }
        }

        private string creator;
        public string Creator
        {
            get
            {
                return this.creator;
            }
            set
            {
                this.creator = value;
                this.RaisePropertyChanged("Creator");
            }
        }

        private string keyword;
        public string Keyword
        {
            get
            {
                return this.keyword;
            }
            set
            {
                this.keyword = value;
                this.RaisePropertyChanged("Keyword");
            }
        }

        private ObservableCollection<Tag> tags;
        public ObservableCollection<Tag> Tags
        {
            get
            {
                return this.tags;
            }
            set
            {
                this.tags = value;
                this.RaisePropertyChanged("Tags");
            }
        }

        internal void Reset()
        {
            this.TrialsVersion = TrialsVersions.FirstOrDefault();
            this.TrialsType = TrialsTypes.FirstOrDefault();
            this.Difficulty = Difficulties.FirstOrDefault();

            this.Creator = string.Empty;
            this.Keyword = string.Empty;

            if (this.Tags != null)
            {
                foreach (var tag in this.Tags)
                    tag.Checked = false;
            }

        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
