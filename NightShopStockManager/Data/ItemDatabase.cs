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

        public async Task<List<Item>> SearchItemsAsync(string search)
        {
            var items = await GetItemsAsync();
            var searchToLower = search.ToLower();
            return items.Where(x => (x.Name != null && x.Name.ToLower().Contains(searchToLower)) || (x.Barcode != null && x.Barcode.Contains(searchToLower))).ToList();
        }

        public async Task<int> SaveItemAsync(Item item)
        {
            await CreateOrGetCombinedStockItems();
            if (item.ID != 0)
            {
                var found = _items.SingleOrDefault(x => x.ID == item.ID);
                if (found != null)
                    _items.Remove(found);

                _items.Add(item);
                var lst = _combinedStockItems.Where(x => x.Item == item.ID);
                foreach (var combo in lst)
                {
                    combo.Name = item.Name;
                }
                return await database.UpdateAsync(item);
            }
            else
            {
                _items.Add(item);
                await database.InsertAsync(item);
                var y = await database.Table<Item>().FirstOrDefaultAsync(x => x.Barcode == item.Barcode);
                item.ID = y.ID;
                return y.ID;
            }
        }

        public async Task<int> DeleteItemAsync(Item item)
        {
            await RemoveItemById(item.ID);
            return await database.DeleteAsync(item);
        }

        private async Task CreateOrGetItems()
        {
            if (_items == null)
                _items = await database.Table<Item>().ToListAsync();
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
            await CreateOrGetCombinedStockItems();
            return _combinedStockItems.OrderBy(x => x.ExpiryDate).ToList();
        }

        public async Task<List<CombinedStockItem>> SearchStockItemsAsync(string search, bool warning)
        {
            var items = await GetStockItemsAsync();
            IEnumerable<CombinedStockItem> query = items;
            if (!string.IsNullOrEmpty(search))
            {
                var searchToLower = search.ToLower();
                query = items.Where(x => (x.Name != null && x.Name.ToLower().Contains(searchToLower)) || (x.Barcode != null && x.Barcode.Contains(searchToLower)));
            }
            if (warning)
                query = query.Where(x => x.ExpiryDate <= DateTime.Now);

            return query.OrderBy(x => x.ExpiryDate).ToList();
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
                var id = await database.InsertAsync(item.CreateStockItem());
                item.ID = id;
                return id;
            }
        }

        public async Task<int> DeleteStockItemAsync(CombinedStockItem item)
        {
            await RemoveStockItemById(item.ID);
            return await database.DeleteAsync(item.CreateStockItem());
        }

        private async Task<List<CombinedStockItem>> LinkToItems(List<StockItem> lst)
        {
            var itms = await GetItemsAsync();
            var result = new List<CombinedStockItem>();
            foreach (var stItem in lst)
            {
                var itm = itms.SingleOrDefault(x => x.ID == stItem.Item);
                if (itm != null)
                {
                    result.Add(new CombinedStockItem
                    {
                        ID = stItem.ID,
                        Item = stItem.Item,
                        ExpiryDate = stItem.ExpiryDate,
                        Name = itm.Name,
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

        #endregion

    }
}