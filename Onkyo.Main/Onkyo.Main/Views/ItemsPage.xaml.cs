using System.ComponentModel;
using Xamarin.Forms;
using System.Linq;
using Onkyo.Main.ViewModels;
using Eiscp.Core.Commands;

namespace Onkyo.Main.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel _viewModel;
        private SLICommand _command;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ItemsViewModel();
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += async delegate 
            {
                //_viewModel.UnsubribeFromCommands();
                await Navigation.PushAsync(new AboutPage(_command.Name));
            };
            ItemsListView.GestureRecognizers.Add(tapRecognizer);
        }       

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel.Items.Count == 0)
                _viewModel.LoadItemsCommand.Execute(null);

            _viewModel.SubscribeToCommands();
        }

        private void ItemsListView_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.CurrentSelection.FirstOrDefault() is SLICommand item)
                _command = item;
        }
    }
}