
namespace Trials.GTC.Framework
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The ActionCommand of T class 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActionCommand<T> : ICommand
    {
        /// <summary>
        /// Events
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the Action
        /// </summary>
        public Action<T> Action { get; set; }

        /// <summary>
        /// Initializes the Command
        /// </summary>
        /// <param name="action"></param>
        public ActionCommand(Action<T> action)
        {
            this.Action = action;
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionCommand()
        {

        }

        /// <summary>
        /// Returns true if the Command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the Command
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            if (this.Action != null)
                this.Action((T)parameter);
        }

        /// <summary>
        /// Executes the Command 
        /// </summary>
        /// <param name="parameter"></param>
        protected void Execute(T parameter)
        {
            if (this.Action != null)
                this.Action(parameter);
        }

        /// <summary>
        /// Raises the ExecuteChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RaiseExecuteChanged(object sender, EventArgs e)
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(sender, e);
        }
    }

    /// <summary>
    /// The ActionCommand of T class 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActionCommand : ICommand
    {
        /// <summary>
        /// Events
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the Action
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Initializes the Command
        /// </summary>
        /// <param name="action"></param>
        public ActionCommand(Action action)
        {
            this.Action = action;
        }

        /// <summary>
        /// Returns true if the Command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the Command
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            if (this.Action != null)
                this.Action();
        }

        /// <summary>
        /// Raises the ExecuteChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RaiseExecuteChanged(object sender, EventArgs e)
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(sender, e);
        }
    }

}

