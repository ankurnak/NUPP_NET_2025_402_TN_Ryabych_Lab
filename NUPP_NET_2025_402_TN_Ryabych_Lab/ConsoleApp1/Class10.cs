// CrudServiceAsync.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly BusTransportationContext _context; // Для SaveChangesAsync та пагінації

    public CrudServiceAsync(IRepository<T> repository, BusTransportationContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<bool> CreateAsync(T element)
    {
        if (element == null) return false;
        await _repository.AddAsync(element);
        return true;
    }

    public async Task<T> ReadAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        // Пагінація: використовуємо DbSet з контексту
        return await _context.Set<T>()
            .Skip(page * amount)
            .Take(amount)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(T element)
    {
        if (element == null) return false;
        await _repository.UpdateAsync(element);
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        if (element == null) return false;
        await _repository.DeleteAsync(element);
        return true;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}