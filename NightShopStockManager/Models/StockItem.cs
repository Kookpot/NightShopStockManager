using SQLite;
using System;

namespace NightShopStockManager
{
    public class StockItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Item { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}