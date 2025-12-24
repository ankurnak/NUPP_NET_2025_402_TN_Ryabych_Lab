using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Коментар: Асинхронний CRUD Інтерфейс з підтримкою IEnumerable
public interface ICrudServiceAsync<T> : IEnumerable<T> where T : class
{
    // Асинхронні методи CRUD
    public Task<bool> CreateAsync(T element);
    public Task<T> ReadAsync(Guid id);
    public Task<IEnumerable<T>> ReadAllAsync();
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount); // Пагінація
    public Task<bool> UpdateAsync(T element);
    public Task<bool> RemoveAsync(T element);

    // Асинхронне збереження/завантаження
    public Task<bool> SaveAsync(string filePath);
    public Task<bool> LoadAsync(string filePath);
}