// models/DriverModel.cs
using System;

public class DriverModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }

    // Foreign Key для зв'язку Один-до-Багатьох
    public Guid? CurrentBusId { get; set; }
    // Навігаційна властивість
    public BusModel CurrentBus { get; set; }
}