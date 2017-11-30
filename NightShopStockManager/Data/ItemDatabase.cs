using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.Linq;
using System;

namespace NightShopStockManager
{
	public class ItemDatabase
	{
		readonly SQLiteAsyncConnection database;

		public ItemDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);
			database.CreateTableAsync<Item>().Wait();
            database.CreateTableAsync<StockItem>().Wait();
            database.CreateTableAsync<Supplier>().Wait();
            database.CreateTableAsync<RecordItem>().Wait();
            database.CreateTableAsync<Transaction>().Wait();
            database.CreateTableAsync<ThrowAwayRecord>().Wait();
            database.CreateTableAsync<BuyRecord>().Wait();
        }

        #region Items

        private List<Item> _items;
		public async Task<List<Item>> GetItemsAsync()
		{
            await CreateOrGetItems();
            return _items.OrderBy(x => x.Name).ToList();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            await CreateOrGetItems();
            return _items.SingleOrDefault(x => x.ID == id);
        }

        private async Task CreateOrGetItems()
        {
            if (_items == null)
                _items = await database.Table<Item>().ToListAsync();
        }

        public async Task<List<Item>> SearchItemsAsync(string search)
        {
            var items = await GetItemsAsync();
            var searchToLower = search.ToLower();
            return items.Where(x => (x.Name != null && x.Name.ToLower().Contains(searchToLower)) || (x.Barcode != null && x.Barcode.Contains(searchToLower))).ToList();
        }

        public Task<int> SaveItemAsync(Item item)
        {
            if (item.ID != 0)
            {
                var found = _items.SingleOrDefault(x => x.ID == item.ID);
                if (found != null)
                    _items.Remove(found);

                _items.Add(item);
                var lst = _combinedStockItems.Where(x => x.Item == item.ID);
                foreach(var combo in lst)
                {
                    combo.Name = item.Name;
                    combo.SellPrice = item.SellPrice;
                }
                return database.UpdateAsync(item);
            }
            else
            {
                _items.Add(item);
                return database.InsertAsync(item);
            }
        }

        public async Task<int> DeleteItemAsync(Item item)
        {
            await RemoveItemById(item.ID);
            return await database.DeleteAsync(item);
        }

        private async Task RemoveItemById(int id)
        {
            await CreateOrGetCombinedStockItems();
            var found = _items.SingleOrDefault(x => x.ID == id);
            if (found != null)
                _items.Remove(found);

            var foundStock = _combinedStockItems.Where(x => x.Item == id);
            foreach (var stock in foundStock)
            {
                _combinedStockItems.Remove(stock);
                await database.DeleteAsync(stock.CreateStockItem());
            }
        }

        #endregion

        #region Stock items

        private List<CombinedStockItem> _combinedStockItems;
        public async Task<List<CombinedStockItem>> GetStockItemsAsync()
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
            return _combinedStockItems;
        }


        public async Task<List<CombinedStockItem>> SearchStockItemsAsync(string search, bool warning)
        {
            var items = await GetStockItemsAsync();
            IEnumerable<CombinedStockItem> query = items;
            if (!string.IsNullOrEmpty(search))
            {
                var searchToLower = search.ToLower();
                query = items.Where(x => (x.Name != null && x.Name.ToLower().Contains(searchToLower)) || (x.Barcode != null && x.Barcode.Contains(searchToLower)) || (x.SupplierName != null && x.SupplierName.ToLower().Contains(searchToLower)));
            }
            if (warning)
                query = query.Where(x => x.ExpiryDate <= DateTime.Now);

            return query.ToList();
        }

        public async Task<List<CombinedStockItem>> LinkToItems(List<StockItem> lst)
        {
            var itms = await GetItemsAsync();
            var suppliers = await GetSuppliersAsync();
            var result = new List<CombinedStockItem>();
            foreach (var stItem in lst)
            {
                var itm = itms.SingleOrDefault(x => x.ID == stItem.Item);
                var sup = suppliers.SingleOrDefault(x => x.ID == stItem.Supplier);
                if (itm != null && sup != null)
                {
                    result.Add(new CombinedStockItem
                    {
                        ID = stItem.ID,
                        Item = stItem.Item,
                        BuyPrice = stItem.BuyPrice,
                        CurrentCount = stItem.CurrentCount,
                        ExpiryDate = stItem.ExpiryDate,
                        Name = itm.Name,
                        SupplierName = sup.Name,
                        Supplier = stItem.Supplier,
                        SellPrice = itm.SellPrice,
                        Barcode = itm.Barcode
                    });
                }
                else
                {
                    await database.DeleteAsync(stItem);
                }
            }
            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task<int> SaveStockItemAsync(CombinedStockItem item)
        {
            await CreateOrGetCombinedStockItems();
            if (item.ID != 0)
            {
                await RemoveStockItemById(item.ID);
                _combinedStockItems.Add(item);
                return await database.UpdateAsync(item.CreateStockItem());
            }
            else
            {
                _combinedStockItems.Add(item);
                await database.InsertAsync(item.CreateBuyRecord());
                return await database.InsertAsync(item.CreateStockItem());
            }
        }

        private async Task CreateOrGetCombinedStockItems()
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
        }

        private async Task RemoveStockItemById(int id)
        {
            await CreateOrGetCombinedStockItems();
            var found = _combinedStockItems.SingleOrDefault(x => x.ID == id);
            if (found != null)
                _combinedStockItems.Remove(found);
        }

        public async Task<int> DeleteStockItemAsync(CombinedStockItem item)
        {
            await RemoveStockItemById(item.ID);
            return await database.DeleteAsync(item.CreateStockItem());
        }

        public async Task<int> AddThowAwayRecord(CombinedStockItem item)
        {
            var rec = new ThrowAwayRecord
            {
                Amount = item.CurrentCount,
                DateTime = DateTime.Now,
                BuyPrice = item.BuyPrice,
                TotalPrice = item.TotalPrice,
                Item = item.Item
            };
            return await database.InsertAsync(rec);
        }

        #endregion

        #region Suppliers

        private List<Supplier> _suppliers;
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            if (_suppliers == null)
                _suppliers = await database.Table<Supplier>().ToListAsync();

            return _suppliers;
        }

        public async Task<List<Supplier>> SearchSuppliersAsync(string search)
        {
            var suppliers = await GetSuppliersAsync();
            var searchToLower = search.ToLower();
            return suppliers.Where(x => x.Name != null && x.Name.ToLower().Contains(searchToLower)).ToList();
        }

        public Task<int> SaveSupplierAsync(Supplier supplier)
        {
            if (supplier.ID != 0)
            {
                var found = _suppliers.SingleOrDefault(x => x.ID == supplier.ID);
                if (found != null)
                    _suppliers.Remove(found);

                var lst = _combinedStockItems.Where(x => x.Supplier == supplier.ID);
                foreach (var combo in lst)
                    combo.SupplierName = supplier.Name;

                _suppliers.Add(supplier);
                return database.UpdateAsync(supplier);
            }
            else
            {
                _suppliers.Add(supplier);
                return database.InsertAsync(supplier);
            }
        }

        public async Task<int> DeleteSupplierAsync(Supplier item)
        {
            await RemoveSupplierById(item.ID);
            return await database.DeleteAsync(item);
        }

        private async Task RemoveSupplierById(int id)
        {
            await CreateOrGetCombinedStockItems();
            var found = _suppliers.SingleOrDefault(x => x.ID == id);
            if (found != null)
                _suppliers.Remove(found);

            var foundStock = _combinedStockItems.Where(x => x.Supplier == id);
            foreach (var stock in foundStock)
            {
                _combinedStockItems.Remove(stock);
                await database.DeleteAsync(stock.CreateStockItem());
            }
        }

        #endregion

        #region Transaction
        
        public async Task<int> SaveTransactionAsync(Transaction transaction)
        {
            return await database.InsertAsync(transaction);
        }

        public async Task<int> SaveRecordItemAsync(RecordItem recordItem)
        {
            return await database.InsertAsync(recordItem);
        }

        #endregion

        #region Reporting

        public async Task<decimal> GetIncome(DateTime start, DateTime end)
        {
            return (await database.QueryAsync<Transaction>($"SELECT * FROM [Transaction] WHERE DateTime > ? AND DateTime < ?", start.Ticks, end.AddDays(1).Ticks)).Sum(x => x.TotalPrice);
        }

        public async Task<decimal> GetExpenses(DateTime start, DateTime end)
        {
            return (await database.QueryAsync<BuyRecord>($"SELECT * FROM [BuyRecord] WHERE DateTime > ? AND DateTime < ?", start.Ticks, end.AddDays(1).Ticks)).Sum(x => x.TotalPrice);
        }

        public async Task<decimal> GetExpiredExpenses(DateTime start, DateTime end)
        {
            return (await database.QueryAsync<ThrowAwayRecord>($"SELECT * FROM [ThrowAwayRecord] WHERE DateTime > ? AND DateTime < ?", start.Ticks, end.AddDays(1).Ticks)).Sum(x => x.TotalPrice);
        }

        public async Task<Dictionary<DateTime, int>> GetSoldDataAsync(Item itm, DateTime start, DateTime end)
        {
            var result = new Dictionary<DateTime, int>();
            var itms = (await database.QueryAsync<RecordItem>($"SELECT * FROM [RecordItem] WHERE Item = ? AND DateTime > ? AND DateTime < ?", itm.ID, start.Ticks, end.AddDays(1).Ticks));
            var currentDate = start;
            while (currentDate <= end)
            {
                result.Add(currentDate, 0);
                currentDate = currentDate.AddDays(1);
            }
            foreach (var recordItm in itms)
            {
                var date = new DateTime(recordItm.DateTime.Year, recordItm.DateTime.Month, recordItm.DateTime.Day);
                result[date] += recordItm.Amount;
            }
            return result;
        }

        public async Task<List<CombinedRecordItem>> GetRecordItemsAsync()
        {
            var lst = await database.Table<RecordItem>().ToListAsync();
            await CreateOrGetItems();
            var cRecs = new List<CombinedRecordItem>();
            foreach(var itm in lst)
            {
                var found = _items.SingleOrDefault(x => x.ID == itm.ID);
                if (found != null)
                {
                    cRecs.Add(new CombinedRecordItem
                    {
                        Name = found.Name,
                        Item = found.ID,
                        Amount = itm.Amount,
                        DateTime = itm.DateTime,
                        SellPrice = itm.SellPrice
                    });
                }
            }
            return cRecs;
        }

        public async Task<List<CombinedThrowAwayRecord>> GetThrowAwayRecordAsync()
        {
            var lst = await database.Table<ThrowAwayRecord>().ToListAsync();
            await CreateOrGetItems();
            var cRecs = new List<CombinedThrowAwayRecord>();
            foreach (var itm in lst)
            {
                var found = _items.SingleOrDefault(x => x.ID == itm.ID);
                if (found != null)
                {
                    cRecs.Add(new CombinedThrowAwayRecord
                    {
                        Name = found.Name,
                        Item = found.ID,
                        Amount = itm.Amount,
                        DateTime = itm.DateTime,
                        BuyPrice = itm.BuyPrice,
                        TotalPrice = itm.TotalPrice
                    });
                }
            }
            return cRecs;
        }

        public async Task<List<CombinedBuyRecord>> GetBuyRecordsAsync()
        {
            var lst = await database.Table<BuyRecord>().ToListAsync();
            await CreateOrGetItems();
            var cRecs = new List<CombinedBuyRecord>();
            await GetSuppliersAsync();
            foreach (var itm in lst)
            {
                var found = _items.SingleOrDefault(x => x.ID == itm.ID);
                var foundSupplier = _suppliers.SingleOrDefault(x => x.ID == itm.Supplier);
                if (found != null) {
                    var comb = new CombinedBuyRecord
                    {
                        Name = found.Name,
                        Item = found.ID,
                        Amount = itm.Amount,
                        DateTime = itm.DateTime,
                        BuyPrice = itm.BuyPrice,
                        TotalPrice = itm.TotalPrice
                    };
                    if (foundSupplier != null)
                    {
                        comb.Supplier = foundSupplier.ID;
                        comb.SupplierName = foundSupplier.Name;
                    }
                    cRecs.Add(comb);
                }
            }
            return cRecs;
        }

        #endregion
    }
}