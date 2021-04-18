using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMWPFDesktopUI.Helpers;

namespace TRMWPFDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;
        private IAPIHelper _apiHelper;

        //dependency injection to say I need it and assign it. 
        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
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
            }
            catch (Exception)
            {
                //bad response from Authenticate(), such as bad name and password
                Console.WriteLine();
            }
        }
    }
}
