using System;
using System.Collections.Generic;
using System.Linq;
// Enum to represent the operational status of the car
public enum OperationalStatus { Active, Idle, Failed }
// Enum to represent mission priority levels
public enum MissionPriority { High, Medium, Low }
// Car Class
public class Car
{
    // Properties
    public string CarID { get; private set; }             // Unique ID for each car
    public int EnergyStatus { get; private set; }         // Battery percentage (0–100)
    public OperationalStatus Status { get; set; }         // Current operational status
    public MissionPriority Priority { get; set; }         // Mission priority
    public List<string> Logs { get; private set; } = new List<string>(); // Activity/incident logs

    // Delegate-based events (alerts sent to MissionControl)
    public delegate void CarEventHandler(string carId, string message);
    public event CarEventHandler OnRouteCompleted;
    public event CarEventHandler OnObstacleDetected;
    public event CarEventHandler OnRestrictedZoneEntered;
   // Constructor with validation
    public Car(string id, int energy, OperationalStatus status, MissionPriority priority)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Car ID cannot be empty.");
        if (energy < 0 || energy > 100) throw new ArgumentException("Energy must be 0–100.");
        CarID = id;
        EnergyStatus = energy;
        Status = status;
        Priority = priority;
        AddLog("Car created.");
    }
    // Add a log entry with timestamp
    public void AddLog(string message)
    {
        Logs.Add($"[{DateTime.Now}] {message}");
    }
 // Update battery energy and check for failure
    public void UpdateEnergy(int newEnergy)
    {
        if (newEnergy < 0 || newEnergy > 100) throw new ArgumentException("Energy must be 0–100.");
        EnergyStatus = newEnergy;
        AddLog($"Energy updated to {EnergyStatus}%.");
     // Trigger incident if battery is critically low
        if (EnergyStatus < 10)
        {
            HandleIncident("Battery failure: Critical low energy.");    } }
  // Simulate navigation attempt
    public void Navigate(string route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            HandleIncident("Navigation error: Invalid route.");
        }
        else
        {
            AddLog($"Car navigating to {route}.");
        }
    }
 // Handle unauthorized override attempt
    public void UnauthorizedOverride()
    {
        HandleIncident("Unauthorized override attempt detected!");
    }
 // Private method to handle incidents
    private void HandleIncident(string message)
    {
        AddLog("INCIDENT: " + message);
        NotifyMissionControl(message);
        Status = OperationalStatus.Failed; // Mark car as failed
    }
    // Notify mission control of an incident
    private void NotifyMissionControl(string message)
    {   Console.WriteLine($"[Mission Control ALERT] Car {CarID}: {message}");   }
  // Event trigger: Route completed
    public void CompleteRoute()
    {
        AddLog("Route completed successfully.");
        OnRouteCompleted?.Invoke(CarID, "Route completed successfully.");
    }
   // Event trigger: Obstacle detected
    public void DetectObstacle()
    {
     AddLog("Obstacle detected!");
        OnObstacleDetected?.Invoke(CarID, "Obstacle detected on path.");
    }
// Event trigger: Restricted zone entered
    public void EnterRestrictedZone()
    {
        AddLog("Restricted zone entered!");
        OnRestrictedZoneEntered?.Invoke(CarID, "Warning: Restricted zone entered.");
    }
    // Display full details of a car
    public void DisplayInfo()
    {
        Console.WriteLine($"\nCarID: {CarID}, Battery: {EnergyStatus}%, Status: {Status}, Priority: {Priority}");
        Console.WriteLine("Logs:");
        foreach (var log in Logs)
            Console.WriteLine(" - " + log);
    }}
// Mission Control Class
public class MissionControl
{
    // Subscribe to car events
    public void Subscribe(Car car)
    {
        car.OnRouteCompleted += Alert;
        car.OnObstacleDetected += Alert;
        car.OnRestrictedZoneEntered += Alert;
    }
 // Alert handler when events are triggered
    private void Alert(string carId, string message)
    {
        Console.WriteLine($"[Mission Control ALERT] Car {carId}: {message}");
    }
}
// Vehicle Manager Class
public class VehicleManager
{
    private List<Car> cars = new List<Car>(); // Collection of all cars

