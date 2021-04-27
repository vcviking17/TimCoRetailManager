using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMWPFDesktopUI.EventModels;
using TRMWPFDesktopUI.Helpers;
using TRMWPFDesktopUI.Library.Api;

namespace TRMWPFDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName = "vince_catalano@hotmail.com";
        private string _password = "Pwd12345.";
        private IAPIHelper _apiHelper;
        IEventAggregator _events;

        //dependency injection to say I need it and assign it. 
        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }
        //match the names on the LoginView
        public string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get 
            {
                bool output = false;
                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output; 
            }
           
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {                
                _errorMessage = value; 
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }


        public bool CanLogIn
        //enable/disable the login button
        //need to look at this to mkae password box work:
        //https://stackoverflow.com/questions/30631522/caliburn-micro-support-for-passwordbox
        {
            get
            {
                bool output = false;
                //we'd want to do more validation than this
                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }

                return output;
            }           
        }

        public async Task LogIn()
        {
            //read the properties if a successful Authenticate call
            try
            {
                var result = await _apiHelper.Authenticate(UserName, Password);
                ErrorMessage = ""; //no error on success

                //capture more information about the user. 
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                _events.PublishOnUIThread(new LogOnEvent());//this broadcasts the event.
            }
            catch (Exception ex)
            {
                //bad response from Authenticate(), such as bad name and password
                ErrorMessage = ex.Message;
            }
        }
    }
}
