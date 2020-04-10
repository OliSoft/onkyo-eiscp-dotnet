using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Onkyo.Main.Services;
using Onkyo.Main.Views;

namespace Onkyo.Main
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();

            Onkyo.Core.Service.UpdateService.Get();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
