using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class ItemPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private Item _item;
        public Item Item {
            get { return _item; }
            set { SetProperty(ref _item, value, "CanDelete"); }
        }

        public bool CanDelete => Item != null && Item.ID != 0;

        #endregion

        #region Command cancel

        private Command _cancel;
        public Command Cancel
        {
            get { return _cancel ?? (_cancel = new Command(async() => await OnCancel())); }
        }

        private async Task OnCancel()
        {
            await _navigationService.GoBackAsync();
        }

        #endregion

        #region Save

        private Command _save;
        public Command Save
        {
            get { return _save ?? (_save = new Command(async () => await OnSave())); }
        }

        private async Task OnSave()
        {
            await App.Database.SaveItemAsync(Item);
            var parameters = new NavigationParameters()
            {
                {"Item", Item }
            };
            await _navigationService.GoBackAsync(parameters);
        }

        #endregion

        #region Delete

        private Command _delete;
        public Command Delete
        {
            get { return _delete ?? (_delete = new Command(async () => await OnDelete())); }
        }

        private async Task OnDelete()
        {
            await App.Database.DeleteItemAsync(Item);
            await _navigationService.GoBackAsync();
        }

        #endregion

        #region Barcode Search

        private Command _barcodeSearch;
        public Command BarcodeSearch
        {
            get { return _barcodeSearch ?? (_barcodeSearch = new Command(async () => await OnBarcodeSearch())); }
        }

        private  async Task OnBarcodeSearch()
        {
            await _navigationService.NavigateAsync("BarcodePage", _parameters);
        }

        #endregion

        #region Constructors

        public ItemPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _parameters = parameters;
            if (parameters.ContainsKey("Item"))
            {
                Item = ((Item)parameters["Item"]).Clone();
            }
            else
            {
                Item = new Item();
            }
            if (parameters.ContainsKey("Barcode"))
            {
                Item.Barcode = (string)parameters["Barcode"];
            }
        }

        #endregion
    }
}