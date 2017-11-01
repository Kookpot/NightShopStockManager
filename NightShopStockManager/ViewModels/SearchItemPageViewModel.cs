using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SearchItemPageViewModel : BaseViewModel
    {
        #region Members

        private RangeEnabledObservableCollection<Item> _items;
        public RangeEnabledObservableCollection<Item> Items
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

        private Item _itemSelected;
        public Item ItemSelected
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

        public SearchItemPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public async override void OnNavigatedTo(NavigationParameters parameters) {
            Items = new RangeEnabledObservableCollection<Item>();
            if (parameters.ContainsKey("Barcode"))
            {
                SearchValue = (string)parameters["Barcode"];
            }
            if (string.IsNullOrEmpty(SearchValue))
            {
                Items.InsertRange(await App.Database.GetItemsAsync());
            }
            else
            {
                await Search();
            }
        }

        private async Task OnItemSelect(Item itm)
        {
            if (itm != null)
            {
                var param = new NavigationParameters
                {
                    { "Item", itm }
                };
                await _navigationService.GoBackAsync(param);
            }
        }

        private async Task Search()
        {
            Items.Clear();
            Items.InsertRange(await App.Database.SearchItemsAsync(SearchValue));
        }

        #endregion
    }
}