using System;
using System.ComponentModel;

namespace NightShopStockManager
{
    public class CombinedStockItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; set; }
        public int Item { get; set; }
        public int Supplier { get; set; }

        private int _currentCount;
        public int CurrentCount {
            get { return _currentCount; }
            set
            {
                if (_currentCount > 0)
                {
                    if (TotalPrice > 0)
                    {
                        _buyPrice = Math.Round(TotalPrice / CurrentCount, 2);
                        OnPropertyChanged("BuyPrice");
                    }
                    else if (BuyPrice > 0)
                    {
                        _totalPrice = Math.Round(value * BuyPrice, 2);
                        OnPropertyChanged("TotalPrice");
                    }
                }
                _currentCount = value;
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice {
            get { return _totalPrice; }
            set
            {
                if (CurrentCount > 0)
                {
                    _buyPrice = Math.Round(value / CurrentCount, 2);
                    OnPropertyChanged("BuyPrice");
                }
                _totalPrice = value;
            }
        }

        private decimal _buyPrice;
        public decimal BuyPrice
        {
            get { return _buyPrice; }
            set
            {
                if (CurrentCount > 0)
                {
                    _totalPrice = Math.Round(value * CurrentCount, 2);
                    OnPropertyChanged("TotalPrice");
                }
                _buyPrice = value;
            }
        }

        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                _expiryDate = value;
                OnPropertyChanged("ExpiryDate");
                OnPropertyChanged("HasExpired");
            }
        }

        public string Name { get; set; }
        public string SupplierName { get; set; }
        public string Barcode { get; set; }
        public decimal SellPrice { get; set; }

        public bool HasExpired {
            get { return ExpiryDate < DateTime.Now; }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public BuyRecord CreateBuyRecord()
        {
            return new BuyRecord()
            {
                ID = ID,
                Item = Item,
                Supplier = Supplier,
                Amount = CurrentCount,
                BuyPrice = BuyPrice,
                TotalPrice = TotalPrice,
                DateTime = DateTime.Now
            };
        }

        public StockItem CreateStockItem()
        {
            return new StockItem()
            {
                ID = ID,
                Item = Item,
                Supplier = Supplier,
                CurrentCount = CurrentCount,
                BuyPrice = BuyPrice,
                ExpiryDate = ExpiryDate
            };
        }

        public CombinedStockItem Clone()
        {
            return new CombinedStockItem()
            {
                ID = ID,
                Name = Name,
                Supplier = Supplier,
                SupplierName = SupplierName,
                Barcode = Barcode,
                SellPrice = SellPrice,
                BuyPrice = BuyPrice,
                ExpiryDate = ExpiryDate,
                CurrentCount = CurrentCount,
                Item = Item
            };
        }
    }
}