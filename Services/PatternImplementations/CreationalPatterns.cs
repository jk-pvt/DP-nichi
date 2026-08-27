using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatternCatalog.Services.PatternImplementations;

public interface ITransport
{
    string VehicleName { get; }
    double BaseSpeedKmH { get; }
    double VehicleMultiplier { get; }
    string Deliver(string cargo, double distanceKm);
    string GetTelemetry(double distanceKm);
}

public class HighwayTruck : ITransport
{
    public string VehicleName => "Highway Freight Truck (Ground)";
    public double BaseSpeedKmH => 70.0;
    public double VehicleMultiplier => 1.0;

    public string Deliver(string cargo, double distanceKm) =>
        $"[Highway Truck] Carrying '{cargo}' over {distanceKm:N0} km via National Express Highway.";

    public string GetTelemetry(double distanceKm) =>
        $"Ground Route: Highway Corridor. Speed: {BaseSpeedKmH} km/h. ETA: {Math.Round(distanceKm / BaseSpeedKmH, 1)} hours.";
}

public class CargoContainerShip : ITransport
{
    public string VehicleName => "Cargo Container Ship (Sea)";
    public double BaseSpeedKmH => 35.0;
    public double VehicleMultiplier => 0.65;

    public string Deliver(string cargo, double distanceKm) =>
        $"[Cargo Ship] Moving '{cargo}' over {distanceKm:N0} km via Maritime Shipping Lane.";

    public string GetTelemetry(double distanceKm) =>
        $"Maritime Route: Port Berth #4. Speed: {BaseSpeedKmH} km/h. ETA: {Math.Ceiling(distanceKm / (BaseSpeedKmH * 24.0))} days.";
}

public class Boeing777Freighter : ITransport
{
    public string VehicleName => "Boeing 777 Cargo Jet (Air)";
    public double BaseSpeedKmH => 820.0;
    public double VehicleMultiplier => 2.5;

    public string Deliver(string cargo, double distanceKm) =>
        $"[Cargo Jet] Flying '{cargo}' over {distanceKm:N0} km via Air Cargo Express Route.";

    public string GetTelemetry(double distanceKm) =>
        $"Air Corridor: FL380 Skyway. Speed: {BaseSpeedKmH} km/h. Flight ETA: {Math.Round(distanceKm / BaseSpeedKmH, 1)} hours.";
}

public class AutonomousDroneFleet : ITransport
{
    public string VehicleName => "Autonomous Drone Fleet (Rapid Air)";
    public double BaseSpeedKmH => 130.0;
    public double VehicleMultiplier => 1.8;

    public string Deliver(string cargo, double distanceKm) =>
        $"[Drone Fleet] Transporting '{cargo}' over {distanceKm:N0} km via Autonomous Low-Altitude Grid.";

    public string GetTelemetry(double distanceKm) =>
        $"Drone Grid: Point-to-Point Direct Flight. Speed: {BaseSpeedKmH} km/h. ETA: {Math.Round(distanceKm / BaseSpeedKmH, 1)} hours.";
}

public abstract class Logistics
{
    public abstract ITransport CreateTransport();

