using System;

// 1. Оголошення Делегата (використовується для Подій)
// Коментар: Делегат
public delegate void ItemLoanedHandler(string title);

// --- БАЗОВИЙ КЛАС 1: LibraryItem ---
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

    // Властивості
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int NumberOfPages { get; set; }

    // Коментар: Статичний конструктор
    static Book()
    {
        // Ініціалізація статичного поля (викликається один раз)
        Console.WriteLine("Статичний конструктор Book: Початок обліку книг.");
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
        Console.WriteLine($"\nКнига: {Title} ({YearPublished})");
        Console.WriteLine($"Автор: {Author}, ISBN: {ISBN}");
    }

    // Коментар: Статичний метод
    public static void DisplayTotalBooks()
    {
        Console.WriteLine($"\nУсього книг у наявності (Статичний метод): {TotalBooksAvailable}");
    }
}

// --- КЛАС-СПАДКОЄМЕЦЬ 2: Magazine (Без змін, тільки для ієрархії) ---
public class Magazine : LibraryItem
{
    public int IssueNumber { get; set; }
    public string Publisher { get; set; }
    public string Category { get; set; }
}


// --- БАЗОВИЙ КЛАС 2: Person ---
public class Person
{
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
        Console.WriteLine($"\n{FirstName} {LastName} прийняв '{item.Title}' назад.");
    }
}

// --- СТВОРЕННЯ МЕТОДУ РОЗШИРЕННЯ ---
// Клас розширення має бути статичним, і метод має приймати 'this'
namespace ConsoleApp1
{
    // Коментар: Клас для Методу розширення
    public static class StringExtension
    {
        // Коментар: Метод розширення
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}