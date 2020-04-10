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
        private static ItemsViewModel _viewModel;
        //private BaseCommand _command;
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = _viewModel ?? new ItemsViewModel();
            CreateTappedGesture();
        }

        private void CreateTappedGesture()
        {
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += TapRecognizer_Tapped;
            ItemsListView.GestureRecognizers.Clear();
            ItemsListView.GestureRecognizers.Add(tapRecognizer);
        }

        private async void TapRecognizer_Tapped(object sender, System.EventArgs e) => await Shell.Current.GoToAsync($"//onkyocontroller//onkyotab/onkyodetails");

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.Items.Count == 0)
                _viewModel.LoadItemsCommand.Execute(null);
            _viewModel.SubscribeToCommands();
            
        }

        protected override void OnDisappearing()
        {
            _viewModel.UnsubribeFromCommands();
            base.OnDisappearing();
        }

        //private void ItemsListView_SelectionChanged(object sender, SelectionChangedEventArgs args)
        //{
        //    if (args.CurrentSelection.FirstOrDefault() is SLICommand item)
        //        _command = item;
        //}
    }
}