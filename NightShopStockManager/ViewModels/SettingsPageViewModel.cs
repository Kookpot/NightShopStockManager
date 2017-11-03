using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
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

        #region Command Supplier Management

        private Command _supplierManagement;
        public Command SupplierManagement
        {
            get { return _supplierManagement ?? (_supplierManagement = new Command(async () => await OnSupplierManagement())); }
        }

        private async Task OnSupplierManagement()
        {
            await _navigationService.NavigateAsync("SupplierManagementPage");
        }

        #endregion

        #region Constructors

        public SettingsPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion
    }
}