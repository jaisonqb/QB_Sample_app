using QBurst.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QBurst.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync(string ClientAssessmentId);
        Task NavigateToAsync<TViewModel>() where TViewModel : IViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : IViewModelBase;
        Task NavigateToAsync(Type viewModelType);

        Task NavigateToAsync(Type viewModelType, object parameter);
        Task NavigateBackAsync();
     }
}
