// models/BusModel.cs
using System;
using System.Collections.Generic;

// Успадковується від VehicleModel для реалізації TPT
public class BusModel : VehicleModel
{
    public int Capacity { get; set; }
    public string RegistrationNumber { get; set; }

    // ------------------------------------
    // 1. Зв'язок ОДИН-ДО-БАГАТЬОХ (One-to-Many)
    // Один автобус може мати багато призначених водіїв
    public ICollection<DriverModel> AssignedDrivers { get; set; } = new List<DriverModel>();

    // 2. Зв'язок БАГАТО-ДО-БАГАТЬОХ (Many-to-Many - Додаткові бали)
    // Зв'язок з RouteModel через BusRouteModel
    public ICollection<BusRouteModel> BusRoutes { get; set; } = new List<BusRouteModel>();

    // (Примітка: для простоти я опустив One-to-One, оскільки це не було прямо вимагається в цій частині, 
    // але його можна додати за допомогою окремої моделі VehicleDetailsModel, як у попередній відповіді.)
}