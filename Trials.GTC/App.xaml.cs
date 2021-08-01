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
using Trials.GTC.GlobalTrackCentral;
using System.Collections.ObjectModel;
using Trials.GTC.Framework;
using System.Windows.Controls.Primitives;

namespace Trials.GTC
{
    public partial class App : Application
    {

        public class TrialsVersion
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public static List<TrialsVersion> trialsVersions = new List<TrialsVersion>()
        {
            new TrialsVersion() { Id = 0, Name = @"Trials 2"},
            new TrialsVersion() { Id = 1, Name = @"Trials HD"},
            new TrialsVersion() { Id = 2, Name = @"Trials Evo"},
            new TrialsVersion() { Id = 3, Name = @"Trials Evo Gold"},
            new TrialsVersion() { Id = 4, Name = @"Trials Fusion PC"},
            new TrialsVersion() { Id = 5, Name = @"Trials Fusion 360"},
            new TrialsVersion() { Id = 6, Name = @"Trials Fusion XB1"},
            new TrialsVersion() { Id = 7, Name = @"Trials Fusion PS4"},
        };

        public static List<TrialsVersion> trialsTypes = new List<TrialsVersion>()
        {
            new TrialsVersion() { Id = 0, Name = @"Trials"},
            new TrialsVersion() { Id = 1, Name = @"Supercross"},
            new TrialsVersion() { Id = 2, Name = @"Skillgame"},
            new TrialsVersion() { Id = 3, Name = @"FMX"},
        };

        public static List<TrialsVersion> difficulties = new List<TrialsVersion>()
        {
            new TrialsVersion() { Id = 0, Name = @"Beginner"},
            new TrialsVersion() { Id = 1, Name = @"Easy"},
            new TrialsVersion() { Id = 2, Name = @"Medium"},
            new TrialsVersion() { Id = 3, Name = @"Hard"},
            new TrialsVersion() { Id = 4, Name = @"Extreme"},
            new TrialsVersion() { Id = 5, Name = @"Ninja 1"},
            new TrialsVersion() { Id = 6, Name = @"Ninja 2"},
            new TrialsVersion() { Id = 7, Name = @"Ninja 3"},
            new TrialsVersion() { Id = 8, Name = @"Ninja 4"},
            new TrialsVersion() { Id = 9, Name = @"Ninja 5"},
        };

        public static ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Root_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var parent = TreeHelper.FindParent<DataGridColumnHeader>(sender as Grid);
            if (parent != null && parent.Content != null)
            {
                var viewModel = parent.DataContext as TracksVM;
                if (viewModel != null)
                    viewModel.UpdateSort(parent.Content.ToString());
                //e.Handled = false;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new MainPage();

            var client = new GlobalTrackCentral.TrackCentralClient();
            client.GetTagsCompleted += new EventHandler<GetTagsCompletedEventArgs>(client_GetTagsCompleted);
            client.GetTagsAsync();

         //   ViewModelLocator.TracksViewModel.PreloadData();
           // ViewModelLocator.TracksViewModel.LoadData();
        }


        void client_GetTagsCompleted(object sender, GetTagsCompletedEventArgs e)
        {
            foreach (var item in e.Result)
                tags.Add(new Tag() { Id = item.Id,  Name = item.Name, IsCompetition = item.IsCompetition} );
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;   

               // MessageBox.Show(e.ExceptionObject.ToString());

                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        public static Frame ContentFrame { get; set; }
    }
}
