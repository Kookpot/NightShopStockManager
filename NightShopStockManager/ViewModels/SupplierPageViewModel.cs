using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SupplierPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private Supplier _supplier;
        public Supplier Supplier
        {
            get { return _supplier; }
            set {
                SetProperty(ref _supplier, value);
                RaisePropertyChanged("CanDelete");
            }
        }

        public bool CanDelete => Supplier != null && Supplier.ID != 0;

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
            await App.Database.SaveSupplierAsync(Supplier);
            var parameters = new NavigationParameters()
            {
                {"Supplier", Supplier }
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
            await App.Database.DeleteSupplierAsync(Supplier);
            await _navigationService.GoBackAsync();
        }

        #endregion

        #region Constructors

        public SupplierPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _parameters = parameters;
            if (parameters.ContainsKey("Supplier"))
            {
                Supplier = ((Supplier)parameters["Supplier"]).Clone();
            }
            else
            {
                Supplier = new Supplier();
            }
        }

        #endregion
    }
}