namespace NightShopStockManager
{
    public class SummaryItem
    {
        public int Amount { get; set; }
        public string Name { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalPrice { get { return SellPrice * Amount; } }
    }
}