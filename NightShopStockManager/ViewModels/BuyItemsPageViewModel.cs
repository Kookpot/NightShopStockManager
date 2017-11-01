using Prism.Commands;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class BuyItemsPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        private Color _backGroundColor = Color.Transparent;
        public Color BackGroundColor
        {
            get { return _backGroundColor; }
            set { SetProperty(ref _backGroundColor, value); }
        }

        #endregion

        #region Command Manual Enter

        DelegateCommand _manualCommand;

        public DelegateCommand ManualCommand => _manualCommand != null ? _manualCommand : (_manualCommand = new DelegateCommand(ManualEditing));
        private void ManualEditing()
        {
            IsAnalyzing = false;
            _navigationService.NavigateAsync("ManualEnteringPage");
        }

        #endregion

        #region Command ScanResult

        private Command _scanResult;
        public Command ScanResult
        {
            get { return _scanResult ?? (_scanResult = new Command(async (x) => await OnScanResult((ZXing.Result)x))); }
        }

        private async Task OnScanResult(ZXing.Result result)
        {
            IsAnalyzing = false;
            await HandleBarcode(result.Text);
            IsAnalyzing = true;
        }

        #endregion

        #region Constructor

        public BuyItemsPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        private async Task HandleBarcode(string barcode)
        {
            IsScanning = false;
            var item = (await App.Database.SearchItemsAsync(barcode)).FirstOrDefault();
            if (item != null)
            {
                var parameters = new NavigationParameters()
                {
                    {"Item", item }
                };
                await _navigationService.NavigateAsync("StockItemPage", parameters);
            }
            else
            {
                var parameters = new NavigationParameters()
                {
                    {"Barcode", barcode }
                };
                await _navigationService.NavigateAsync("ItemPage", parameters);
            }
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Barcode"))
            {
                await HandleBarcode((string)parameters["Barcode"]);
            }
            else if (parameters.ContainsKey("Item"))
            {
                await _navigationService.NavigateAsync("StockItemPage", parameters);
            }
            else
            {
                IsScanning = true;
            }
        }

        #endregion
    }
}