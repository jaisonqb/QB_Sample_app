using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QBurst.Navigation
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string title, string message, string buttonLabel)
        {
            return App.Current.MainPage.DisplayAlert(title, message, buttonLabel);
        }

        public async Task<bool> ShowAlertAsync(string title, string message, string okbuttonLabel, string cancelButtonlabel)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, okbuttonLabel, cancelButtonlabel);
        }
    }
}
