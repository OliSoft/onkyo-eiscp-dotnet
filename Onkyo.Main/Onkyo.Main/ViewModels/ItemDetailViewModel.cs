using Eiscp.Core.Commands;
using Onkyo.Main.Models;
using Xamarin.Forms;

namespace Onkyo.Main.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;

            MessagingCenter.Instance.Subscribe<BaseCommand, string>(this, "UPDATE_AVR_COMMANDS", (cmd, resp) =>
            {
                if (cmd is SLICommand sliCmd && sliCmd.IsCommand(resp))
                {

                    
                }
            });
        }
    }
}
