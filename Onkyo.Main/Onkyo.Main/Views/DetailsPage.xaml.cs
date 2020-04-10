using Onkyo.Main.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Onkyo.Main.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage() : this(null) { }

        public DetailsPage(string name)
        {
            InitializeComponent();
            BindingContext = new DetailsViewModel(name);
        }
    }
}