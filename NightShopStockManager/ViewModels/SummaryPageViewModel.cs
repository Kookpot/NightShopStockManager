using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SummaryPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private RangeEnabledObservableCollection<SummaryItem> _items;
        public RangeEnabledObservableCollection<SummaryItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public decimal TotalPrice
        {
            get { return _items == null ? 0 : _items.Sum(x => x.TotalPrice); }
        }

        #endregion

        #region Command cancel

        private Command _cancel;
        public Command Cancel
        {
            get { return _cancel ?? (_cancel = new Command(async () => await OnCancel())); }
        }

        private async Task OnCancel()
        {
            await _navigationService.GoBackAsync();
        }

        #endregion

        #region Constructors

        public SummaryPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            parameters = _parameters;
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            _parameters = parameters;
            if (parameters.ContainsKey("CurrentItems"))
            {
                var items = (Dictionary<int, int>)parameters["CurrentItems"];
                var sumItems = new RangeEnabledObservableCollection<SummaryItem>();
                foreach (var id in items.Keys) {
                    var dbItem = await App.Database.GetItemByIdAsync(id);
                    sumItems.Add(new SummaryItem()
                    {
                        Amount = items[id],
                        SellPrice = dbItem.SellPrice,
                        Name = dbItem.Name,
                    });
                }
                Items = sumItems;
                RaisePropertyChanged("TotalPrice");
            }
        }

        #endregion
    }
}