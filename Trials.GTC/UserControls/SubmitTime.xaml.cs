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
using Appbyfex.Validators;
using Trials.GTC.ViewModel;

namespace Trials.GTC.UserControls
{
    public partial class SubmitTime
    {
        NewTimeVM VM = ViewModelLocator.NewTimeVM;

        public SubmitTime()
        {
            ValidatorService.Reset();
            InitializeComponent();
            this.InitValidators();

            this.DataContext = VM;
            this.VM.Completed += new EventHandler(VM_Completed);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.VM.Completed -= new EventHandler(VM_Completed); 
            base.OnClosing(e);
        }
        void VM_Completed(object sender, EventArgs e)
        {
            ViewModelLocator.TrackVM.LoadTimes();
            this.Close();
        }

        protected override void OnOpened()
        {
            ValidatorService.Validate();
        }

        private void InitValidators()
        {
            ValidationDelegate timeStamp = new ValidationDelegate(this.validateTimeStamp);
            this.tbTime.SetCustomValidation(timeStamp);
        }

        bool validateTimeStamp(UIElement uiElement)
        {
            var textBox = uiElement as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                    return false;

                TimeSpan ts;
                var isTs = TimeSpan.TryParse(textBox.Text, out ts);
                return isTs && ts.Ticks > 0;
            }

            return false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool hasValidated = ValidatorService.Validate();

            if (hasValidated)
                this.VM.Submit();
        }
    }
}
