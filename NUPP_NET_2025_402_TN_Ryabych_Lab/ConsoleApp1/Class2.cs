using System;
using System.Collections.Generic;

public interface ICrudService<T> where T : class
{
    void Create(T element);
    T Read(Guid id);
    IEnumerable<T> ReadAll();
    void Update(T element);
    void Remove(T element);

    // Коментар: Нові методи для серіалізації/десеріалізації
    void Save(string filePath);
    void Load(string filePath);
}