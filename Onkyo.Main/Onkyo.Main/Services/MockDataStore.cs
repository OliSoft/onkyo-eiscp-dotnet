using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eiscp.Core.Commands;
using Onkyo.Main.Models;

namespace Onkyo.Main.Services
{
    public class MockDataStore : IDataStore<BaseCommand>
    {
        readonly List<BaseCommand> items;

        public MockDataStore()
        {
            items = new List<BaseCommand>();
            var mockItems = new List<BaseCommand>
            {
                new SLICommand("TV"),
                new SLICommand("DVD"),
                new SLICommand("phono"),
                new SLICommand("fm"),
                new SLICommand("PC")
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(BaseCommand item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(BaseCommand item)
        {
            var oldItem = items.Where((BaseCommand arg) => arg.Key == item.Key).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string key)
        {
            var oldItem = items.Where((BaseCommand arg) => arg.Key == key).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<BaseCommand> GetItemAsync(string key)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Key == key));
        }

        public async Task<IEnumerable<BaseCommand>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}