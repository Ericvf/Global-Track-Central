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

namespace Trials.GTC.Views
{
    public partial class Event 
    {
        private EventVM VM = new EventVM();

        public Event()
        {
            InitializeComponent();

            this.DataContext = VM;
            this.ContentPanel.DataContext = ViewModelLocator.TracksVM;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("event"))
            {
                var tagName = this.NavigationContext.QueryString["event"];
                var tag = App.tags.Where(t => t.Name == tagName).FirstOrDefault();
                if (tag != null && tag.IsCompetition)
                {
                    VM.LoadData(tag);
                    ViewModelLocator.TracksVM.Reset();
                    tag.Checked = true;
                    this.Title = tag.Name;

                }
                else
                {
                    var uri = new Uri("/All", UriKind.RelativeOrAbsolute);

                    App.ContentFrame.Navigate(uri);
                    return;
                }
            }

            ViewModelLocator.TracksVM.LoadData();

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

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            EventVM.Rider obj = (EventVM.Rider)tb.DataContext;
            tb.Text = (VM.Riders.IndexOf(obj) + 1).ToString();
        }
    }
}
