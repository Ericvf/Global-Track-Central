using System;
using System.Windows;
using System.Windows.Controls;
using Trials.GTC.GlobalTrackCentral;
using Trials.GTC.ViewModel;
using Trials.GTC.UserControls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows.Navigation;
using System.Diagnostics;
using AnimationExtensions;
using Trials.GTC.AnimationExtensions;

namespace Trials.GTC
{
    public partial class MainPage : UserControl
    {
        bool isStarted = false;

        public MainPage()
        {
            InitializeComponent();
            App.ContentFrame = this.ContentFrame;

            this.user.DataContext = ViewModelLocator.UserVM;


            this.InitAx();

        }

        private void InitAx()
        {
            this.imgAppByFex.RenderTransformOrigin = new Point(0.5, 0.5);
            this.imgBackground.Hide();
            this.imgBanner.Hide();
            this.imgAppByFex.Hide();
            this.imgPresents.Hide();
            this.imgGTC.Hide();
            this.user.Hide();

            this.LinksBorder.Hide();

             var splashfex = this.imgAppByFex
                .Scale(0.5, 0.5)
                .Fade(1, 1000, Eq.OutSine)
                .Scale(1, 1, 1000, Eq.OutElastic)
                .Then()
                .Move(0, 200, 500, Eq.InBack)
                .Fade(0, 500);

            var splashpresents = this.imgPresents
                    .Move(0, -200)
                    .Move(0, 0, 750, Eq.OutBack)
                    .Fade(1, 750)
                    .Wait(500)
                    .Move(0, 200, 500, Eq.InBack)
                    .Fade(0, 500);

            var bg = this.imgBackground
                .Fade(1, 750);

            var splashGTC = this.imgGTC
                .Move(0, -200)
                .Move(0, 0, 750, Eq.OutBack)
                .Fade(1, 750)
                .Wait(1000)
                .Fade(0, 1000);

            var banner = this.imgBanner
                .Fade(1, 750, Eq.OutSine);

            var links = LinksStackPanel.FindChilden<HyperlinkButton>();
            links.Hide();

            var menu = this.LinksBorder
                .Move(0, -50)
                .Move(0, 0, 500, Eq.OutBack)
                .Fade(1, 500)
                .New()
                .For(links, (i, l) => l
                .Move(y: -50)
                .Wait(i * 100)
                .Fade(1, 750, Eq.OutSine)
                .Move(duration: 750, eq: Eq.OutBack)
            );

            var user = this.user.SlideIn();

            this.splashAx = Ax.New()
                // .AndThen(splashfex)
                // .AndThen(splashpresents)
                 .And(bg)
                // .And(splashGTC)
                //.Then()

                .And(banner)
                .Then()
                .And(menu)
                .AndThen(user);
        }

        Animation splashAx = Ax.New();

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (this.isStarted)
                return;

            this.isStarted = true;
            e.Cancel = true;

            splashAx.Do((n) =>
            {
                ContentFrame.Navigate(e.Uri);
            })
            .Play();


        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as HyperlinkButton;
            int index = Int32.Parse(button.Tag.ToString());

            var searchVM = ViewModelLocator.TracksVM.Search;

            ViewModelLocator.TracksVM.Reset();
            searchVM.TrialsVersion = searchVM.TrialsVersions[index];

            ViewModelLocator.TracksVM.LoadData();

        }

        private void SubmitTrack_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModelLocator.UserVM.IsAuthenticated)
            {
                var window = new NewTrackWindow();
                window.Show();
            }
            else
            {
                var window = new LoginWindow();
                window.Show();
            }
        }

        private void AllTracks_Click(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.TracksVM.Reset();
            ViewModelLocator.TracksVM.LoadData();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.Show();
        }
    }
}