    // Add new car to the system
    public void AddCar(Car car)
    {
        cars.Add(car);
        Console.WriteLine($"Car {car.CarID} added successfully.");
    }
    // Remove car by ID
    public void RemoveCar(string carID)
    {
        Car car = cars.FirstOrDefault(c => c.CarID == carID);
        if (car != null)
        {
            cars.Remove(car);
            Console.WriteLine($"Car {carID} removed.");
        }
        else
        {
            Console.WriteLine("Car not found.");
        }
    }
  // Update car details
    public void UpdateCar(string carID, int battery, OperationalStatus status, MissionPriority priority)
    {
        Car car = cars.FirstOrDefault(c => c.CarID == carID);
        if (car != null)
        {
            car.UpdateEnergy(battery);
            car.Status = status;
            car.Priority = priority;
            car.AddLog("Car details updated.");
            Console.WriteLine($"Car {car.CarID} updated.");
        }
 else
        {
            Console.WriteLine("Car not found.");
        }
    }
   // Display all cars
    public void DisplayCars()
    {
        Console.WriteLine("\n--- All Cars ---");
        foreach (var car in cars)
        {
            Console.WriteLine($"ID: {car.CarID}, Battery: {car.EnergyStatus}%, Status: {car.Status}, Priority: {car.Priority}");
        }
    }
  // Display logs of a specific car
    public void DisplayCarLogs(string carID)
    {
        Car car = cars.FirstOrDefault(c => c.CarID == carID);
        if (car != null)
        {
            Console.WriteLine($"\nLogs for Car {car.CarID}:");
            foreach (var log in car.Logs)
                Console.WriteLine(" - " + log);
        }
        else
            Console.WriteLine("Car not found.");
    }
  // Get a car object by ID
    public Car GetCar(string carID)
    {
        return cars.FirstOrDefault(c => c.CarID == carID);
    }
  // Filter cars by operational status
    public List<Car> FilterByStatus(OperationalStatus status)
    {
        return cars.Where(c => c.Status == status).ToList();
    }
  // Get all high-priority cars
    public List<Car> GetHighPriorityCars()
    {
        return cars.Where(c => c.Priority == MissionPriority.High).ToList();
    }
 // Display operational statistics
    public void DisplayStatistics()
    {
        int total = cars.Count;
        int active = cars.Count(c => c.Status == OperationalStatus.Active);
        double avgBattery = cars.Any() ? cars.Average(c => c.EnergyStatus) : 0;
        Console.WriteLine($"\nTotal Cars: {total}, Active: {active}, Average Battery: {avgBattery:F2}%");
    }
}

// Main Program (Control Panel)
class Program
{
    static void Main()
    {
        VehicleManager manager = new VehicleManager();
        MissionControl control = new MissionControl();
  while (true)
        {   // Menu options
            Console.WriteLine("\n--- Tesla Self-Driving Car Simulation ---");
            Console.WriteLine("1. Add Car");
            Console.WriteLine("2. Update Car");
            Console.WriteLine("3. Remove Car");
            Console.WriteLine("4. View All Cars");
            Console.WriteLine("5. View Car Logs");
            Console.WriteLine("6. Simulate Incident (Battery Failure)");
            Console.WriteLine("7. Trigger Car Event (Route/Obstacle/Restricted Zone)");
            Console.WriteLine("8. Show Statistics");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();
  switch (choice)
            {       case "1": // Add new car
                    Console.Write("Enter Car ID: ");
                    string id = Console.ReadLine();
                    Console.Write("Enter Battery (0-100): ");
                    int battery = int.Parse(Console.ReadLine());
   Console.Write("Enter Status (Active/Idle/Failed): ");
                    OperationalStatus status = Enum.Parse<OperationalStatus>(Console.ReadLine(), true);
                    Console.Write("Enter Priority (High/Medium/Low): ");
                    MissionPriority priority = Enum.Parse<MissionPriority>(Console.ReadLine(), true);
                    Car car = new Car(id, battery, status, priority);
                    control.Subscribe(car); // Subscribe MissionControl to this car
                    manager.AddCar(car);
                    break;
  case "2": // Update car details
                    Console.Write("Enter Car ID to update: ");
                    string updateID = Console.ReadLine();
                    Console.Write("Enter Battery (0-100): ");
                    int newBattery = int.Parse(Console.ReadLine());
                    Console.Write("Enter Status (Active/Idle/Failed): ");
                    OperationalStatus newStatus = Enum.Parse<OperationalStatus>(Console.ReadLine(), true);
                    Console.Write("Enter Priority (High/Medium/Low): ");
                    MissionPriority newPriority = Enum.Parse<MissionPriority>(Console.ReadLine(), true);
                    manager.UpdateCar(updateID, newBattery, newStatus, newPriority);
                    break;
                   case "3": // Remove car
                    Console.Write("Enter Car ID to remove: ");
                    string removeID = Console.ReadLine();
                    manager.RemoveCar(removeID);
                    break;
    case "4":
 // Display all cars
   manager.DisplayCars();
                    break;
 case "5": // Display logs of a car
                    Console.Write("Enter Car ID to view logs: ");
                    string logID = Console.ReadLine();
                    manager.DisplayCarLogs(logID);
                    break;
 case "6": // Simulate incident: Battery failure
                    Console.Write("Enter Car ID to simulate incident: ");
                    string incidentID = Console.ReadLine();
                    Car incidentCar = manager.GetCar(incidentID);
                    if (incidentCar != null) incidentCar.UpdateEnergy(5); // Force battery low
                    else Console.WriteLine("Car not found.");
                    break;
 case "7": // Trigger car events
                    Console.Write("Enter Car ID to trigger event: ");
                    string eventID = Console.ReadLine();
                    Car eventCar = manager.GetCar(eventID);
                    if (eventCar != null)
                    {
                        Console.WriteLine("Select Event: 1. Route Completed 2. Obstacle 3. Restricted Zone");
                        string eChoice = Console.ReadLine();
                        if (eChoice == "1") eventCar.CompleteRoute();
                        else if (eChoice == "2") eventCar.DetectObstacle();
                        else if (eChoice == "3") eventCar.EnterRestrictedZone();
  else Console.WriteLine("Invalid event choice.");
                    }
                    else Console.WriteLine("Car not found.");
                    break;
    case "8": // Show overall statistics
                    manager.DisplayStatistics();
                    break;
  case "0": // Exit program
                    return;
  default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

        }
    }
}


