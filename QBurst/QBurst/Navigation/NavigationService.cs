using QBurst.Common;
using QBurst.ViewModels.Base;
using QBurst.ViewModels.Interfaces;
using QBurst.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QBurst.Navigation
{
    class NavigationService : INavigationService
    {
        private readonly IDialogService _dialogService;
        protected readonly Dictionary<Type, Type> _mappings;
        protected Xamarin.Forms.Application CurrentApplication
        {
            get
            {
                return Xamarin.Forms.Application.Current;
            }
        }
        public NavigationService(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _mappings = new Dictionary<Type, Type>();
            CreatePageViewModelMappings();
        }
        private void CreatePageViewModelMappings()
        {
            _mappings.Add(typeof(ILoginVM), typeof(LoginPage));

        }
        public Task NavigateBackAsync()
        {
            throw new NotImplementedException();
        }
        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }

            return _mappings[viewModelType];
        }
        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"Mapping type for {viewModelType} is not a page");
            }

            // Page page = Activator.CreateInstance(pageType) as Page;
            Page page = ViewModelLocator.Instance.Resolve(pageType) as Page;

            //ViewModelBase viewModel = ViewModelLocator.Instance.Resolve(viewModelType) as ViewModelBase;
            //  page.BindingContext = viewModel;

            if (page is IPageWithParameters)
            {
                ((IPageWithParameters)page).InitializeWith(parameter);
            }

            return page;
        }
        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            
            Page page = CreateAndBindPage(viewModelType, parameter);
             
            if (page is MainPage)
            {

                if (CurrentApplication?.MainPage?.Navigation?.NavigationStack != null)
                {
                    bool found = false;
                    foreach (var pageInStack in CurrentApplication?.MainPage?.Navigation?.NavigationStack)
                    {
                        if (pageInStack is MainPage)
                        {
                            found = true;
                            await CurrentApplication.MainPage.Navigation.PopToRootAsync();
                            return;
                        }
                    }
                    if (!found)
                    {
                        CurrentApplication.MainPage = new CustomNavigationPage(page);
                    }
                }
                 
            }
            
            else
            {
                var navigationPage = CurrentApplication.MainPage as CustomNavigationPage;
                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    CurrentApplication.MainPage = new CustomNavigationPage(page);
                }
            }

            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
            //  GC.Collect();
        }
        public Task InitializeAsync(string AppLinkUrl)
        {
            //Page page = CreateAndBindPage(typeof(ILoginVM), null);
            return NavigateToAsync<ILoginVM>();
        }
            public Task NavigateToAsync<TViewModel>() where TViewModel : IViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : IViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }
    }
}
