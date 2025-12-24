// models/BusRouteModel.cs
using System;

// Цей клас є сутністю-з'єднувачем для зв'язку Багато-до-Багатьох
public class BusRouteModel
{
    // Композитний ключ
    public Guid BusId { get; set; }
    public BusModel Bus { get; set; }

    public Guid RouteId { get; set; }
    public RouteModel Route { get; set; }
}