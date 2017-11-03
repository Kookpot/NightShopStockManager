using SQLite;

namespace NightShopStockManager
{
    public class Supplier
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }

        public Supplier Clone()
        {
            return new Supplier()
            {
                ID = ID,
                Name = Name
            };
        }
    }
}