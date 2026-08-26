using System;
using System.Collections.Generic;

namespace DesignPatternCatalog.Services.PatternImplementations;

public interface ITransport
{
    string Deliver(string cargoDescription);
    double CalculateCost(double distanceKm);
    string GetTelemetry(double distanceKm);
}

public class HighwayTruck : ITransport
{
    public string Deliver(string cargoDescription) =>
        $"[Truck Transport] Carrying '{cargoDescription}' via National Highway Corridor.";

    public double CalculateCost(double distanceKm) => distanceKm * 145.00;

    public string GetTelemetry(double distanceKm) =>
        $"Ground Transit ETA: {Math.Round(distanceKm / 75.0, 1)} hours. Temperature: Ambient. Secure palletized freight.";
}

public class CargoContainerShip : ITransport
{
    public string Deliver(string cargoDescription) =>
        $"[Maritime Cargo Ship] Transporting '{cargoDescription}' overseas across maritime lanes.";

    public double CalculateCost(double distanceKm) => distanceKm * 35.50;

    public string GetTelemetry(double distanceKm) =>
        $"Maritime Voyage ETA: {Math.Ceiling(distanceKm / 600.0)} days. Dispatched via Port Berth #4. High capacity.";
}

public class Boeing777Freighter : ITransport
{
    public string Deliver(string cargoDescription) =>
        $"[Express Air Cargo] Flying '{cargoDescription}' via dedicated air cargo express.";

    public double CalculateCost(double distanceKm) => distanceKm * 380.00;

    public string GetTelemetry(double distanceKm) =>
        $"Express Flight ETA: {Math.Round(distanceKm / 850.0, 1)} hours. Dispatched from Air Cargo Terminal Gate.";
}

public abstract class Logistics
{
    public abstract ITransport CreateTransport();

    public string PlanDelivery(string cargo, double distanceKm)
    {
        ITransport transport = CreateTransport();
        string delivery = transport.Deliver(cargo);
        double cost = transport.CalculateCost(distanceKm);
        string telemetry = transport.GetTelemetry(distanceKm);
        string trackingId = $"TRK-{DateTime.Now:mmss}-{Math.Abs(cargo.GetHashCode() % 10000):D4}";

        return $"[FACTORY METHOD EXECUTION MANIFEST]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Creator:      {GetType().Name}.CreateTransport()\n" +
               $"• Concrete Product:    {transport.GetType().Name} (implements ITransport)\n" +
               $"• Tracking Number:     {trackingId}\n" +
               $"• Cargo Description:   \"{cargo}\"\n" +
               $"• Transit Distance:    {distanceKm:N0} km\n" +
               $"• Total Shipping Cost: ₹{cost:N2}\n" +
               $"• Dispatch Telemetry:  {telemetry}\n" +
               $"• Action Execution:    {delivery}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [CONFIRMED] Factory cleanly decoupled caller from {transport.GetType().Name}.";
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

public interface IButton { string Render(); }
public interface ICheckbox { string Render(); }
public interface ITextBox { string Render(); }
public interface IWindow { string Render(); }

public interface IUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
    ITextBox CreateTextBox();
    IWindow CreateWindow();
}

public class MacButton : IButton { public string Render() => "MacButton (SF Pro typography, Glass acrylic highlight)"; }
public class MacCheckbox : ICheckbox { public string Render() => "MacCheckbox (Cupertino accent toggle switch)"; }
public class MacTextBox : ITextBox { public string Render() => "MacTextBox (Inset rounded field with Cmd+K Spotlight focus)"; }
public class MacWindow : IWindow { public string Render() => "MacWindow (Glass Acrylic Backdrop, Traffic Light Caption Controls)"; }

public class MacUIFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
    public ITextBox CreateTextBox() => new MacTextBox();
    public IWindow CreateWindow() => new MacWindow();
}

public class WindowsButton : IButton { public string Render() => "WinButton (Segoe UI Variable, Fluent elevation, 4px corner)"; }
public class WindowsCheckbox : ICheckbox { public string Render() => "WinCheckbox (Fluent square check glyph with accent border)"; }
public class WindowsTextBox : ITextBox { public string Render() => "WinTextBox (Underline accent highlight with Win+V history)"; }
public class WindowsWindow : IWindow { public string Render() => "WinWindow (Mica Acrylic Shell, Windows 11 Snap Layout Assist)"; }

public class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    public ITextBox CreateTextBox() => new WindowsTextBox();
    public IWindow CreateWindow() => new WindowsWindow();
}

public class LinuxButton : IButton { public string Render() => "GtkButton (Adwaita Flat High-Contrast Theme)"; }
public class LinuxCheckbox : ICheckbox { public string Render() => "GtkCheckbox (Standard GNOME check toggle)"; }
public class LinuxTextBox : ITextBox { public string Render() => "GtkTextBox (Monospace Cantarell typography)"; }
public class LinuxWindow : IWindow { public string Render() => "GtkWindow (Wayland Native Surface, Client-Side Decorations)"; }

public class LinuxUIFactory : IUIFactory
{
    public IButton CreateButton() => new LinuxButton();
    public ICheckbox CreateCheckbox() => new LinuxCheckbox();
    public ITextBox CreateTextBox() => new LinuxTextBox();
    public IWindow CreateWindow() => new LinuxWindow();
}

public class CrossPlatformApp
{
    private readonly IButton _button;
    private readonly ICheckbox _checkbox;
    private readonly ITextBox _textBox;
    private readonly IWindow _window;
    private readonly string _factoryName;

    public CrossPlatformApp(IUIFactory factory)
    {
        _factoryName = factory.GetType().Name;
        _window = factory.CreateWindow();
        _button = factory.CreateButton();
        _checkbox = factory.CreateCheckbox();
        _textBox = factory.CreateTextBox();
    }

