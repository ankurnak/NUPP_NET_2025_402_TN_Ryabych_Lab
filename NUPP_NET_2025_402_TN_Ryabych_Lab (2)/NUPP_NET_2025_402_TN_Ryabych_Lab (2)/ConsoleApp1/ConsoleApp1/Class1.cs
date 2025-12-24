using System;
using System.Linq;
using System.Collections.Generic; // Хоча тут не використовується, часто потрібно
using System.Threading; // Для Random

// 1. Оголошення Делегата (використовується для Подій)
// Коментар: Делегат
public delegate void ItemLoanedHandler(string title);

// --- БАЗОВИЙ КЛАС 1: LibraryItem (Батьківський клас для книг і журналів) ---
public class LibraryItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int YearPublished { get; set; }

    // Коментар: Подія
    public event ItemLoanedHandler ItemLoaned;

    // Коментар: Конструктор (Без параметрів)
    public LibraryItem()
    {
        Id = Guid.NewGuid(); // Присвоюємо Id при створенні об'єкта
    }

    // Коментар: Метод
    public void LoanOut()
    {
        Console.WriteLine($"\n-> {Title} видано.");
        // Виклик події
        ItemLoaned?.Invoke(this.Title);
    }
}

// --- КЛАС-СПАДКОЄМЕЦЬ 1: Book ---
public class Book : LibraryItem
{
    // Коментар: Статичне поле
    public static int TotalBooksAvailable = 0;

    // Примітка: використовуємо ThreadLocal<Random> для потокобезпечного генерування випадкових чисел
    private static ThreadLocal<Random> Rnd = new ThreadLocal<Random>(() => new Random());

    // Властивості
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int NumberOfPages { get; set; }

    // Коментар: Статичний конструктор
    static Book()
    {
        Console.WriteLine("[Статичний конструктор Book] Ініціалізовано облік книг.");
    }

    // Коментар: Конструктор (З параметрами)
    public Book(string title, string author) : base()
    {
        this.Title = title;
        this.Author = author;
        TotalBooksAvailable++;
    }

    // Коментар: Метод
    public void PrintDetails()
    {
        Console.WriteLine($"Книга: {Title} ({YearPublished}), Автор: {Author}, ISBN: {ISBN}");
    }

    // Коментар: Статичний метод
    public static void DisplayTotalBooks()
    {
        Console.WriteLine($"Усього книг у наявності (Статичний метод): {TotalBooksAvailable}");
    }

    // Коментар: Статичний метод для генерації даних (Фабричний метод)
    public static Book CreateNewWithGeneratedData()
    {
        int bookNumber = TotalBooksAvailable + 1;

        Book newBook = new Book($"Тестова Книга {bookNumber}", $"Тест. Автор {bookNumber}")
        {
            // Випадкові дані для інших властивостей
            ISBN = $"ISBN-{Rnd.Value.Next(1000, 9999)}",
            NumberOfPages = Rnd.Value.Next(100, 500),
            YearPublished = DateTime.Now.Year
        };
        Console.WriteLine($"[Статична Фабрика] Створено: {newBook.Title}");
        return newBook;
    }
}

// --- КЛАС-СПАДКОЄМЕЦЬ 2: Magazine ---
public class Magazine : LibraryItem
{
    public int IssueNumber { get; set; }
    public string Publisher { get; set; }
    public string Category { get; set; }
}


// --- БАЗОВИЙ КЛАС 2: Person (Батьківський клас для бібліотекаря) ---
public class Person
{
    // Примітка: використовується PersonId, тому що Id вже є в LibraryItem.
    public Guid PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Конструктор
    public Person()
    {
        PersonId = Guid.NewGuid();
    }
}

// --- КЛАС-СПАДКОЄМЕЦЬ 3: Librarian ---
public class Librarian : Person
{
    private static ThreadLocal<Random> Rnd = new ThreadLocal<Random>(() => new Random());

    public int EmployeeId { get; set; }
    public string Department { get; set; }
    public DateTime DateHired { get; set; }

    // Коментар: Конструктор (З параметрами)
    public Librarian(string firstName, string lastName, int id)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.EmployeeId = id;
        this.DateHired = DateTime.Now;
    }

    // Коментар: Метод
    public void CheckInItem(LibraryItem item)
    {
        Console.WriteLine($"{FirstName} {LastName} прийняв '{item.Title}' назад.");
    }

    // Коментар: Статичний метод для генерації даних (Фабричний метод)
    public static Librarian CreateNewWithGeneratedData()
    {
        // Генеруємо унікальний ID співробітника
        int employeeId = Rnd.Value.Next(1000, 9999);

        // Використовуємо конструктор
        Librarian newLibrarian = new Librarian($"Працівник_{employeeId}", $"Тестовий", employeeId)
        {
            Department = "Новий Відділ"
        };
        Console.WriteLine($"[Статична Фабрика] Створено бібліотекаря: {newLibrarian.FirstName}");
        return newLibrarian;
    }
}

// --- СТВОРЕННЯ МЕТОДУ РОЗШИРЕННЯ ---
// Клас розширення має бути статичним
namespace ConsoleApp1
{
    // Коментар: Клас для Методу розширення
    public static class StringExtension
    {
        // Коментар: Метод розширення
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            // Використовуємо TextInfo для перетворення першої літери кожного слова
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}