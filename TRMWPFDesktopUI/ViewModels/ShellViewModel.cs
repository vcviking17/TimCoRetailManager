using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TRMWPFDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>  //we will only put ViewModels in the object
                                                     //this will allow us to display a form on the window.
    {
        private LoginViewModel _loginVM;

        public ShellViewModel(LoginViewModel loginVM)
        {
            _loginVM = loginVM; //constructor injection
            ActivateItem(_loginVM); //activate the login on the ShellView
        }
    }
}
