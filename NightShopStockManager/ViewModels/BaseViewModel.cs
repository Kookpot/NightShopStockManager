using Prism.Mvvm;
using Prism.Navigation;

namespace NightShopStockManager.ViewModels
{
    public class BaseViewModel : BindableBase, INavigationAware
    {
        protected readonly INavigationService _navigationService;

        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        public virtual void OnNavigatedTo(NavigationParameters parameters) { }

        public virtual void OnNavigatingTo(NavigationParameters parameters) { }

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}