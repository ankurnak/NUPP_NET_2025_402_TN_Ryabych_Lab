using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent; // Для ConcurrentBag
using System.Threading; // Для Parallel.For, Lock, Semaphore, AutoResetEvent

namespace ConsoleApp1
{
    class Program
    {
        // Метод, який буде викликатися подією (Тестування Делегатів та Подій)
        static void HandleItemLoaned(string title)
        {
            Console.WriteLine($"[ПОДІЯ] Повідомлення: Успішно видано '{title}'.");
        }

        // Асинхронна функція для паралельного створення та аналізу (Завдання 3)
        static async Task RunParallelCreationAndAnalysisAsync(ICrudServiceAsync<Book> service, int count)
        {
            Console.WriteLine($"\n============= БЛОК 2: ПАРАЛЕЛЬНЕ СТВОРЕННЯ ТА АНАЛІЗ ({count} об'єктів) =============");

            // ConcurrentBag для потокобезпечного збору створених об'єктів
            var createdBooks = new ConcurrentBag<Book>();

            var startTime = DateTime.Now;

            // Коментар: Використання Parallel.For для паралельного створення
            Parallel.For(0, count, (i) =>
            {
                // Створення об'єкта за допомогою статичного фабричного методу
                // Примітка: метод CreateNewWithGeneratedData() має бути в класі Book
                Book newBook = Book.CreateNewWithGeneratedData();
                createdBooks.Add(newBook);
            });

            // Додавання всіх об'єктів до CRUD сервісу (асинхронно)
            var creationTasks = createdBooks.Select(b => service.CreateAsync(b)).ToList();
            await Task.WhenAll(creationTasks);

            var duration = DateTime.Now - startTime;

            Console.WriteLine($"\n[Паралельне виконання] Створення та додавання {count} об'єктів зайняло: {duration.TotalSeconds:F2} сек.");

            // --- Статистичний Аналіз (LINQ) ---
            var allBooks = await service.ReadAllAsync();
            var pageCounts = allBooks.Select(b => b.NumberOfPages).ToList();

            if (pageCounts.Any())
            {
                // Коментар: Використання LINQ (Min, Max, Average)
                double minPages = pageCounts.Min();
                double maxPages = pageCounts.Max();
                double avgPages = pageCounts.Average();

                Console.WriteLine("\n--- Статистичний Аналіз (NumberOfPages) ---");
                Console.WriteLine($"Мінімальна кількість сторінок: {minPages:F0}");
                Console.WriteLine($"Максимальна кількість сторінок: {maxPages:F0}");
                Console.WriteLine($"Середня кількість сторінок: {avgPages:F2}");
            }

            // Збереження згенерованої колекції у файл
            await service.SaveAsync("parallel_books_data.json");

            Console.WriteLine("============================================================\n");
        }


        // Точка входу: Main змінено на async Task Main
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // =================================================================
            // === БЛОК 0: ТЕСТУВАННЯ ПРИМІТИВІВ СИНХРОНІЗАЦІЇ (Завдання 4) ====
            // =================================================================

            Console.WriteLine("\n============= БЛОК 0: ТЕСТУВАННЯ ПРИМІТИВІВ СИНХРОНІЗАЦІЇ =============");
            // Примітка: Клас SynchronizationExamples має бути визначений в окремому файлі
            SynchronizationExamples syncDemo = new SynchronizationExamples();

            // --- Lock Demo ---
            Console.WriteLine("\n--- Lock Demo ---");
            List<Task> lockTasks = new List<Task>();
            for (int i = 1; i <= 5; i++)
            {
                int id = i;
                lockTasks.Add(Task.Run(() => syncDemo.DemonstrateLock(id)));
            }
            await Task.WhenAll(lockTasks);

            // --- SemaphoreSlim Demo ---
            Console.WriteLine("\n--- SemaphoreSlim Demo (Concurrent limit: 2) ---");
            List<Task> semaphoreTasks = new List<Task>();
            for (int i = 1; i <= 5; i++)
            {
                semaphoreTasks.Add(syncDemo.DemonstrateSemaphoreAsync(i));
            }
            await Task.WhenAll(semaphoreTasks);

            // --- AutoResetEvent Demo ---
            Console.WriteLine("\n--- AutoResetEvent Demo ---");
            Task waiterTask = Task.Run(() => syncDemo.WaiterThread());
            Task signalerTask = Task.Run(() => syncDemo.SignalerThread());

