using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Trials.GTC.ViewModel;
using Trials.GTC.GlobalTrackCentral;
using Appbyfex.Validators;
using System.Windows.Browser;

namespace Trials.GTC.UserControls
{
    public partial class NewTrackWindow : ChildWindow
    {
        NewTrackVM VM = ViewModelLocator.NewTrackVM;

        public NewTrackWindow()
        {
            ValidatorService.Reset();
            InitializeComponent();

            this.DataContext = this.VM;
            this.VM.NewTrackCompleted += new EventHandler(VM_NewTrackCompleted);
            this.VM.Reset();

            this.InitValidators();

        }
        
        protected override void OnOpened()
        {
            if (!ViewModelLocator.UserVM.Roles.Contains("Admin") &&
                !ViewModelLocator.UserVM.Roles.Contains("Moderator"))
            {
                this.tbLinkId.IsReadOnly = true;
            }

            ValidatorService.Validate();
            
            base.OnOpened();
        }

        public NewTrackWindow(Track track)
            : this()
        {
            //ValidatorService.Reset();
            //InitializeComponent();

            //this.DataContext = this.VM;
            //this.VM.NewTrackCompleted += new EventHandler(VM_NewTrackCompleted);
            this.VM.LoadTrack(track);

            //this.InitValidators();
        }

        private void InitValidators()
        {
            // Create a custom validation script
            ValidationDelegate timeStamp = new ValidationDelegate(this.validateTimeStamp);
            ValidationDelegate tags = new ValidationDelegate(this.validateTags);

            // Set the custom validation on the UIElement
            this.tbTimeUltimate.SetCustomValidation(timeStamp);
            this.tbTimePlatinum.SetCustomValidation(timeStamp);
            this.tbTimeGold.SetCustomValidation(timeStamp);
            this.tbTimeSilver.SetCustomValidation(timeStamp);

            this.lbTags.SetCustomValidation(tags);
        }

        bool validateTimeStamp(UIElement uiElement)
        {
            var textBox = uiElement as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                    return true;

                TimeSpan ts;
                return TimeSpan.TryParse(textBox.Text, out ts);
            }

            return false;
        }

        bool validateTags(UIElement uiElement)
        {
            var tags = from t in this.VM.Tags
                       where t.Checked
                       select t;

            return tags.Count() > 1;
        }

        void VM_NewTrackCompleted(object sender, EventArgs e)
        {
            try
            {
                ViewModelLocator.TracksVM.LoadData();
                ViewModelLocator.TrackVM.Reload();
            }
            finally
            {
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool hasValidated = ValidatorService.Validate();

            if (hasValidated)
                VM.Submit();
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && !string.IsNullOrEmpty(this.VM.Title))
            {
                try
                {
                    HtmlPage.Window.Navigate(new Uri("http://www.youtube.com/results?search_query=trials+evolution+" + this.VM.Title), "_blank");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorry, unable to open the specified link." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error", MessageBoxButton.OK);
                }
            }

            e.Handled = false;
            return;
        }
    }
}