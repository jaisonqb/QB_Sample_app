using System;
using Xamarin.Forms;
using QBurst.Views;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using QBurst.ViewModels.Base;
using QBurst.Navigation;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace QBurst
{
	public partial class App : Application
	{
		
		public App ()
		{
			InitializeComponent();

            InitNavigation(null);
            //MainPage = new MainPage();
		}
        private Task InitNavigation(string AppLinkUrl)
        {
            var navigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            return navigationService.InitializeAsync(AppLinkUrl);
        }
        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
