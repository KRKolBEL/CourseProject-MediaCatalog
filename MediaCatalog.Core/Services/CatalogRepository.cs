using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;                 // <-- для JsonSerializer, JsonSerializerOptions
using System.Text.RegularExpressions;
using MediaCatalog.Core.Models;

namespace MediaCatalog.Core.Services
{
    public class CatalogRepository
    {
        private readonly string _filePath;
        public List<MediaItem> Items { get; private set; }

        public CatalogRepository(string filePath = "catalog.json")
        {
            _filePath = filePath;
            Items = new List<MediaItem>();
            Load();
        }

        public void Add(MediaItem item)
        {
            // Регулярные выражения (п.4)
            if (!Regex.IsMatch(item.Year.ToString(), @"^(19|20)\d{2}$"))
                throw new ArgumentException("Год должен быть в формате 19xx или 20xx");

            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("Название не может быть пустым");

            Items.Add(item);
            Logger.Log($"Добавлен элемент: {item.Title} ({item.Year})");
            Save();
        }

        public void Delete(Guid id)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                Items.Remove(item);
                Logger.Log($"Удален элемент: {item.Title}");
                Save();
            }
        }

        public IEnumerable<MediaItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Items;
            return Items.Where(x => x.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                                 || x.Genre.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        private void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Items, options);
            File.WriteAllText(_filePath, json);
        }

        private void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                Items = JsonSerializer.Deserialize<List<MediaItem>>(json) ?? new List<MediaItem>();
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка загрузки JSON: {ex.Message}");
            }
        }
    }
}