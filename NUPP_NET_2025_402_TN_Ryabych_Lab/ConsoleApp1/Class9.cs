// ICrudServiceAsync.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICrudServiceAsync<T> where T : class
{
    // Тип Id змінено на Guid для узгодженості
    public Task<bool> CreateAsync(T element);
    public Task<T> ReadAsync(Guid id);
    public Task<IEnumerable<T>> ReadAllAsync();
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    public Task<bool> UpdateAsync(T element);
    public Task<bool> RemoveAsync(T element);
    public Task SaveAsync();
}