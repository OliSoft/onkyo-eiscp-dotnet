using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Onkyo.Main.Views;
using Eiscp.Core.Commands;
using System.Linq;

namespace Onkyo.Main.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<BaseCommand> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Commands";
            Items = new ObservableCollection<BaseCommand>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, BaseCommand>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as BaseCommand;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
        }

        public void SubscribeToCommands()
        {
            MessagingCenter.Instance.Subscribe<BaseCommand, string>(this, "UPDATE_AVR_COMMANDS", (cmd, resp) =>
            {
                AdaptCommands(cmd, resp);
            });
        }

        public void UnsubribeFromCommands()
        {
            MessagingCenter.Instance.Unsubscribe<BaseCommand, string>(this, "UPDATE_AVR_COMMANDS");
        }

        public static readonly Func<BaseCommand, SLICommand, bool> GetSelectedCommand = (cmd, sli) =>
                    (cmd is SLICommand sCmd) && sCmd.Test == sli.Response.ToInt();

        private void AdaptCommands(BaseCommand cmd, string resp)
        {
            if (cmd is PWRCommand pwrCmd && pwrCmd.IsCommand(resp))
            {
                IsEnabled = pwrCmd.Response.ToBoolean();
            }
            else
            if (cmd is SLICommand sliCmd && sliCmd.IsCommand(resp))
            {
                var selCmd = Items.FirstOrDefault(cmdItem => GetSelectedCommand(cmdItem, sliCmd));
                if (selCmd != null && IsEnabled)
                {
                    _selectedCommand = selCmd;
                    OnPropertyChanged(nameof(SelectedCommand));
                }
            }
        }

        private BaseCommand _selectedCommand;
        public BaseCommand SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                if (SetProperty(ref _selectedCommand, value))
                {
                    if (_selectedCommand is SLICommand sli)
                    {
                        _selectedCommand.IsSelected = true;
                        var command = string.Format("{0}{1:D2}", sli.Key, sli.Test).ToUpper();
                        Onkyo.Core.Service.UpdateService.Send(command);
                    }
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }



        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}