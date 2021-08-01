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
using System.Windows.Navigation;
using Trials.GTC.ViewModel;
using Appbyfex.Validators;

namespace Trials.GTC.UserControls
{
    public partial class LoginWindow 
    {
        public LoginWindow()
        {
            var vm = ViewModelLocator.UserVM;
            ValidatorService.Reset();
            vm.Reset();

            InitializeComponent();

            
            vm.Success += new EventHandler(vm_Success);
            this.DataContext = vm;

        }

        void vm_Success(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LostPasswordClick(object sender, RoutedEventArgs e)
        {
            this.tabs.SelectedIndex = 2;
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            this.tabs.SelectedIndex = 1;
        }
    }
}
