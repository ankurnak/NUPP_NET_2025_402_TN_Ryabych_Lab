using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO; // <--- ДОДАНО: Для роботи з файлами
using System.Text.Json; // <--- ДОДАНО: Для серіалізації JSON

// ... (Весь інший код CrudService залишається без змін)
public class CrudService<T> : ICrudService<T> where T : class
{
    private readonly Dictionary<Guid, T> _storage = new Dictionary<Guid, T>();

    // ... (Методи GetId, Create, Read, ReadAll, Update, Remove)
    // ... (Переконайтеся, що ви залишили їхній код із попереднього кроку)

    // Коментар: Метод Save - Серіалізація даних у файл
    public void Save(string filePath)
    {
        try
        {
            // Серіалізуємо внутрішній словник у JSON-рядок
            // WriteIndented = true робить JSON більш читабельним
            string jsonString = JsonSerializer.Serialize(_storage, new JsonSerializerOptions { WriteIndented = true });

            // Зберігаємо рядок у файл
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine($"\n[SAVE] Дані сервісу {typeof(T).Name} успішно збережено у файл: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[SAVE ERROR] Виникла помилка під час збереження: {ex.Message}");
        }
    }

    // Коментар: Метод Load - Десеріалізація даних із файлу
    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"\n[LOAD] Файл '{filePath}' не знайдено. Створюємо нове сховище.");
            _storage.Clear(); // Забезпечуємо пусте сховище
            return;
        }

        try
        {
            // Читаємо JSON-рядок із файлу
            string jsonString = File.ReadAllText(filePath);

            // Десеріалізуємо його назад у словник
            var loadedData = JsonSerializer.Deserialize<Dictionary<Guid, T>>(jsonString);

            // Очищуємо поточне сховище та завантажуємо нові дані
            _storage.Clear();
            foreach (var item in loadedData)
            {
                _storage.Add(item.Key, item.Value);
            }
            Console.WriteLine($"\n[LOAD] Успішно завантажено {loadedData.Count} елементів типу {typeof(T).Name} із файлу: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[LOAD ERROR] Виникла помилка під час завантаження: {ex.Message}");
            // У випадку помилки, краще залишити сховище порожнім
            _storage.Clear();
        }
    }
}