    public string PlanDelivery(string cargo, double distanceKm, double customRatePerKm, HashSet<string> addOns)
    {
        ITransport transport = CreateTransport();
        
        double baseCost = distanceKm * customRatePerKm * transport.VehicleMultiplier;
        double addOnCost = 0;
        var addOnLines = new List<string>();

        if (addOns.Any(a => a.Contains("Express 24/7")))
        {
            double fee = baseCost * 0.15;
            addOnCost += fee;
            addOnLines.Add($"  + Express Priority Surcharge (15%): ₹{fee:N2}");
        }
        if (addOns.Any(a => a.Contains("Refrigeration")))
        {
            double fee = 500.00;
            addOnCost += fee;
            addOnLines.Add($"  + Climate Controlled Refrigeration: ₹{fee:N2}");
        }
        if (addOns.Any(a => a.Contains("Hazardous")))
        {
            double fee = 1200.00;
            addOnCost += fee;
            addOnLines.Add($"  + Hazardous Material Safety Handling: ₹{fee:N2}");
        }
        if (addOns.Any(a => a.Contains("Insurance")))
        {
            double fee = baseCost * 0.025;
            addOnCost += fee;
            addOnLines.Add($"  + Full Cargo Insurance Protection (2.5%): ₹{fee:N2}");
        }

        double totalCost = baseCost + addOnCost;
        string trackingId = $"TRK-{DateTime.Now:mmss}-{Math.Abs(cargo.GetHashCode() % 10000):D4}";

        string addOnsManifest = addOnLines.Count > 0
            ? string.Join("\n", addOnLines) + "\n"
            : "  (None selected)\n";

        return $"[FACTORY METHOD DISPATCH MANIFEST]\n" +
               $"------------------------------------------------------------\n" +
               $"• Factory Creator:     {GetType().Name}.CreateTransport()\n" +
               $"• Vehicle Product:     {transport.VehicleName}\n" +
               $"• Tracking Number:     {trackingId}\n" +
               $"• Cargo Description:   \"{cargo}\"\n" +
               $"• Transit Distance:    {distanceKm:N0} km\n" +
               $"• Base Rate per Km:    ₹{customRatePerKm:F2} (Vehicle Multiplier: x{transport.VehicleMultiplier:F2})\n" +
               $"• Base Transit Cost:   ₹{baseCost:N2}\n" +
               $"• Optional Add-ons:\n{addOnsManifest}" +
               $"• Total Shipping Cost: ₹{totalCost:N2}\n" +
               $"• Dynamic Telemetry:   {transport.GetTelemetry(distanceKm)}\n" +
               $"• Execution Log:       {transport.Deliver(cargo, distanceKm)}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: Vehicle instantiated dynamically with fully computed pricing & telemetry.";
    }
}

public class RoadLogistics : Logistics
{
    public override ITransport CreateTransport() => new HighwayTruck();
}

public class SeaLogistics : Logistics
{
    public override ITransport CreateTransport() => new CargoContainerShip();
}

public class AirLogistics : Logistics
{
    public override ITransport CreateTransport() => new Boeing777Freighter();
}

public class DroneLogistics : Logistics
{
    public override ITransport CreateTransport() => new AutonomousDroneFleet();
}

public interface IButton { string Render(string label, string accent); }
public interface ICheckbox { string Render(string label, string accent); }
public interface ITextBox { string Render(string label, string accent); }
public interface IWindow { string Render(string title, string accent); }

public interface IUIFactory
{
    string FamilyName { get; }
    IButton CreateButton();
    ICheckbox CreateCheckbox();
    ITextBox CreateTextBox();
    IWindow CreateWindow();
}

public class MacButton : IButton { public string Render(string label, string accent) => $"MacButton [\"{label}\"] (SF Pro Font, 8px Corner Radius, Glow: {accent})"; }
public class MacCheckbox : ICheckbox { public string Render(string label, string accent) => $"MacCheckbox [\"{label}\"] (Cupertino Rounded Switch, Active Tint: {accent})"; }
public class MacTextBox : ITextBox { public string Render(string label, string accent) => $"MacTextBox [\"{label}\"] (Inset Field, Spotlight Focus: {accent})"; }
public class MacWindow : IWindow { public string Render(string title, string accent) => $"MacWindow [\"{title}\"] (Glass Acrylic Backdrop, Traffic Light Caption Controls, Accent: {accent})"; }

public class MacUIFactory : IUIFactory
{
    public string FamilyName => "macOS Sequoia UI Suite";
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
    public ITextBox CreateTextBox() => new MacTextBox();
    public IWindow CreateWindow() => new MacWindow();
}

public class WindowsButton : IButton { public string Render(string label, string accent) => $"WinButton [\"{label}\"] (Segoe UI Variable, Fluent Elevation, Accent: {accent})"; }
public class WindowsCheckbox : ICheckbox { public string Render(string label, string accent) => $"WinCheckbox [\"{label}\"] (Fluent Square Check Glyph with {accent} Border)"; }
public class WindowsTextBox : ITextBox { public string Render(string label, string accent) => $"WinTextBox [\"{label}\"] (Underline Highlight in {accent}, Clipboard History)"; }
public class WindowsWindow : IWindow { public string Render(string title, string accent) => $"WinWindow [\"{title}\"] (Mica Shell, Windows 11 Snap Assist, Border: {accent})"; }

