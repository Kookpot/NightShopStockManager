using Prism.Commands;
using Prism.Navigation;

namespace NightShopStockManager.ViewModels
{
    public class ManualEnteringPageViewModel : BaseViewModel
    {
        private string _barcode;
        public string  Barcode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }

        #region Entry Completed

        DelegateCommand _EntryCompleteCommand;
        public DelegateCommand EntryCompleteCommand => _EntryCompleteCommand ?? (_EntryCompleteCommand = new DelegateCommand(CompletingEntry));

        private async void CompletingEntry()
        {
            var parameters = new NavigationParameters { { "Barcode", Barcode } };
            await _navigationService.GoBackAsync(parameters);
        }

        #endregion

        #region Constructor 

        public ManualEnteringPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion
    }
}