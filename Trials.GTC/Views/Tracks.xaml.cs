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
using System.Windows.Controls.Primitives;
using Trials.GTC.Framework;
using AnimationExtensions;
using Trials.GTC.UserControls;
using System.Windows.Navigation;
using System.Windows.Browser;
using System.ComponentModel;

namespace Trials.GTC.Views
{
    public partial class Tracks : Page
    {
        private TracksVM VM = ViewModelLocator.TracksVM;
        //public event EventHandler Navigate;

        public Tracks()
        {
            InitializeComponent();
            this.DataContext = VM;

            VM.LoadStarted += new EventHandler(VM_LoadStarted);
            VM.LoadCompleted += new EventHandler(VM_LoadCompleted);
        }

        void VM_LoadStarted(object sender, EventArgs e)
        {
            this.Hide();

            var a1 = this.Fade(1, 1000, Eq.OutSine);
            a1.Play();

            this.ContentPanel.Hide();
        }

        void VM_LoadCompleted(object sender, EventArgs e)
        {
            var a1 = this.ContentPanel.Fade(1, 1000, Eq.OutSine);
            a1.Play();
            
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("event"))
            {
                this.Search.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (this.NavigationContext.QueryString.ContainsKey("tag"))
            {
                var tagName = this.NavigationContext.QueryString["tag"];
                var tag = App.tags.Where(t => t.Name == tagName).FirstOrDefault();
                if (tag != null)
                {
                    this.VM.Reset();
                    tag.Checked = true;
                }
            }

            if (this.NavigationContext.QueryString.ContainsKey("creator"))
            {
                this.VM.Reset();
                this.VM.Search.Creator = this.NavigationContext.QueryString["creator"];
            }


            this.VM.LoadData();

            var a1 = this.Fade(1, 1000, Eq.OutSine);
            a1.Play();

            base.OnNavigatedTo(e);
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dataGrid.CurrentColumn != null)
                this.dataGrid.CurrentColumn = this.dataGrid.Columns[0];
        }

        private void TrackItem_Click(object sender, RoutedEventArgs e)
        {
            //if (this.Navigate != null)
            //    this.Navigate(this, null);

            var l = sender as HyperlinkButton;
            if (l != null)
            {
                var uri = new Uri("/Track/" + (l as HyperlinkButton).Tag.ToString(), UriKind.RelativeOrAbsolute);
                App.ContentFrame.Navigate(uri);
            }

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            this.VM.ShowSearch();
        }

        private void CancelSearch_Click(object sender, RoutedEventArgs e)
        {
            this.VM.Reset();
            this.VM.LoadData();
        }

        private void ResetSearch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.VM.Reset();
            this.VM.LoadData();
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

    }
}