public class WindowsUIFactory : IUIFactory
{
    public string FamilyName => "Windows 11 Fluent Suite";
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    public ITextBox CreateTextBox() => new WindowsTextBox();
    public IWindow CreateWindow() => new WindowsWindow();
}

public class LinuxButton : IButton { public string Render(string label, string accent) => $"GtkButton [\"{label}\"] (Adwaita Flat Theme, High Contrast: {accent})"; }
public class LinuxCheckbox : ICheckbox { public string Render(string label, string accent) => $"GtkCheckbox [\"{label}\"] (Standard GNOME Check Toggle with {accent} Accent)"; }
public class LinuxTextBox : ITextBox { public string Render(string label, string accent) => $"GtkTextBox [\"{label}\"] (Cantarell Monospace Font, Cursor: {accent})"; }
public class LinuxWindow : IWindow { public string Render(string title, string accent) => $"GtkWindow [\"{title}\"] (Wayland Native Surface, Client-Side Decorations, Accent: {accent})"; }

public class LinuxUIFactory : IUIFactory
{
    public string FamilyName => "Linux GNOME GTK4 Suite";
    public IButton CreateButton() => new LinuxButton();
    public ICheckbox CreateCheckbox() => new LinuxCheckbox();
    public ITextBox CreateTextBox() => new LinuxTextBox();
    public IWindow CreateWindow() => new LinuxWindow();
}

public class CyberpunkButton : IButton { public string Render(string label, string accent) => $"NeonButton [\"{label}\"] (Angled 45° Cyber Cutout, Neon Pulse: {accent})"; }
public class CyberpunkCheckbox : ICheckbox { public string Render(string label, string accent) => $"NeonCheckbox [\"{label}\"] (Hexagonal Matrix Toggle in {accent})"; }
public class CyberpunkTextBox : ITextBox { public string Render(string label, string accent) => $"NeonTextBox [\"{label}\"] (Terminal CRT Scanlines with {accent} Glow)"; }
public class CyberpunkWindow : IWindow { public string Render(string title, string accent) => $"NeonWindow [\"{title}\"] (Dark Carbon Chassis, Holographic Header: {accent})"; }

public class CyberpunkUIFactory : IUIFactory
{
    public string FamilyName => "Cyberpunk Neon Dark Suite";
    public IButton CreateButton() => new CyberpunkButton();
    public ICheckbox CreateCheckbox() => new CyberpunkCheckbox();
    public ITextBox CreateTextBox() => new CyberpunkTextBox();
    public IWindow CreateWindow() => new CyberpunkWindow();
}

public class DynamicUIApp
{
    private readonly IUIFactory _factory;

    public DynamicUIApp(IUIFactory factory)
    {
        _factory = factory;
    }

    public string BuildComponentFamily(string appTitle, string buttonText, string accentColor, HashSet<string> optionalComponents)
    {
        var window = _factory.CreateWindow();
        var button = _factory.CreateButton();
        var checkbox = _factory.CreateCheckbox();
        var textBox = _factory.CreateTextBox();

        var lines = new List<string>
        {
            $"   [1] Window Shell:   {window.Render(appTitle, accentColor)}",
            $"   [2] Action Button:  {button.Render(buttonText, accentColor)}",
            $"   [3] Toggle Switch:  {checkbox.Render("Enable Auto-Sync", accentColor)}",
            $"   [4] Input Field:    {textBox.Render("Enter Query...", accentColor)}"
        };

        if (optionalComponents.Any(c => c.Contains("Search Bar")))
            lines.Add($"   [5] Search Widget:  {_factory.CreateTextBox().Render("Global Search (Cmd+K / Ctrl+K)", accentColor)}");
        if (optionalComponents.Any(c => c.Contains("Notification")))
            lines.Add($"   [6] Badge Widget:   BadgeIndicator [\"3 Unread\"] ({accentColor} Glow)");
        if (optionalComponents.Any(c => c.Contains("Status Bar")))
            lines.Add($"   [7] Status Footer:  StatusBar [\"Connected - 24ms Latency\"] (Accent: {accentColor})");
        if (optionalComponents.Any(c => c.Contains("Touch Screen")))
            lines.Add($"   [8] Touch Mode:     TouchLayout [Large 48px Touch Targets Enabled]");

        return $"[ABSTRACT FACTORY DYNAMIC MANUFACTURE]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Factory:      {_factory.FamilyName} (implements IUIFactory)\n" +
               $"• Application Title:   \"{appTitle}\"\n" +
               $"• Button Action Text:  \"{buttonText}\"\n" +
               $"• Active Theme Accent: {accentColor}\n" +
               $"• Manufactured Component Family ({lines.Count} Controls):\n" +
               string.Join("\n", lines) + "\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: All components manufactured dynamically with 100% theme compatibility.";
    }
}

