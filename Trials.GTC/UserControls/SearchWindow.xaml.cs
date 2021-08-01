using System;
using System.Windows;
using System.Windows.Controls;
using Trials.GTC.ViewModel;

namespace Trials.GTC.UserControls
{
    public partial class SearchWindow : ChildWindow
    {
        public SearchWindow()
        {
            InitializeComponent();

            this.DataContext = ViewModelLocator.TracksVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}