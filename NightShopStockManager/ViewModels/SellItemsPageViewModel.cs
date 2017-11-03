using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class SellItemsPageViewModel : BaseViewModel
    {
        #region Members

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set { SetProperty(ref _counter, value); }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set {
                SetProperty(ref _totalPrice, value);
                RaisePropertyChanged("Total");
            }
        }

        private string _total;
        public string Total
        {
            get { return "€ " + _totalPrice; }
            set { SetProperty(ref _total, value); }
        }

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

        private Dictionary<int, int> _currentItems = new Dictionary<int, int>();
        private Item _lastItem;

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

        #region Command Plus

        private Command _plusCommand;
        public Command PlusCommand
        {
            get { return _plusCommand ?? (_plusCommand = new Command(() => OnPlus())); }
        }

        private void OnPlus()
        {
            if (_lastItem != null)
            {
                _currentItems[_lastItem.ID]++;
                Counter++;
                TotalPrice += _lastItem.SellPrice;
            }
        }

        #endregion

        #region Command Minus

        private Command _minusCommand;
        public Command MinusCommand
        {
            get { return _minusCommand ?? (_minusCommand = new Command(() => OnMinus())); }
        }

        private void OnMinus()
        {
            if (Counter > 0 && _lastItem != null)
            {
                _currentItems[_lastItem.ID]--;
                TotalPrice -= _lastItem.SellPrice;
                Counter--;
            }
        }

        #endregion

        #region Command Cancel

        private Command _cancelCommand;
        public Command CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new Command(() => OnCancel())); }
        }

        private void OnCancel()
        {
            _currentItems = new Dictionary<int, int>();
            _lastItem = null;
            TotalPrice = 0;
            Counter = 0;
        }

        #endregion

        #region Command Summary

        private Command _summaryCommand;
        public Command SummaryCommand
        {
            get { return _summaryCommand ?? (_summaryCommand = new Command(() => OnSummary())); }
        }

        private void OnSummary()
        {
            var parameters = new NavigationParameters
            {
                { "LastItem", _lastItem },
                { "Counter", Counter },
                { "CurrentItems", _currentItems }
            };
            _navigationService.NavigateAsync("SummaryPage", parameters);
        }

        #endregion

        #region Command Done

        private Command _doneCommand;
        public Command DoneCommand
        {
            get { return _doneCommand ?? (_doneCommand = new Command(async() => await OnDone())); }
        }

        private async Task OnDone()
        {
            var records = new List<int>();

            var transaction = new Transaction()
            {
                DateTime = DateTime.Now,
                TotalPrice = TotalPrice
            };
            transaction.ID = await App.Database.SaveTransactionAsync(transaction);


            foreach (var id in _currentItems.Keys)
            {
                var itm = await App.Database.GetItemByIdAsync(id);
                var stockItems = (await App.Database.GetStockItemsAsync()).Where(x => x.Item == id).OrderBy(x => x.ExpiryDate).ToList();
                var count = _currentItems[id];
                while(count > 0)
                {
                    var first = stockItems.FirstOrDefault();
                    if (first != null)
                    {
                        if (first.CurrentCount <= count)
                        {
                            await App.Database.DeleteStockItemAsync(first);
                            count -= first.CurrentCount;
                            stockItems.Remove(first);
                            
                        }
                        else
                        {
                            first.CurrentCount -= count; 
                            await App.Database.SaveStockItemAsync(first);
                            count = 0;
                        }
                    }
                }

                var recordId = await App.Database.SaveRecordItemAsync(new RecordItem()
                {
                    DateTime = DateTime.Now,
                    SellPrice = itm.SellPrice,
                    Item = itm.ID,
                    Amount = _currentItems[id],
                    TotalPrice = itm.SellPrice * _currentItems[id],
                    Transaction = transaction.ID
                });
            }

            OnCancel();
        }

        #endregion

        #region Constructor

        public SellItemsPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Methods

        private async Task HandleBarcode(string barcode)
        {
            var item = (await App.Database.SearchItemsAsync(barcode)).FirstOrDefault();
            if (item != null)
            {
                if (_currentItems.ContainsKey(item.ID))
                {
                    _currentItems[item.ID]++;
                }
                else
                {
                    _currentItems.Add(item.ID, 1);
                }
                _lastItem = item;
                BackGroundColor = Color.DarkOliveGreen;
                DependencyService.Get<IMedia>().PlayOk();
                TotalPrice += item.SellPrice; 
                Counter = 1;
            }
            else
            {
                BackGroundColor = Color.Red;
                Counter = 0;
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
            if (parameters.ContainsKey("LastItem"))
            {
                _lastItem = (Item)parameters["LastItem"];
            }
            if (parameters.ContainsKey("Counter"))
            {
                Counter = (int)parameters["Counter"];
            }
            if (parameters.ContainsKey("CurrentItems"))
            {
                _currentItems = (Dictionary<int, int>)parameters["CurrentItems"];
            }
        }

        #endregion
    }
}