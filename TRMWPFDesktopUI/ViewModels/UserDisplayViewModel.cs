using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMWPFDesktopUI.Library.Api;
using TRMWPFDesktopUI.Library.Models;

namespace TRMWPFDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;

        BindingList<UserModel> _users;

        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        public BindingList<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        //we want to wait until the view loads before starting since it's async
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers(); //load users when the view is loaded
            }
            catch (Exception ex)
            {
                //throw;
                //We want the Sales page to close down and alert the user (MessageBox)

                //create a dynamic list of settings
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                    _window.ShowDialog(_status, null, settings);  //show the _status window as popup (window dislag)
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    _window.ShowDialog(_status, null, settings);  //show the _status window as popup (window dislag)
                }

                //we can show a second dialog by repeating the calls
                //_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                //_window.ShowDialog(_status, null, settings);  //show the _status window as popup (window dislag)

                TryClose(); //after they close the dialog box, close the sales form:
            }
        }
        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();  //returns a list of UserModel
            Users = new BindingList<UserModel>(userList);
        }
    }
}
