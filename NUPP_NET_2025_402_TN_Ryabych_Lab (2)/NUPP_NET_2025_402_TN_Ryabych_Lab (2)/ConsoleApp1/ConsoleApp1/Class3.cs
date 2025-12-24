using System;
using System.Collections;
using System.Collections.Concurrent; // <--- ConcurrentDictionary для потокобезпеки
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks; // <--- Для асинхронності
using System.IO;

// Коментар: Асинхронний, Потокобезпечний CRUD Сервіс
public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    // Коментар: Потокобезпечна колекція .NET
    private readonly ConcurrentDictionary<Guid, T> _storage = new ConcurrentDictionary<Guid, T>();

    private Guid GetId(T element)
    {
        // Метод залишається синхронним, оскільки використовує рефлексію
        var idProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("PersonId");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
        {
            throw new InvalidOperationException($"Тип T ({typeof(T).Name}) не має Guid Id/PersonId.");
        }
        return (Guid)idProperty.GetValue(element);
    }

    // --- АСИНХРОННІ CRUD ОПЕРАЦІЇ ---

    // Коментар: CreateAsync (Потокобезпека забезпечується ConcurrentDictionary)
    public Task<bool> CreateAsync(T element)
    {
        Guid id = GetId(element);
        bool added = _storage.TryAdd(id, element);
        Console.WriteLine($"[CREATE ASYNC] Додано {typeof(T).Name} з ID: {id} -> {added}");
        return Task.FromResult(added);
    }

    // Коментар: ReadAsync
    public Task<T> ReadAsync(Guid id)
    {
        _storage.TryGetValue(id, out T element);
        return Task.FromResult(element);
    }

    // Коментар: ReadAllAsync
    public Task<IEnumerable<T>> ReadAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_storage.Values);
    }

    // Коментар: ReadAllAsync з Пагінацією
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page <= 0) page = 1;
        if (amount <= 0) amount = 10;

        // Використовуємо Linq для пагінації
        var results = _storage.Values
            .Skip((page - 1) * amount) // Пропускаємо елементи попередніх сторінок
            .Take(amount)             // Беремо елементи для поточної сторінки
            .ToList();

        Console.WriteLine($"[PAGINATION] Сторінка {page}, Елементів: {results.Count}");
        return Task.FromResult<IEnumerable<T>>(results);
    }

    // Коментар: UpdateAsync
    public Task<bool> UpdateAsync(T element)
    {
        Guid id = GetId(element);
        // Замінюємо елемент за ключем. ConcurrentDictionary вимагає old value, 
        // але для простого CRUD ми можемо його просто замінити (TryUpdate)
        bool success = _storage.TryUpdate(id, element, _storage[id]);

        if (success)
        {
            Console.WriteLine($"[UPDATE ASYNC] Оновлено {typeof(T).Name} з ID: {id}");
        }
        else
        {
            // Якщо елемент не існує, його можна додати або повернути false
            // У цьому випадку, якщо не вдалося оновити, повертаємо false
        }
        return Task.FromResult(success);
    }

    // Коментар: RemoveAsync
    public Task<bool> RemoveAsync(T element)
    {
        Guid id = GetId(element);
        bool removed = _storage.TryRemove(id, out _);
        Console.WriteLine($"[REMOVE ASYNC] Видалено {typeof(T).Name} з ID: {id} -> {removed}");
        return Task.FromResult(removed);
    }

    // --- АСИНХРОННЕ ЗБЕРЕЖЕННЯ/ЗАВАНТАЖЕННЯ ---

    // Коментар: SaveAsync (Асинхронна файлова операція)
    public async Task<bool> SaveAsync(string filePath)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_storage, new JsonSerializerOptions { WriteIndented = true });

            // Використовуємо асинхронний метод файлової операції
            await File.WriteAllTextAsync(filePath, jsonString);
            Console.WriteLine($"\n[SAVE ASYNC] Дані збережено у файл: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[SAVE ASYNC ERROR] Помилка: {ex.Message}");
            return false;
        }
    }

    // Коментар: LoadAsync (Асинхронна файлова операція)
    public async Task<bool> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"\n[LOAD ASYNC] Файл '{filePath}' не знайдено. Створюємо пусте сховище.");
            _storage.Clear();
            return false;
        }

        try
        {
            // Використовуємо асинхронний метод файлової операції
            string jsonString = await File.ReadAllTextAsync(filePath);

            var loadedData = JsonSerializer.Deserialize<Dictionary<Guid, T>>(jsonString);

            // Додавання елементів у потокобезпечний словник
            _storage.Clear();
            foreach (var item in loadedData)
            {
                _storage.TryAdd(item.Key, item.Value);
            }
            Console.WriteLine($"\n[LOAD ASYNC] Успішно завантажено {_storage.Count} елементів із файлу: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[LOAD ASYNC ERROR] Помилка: {ex.Message}");
            _storage.Clear();
            return false;
        }
    }

    // --- РЕАЛІЗАЦІЯ IEnumerable ---

    // Коментар: Реалізація IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        return _storage.Values.GetEnumerator();
    }

    // Коментар: Реалізація IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}