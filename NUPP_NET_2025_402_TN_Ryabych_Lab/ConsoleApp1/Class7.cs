// IRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepository<T> where T : class
{
    // Узгоджуємо тип Id з нашими моделями (Guid)
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}