    public string RenderSuite(string appTitle)
    {
        return $"[ABSTRACT FACTORY EXECUTION]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Factory:       {_factoryName} (implements IUIFactory)\n" +
               $"• Application Context:  \"{appTitle}\"\n" +
               $"• Manufactured Family:\n" +
               $"   [1] {_window.Render()}\n" +
               $"   [2] {_button.Render()}\n" +
               $"   [3] {_checkbox.Render()}\n" +
               $"   [4] {_textBox.Render()}\n" +
               $"------------------------------------------------------------\n" +
               $"VERIFICATION: All 4 controls guaranteed 100% compatible across the UI family.";
    }
}

public class Computer
{
    public string Motherboard { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string GPU { get; set; } = string.Empty;
    public int RamGB { get; set; }
    public int StorageGB { get; set; }
    public bool HasLiquidCooling { get; set; }
    public bool HasRGB { get; set; }
    public double TotalPrice { get; set; }
    public int EstimatedWattage { get; set; }

    public string GetSummary()
    {
        string tier = TotalPrice > 200000 ? "TIER 1: ULTRA ENTHUSIAST (4K 144Hz Gaming / 8K Video Rendering)" :
                      TotalPrice > 100000 ? "TIER 2: HIGH PERFORMANCE (1440p High Refresh / Content Creation)" :
                      "TIER 3: BUDGET PRODUCTIVITY (1080p Esports / Daily Dev)";

        return $"[BUILDER PATTERN STEP-BY-STEP ASSEMBLY]\n" +
               $"------------------------------------------------------------\n" +
               $"1. Builder.SetMotherboard(\"{Motherboard}\")\n" +
               $"2. Builder.SetCPU(\"{CPU}\")\n" +
               $"3. Builder.SetGPU(\"{GPU}\")\n" +
               $"4. Builder.SetRAM({RamGB}GB DDR5-6000MHz)\n" +
               $"5. Builder.SetStorage({StorageGB}GB PCIe Gen4 NVMe)\n" +
               $"6. Builder.SetCooling(\"{(HasLiquidCooling ? "360mm AIO Liquid Cooler" : "Dual-Tower Air Cooler")}\")\n" +
               $"7. Builder.SetRGB({HasRGB})\n" +
               $"8. Computer customPC = Builder.Build();\n" +
               $"------------------------------------------------------------\n" +
               $"SPECS SUMMARY:\n" +
               $"• Category:          {tier}\n" +
               $"• Estimated Power:   {EstimatedWattage} Watts (Recommended PSU: {Math.Ceiling(EstimatedWattage * 1.3 / 100) * 100}W)\n" +
               $"• Total Build Price: ₹{TotalPrice:N2}\n" +
               $"STATUS: [SUCCESS] Object assembled step-by-step without telescoping constructors.";
    }
}

public interface IComputerBuilder
{
    IComputerBuilder SetMotherboard(string mb);
    IComputerBuilder SetCPU(string cpu, double price, int watts);
    IComputerBuilder SetGPU(string gpu, double price, int watts);
    IComputerBuilder SetRAM(int ramGb, double price);
    IComputerBuilder SetStorage(int storageGb, double price);
    IComputerBuilder SetCooling(bool liquid, double price, int watts);
    IComputerBuilder SetRGB(bool rgb, double price);
    Computer Build();
}

public class CustomComputerBuilder : IComputerBuilder
{
    private Computer _computer = new() { Motherboard = "ASUS ROG Strix Gaming", TotalPrice = 32000, EstimatedWattage = 150 };

    public void Reset() => _computer = new() { Motherboard = "ASUS ROG Strix Gaming", TotalPrice = 32000, EstimatedWattage = 150 };

    public IComputerBuilder SetMotherboard(string mb) { _computer.Motherboard = mb; return this; }
    public IComputerBuilder SetCPU(string cpu, double price, int watts) { _computer.CPU = cpu; _computer.TotalPrice += price; _computer.EstimatedWattage += watts; return this; }
    public IComputerBuilder SetGPU(string gpu, double price, int watts) { _computer.GPU = gpu; _computer.TotalPrice += price; _computer.EstimatedWattage += watts; return this; }
    public IComputerBuilder SetRAM(int ramGb, double price) { _computer.RamGB = ramGb; _computer.TotalPrice += price; return this; }
    public IComputerBuilder SetStorage(int storageGb, double price) { _computer.StorageGB = storageGb; _computer.TotalPrice += price; return this; }
    public IComputerBuilder SetCooling(bool liquid, double price, int watts) { _computer.HasLiquidCooling = liquid; _computer.TotalPrice += price; _computer.EstimatedWattage += watts; return this; }
    public IComputerBuilder SetRGB(bool rgb, double price) { _computer.HasRGB = rgb; _computer.TotalPrice += price; return this; }

    public Computer Build()
    {
        Computer result = _computer;
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
    public string ThemeColor { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string ReferenceCode { get; set; } = string.Empty;
    public List<string> Sections { get; set; } = new();

    public DocumentTemplate Clone()
    {
        return new DocumentTemplate
        {
            Title = this.Title,
            ThemeColor = this.ThemeColor,
            Recipient = this.Recipient,
            ReferenceCode = this.ReferenceCode,
            Sections = new List<string>(this.Sections)
        };
    }
}

public sealed class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> _instance = new(() => new AppConfiguration());
    public static AppConfiguration Instance => _instance.Value;

    public DateTime InitializedAt { get; } = DateTime.UtcNow;
    public string Environment { get; set; } = "Production";
    public int MaxConnections { get; set; } = 50;
    public bool RedisCacheEnabled { get; set; } = false;
    public bool AuditLoggingEnabled { get; set; } = true;

    private AppConfiguration() { }
}
