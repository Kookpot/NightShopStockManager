using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SupplierManagementPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;
        private bool _select;

        private RangeEnabledObservableCollection<Supplier> _suppliers;
        public RangeEnabledObservableCollection<Supplier> Suppliers
        {
            get { return _suppliers; }
            set { SetProperty(ref _suppliers, value); }
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

        private Supplier _supplierSelected;
        public Supplier SupplierSelected
        {
            get
            {
                return _supplierSelected;
            }
            set
            {
                if (_supplierSelected != value)
                {
                    SetProperty(ref _supplierSelected, value);
#pragma warning disable 4014
                    OnSupplierSelect(value);
#pragma warning restore 4014
                }
            }
        }

        #endregion

        #region Command Supplier Add

        private Command _supplierAdd;
        public Command SupplierAdd
        {
            get { return _supplierAdd ?? (_supplierAdd = new Command(async() => await OnSupplierAdd())); }
        }

        private async Task OnSupplierAdd()
        {
            await _navigationService.NavigateAsync("SupplierPage");
        }

        #endregion

        #region Constructors

        public SupplierManagementPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public async override void OnNavigatedTo(NavigationParameters parameters) {
            Suppliers = new RangeEnabledObservableCollection<Supplier>();
            Suppliers.InsertRange(await App.Database.GetSuppliersAsync());
            if (parameters.ContainsKey("Select"))
            {
                _parameters = parameters;
                _select = (bool)parameters["Select"];
            }
        }

        private async Task OnSupplierSelect(Supplier itm)
        {
            if (itm != null)
            {
                if (_select)
                {
                    _parameters.Add("Supplier", itm);
                    await _navigationService.GoBackAsync(_parameters);
                }
                else
                {
                    var param = new NavigationParameters
                    {
                        { "Supplier", itm }
                    };
                    await _navigationService.NavigateAsync("SupplierPage", param);
                }
            }
        }

        private async Task Search()
        {
            Suppliers.Clear();
            Suppliers.InsertRange(await App.Database.SearchSuppliersAsync(SearchValue));
        }

        #endregion
    }
}