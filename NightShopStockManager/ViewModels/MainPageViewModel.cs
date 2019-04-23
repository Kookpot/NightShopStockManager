using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Command Item Management

        private Command _itemManagement;
        public Command ItemManagement
        {
            get { return _itemManagement ?? (_itemManagement = new Command(async () => await OnItemManagement())); }
        }

        private async Task OnItemManagement()
        {
            await _navigationService.NavigateAsync("ItemManagementPage");
        }

        #endregion

        #region Command Stock Management

        private Command _stockManagement;
        public Command StockManagement
        {
            get { return _stockManagement ?? (_stockManagement = new Command(async () => await OnStockManagement())); }
        }

        private async Task OnStockManagement()
        {
            await _navigationService.NavigateAsync("StockManagementPage");
        }

        #endregion

        #region Command Buy Items

        private Command _buyItems;
        public Command BuyItems
        {
            get { return _buyItems ?? (_buyItems = new Command(async () => await OnBuyItems())); }
        }

        private async Task OnBuyItems()
        {
            await _navigationService.NavigateAsync("BuyItemsPage");
        }

        #endregion

        #region Constructors

        public MainPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion
    }
}