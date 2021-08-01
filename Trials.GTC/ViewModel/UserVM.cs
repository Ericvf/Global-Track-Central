using System.ComponentModel;
using Trials.GTC.Framework;
using Trials.GTC.GlobalTrackCentral;
using System.Collections.Generic;
using System;
using System.IO.IsolatedStorage;
using Appbyfex.Validators;

namespace Trials.GTC.ViewModel
{
    public class UserVM : INotifyPropertyChanged
    {
        static TrackCentralClient client = new TrackCentralClient();
        public event EventHandler Success;

        public UserVM()
        {
            client.LoginUserCompleted += new System.EventHandler<LoginUserCompletedEventArgs>(client_LoginUserCompleted);
            client.RegisterUserCompleted += new EventHandler<RegisterUserCompletedEventArgs>(client_RegisterUserCompleted);
            client.ResetUserCompleted += new EventHandler<ResetUserCompletedEventArgs>(client_ResetUserCompleted);
            this.InitCommands();

            if (IsolatedStorageSettings.SiteSettings.Contains("username") &&
                 IsolatedStorageSettings.SiteSettings.Contains("password"))
            {
                this.UserName = IsolatedStorageSettings.SiteSettings["username"].ToString();
                this.Password = IsolatedStorageSettings.SiteSettings["password"].ToString();
                this.Remember = true;
                this.LoginCommand_Execute();
            }
        }

        void client_ResetUserCompleted(object sender, ResetUserCompletedEventArgs e)
        {
            
        }

        void client_RegisterUserCompleted(object sender, RegisterUserCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.IsAuthenticated = false;
                this.IsAuthenticating = false;
                this.ErrorMessage = e.Error.Message;
            }
            else
            {
                this.IsAuthenticated = true;
                this.Id = e.Result.Id;
                this.EmailAddress = e.Result.EmailAddress;
                this.Roles = new List<string>(e.Result.Roles);

                this.RaiseSuccess();
            }
        }

        void client_LoginUserCompleted(object sender, LoginUserCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.IsAuthenticating = false;
                this.IsAuthenticated = false;
                this.ErrorMessage = e.Error.Message;
            }
            else
            {
                this.IsAuthenticated = true;
                this.Id = e.Result.Id;
                this.EmailAddress = e.Result.EmailAddress;
                this.Roles = new List<string>(e.Result.Roles);

                if (this.Remember)
                {
                    IsolatedStorageSettings.SiteSettings["username"] = this.UserName;
                    IsolatedStorageSettings.SiteSettings["password"] = this.Password;
                }
                else
                {
                    IsolatedStorageSettings.SiteSettings.Remove("username");
                    IsolatedStorageSettings.SiteSettings.Remove("password");
                }

                IsolatedStorageSettings.SiteSettings.Save();

                this.RaiseSuccess();
            }
        }

        private void InitCommands()
        {
            this.loginCommand = new ActionCommand(this.LoginCommand_Execute);
            this.logoutCommand = new ActionCommand(this.LogoutCommand_Execute);
            this.registerCommand = new ActionCommand(this.RegisterCommand_Execute);
            this.lostPasswordCommand = new ActionCommand(this.LostPasswordCommand_Execute);
        }

        private bool isAuthenticating;
        public bool IsAuthenticating
        {
            get
            {
                return this.isAuthenticating;
            }
            set
            {
                this.isAuthenticating = value;
                this.RaisePropertyChanged("IsAuthenticating");
            }
        }

        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get
            {
                return this.isAuthenticated;
            }
            set
            {
                this.isAuthenticated = value;
                this.RaisePropertyChanged("IsAuthenticated");
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                this.RaisePropertyChanged("UserName");
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
                this.RaisePropertyChanged("ErrorMessage");
            }
        }

        private List<string> roles;
        public List<string> Roles
        {
            get
            {
                return this.roles;
            }
            set
            {
                this.roles = value;
                this.RaisePropertyChanged("Roles");
            }
        }

        private Guid? id;
        public Guid? Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                this.RaisePropertyChanged("Id");
            }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get
            {
                return this.emailAddress;
            }
            set
            {
                this.emailAddress = value;
                this.RaisePropertyChanged("EmailAddress");
            }
        }

        private bool remember;
        public bool Remember
        {
            get
            {
                return this.remember;
            }
            set
            {
                this.remember = value;
                this.RaisePropertyChanged("Remember");
            }
        }

        public ActionCommand logoutCommand;
        public ActionCommand LogoutCommand
        {
            get
            {
                return logoutCommand;
            }
        }
        public void LogoutCommand_Execute()
        {
            this.IsAuthenticated = false;
            this.UserName = null;
            this.Password = null;
            this.EmailAddress = null;
            this.Id = null;

            IsolatedStorageSettings.SiteSettings.Remove("username");
            IsolatedStorageSettings.SiteSettings.Remove("password");
            IsolatedStorageSettings.SiteSettings.Save();
        }

        public ActionCommand loginCommand;
        public ActionCommand LoginCommand
        {
            get
            {
                return loginCommand;
            }
        }
        public void LoginCommand_Execute()
        {
            bool hasValidated = ValidatorService.Validate("login");

            if (hasValidated)
            {
                this.IsAuthenticating = true;
                this.ErrorMessage = null;
                client.LoginUserAsync(this.UserName, this.Password);
            }
        }

        public ActionCommand registerCommand;
        public ActionCommand RegisterCommand
        {
            get
            {
                return registerCommand;
            }
        }
        public void RegisterCommand_Execute()
        {
               bool hasValidated = ValidatorService.Validate("register");

               if (hasValidated)
               {
                   this.IsAuthenticating = true;
                   this.ErrorMessage = null;
                   client.RegisterUserAsync(this.UserName, this.Password, this.EmailAddress);
               }
        }

        public ActionCommand lostPasswordCommand;
        public ActionCommand LostPasswordCommand
        {
            get
            {
                return lostPasswordCommand;
            }
        }
        public void LostPasswordCommand_Execute()
        {
               bool hasValidated = ValidatorService.Validate("reset");
               if (hasValidated)
               {
                   client.ResetUserAsync(this.UserName, this.EmailAddress); 
                   //this.IsAuthenticating = true;
                   //this.ErrorMessage = null;
                   //client.RegisterUserAsync(this.UserName, this.Password, this.EmailAddress);
               }
        }

        public void RaiseSuccess()
        {
            if (this.Success != null)
                this.Success(null, null);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void Reset()
        {
            this.ErrorMessage = null;
            this.IsAuthenticated = false;
            this.IsAuthenticating = false;
        }
    }
}
