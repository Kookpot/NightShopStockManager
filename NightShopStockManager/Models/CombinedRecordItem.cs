using SQLite;
using System;

namespace NightShopStockManager
{
    public class CombinedRecordItem
    {
        public int Item { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}