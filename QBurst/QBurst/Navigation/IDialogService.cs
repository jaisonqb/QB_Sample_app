using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QBurst.Navigation
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonLabel);
        Task<bool> ShowAlertAsync(string title, string message, string okbuttonLabel, string cancelButtonlabel);
    }
}
