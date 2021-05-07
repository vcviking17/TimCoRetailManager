﻿using Caliburn.Micro;
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

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserName = value.Email;
                UserRoles.Clear();
                //create a new binding list from the list of strings (roles.select)
                UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                LoadRoles(); //can't await in a property, but it works.  Will change later. 
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => _selectedUserRole);
            }
        }

        private string _selectedAvailableRole;

        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);
            }
        }

        private string _selectedUserName;

        public string SelectedUserName
        {
            get 
            { 
                return _selectedUserName; 
            }
            set 
            { 
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _UserRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _UserRoles; }
            set 
            {
                _UserRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string> _availableRoles = new BindingList<string>();

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                //we only want to show what roles the user doesn't have

                NotifyOfPropertyChange(() => AvailableRoles);
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

        private async Task LoadRoles()
        {
            var roles = await _userEndpoint.GetAllRoles();
            foreach (var role in roles)
            {
                //indexof returns -1 if it doesn't find it.
                if (UserRoles.IndexOf(role.Value) < 0) 
                {
                    AvailableRoles.Add(role.Value);
                }
            }
        }

        public async Task AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);
            //change the lists to account for the added role
            UserRoles.Add(SelectedAvailableRole);
            AvailableRoles.Remove(SelectedAvailableRole);
        }

        public async Task RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);
            //change the lists to account for the added role;
            AvailableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);
        }
    }
}
