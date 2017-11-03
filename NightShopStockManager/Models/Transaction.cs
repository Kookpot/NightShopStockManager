using System;
using SQLite;

namespace NightShopStockManager
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}