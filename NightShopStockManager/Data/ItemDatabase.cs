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
        }

        private List<Item> _items;
		public async Task<List<Item>> GetItemsAsync()
		{
            if (_items == null)
            {
                _items = await database.Table<Item>().ToListAsync();
            }
            return _items.OrderBy(x => x.Name).ToList();
        }

        public async Task<List<Item>> SearchItemsAsync(string search)
        {
            var items = await GetItemsAsync();
            var searchToLower = search.ToLower();
            return items.Where(x => x.Name.ToLower().Contains(searchToLower) || x.Barcode.Contains(searchToLower)).ToList();
        }

        public async Task<List<CombinedStockItem>> SearchStockItemsAsync(string search, bool warning)
        {
            var items = await GetStockItemsAsync();
            IEnumerable<CombinedStockItem> query = items;
            if (!string.IsNullOrEmpty(search))
            {
                var searchToLower = search.ToLower();
                query = items.Where(x => x.Name.ToLower().Contains(searchToLower) || x.Barcode.Contains(searchToLower));
            }
            if (warning)
            {
                query = query.Where(x => x.ExpiryDate <= DateTime.Now);
            }
            return query.ToList();
        }

        public async Task<List<CombinedStockItem>> SearchStockItemsExpiredAsync(string search)
        {
            var items = await GetStockItemsAsync();
            var searchToLower = search.ToLower();
            return items.Where(x => x.HasExpired && x.Name.ToLower().Contains(searchToLower) || x.Barcode.Contains(searchToLower)).ToList();
        }

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

        public async Task<List<CombinedStockItem>> GetStockItemsExpiredAsync()
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
            return _combinedStockItems.Where(x => x.HasExpired).ToList();
        }

        public async Task<List<CombinedStockItem>> LinkToItems(List<StockItem> lst)
        {
            var itms = await GetItemsAsync();
            var result = new List<CombinedStockItem>();
            foreach (var stItem in lst)
            {
                var itm = itms.SingleOrDefault(x => x.ID == stItem.Item);
                if (itm != null)
                {
                    result.Add(new CombinedStockItem()
                    {
                        ID = stItem.ID,
                        Item = stItem.Item,
                        BuyPrice = stItem.BuyPrice,
                        CurrentCount = stItem.CurrentCount,
                        ExpiryDate = stItem.ExpiryDate,
                        Name = itm.Name,
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

        public Task<int> SaveItemAsync(Item item)
		{
			if (item.ID != 0)
			{
                var found = _items.SingleOrDefault(x => x.ID == item.ID);
                if (found != null) {
                    _items.Remove(found);
                }
                _items.Add(item);
                return database.UpdateAsync(item);
			}
			else
            {
                _items.Add(item);
				return database.InsertAsync(item);
			}
		}

        public async Task<int> SaveStockItemAsync(CombinedStockItem item)
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
            if (item.ID != 0)
            {
                await RemoveStockItemById(item.ID);
                _combinedStockItems.Add(item);
                return await database.UpdateAsync(item.CreateStockItem());
            }
            else
            {
                _combinedStockItems.Add(item);
                return await database.InsertAsync(item.CreateStockItem());
            }
        }

        private async Task RemoveItemById(int id)
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
            var found = _items.SingleOrDefault(x => x.ID == id);
            if (found != null)
            {
                _items.Remove(found);
            }
            var foundStock = _combinedStockItems.Where(x => x.Item == id);
            foreach(var stock in foundStock)
            {
                _combinedStockItems.Remove(stock);
            }
        }

        private async Task RemoveStockItemById(int id)
        {
            if (_combinedStockItems == null)
            {
                var lst = await database.Table<StockItem>().ToListAsync();
                _combinedStockItems = await LinkToItems(lst);
            }
            var found = _combinedStockItems.SingleOrDefault(x => x.ID == id);
            if (found != null)
            {
                _combinedStockItems.Remove(found);
            }
        }

        public async Task<int> DeleteItemAsync(Item item)
		{
            await RemoveItemById(item.ID);
            return await database.DeleteAsync(item);
		}

        public async Task<int> DeleteStockItemAsync(CombinedStockItem item)
        {
            await RemoveStockItemById(item.ID);
            return await database.DeleteAsync(item.CreateStockItem());
        }
    }
}