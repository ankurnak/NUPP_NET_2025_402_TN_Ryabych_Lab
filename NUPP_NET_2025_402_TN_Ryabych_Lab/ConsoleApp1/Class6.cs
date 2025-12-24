// BusTransportationContext.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

// Наслідується від DbContext
public class BusTransportationContext : DbContext
{
    // DbSet для базового класу (для TPT)
    public DbSet<VehicleModel> Vehicles { get; set; }
    // DbSet для похідних класів
    public DbSet<BusModel> Buses { get; set; }
    public DbSet<DriverModel> Drivers { get; set; }
    public DbSet<RouteModel> Routes { get; set; }
    public DbSet<BusRouteModel> BusRoutes { get; set; } // Для M-to-M

    public BusTransportationContext(DbContextOptions<BusTransportationContext> options)
        : base(options) { }

    // Використовуємо OnConfiguring тільки якщо не використовуємо DI
    // Якщо використовується DI (як у Program.cs), цей метод не потрібен.
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("Data Source=BusTransportationDB.db");
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ------------------------------------
        // 1. Налаштування Наслідування (Table-per-Type - TPT)

        // TPT: Базовий клас VehicleModel та похідний клас BusModel 
        // матимуть окремі таблиці.
        modelBuilder.Entity<VehicleModel>().ToTable("Vehicles"); // Базова таблиця
        modelBuilder.Entity<BusModel>().ToTable("Buses");       // Похідна таблиця

        // ------------------------------------
        // 2. Налаштування Зв'язку ОДИН-ДО-БАГАТЬОХ (One-to-Many)

        modelBuilder.Entity<DriverModel>()
            .HasOne(d => d.CurrentBus)         // Водій має один автобус
            .WithMany(b => b.AssignedDrivers)  // Автобус має багато водіїв
            .HasForeignKey(d => d.CurrentBusId) // Foreign Key у DriverModel
            .IsRequired(false)                 // Дозволяє NULL (водій може бути без автобуса)
            .OnDelete(DeleteBehavior.SetNull); // При видаленні автобуса FK стає NULL

        // ------------------------------------
        // 3. Налаштування Зв'язку БАГАТО-ДО-БАГАТЬОХ (Many-to-Many)

        modelBuilder.Entity<BusRouteModel>()
            .HasKey(br => new { br.BusId, br.RouteId }); // Композитний ключ

        // Зв'язок з BusModel
        modelBuilder.Entity<BusRouteModel>()
            .HasOne(br => br.Bus)
            .WithMany(b => b.BusRoutes)
            .HasForeignKey(br => br.BusId);

        // Зв'язок з RouteModel
        modelBuilder.Entity<BusRouteModel>()
            .HasOne(br => br.Route)
            .WithMany(r => r.BusRoutes)
            .HasForeignKey(br => br.RouteId);

        base.OnModelCreating(modelBuilder);
    }
}