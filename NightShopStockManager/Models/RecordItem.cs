using SQLite;
using System;

namespace NightShopStockManager
{
    public class RecordItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Item { get; set; }
        public int Amount { get; set; }
        public int Transaction { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}