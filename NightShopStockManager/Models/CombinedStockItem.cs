using System;
using System.ComponentModel;

namespace NightShopStockManager
{
    public class CombinedStockItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; set; }
        public int Item { get; set; }

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
        public string Barcode { get; set; }

        public bool HasExpired {
            get { return ExpiryDate < DateTime.Now; }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        public StockItem CreateStockItem()
        {
            return new StockItem
            {
                ID = ID,
                Item = Item,
                ExpiryDate = ExpiryDate
            };
        }

        public CombinedStockItem Clone()
        {
            return new CombinedStockItem
            {
                ID = ID,
                Name = Name,
                Barcode = Barcode,
                ExpiryDate = ExpiryDate,
                Item = Item
            };
        }
    }
}