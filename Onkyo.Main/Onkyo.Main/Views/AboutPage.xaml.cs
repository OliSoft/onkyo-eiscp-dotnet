using Onkyo.Main.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onkyo.Main.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage() : this(null) { }

        public AboutPage(string name)
        {
            InitializeComponent();
            BindingContext = new AboutViewModel(name);
        }
    }
}