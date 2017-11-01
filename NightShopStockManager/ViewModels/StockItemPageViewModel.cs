﻿using Prism.Navigation;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class StockItemPageViewModel : BaseViewModel
    {
        #region Members

        private NavigationParameters _parameters;

        private CombinedStockItem _stockItem;
        public CombinedStockItem StockItem
        {
            get { return _stockItem; }
            set { SetProperty(ref _stockItem, value); }
        }

        public bool CanDelete => StockItem != null && StockItem.ID != 0;

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

        #region Save

        private Command _save;
        public Command Save
        {
            get { return _save ?? (_save = new Command(async () => await OnSave())); }
        }

        private async Task OnSave()
        {
            await App.Database.SaveStockItemAsync(StockItem);
            await _navigationService.GoBackAsync();
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
            await App.Database.DeleteStockItemAsync(StockItem);
            await _navigationService.GoBackAsync();
        }

        #endregion

        #region Barcode Search

        private Command _itemSearch;
        public Command ItemSearch
        {
            get { return _itemSearch ?? (_itemSearch = new Command(async () => await OnItemSearch())); }
        }

        private async Task OnItemSearch()
        {
            await _navigationService.NavigateAsync("SearchItemPage", _parameters);
        }

        #endregion

        #region Constructors

        public StockItemPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _parameters = parameters;
            if (parameters.ContainsKey("StockItem"))
            {
                StockItem = (CombinedStockItem)parameters["StockItem"];
            }
            else
            {
                StockItem = new CombinedStockItem
                {
                    ExpiryDate = DateTime.Now
                };
            }
            if (parameters.ContainsKey("Item"))
            {
                var itm = (Item)parameters["Item"];
                StockItem.Barcode = itm.Barcode;
                StockItem.Item = itm.ID;
                StockItem.Name = itm.Name;
                StockItem.SellPrice = itm.SellPrice;
            }
        }

        #endregion
    }
}