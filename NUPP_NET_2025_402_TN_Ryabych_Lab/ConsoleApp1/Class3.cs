// models/RouteModel.cs
using System;
using System.Collections.Generic;

public class RouteModel
{
    public Guid Id { get; set; }
    public string RouteName { get; set; }
    public double DistanceKm { get; set; }

    // Навігаційна властивість для M-to-M (через проміжну таблицю)
    public ICollection<BusRouteModel> BusRoutes { get; set; } = new List<BusRouteModel>();
}