public class Computer
{
    public string BuildName { get; set; } = "Custom Workstation";
    public double TargetBudget { get; set; } = 150000;
    public string Motherboard { get; set; } = "ASUS ROG Gaming Board";
    public double MotherboardPrice { get; set; } = 28000;
    public int MotherboardWatts { get; set; } = 75;

    public string CPU { get; set; } = string.Empty;
    public double CpuPrice { get; set; }
    public int CpuWatts { get; set; }

    public string GPU { get; set; } = string.Empty;
    public double GpuPrice { get; set; }
    public int GpuWatts { get; set; }

    public string RAM { get; set; } = string.Empty;
    public double RamPrice { get; set; }
    public int RamWatts { get; set; }

    public string Storage { get; set; } = string.Empty;
    public double StoragePrice { get; set; }
    public int StorageWatts { get; set; }

    public string Cooling { get; set; } = string.Empty;
    public double CoolingPrice { get; set; }
    public int CoolingWatts { get; set; }

    public string RGB { get; set; } = string.Empty;
    public double RgbPrice { get; set; }
    public int RgbWatts { get; set; }

    public string PSU { get; set; } = string.Empty;
    public double PsuPrice { get; set; }

    public string GetSummary()
    {
        double totalPrice = MotherboardPrice + CpuPrice + GpuPrice + RamPrice + StoragePrice + CoolingPrice + RgbPrice + PsuPrice;
        int totalWatts = MotherboardWatts + CpuWatts + GpuWatts + RamWatts + StorageWatts + CoolingWatts + RgbWatts;
        int recommendedPsu = (int)(Math.Ceiling(totalWatts * 1.35 / 50.0) * 50);

        double budgetDiff = TargetBudget - totalPrice;
        string budgetStatus = budgetDiff >= 0
            ? $"✅ UNDER BUDGET by ₹{budgetDiff:N2} ({(totalPrice / TargetBudget * 100):F1}% of limit utilized)"
            : $"⚠️ OVER BUDGET by ₹{Math.Abs(budgetDiff):N2} ({(totalPrice / TargetBudget * 100):F1}% of limit utilized)";

        return $"[BUILDER STEP-BY-STEP COMPUTER ASSEMBLY]\n" +
               $"------------------------------------------------------------\n" +
               $"• Rig Name:            \"{BuildName}\"\n" +
               $"• Target Budget Limit: ₹{TargetBudget:N2}\n\n" +
               $"STEP-BY-STEP BUILD MANIFEST:\n" +
               $"  1. SetMotherboard:   {Motherboard} (₹{MotherboardPrice:N0}, {MotherboardWatts}W)\n" +
               $"  2. SetCPU:           {CPU} (₹{CpuPrice:N0}, {CpuWatts}W)\n" +
               $"  3. SetGPU:           {GPU} (₹{GpuPrice:N0}, {GpuWatts}W)\n" +
               $"  4. SetRAM:           {RAM} (₹{RamPrice:N0}, {RamWatts}W)\n" +
               $"  5. SetStorage:       {Storage} (₹{StoragePrice:N0}, {StorageWatts}W)\n" +
               $"  6. SetCooling:       {Cooling} (₹{CoolingPrice:N0}, {CoolingWatts}W)\n" +
               $"  7. SetLighting:      {RGB} (₹{RgbPrice:N0}, {RgbWatts}W)\n" +
               $"  8. SetPowerSupply:   {PSU} (₹{PsuPrice:N0})\n" +
               $"------------------------------------------------------------\n" +
               $"CALCULATED METRICS:\n" +
               $"• Total Build Cost:    ₹{totalPrice:N2}\n" +
               $"• Total Power Draw:    {totalWatts} Watts (Recommended PSU: {recommendedPsu}W)\n" +
               $"• Budget Analysis:     {budgetStatus}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [SUCCESS] Computer constructed step-by-step with 100% dynamic calculations.";
    }
}

