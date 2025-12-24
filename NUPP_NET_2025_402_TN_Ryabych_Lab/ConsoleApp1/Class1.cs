// models/VehicleModel.cs
using System;

public abstract class VehicleModel
{
    // Ідентифікатор (Primary Key), який буде спільним для всіх похідних класів
    public Guid Id { get; set; }
    public string Manufacturer { get; set; }
    public int YearOfProduction { get; set; }
}