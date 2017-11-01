﻿using System.Collections.Generic;
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
        }

        #region Items

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

        public Task<int> SaveItemAsync(Item item)
        {
            if (item.ID != 0)
            {
                var found = _items.SingleOrDefault(x => x.ID == item.ID);
                if (found != null)
                {
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
            {
                _items.Remove(found);
            }
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
                query = items.Where(x => x.Name.ToLower().Contains(searchToLower) || x.Barcode.Contains(searchToLower) || x.SupplierName.ToLower().Contains(searchToLower));
            }
            if (warning)
            {
                query = query.Where(x => x.ExpiryDate <= DateTime.Now);
            }
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
                    result.Add(new CombinedStockItem()
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
            {
                _combinedStockItems.Remove(found);
            }
        }

        public async Task<int> DeleteStockItemAsync(CombinedStockItem item)
        {
            await RemoveStockItemById(item.ID);
            return await database.DeleteAsync(item.CreateStockItem());
        }

        #endregion

        #region Suppliers

        private List<Supplier> _suppliers;
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            if (_suppliers == null)
            {
                _suppliers = await database.Table<Supplier>().ToListAsync();
            }
            return _suppliers;
        }

        public async Task<List<Supplier>> SearchSuppliersAsync(string search)
        {
            var suppliers = await GetSuppliersAsync();
            var searchToLower = search.ToLower();
            return suppliers.Where(x => x.Name.ToLower().Contains(searchToLower)).ToList();
        }

        public Task<int> SaveSupplierAsync(Supplier supplier)
        {
            if (supplier.ID != 0)
            {
                var found = _suppliers.SingleOrDefault(x => x.ID == supplier.ID);
                if (found != null)
                {
                    _suppliers.Remove(found);
                }
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
            {
                _suppliers.Remove(found);
            }
            var foundStock = _combinedStockItems.Where(x => x.Supplier == id);
            foreach (var stock in foundStock)
            {
                _combinedStockItems.Remove(stock);
                await database.DeleteAsync(stock.CreateStockItem());
            }
        }

        #endregion
    }
}