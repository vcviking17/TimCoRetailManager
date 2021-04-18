using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMWPFDesktopUI.Helpers;
using TRMWPFDesktopUI.ViewModels;

namespace TRMWPFDesktopUI
{
    class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
                PasswordBoxHelper.BoundPasswordProperty,
                "Password",
                "PasswordChanged");
        }

        //where the actual instantiation happenes.  
        //where the container knows hat to connect to what
        //Configure() gets run once at the beginning of the application.
        protected override void Configure()
        {
            //whenever we ask for a container instance, it will return the instance
            _container.Instance(_container);
            //handle the idea of bringing windows in and out
            //can pass event messaging throughout the aplpication.
            //One piece can raise and event and another piece can react to it. 
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper, APIHelper>();
            //singleton means create one instance of the class for the scope of the container/application
            //ShellViewModel asks for an EventAggregator, it will get the first EventAggregator.
            //If another one asks for it, the same one is returned.
            //Almost like a static class. 

            //Reflexion is intensive, but only running it once
            GetType().Assembly.GetTypes()  //for the current assembly running, get every type in the application
                .Where(type => type.IsClass)  //we only want classes
                .Where(type => type.Name.EndsWith("ViewModel")) //we only want name to end with ViewModel  (ShellViewModel class)
                .ToList()  //create that list
                .ForEach(viewModelType => _container.RegisterPerRequest(  //for each item in the list
                    viewModelType, viewModelType.ToString(), viewModelType));  //register each item to instantiate
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //on statup, launch ShellViewModel as base view
            DisplayRootViewFor<ShellViewModel>();
        }
        protected override object GetInstance(Type service, string key)
        {
            //when I pass a type and name, we'll get the instance.
            return _container.GetInstance(service, key);
        }
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
