using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace AppGhiChuWinStore
{
    class MessageDialogHelper
    {
        public async void Show(string content, string title)
        {
            MessageDialog messageDialog = new MessageDialog(content, title);
            messageDialog.Commands.Add(new UICommand("Không", new UICommandInvokedHandler(CommandHandlers)));
            messageDialog.Commands.Add(new UICommand("Có", new UICommandInvokedHandler(CommandHandlers)));
            await messageDialog.ShowAsync();
        }
        public async void CommandHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                case "Không":
                    //Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-notifications://"));
                    break;
                case "Có":
                    //App.Current.Terminate();
                    break;
                //end.
            }
        }
    }
}

