using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMWPFDesktopUI.EventModels;
using TRMWPFDesktopUI.Library.Api;
using TRMWPFDesktopUI.Library.Models;

namespace TRMWPFDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent> // ,Ihandle <string>  --if we needed multiple  
                                                        //we will only put ViewModels in the object
                                                     //this will allow us to display a form on the window.
                                                       //IHandle handles the event
    {
        private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        //private SimpleContainer _container;

        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events, SalesViewModel salesVM, SimpleContainer container, ILoggedInUserModel user,
            IAPIHelper apiHelper)
        {
            _events = events;
            _loginVM = loginVM; //constructor injection
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            //_container = container;

            _events.Subscribe(this);  //“this” represents the current instance of this class.  You have to tell it who is subscribing (this).
                                      //When an event happens, I’m going to send the event to subscribers, even if they aren’t listening for that event. 

            // ActivateItem(_loginVM); //activate the login on the ShellView
            //ActivateItem(_container.GetInstance<LoginViewModel>());  //get a new instance of it and put it in _loginVM to wipe out _loginVM without any data
            ActivateItem(IoC.Get<LoginViewModel>());  //Do it this way and we don't need the container in dependency injection (Caliburn Micro)
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            //reset login credentials
            _user.ResetUserModel();
            _apiHelper.LogOffUser(); //clear out header 
            //close out everything and activate loginViewModel
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
     

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
            
        }
        
        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM); //when we log in, it'll close out the LoginView and open SalesView since we can only have one active item. 
                                    //It doesn't destroy the loginview since the instance is still there _login.  But it would stil have the user
                                    //name and password in it since that's how we left it. 
                                    //_loginVM = _container.GetInstance<LoginViewModel>();  //get a new instance of it and put it in _loginVM to wipe out _loginVM without any data
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