            await Task.WhenAll(waiterTask, signalerTask);
            Console.WriteLine("[AutoResetEvent Demo] Демонстрація завершена.");

            Console.WriteLine("============================================================\n");


            // =================================================================
            // === БЛОК 1: ТЕСТУВАННЯ БАЗОВИХ КОНСТРУКЦІЙ (Завдання 2) ========
            // =================================================================

            Console.WriteLine("\n============= БЛОК 1: ТЕСТУВАННЯ БАЗОВИХ КОНСТРУКЦІЙ (Завдання 2) =============");

            // Статичні елементи
            Book.DisplayTotalBooks();
            Book myBook = new Book("квіти зла", "шарль бодлер") { YearPublished = 1857, ISBN = "123-456" };
            Librarian chiefLibrarian = new Librarian("Олена", "Коваленко", 101);

            // Події та Методи
            myBook.ItemLoaned += HandleItemLoaned;
            myBook.LoanOut();
            chiefLibrarian.CheckInItem(myBook);

            // Метод розширення
            string rawName = "бібліотека";
            string formattedName = rawName.ToTitleCase();
            Console.WriteLine($"\n[Метод розширення] '{rawName}' -> '{formattedName}'");

            Book anotherBook = new Book("Фауст", "Гете");
            Book.DisplayTotalBooks();
            Console.WriteLine("============================================================\n");


            // =================================================================
            // === БЛОК 2: ПАРАЛЕЛЬНЕ СТВОРЕННЯ ТА АНАЛІЗ (Виклик) =============
            // =================================================================

            ICrudServiceAsync<Book> parallelBookService = new CrudServiceAsync<Book>();

            // Запускаємо паралельне створення понад 1000 об'єктів
            await RunParallelCreationAndAnalysisAsync(parallelBookService, 1500);


            // =================================================================
            // === БЛОК 3: ТЕСТУВАННЯ АСИНХРОННОГО CRUD (Завдання 1) ===========
            // =================================================================

            Console.WriteLine("============= БЛОК 3: ТЕСТУВАННЯ АСИНХРОННОГО CRUD (Завдання 1) =============");

            string asyncFilePath = "async_books_data.json";

            // Створення нового сервісу для тестування (наприклад, для пагінації на меншій вибірці)
            ICrudServiceAsync<Book> asyncBookService = new CrudServiceAsync<Book>();

            // Завантаження, якщо файл існує
            await asyncBookService.LoadAsync(asyncFilePath);

            // Створення 12 тестових елементів
            Console.WriteLine("\n[ASYNC CREATE] Створення 12 тестових елементів...");
            for (int i = 1; i <= 12; i++)
            {
                await asyncBookService.CreateAsync(new Book($"Книга А-{i}", $"Автор А-{i}") { YearPublished = 2000 + i, ISBN = i.ToString("000") });
            }

            await asyncBookService.SaveAsync(asyncFilePath);

            // READ ALL ASYNC з ПАГІНАЦІЄЮ
            Console.WriteLine("\n[ASYNC READ] Пагінація (Сторінка 1, по 5 елементів):");
            var page1 = await asyncBookService.ReadAllAsync(page: 1, amount: 5);
            foreach (var b in page1)
            {
                Console.WriteLine($"- Пагінація: {b.Title}");
            }

            // UPDATE та READ за ID
            var bookToUpdate = asyncBookService.ReadAllAsync().Result.First(b => b.Title == "Книга А-1");
            bookToUpdate.Author = "Новий Автор (Оновлено)";
            await asyncBookService.UpdateAsync(bookToUpdate);

            var readUpdatedBook = await asyncBookService.ReadAsync(bookToUpdate.Id);
            Console.WriteLine($"\n[ASYNC READ] Прочитано оновлений елемент: {readUpdatedBook.Title}, {readUpdatedBook.Author}");

            // REMOVE ASYNC
            var bookToRemove = asyncBookService.ReadAllAsync().Result.Last();
            await asyncBookService.RemoveAsync(bookToRemove);

            // Тестування IEnumerable (foreach)
            Console.WriteLine("\n[ASYNC FOREACH] Кількість елементів після видалення:");
            int count = 0;
            foreach (var book in asyncBookService)
            {
                count++;
            }
            Console.WriteLine($"-> Загальна кількість: {count}");

            Console.WriteLine("============================================================\n");

            Console.ReadKey();
        }
    }
}