using Prism.Commands;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SellItemsPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
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
            get { return _scanResult ?? (_scanResult = new Command(async(x) => await OnScanResult((ZXing.Result)x))); }
        }

        private async Task OnScanResult(ZXing.Result result)
        {
            IsAnalyzing = false;
            await HandleBarcode(result.Text);
            IsAnalyzing = true;
        }

        #endregion

        #region Constructor

        public SellItemsPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        private async Task HandleBarcode(string barcode)
        {
            var stockItem = (await App.Database.SearchStockItemsAsync(barcode, false)).OrderBy(x => x.ExpiryDate).FirstOrDefault();
            if (stockItem != null)
            {
                stockItem.CurrentCount--;
                if (stockItem.CurrentCount == 0)
                {
                    await App.Database.DeleteStockItemAsync(stockItem);
                }
                else
                {
                    await App.Database.SaveStockItemAsync(stockItem);
                }
                BackGroundColor = Color.DarkOliveGreen;
                DependencyService.Get<IMedia>().PlayOk();
            }
            else
            {
                BackGroundColor = Color.Red;
                DependencyService.Get<IMedia>().PlayNOk();
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                Device.StartTimer(new System.TimeSpan(0, 0, 1), () =>
                {
                    BackGroundColor = Color.Transparent;
                    return false;
                });
            });
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Barcode"))
            {
                await HandleBarcode((string)parameters["Barcode"]);
            }
        }

        #endregion
    }
}