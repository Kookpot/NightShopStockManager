using Prism.Navigation;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class BarcodePageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        #endregion

        #region Command ScanResult

        private Command _scanResult;
        public Command ScanResult
        {
            get { return _scanResult ?? (_scanResult = new Command((x) => OnScanResult((ZXing.Result)x))); }
        }

        private void OnScanResult(ZXing.Result result)
        {
            IsAnalyzing = false;
            Device.BeginInvokeOnMainThread(async () =>
            {
                _parameters.Add("Barcode", result.Text);
                await _navigationService.GoBackAsync(_parameters);
            });
        }

        #endregion

        #region Constructor

        public BarcodePageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _parameters = parameters;
        }
        
        #endregion
    }
}