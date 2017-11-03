using SQLite;
using System;

namespace NightShopStockManager
{
    public class CombinedThrowAwayRecord
    {
        public int Item { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}