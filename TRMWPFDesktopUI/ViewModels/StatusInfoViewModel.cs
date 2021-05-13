using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMWPFDesktopUI.ViewModels
{
    public class StatusInfoViewModel : Screen
    {
        public string Header { get; private set; }
        public string Message { get; private set; }

        public void UpdateMessage(string header, string message)
        {
            Header = header;
            Message = message;

            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => Message);
        }

        public void Close()  //Close button on the form
        {
            TryCloseAsync();  //close the existing form...not the whole application (this.close would do the whole application)
        }
    }
}
