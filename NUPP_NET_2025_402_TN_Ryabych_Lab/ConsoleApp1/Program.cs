// Program.cs
public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Налаштування контейнера впровадження залежностей (DI)
        var serviceProvider = new ServiceCollection()
            // Реєстрація DbContext. Використовуємо SQLite
            .AddDbContext<BusTransportationContext>(options =>
                options.UseSqlite("Data Source=BusTransportationDB.db"))

            // Реєстрація Репозиторію
            .AddScoped(typeof(IRepository<>), typeof(GenericRepository<>))

            // Реєстрація CRUD Сервісу
            .AddScoped(typeof(ICrudServiceAsync<>), typeof(CrudServiceAsync<>))
            .BuildServiceProvider();

        // 2. Ініціалізація бази даних та застосування міграцій (або створення)
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BusTransportationContext>();
            // Використовуйте .Database.Migrate() для повноцінних міграцій
            // Для SQLite можна використати:
            context.Database.EnsureCreated();
            Console.WriteLine("База даних ініціалізована (або вже існує).");
        }

        // 3. Демонстрація роботи через CRUD Сервіс
        using (var scope = serviceProvider.CreateScope())
        {
            var busService = scope.ServiceProvider.GetRequiredService<ICrudServiceAsync<BusModel>>();

            // --- Створення (CREATE) ---
            var newBus = new BusModel
            {
                Id = Guid.NewGuid(),
                Manufacturer = "Ikarus",
                YearOfProduction = 1995,
                Capacity = 50,
                RegistrationNumber = "AA1111BB"
            };
            await busService.CreateAsync(newBus);
            await busService.SaveAsync();
            Console.WriteLine($"Створено автобус {newBus.Manufacturer} з ID: {newBus.Id}");

            // --- Читання (READ) ---
            var busFromDb = await busService.ReadAsync(newBus.Id);
            Console.WriteLine($"Зчитано: {busFromDb.RegistrationNumber}, рік: {busFromDb.YearOfProduction}");

            // --- Оновлення (UPDATE) ---
            busFromDb.Capacity = 55;
            await busService.UpdateAsync(busFromDb);
            await busService.SaveAsync();
            Console.WriteLine($"Місткість оновлено до {busFromDb.Capacity}");

            // --- Видалення (DELETE) ---
            // await busService.RemoveAsync(busFromDb);
            // await busService.SaveAsync();
            // Console.WriteLine("Автобус видалено.");

            Console.WriteLine("Демонстрація завершена.");
        }
    }
}

internal class ServiceCollection
{
    public ServiceCollection()
    {
    }

    internal object AddDbContext<T>(Func<object, object> value)
    {
        throw new NotImplementedException();
    }
}