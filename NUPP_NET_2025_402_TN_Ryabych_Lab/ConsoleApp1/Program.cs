using System;
using System.Linq; // Потрібно для ToList() та Count()
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        // Метод, який буде викликатися подією
        static void HandleItemLoaned(string title)
        {
            Console.WriteLine($"[ПОДІЯ] Повідомлення: Успішно видано '{title}'.");
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // =================================================================
            // === БЛОК 1: ТЕСТУВАННЯ КОНСТРУКЦІЙ (Завдання 2) ================
            // =================================================================

            Console.WriteLine("\n============= ТЕСТУВАННЯ КОНСТРУКЦІЙ (Завдання 2) =============");

            // Статичні елементи
            Book.DisplayTotalBooks();

            // Конструктори та Методи
            Book myBook = new Book("квіти зла", "шарль бодлер") { YearPublished = 1857, ISBN = "123-456" };
            Librarian chiefLibrarian = new Librarian("Олена", "Коваленко", 101);

            // Подія та Делегат
            myBook.ItemLoaned += HandleItemLoaned;
            myBook.LoanOut();
            chiefLibrarian.CheckInItem(myBook);

            // Метод розширення
            string rawName = "іван";
            string formattedName = rawName.ToTitleCase();
            Console.WriteLine($"\nВідформатоване (через метод розширення): '{formattedName}'");

            // Статичний метод
            Book anotherBook = new Book("Фауст", "Гете");
            Book.DisplayTotalBooks();

            Console.WriteLine("============================================================\n");

            // =================================================================
            // === БЛОК 2: ТЕСТУВАННЯ CRUD СЕРВІСУ (Завдання 3 та 4) ===========
            // =================================================================

            Console.WriteLine("============= ТЕСТУВАННЯ CRUD СЕРВІСУ (Завдання 3 & 4) =============");

            // Створення дженерик-сервісу
            ICrudService<Book> bookService = new CrudService<Book>();

            // 1. CREATE
            Book book1 = new Book("Кобзар", "Тарас Шевченко") { YearPublished = 1840, ISBN = "111" };
            Book book2 = new Book("Лісова пісня", "Леся Українка") { YearPublished = 1911, ISBN = "222" };
            bookService.Create(book1);
            bookService.Create(book2);

            // 2. READ ALL
            Console.WriteLine("\n[CRUD] Усі книги після створення:");
            var allBooks = bookService.ReadAll().ToList();
            foreach (var b in allBooks)
            {
                Console.WriteLine($"- ID: {b.Id.ToString().Substring(0, 4)}... - {b.Title}");
            }

            // 3. UPDATE
            book1.Author = "Т. Г. Шевченко (Оновлено)";
            bookService.Update(book1);

            // 4. READ (за ID)
            Console.WriteLine("\n[CRUD] Читання оновленого елемента:");
            Book updatedBook = bookService.Read(book1.Id);
            Console.WriteLine($"-> Прочитано: {updatedBook.Title}, Автор: {updatedBook.Author}");

            // 5. REMOVE
            bookService.Remove(book2);

            string filePath = "books_data.json"; // Ім'я файлу буде створено у теці проєкту

            // 6. ЗБЕРЕЖЕННЯ ДАНИХ (SAVE)
            // Збережемо два створені елементи (book1, book2) у файл
            bookService.Save(filePath);

            // 7. Створення нового сервісу для тестування ЗАВАНТАЖЕННЯ (LOAD)
            ICrudService<Book> loadedBookService = new CrudService<Book>();

            // 8. ЗАВАНТАЖЕННЯ ДАНИХ (LOAD)
            loadedBookService.Load(filePath);

            // 9. Перевірка завантажених даних
            Console.WriteLine("\n[LOAD CHECK] Книги, завантажені у новий сервіс:");
            var loadedBooks = loadedBookService.ReadAll().ToList();
            foreach (var b in loadedBooks)
            {
                Console.WriteLine($"- {b.Title} (Автор: {b.Author})");
            }

            // Очищення файлу після тестування (за бажанням)
            // File.Delete(filePath); 

            Console.ReadKey();

            Console.WriteLine($"\n[CRUD] Кількість книг після видалення: {bookService.ReadAll().Count()}");

            Console.WriteLine("============================================================\n");

            Console.ReadKey();
        }

    }
}