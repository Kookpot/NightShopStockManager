using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class StockManagementPageViewModel : BaseViewModel
    {
        #region Members

        private bool _warning;
        public bool Warning
        {
            get { return _warning; }
            set
            {
                SetProperty(ref _warning, value);
#pragma warning disable 4014
                Search();
#pragma warning restore 4014
            }
        }

        private RangeEnabledObservableCollection<CombinedStockItem> _items;
        public RangeEnabledObservableCollection<CombinedStockItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private string _searchValue;
        public string SearchValue
        {
            get { return _searchValue; }
            set
            {
                SetProperty(ref _searchValue, value);
#pragma warning disable 4014
                Search();
#pragma warning restore 4014
            }
        }

        private CombinedStockItem _itemSelected;
        public CombinedStockItem ItemSelected
        {
            get
            {
                return _itemSelected;
            }
            set
            {
                if (_itemSelected != value)
                {
                    SetProperty(ref _itemSelected, value);
#pragma warning disable 4014
                    OnItemSelect(value);
#pragma warning restore 4014
                }
            }
        }

        #endregion

        #region Command ItemAdd

        private Command _itemAdd;
        public Command ItemAdd
        {
            get { return _itemAdd ?? (_itemAdd = new Command(async () => await OnItemAdd())); }
        }

        private async Task OnItemAdd()
        {
            await _navigationService.NavigateAsync("StockItemPage");
        }

        #endregion

        #region Command Barcode

        private Command _barcodeSearch;
        public Command BarcodeSearch
        {
            get { return _barcodeSearch ?? (_barcodeSearch = new Command(async (x) => await OnBarcodeSearch())); }
        }

        private async Task OnBarcodeSearch()
        {
            await _navigationService.NavigateAsync("BarcodePage");
        }

        #endregion

        #region Constructors

        public StockManagementPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            Items = new RangeEnabledObservableCollection<CombinedStockItem>();
            if (parameters.ContainsKey("Barcode"))
                SearchValue = (string)parameters["Barcode"];

            if (string.IsNullOrEmpty(SearchValue))
                Items.InsertRange(await App.Database.GetStockItemsAsync());
            else
                await Search();
        }

        private async Task OnItemSelect(CombinedStockItem itm)
        {
            if (itm != null)
            {
                var param = new NavigationParameters
                {
                    { "StockItem", itm.Clone() }
                };
                await _navigationService.NavigateAsync("StockItemPage", param);
            }
        }

        private async Task Search()
        {
            Items.Clear();
            Items.InsertRange(await App.Database.SearchStockItemsAsync(SearchValue, Warning));
        }

        #endregion
    }
}