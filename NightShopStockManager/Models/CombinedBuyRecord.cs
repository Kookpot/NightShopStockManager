using SQLite;
using System;

namespace NightShopStockManager
{
    public class CombinedBuyRecord
    {
        public int Item { get; set; }
        public string Name { get; set; }
        public int Supplier { get; set; }
        public string SupplierName { get; set; }
        public int Amount { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}