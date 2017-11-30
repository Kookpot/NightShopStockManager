using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class ItemManagementPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;
        private bool _select;

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

        #region Command ItemAdd

        private Command _itemAdd;
        public Command ItemAdd
        {
            get { return _itemAdd ?? (_itemAdd = new Command(async() => await OnItemAdd())); }
        }

        private async Task OnItemAdd()
        {
            await _navigationService.NavigateAsync("ItemPage");
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

        public ItemManagementPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public async override void OnNavigatedTo(NavigationParameters parameters) {
            Items = new RangeEnabledObservableCollection<Item>();

            if (parameters.ContainsKey("Barcode"))
                SearchValue = (string)parameters["Barcode"];

            if (parameters.ContainsKey("Select"))
            {
                _parameters = parameters;
                _select = (bool)parameters["Select"];
            }

            if (string.IsNullOrEmpty(SearchValue))
                Items.InsertRange(await App.Database.GetItemsAsync());
            else
                await Search();
        }

        private async Task OnItemSelect(Item itm)
        {
            if (itm != null)
            {
                if (_select)
                {
                    _parameters.Add("Item", itm);
                    await _navigationService.GoBackAsync(_parameters);
                }
                else
                {
                    var param = new NavigationParameters
                    {
                        { "Item", itm }
                    };
                    await _navigationService.NavigateAsync("ItemPage", param);
                }
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