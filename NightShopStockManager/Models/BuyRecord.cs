﻿using SQLite;
using System;

namespace NightShopStockManager
{
    public class BuyRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Item { get; set; }
        public int Supplier { get; set; }
        public int Amount { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}