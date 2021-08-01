using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Trials.GTC.Triggers
{

    public class ExecuteCommandAction : TriggerAction<DependencyObject>
    {
        #region Properties
        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // We use a DependencyProperty so we can bind commands directly rather 
        // than have to use reflection info to find them 
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteCommandAction), null);
        #endregion Properties

        protected override void Invoke(object parameter)
        {
            ICommand command = Command ?? GetCommand(AssociatedObject);
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