public interface IComputerBuilder
{
    IComputerBuilder SetBuildName(string name);
    IComputerBuilder SetTargetBudget(double budget);
    IComputerBuilder SetMotherboard(string mb, double price, int watts);
    IComputerBuilder SetCPU(string cpu, double price, int watts);
    IComputerBuilder SetGPU(string gpu, double price, int watts);
    IComputerBuilder SetRAM(string ram, double price, int watts);
    IComputerBuilder SetStorage(string storage, double price, int watts);
    IComputerBuilder SetCooling(string cooling, double price, int watts);
    IComputerBuilder SetRGB(string rgb, double price, int watts);
    IComputerBuilder SetPSU(string psu, double price);
    Computer Build();
}

public class CustomComputerBuilder : IComputerBuilder
{
    private Computer _pc = new();

    public void Reset() => _pc = new();

    public IComputerBuilder SetBuildName(string name) { _pc.BuildName = name; return this; }
    public IComputerBuilder SetTargetBudget(double budget) { _pc.TargetBudget = budget; return this; }
    public IComputerBuilder SetMotherboard(string mb, double price, int watts) { _pc.Motherboard = mb; _pc.MotherboardPrice = price; _pc.MotherboardWatts = watts; return this; }
    public IComputerBuilder SetCPU(string cpu, double price, int watts) { _pc.CPU = cpu; _pc.CpuPrice = price; _pc.CpuWatts = watts; return this; }
    public IComputerBuilder SetGPU(string gpu, double price, int watts) { _pc.GPU = gpu; _pc.GpuPrice = price; _pc.GpuWatts = watts; return this; }
    public IComputerBuilder SetRAM(string ram, double price, int watts) { _pc.RAM = ram; _pc.RamPrice = price; _pc.RamWatts = watts; return this; }
    public IComputerBuilder SetStorage(string storage, double price, int watts) { _pc.Storage = storage; _pc.StoragePrice = price; _pc.StorageWatts = watts; return this; }
    public IComputerBuilder SetCooling(string cooling, double price, int watts) { _pc.Cooling = cooling; _pc.CoolingPrice = price; _pc.CoolingWatts = watts; return this; }
    public IComputerBuilder SetRGB(string rgb, double price, int watts) { _pc.RGB = rgb; _pc.RgbPrice = price; _pc.RgbWatts = watts; return this; }
    public IComputerBuilder SetPSU(string psu, double price) { _pc.PSU = psu; _pc.PsuPrice = price; return this; }

    public Computer Build()
    {
        Computer result = _pc;
        Reset();
        return result;
    }
}

public interface IPrototype<T>
{
    T Clone();
}

public class DocumentTemplate : IPrototype<DocumentTemplate>
{
    public string Title { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string ReferenceCode { get; set; } = string.Empty;
    public string PriorityWatermark { get; set; } = "Standard";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public List<string> Sections { get; set; } = new();

    public DocumentTemplate Clone()
    {
        return new DocumentTemplate
        {
            Title = this.Title,
            Recipient = this.Recipient,
            ReferenceCode = this.ReferenceCode,
            PriorityWatermark = this.PriorityWatermark,
            CreatedDate = this.CreatedDate,
            Sections = new List<string>(this.Sections)
        };
    }
}

public sealed class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> _instance = new(() => new AppConfiguration());
    public static AppConfiguration Instance => _instance.Value;

    public DateTime InitializedAt { get; } = DateTime.UtcNow;
    public string HostUri { get; set; } = "db-cluster.internal:5432";
    public int MaxThreads { get; set; } = 64;
    public string Environment { get; set; } = "Production";
    public string CachePolicy { get; set; } = "Standard TTL (15m)";
    public bool RedisCluster { get; set; } = true;
    public bool TlsEncryption { get; set; } = true;
    public bool AuditLogging { get; set; } = true;
    public bool AutoScaling { get; set; } = false;
    public int RequestCounter { get; set; } = 1042;

    private AppConfiguration() { }
}
