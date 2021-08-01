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
    public class NewTrackVM : INotifyPropertyChanged
    {
        private static TrackCentralClient client;

        public event EventHandler NewTrackCompleted;

        public List<App.TrialsVersion> TrialsVersions
        {
            get
            {
                return App.trialsVersions;
            }
        }
        public List<App.TrialsVersion> TrialsTypes
        {
            get
            {
                return App.trialsTypes;
            }
        }
        public List<App.TrialsVersion> Difficulties
        {
            get
            {
                return App.difficulties;
            }
        }

        public NewTrackVM()
        {
            client = new TrackCentralClient();
            client.NewTrackCompleted += new EventHandler<NewTrackCompletedEventArgs>(client_NewTrackCompleted);

            this.trialsVersion = App.trialsVersions.FirstOrDefault();
            this.difficulty = App.difficulties.FirstOrDefault();
            this.trialsType = App.trialsTypes.FirstOrDefault();

            this.tags = new ObservableCollection<Tag>();
            this.events = new ObservableCollection<Tag>();

            foreach (var tag in App.tags)
            {
                if (tag.IsCompetition)
                {
                    this.events.Add(new Tag()
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                    });
                }
                else
                {
                    this.tags.Add(new Tag()
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                    });
                }
            }

            this.InitCommands();
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

        private DateTime created = DateTime.UtcNow;
        public DateTime Created
        {
            get
            {
                return this.created;
            }
            set
            {
                this.created = value;
                this.RaisePropertyChanged("Created");
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                this.RaisePropertyChanged("Description");
            }
        }

        private string hyperlink;
        public string Hyperlink
        {
            get
            {
                return this.hyperlink;
            }
            set
            {
                this.hyperlink = value;
                this.RaisePropertyChanged("Hyperlink");
            }
        }

        private string linkId;
        public string LinkId
        {
            get
            {
                return this.linkId;
            }
            set
            {
                this.linkId = value;
                this.RaisePropertyChanged("LinkId");
            }
        }

        private byte[] fileStream;
        public byte[] FileStream
        {
            get
            {
                return this.fileStream;
            }
            set
            {
                this.fileStream = value;
                this.RaisePropertyChanged("FileStream");
            }
        }

        private double rating = 5f;
        public double Rating
        {
            get
            {
                return this.rating;
            }
            set
            {
                this.rating = value;
                this.RaisePropertyChanged("Rating");
            }
        }

        private string timeUltimate;
        public string TimeUltimate
        {
            get
            {
                return this.timeUltimate;
            }
            set
            {
                this.timeUltimate = value;
                this.RaisePropertyChanged("TimeUltimate");
            }
        }

        private string timePlat;
        public string TimePlatinum
        {
            get
            {
                return this.timePlat;
            }
            set
            {
                this.timePlat = value;
                this.RaisePropertyChanged("TimePlatinum");
            }
        }

        private string timeGold;
        public string TimeGold
        {
            get
            {
                return this.timeGold;
            }
            set
            {
                this.timeGold = value;
                this.RaisePropertyChanged("TimeGold");
            }
        }

        private string timeSilver;
        public string TimeSilver
        {
            get
            {
                return this.timeSilver;
            }
            set
            {
                this.timeSilver = value;
                this.RaisePropertyChanged("TimeSilver");
            }
        }

        private string scoreCoefficient;
        public string ScoreCoefficient
        {
            get
            {
                return this.scoreCoefficient;
            }
            set
            {
                this.scoreCoefficient = value;
                this.RaisePropertyChanged("ScoreCoefficient");
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


        private ObservableCollection<Tag> events;
        public ObservableCollection<Tag> Events
        {
            get
            {
                return this.events;
            }
            set
            {
                this.events = value;
                this.RaisePropertyChanged("Events");
            }
        }


        #region Commands

        private BitmapImage thumbnail;
        public BitmapImage Thumbnail
        {
            get
            {
                return this.thumbnail;
            }
            set
            {
                this.thumbnail = value;
                this.RaisePropertyChanged("Thumbnail");
            }
        }

        private ActionCommand _cancelCommand;
        public ActionCommand CancelCommand
        {
            get
            {
                return this._cancelCommand;
            }
            set
            {
                this._cancelCommand = value;
            }
        }

        private ActionCommand _submitCommand;
        public ActionCommand SubmitCommand
        {
            get
            {
                return this._submitCommand;
            }
            set
            {
                this._submitCommand = value;
            }
        }


        private ActionCommand _thumbnailCommand;
        public ActionCommand ThumbnailCommand
        {
            get
            {
                return this._thumbnailCommand;
            }
            set
            {
                this._thumbnailCommand = value;
            }
        }


        private void InitCommands()
        {
            this._cancelCommand = new ActionCommand(this.CommandCancel_Execute);
            this._submitCommand = new ActionCommand(this.CommandSubmit_Execute);
            this._thumbnailCommand = new ActionCommand(this.CommandThumbnail_Execute);
        }

        public void CommandCancel_Execute()
        {
            this.Reset();
        }

        public void CommandSubmit_Execute()
        {
            this.Submit();
        }


        void client_NewTrackCompleted(object sender, NewTrackCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
                return;

            }
            if (this.NewTrackCompleted != null)
                this.NewTrackCompleted(sender, e);
        }

        public void CommandThumbnail_Execute()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = @"png (*.png)|*.png|jpg(*.jpg)|*.jpg;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                FileStream reader = dlg.File.OpenRead();

                byte[] bytes = new byte[reader.Length];
                reader.Read(bytes, 0, (int)reader.Length);

                if (bytes.Length > 1024 * 300)
                {
                    MessageBox.Show("The thumbnail you have selected is too large, please keep the filesize under 300kb!");
                    return;
                }

                BitmapImage img = new BitmapImage();
                img.SetSource(reader);

                this.FileStream = bytes;
                this.Thumbnail = img;
            }
        }

        #endregion

        private static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        internal void LoadTrack(Track track)
        {
            this.LinkId = track.LinkId;
            
            this.Title = track.Name;
            this.Description = track.Description;
            this.TrialsVersion = App.trialsVersions.FirstOrDefault(f => f.Id == track.TrialsVersion);
            this.TrialsType = App.trialsTypes.FirstOrDefault(f => f.Id == track.TrialsType);
            this.Difficulty = App.difficulties.FirstOrDefault(f => f.Id == track.Difficulty);
            this.Rating = (double)track.Rating;
            this.Creator = track.Creator;
            this.Created = track.Created;
            this.Hyperlink = track.Hyperlink;
            this.TimeUltimate = track.TimeUltimate.ToString();
            this.TimePlatinum = track.TimePlatinum.ToString();
            this.TimeGold = track.TimeGold.ToString();
            this.TimeSilver = track.TimeSilver.ToString();

            if (track.ScoreCoefficient.HasValue)
                this.ScoreCoefficient = track.ScoreCoefficient.Value.ToString(CultureInfo.InvariantCulture);
            else
                this.ScoreCoefficient = "0.0";

            foreach (var tag in this.tags)
            {
                bool isChecked = track.Tags.Contains(tag.Id);
                tag.Checked = isChecked;
            }

            foreach (var tag in this.events)
            {
                bool isChecked = track.Tags.Contains(tag.Id);
                tag.Checked = isChecked;
            }
        }

        internal void Reset()
        {
            //this.TrialsVersion = App.trialsVersions.FirstOrDefault();
            //this.Difficulty = App.difficulties.FirstOrDefault();
            //this.TrialsType = App.trialsTypes.FirstOrDefault();
            this.Creator = string.Empty;
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.LinkId = CreateRandomPassword(8);
            this.Thumbnail = null;

            if (ViewModelLocator.UserVM.IsAuthenticated)
                this.Creator = ViewModelLocator.UserVM.UserName;

            //if (this.Tags != null)
            //{
            //    foreach (var tag in this.Tags)
            //        tag.Checked = false;
            //}
        }

        internal void Submit()
        {



            var n = new NewTrack();
            n.Submitted = (Guid)ViewModelLocator.UserVM.Id;
            n.LinkId = this.LinkId;
            n.Name = this.Title;
            n.Description = this.description;
            n.TrialsVersion = this.TrialsVersion.Id;
            n.TrialsType = this.TrialsType.Id;
            n.Difficulty = this.Difficulty.Id;
            n.Rating = this.Rating;
            n.Creator = this.Creator;
            n.Created = this.Created;
            n.Hyperlink = this.Hyperlink;

            if (!string.IsNullOrEmpty(this.TimeUltimate))
                n.TimeUltimate = TimeSpan.Parse(this.TimeUltimate);
            if (!string.IsNullOrEmpty(this.TimePlatinum))
                n.TimePlatinum = TimeSpan.Parse(this.TimePlatinum);
            if (!string.IsNullOrEmpty(this.TimeGold))
                n.TimeGold = TimeSpan.Parse(this.TimeGold);
            if (!string.IsNullOrEmpty(this.TimeSilver))
                n.TimeSilver = TimeSpan.Parse(this.TimeSilver);
            if (!string.IsNullOrEmpty(this.ScoreCoefficient))
                n.ScoreCoeffient = double.Parse(this.ScoreCoefficient, CultureInfo.InvariantCulture);

            if (this.FileStream != null)
                n.Image = this.FileStream;

            n.Tags = new ObservableCollection<Guid>();          

            if (this.Tags != null)
            {
                foreach (var t in this.Tags)
                    if (t.Checked) n.Tags.Add(t.Id);

                foreach (var t in this.Events)
                    if (t.Checked) n.Tags.Add(t.Id);
            }

            client.NewTrackAsync(n);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
