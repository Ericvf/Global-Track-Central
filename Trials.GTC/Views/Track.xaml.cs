using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Trials.GTC.ViewModel;
using System.Windows.Browser;
using Trials.GTC.UserControls;
using AnimationExtensions;
using Trials.GTC.AnimationExtensions;
using System.Windows.Media.Imaging;


namespace Trials.GTC.Views
{
   
    public partial class TrackDetailsView : Page
    {
        private TrackVM VM = ViewModelLocator.TrackVM;
        private bool hasAnimated = false;

        public TrackDetailsView()
        {
            InitializeComponent();

            this.VM.LoadCompleted += new EventHandler(VM_LoadCompleted);
            this.DataContext = VM;

            this.Window.RenderTransformOrigin = new Point(0.5, 0.5);
            this.Window.Hide();
            this.TitleText.Hide();
            this.Data.Hide();
            this.Buttons.Hide();
            this.ThumbnailGrid.Hide();
            this.medalTimes.Hide();
            this.Youtube.Hide();
            this.Tweets.Hide();
            this.Times.Hide();
            //this.VM.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(VM_PropertyChanged);
        }

        //void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Tweets" && VM.Tweets.Count > 0)
        //        Tweets.SlideIn().Play();
        //}

        void VM_LoadCompleted(object sender, EventArgs e)
        {
            if (hasAnimated)
                return;

            hasAnimated = true;

            var a1 = this.Window
              //.Move(-300, 0)
              //.Move(0, 0, 750, Eq.OutBack)
              .Scale(0.5, 0.5)
              .Scale(duration: 500, eq: Eq.OutBack)
              .Fade(0.7, 750);

            var track = this.VM.Track;
            bool hasMedals = (track.TimeUltimate != null && track.TimeUltimate.Value.Ticks > 0 &&
                track.TimeGold != null && track.TimeGold.Value.Ticks > 0 &&
                track.TimePlatinum != null && track.TimePlatinum.Value.Ticks > 0 &&
                track.TimeSilver != null && track.TimeSilver.Value.Ticks > 0 &&
                track.ScoreCoefficient.HasValue && track.ScoreCoefficient.Value > 0);

            Ax.New().AndThen(a1)
                .AndThen(TitleText.SlideIn())
                .AndThen(Data.SlideIn())
                //.AndIfThen(Description.SlideIn(), !string.IsNullOrEmpty(VM.Track.Description))
                
                .AndThen(Buttons.SlideIn(200))
                .AndThen(ThumbnailGrid.SlideIn(200))
                .AndIfThen(medalTimes.SlideIn(200), hasMedals)
                .AndIfThen(Youtube.SlideIn(200), !string.IsNullOrEmpty(VM.Track.Hyperlink))
                .AndThen(Times.SlideIn(200))
                .AndThen(Tweets.SlideIn(200))

           .Play();

            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                var roles = ViewModelLocator.UserVM.Roles;
                if (roles.Contains("Admin") || VM.Track.Submitted == ViewModelLocator.UserVM.Id || VM.Track.Creator == ViewModelLocator.UserVM.UserName)
                    this.btnDelete.Visibility = System.Windows.Visibility.Visible;

                //if (roles.Contains("Admin") || roles.Contains("Moderator") || VM.Track.Creator == ViewModelLocator.UserVM.UserName)
                //if (ViewModelLocator.UserVM.IsAuthenticated)
                {
                }
            }

            if (VM.Track.TrialsVersion == 0)
                this.tbTimes.Visibility = System.Windows.Visibility.Collapsed;

            this.btnEdit.Visibility = System.Windows.Visibility.Visible;
            this.Title = this.VM.Track.Name;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.VM.Track = null;

            if (this.VM.Track == null)
                this.VM.LoadData(this.NavigationContext.QueryString["id"]);

            base.OnNavigatedTo(e);
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dataGrid.CurrentColumn != null)
                this.dataGrid.CurrentColumn = this.dataGrid.Columns[0];
        }

        private void Thumb_ImageOpened(object sender, RoutedEventArgs e)
        {
            // Find sender
            var image = sender as Image;

            // Handle arguments
            if (image == null)
                throw new ArgumentNullException(@"Sender is not an Image.");

            // Create storyboard
            var storyboard = new Storyboard();

            // Create animation for the opacity
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            animation.From = 0.0d;
            animation.To = 1.0d;

            // Add he animation to the Storyboard
            storyboard.Children.Add(animation);

            // Apply the Storyboard to the Image
            Storyboard.SetTarget(storyboard, image);

            // Apply the animation to the OpacityProperty
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.OpacityProperty));

            // Start animation
            storyboard.Begin();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MouseOverAnim.Begin();
            this.MouseOutAnim.Stop();
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            this.MouseOverAnim.Stop();
            this.MouseOutAnim.Begin();
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                try
                {
                    var url = button.Tag.ToString();

                    
                    
                    HtmlPage.Window.Navigate(new Uri(url), "_blank");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorry, unable to open the specified link." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error", MessageBoxButton.OK);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                var window = new NewTrackWindow(this.VM.Track);
                window.Show();
            }
            else
            {
                var window = new LoginWindow();
                window.Show();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.VM.DeleteTrack();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null && img.Tag != null)
            {
                try
                {
                    HtmlPage.Window.Navigate(new Uri(img.Tag.ToString()), "_blank");
                }
                catch
                {

                }
            }
        }

        private void SubmitTime_Click(object sender, MouseButtonEventArgs e)
        {
            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                ViewModelLocator.NewTimeVM.Reset(this.VM.Track);
                var window = new SubmitTime();
                window.Show();
            }
            else
            {
                var window = new LoginWindow();
                window.Show();
            }
        }

        private void AddMessage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                var window = new NewMessage(VM.Track.Id);
                window.Show();
            }
            else
            {
                var window = new LoginWindow();
                window.Show();
            }
        }
        //bool ytLock = false;
        private void Thumbnail_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //if (ytLock)
            //    return;

            //ytLock = true;

            
            
        }
    }

   
}
