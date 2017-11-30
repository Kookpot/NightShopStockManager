using SQLite;
using System.ComponentModel;

namespace NightShopStockManager
{
	public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
		public string Name { get; set; }

        private string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                OnPropertyChanged("Barcode");
            }
        }

        public decimal SellPrice { get; set; }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Item Clone()
        {
            return new Item
            {
                ID = ID,
                Name = Name,
                Barcode = Barcode,
                SellPrice = SellPrice
            };
        }
    }
}