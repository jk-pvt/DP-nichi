using System;
using System.Collections.Generic;
using System.Linq;
using DesignPatternCatalog.Models;
using DesignPatternCatalog.Services.PatternImplementations;

namespace DesignPatternCatalog.Services;

public class PatternRepository
{
    private static readonly Lazy<PatternRepository> _instance = new(() => new PatternRepository());
    public static PatternRepository Instance => _instance.Value;

    private readonly List<PatternItem> _patterns;

    public PatternRepository()
    {
        _patterns = InitializePatterns();
    }

    public IReadOnlyList<PatternItem> GetAllPatterns() => _patterns;

    public PatternItem? GetPatternById(string id) =>
        _patterns.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<PatternItem> GetPatternsByCategory(PatternCategory category) =>
        _patterns.Where(p => p.Category == category);

    private List<PatternItem> InitializePatterns()
    {
        return new List<PatternItem>
        {
            new PatternItem
            {
                Id = "factory-method",
                Name = "Factory Method",
                Category = PatternCategory.Creational,
                AccentColor = "#FFFFFF",
                IconGeometry = "M3,4H7V8H3V4M9,5V7H21V5H9M3,10H7V14H3V10M9,11V13H21V11H9M3,16H7V20H3V16M9,17V19H21V17H9Z",
                DefinitionLine1 = "Provides a method to create objects without specifying the exact class to create.",
                DefinitionLine2 = "Lets subclasses decide which object to make at runtime.",
                RealLifeTitle = "Real-Life Example: Delivery Services",
                RealLifeAnalogy = "Imagine a delivery company. At first, you only deliver packages by truck. Later, you want to add ships and airplanes. Instead of changing your whole code each time you add a new vehicle, you use a Factory method that creates the right vehicle based on where the package is going.",
                RealLifeProblem = "If your code creates trucks directly with 'new Truck()', adding ships or airplanes later means finding and changing code in many different places.",
                RealLifeSolution = "Create a single method that makes the vehicle for you. Adding a new vehicle type is as simple as creating a new subclass without touching the rest of your app.",
                DemoTitle = "Delivery Dispatch Simulator",
                DemoDescription = "Pick a transport type and distance to see how the Factory creates the vehicle and calculates cost.",
                InputLabel1 = "Package Description",
                DefaultInput1 = "Medical Supplies",
                InputLabel2 = "Distance (km)",
                DefaultInput2 = "1250",
                OptionList1 = new List<string> { "Highway Truck (Road)", "Cargo Ship (Sea)", "Airplane (Air)" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Dispatch Vehicle", Parameter = "dispatch", Description = "Calls CreateTransport() and runs delivery." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string cargo = string.IsNullOrWhiteSpace(ctx.Input1) ? "General Goods" : ctx.Input1;
                    if (!double.TryParse(ctx.Input2, out double distance)) distance = 1000;
                    string mode = ctx.SelectedOption1;

                    Logistics logistics = mode.Contains("Ship") ? new SeaLogistics() :
                                          mode.Contains("Air") ? new AirLogistics() :
                                          new RoadLogistics();

                    return logistics.PlanDelivery(cargo, distance);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "ITransport.cs",
                        Role = "Product Interface",
                        Description = "Defines what every delivery vehicle must be able to do.",
                        Code = @"namespace DeliveryApp;

public interface ITransport
{
    string Deliver(string cargo);
    double CalculateCost(double distanceKm);
}"
                    },
                    new()
                    {
                        FileName = "Truck.cs",
                        Role = "Concrete Product (Truck)",
                        Description = "Road delivery implementation.",
                        Code = @"using DeliveryApp;

public class Truck : ITransport
{
    public string Deliver(string cargo) => $""[Truck] Carrying '{cargo}' by road."";
    public double CalculateCost(double distanceKm) => distanceKm * 145.00;
}"
                    },
                    new()
                    {
                        FileName = "Logistics.cs",
                        Role = "Factory Base & Subclasses",
                        Description = "The factory that creates the vehicle.",
                        Code = @"using DeliveryApp;

public abstract class Logistics
{
    public abstract ITransport CreateTransport();

    public string PlanDelivery(string cargo, double distanceKm)
    {
        ITransport transport = CreateTransport();
        return transport.Deliver(cargo) + $"" Cost: ₹{transport.CalculateCost(distanceKm):F2}"";
    }
}

public class RoadLogistics : Logistics
{
    public override ITransport CreateTransport() => new Truck();
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "abstract-factory",
                Name = "Abstract Factory",
                Category = PatternCategory.Creational,
                AccentColor = "#FFFFFF",
                IconGeometry = "M2,4H8V10H2V4M10,4H22V6H10V4M10,8H18V10H10V8M2,14H8V20H2V14M10,14H22V16H10V14M10,18H18V20H10V18Z",
                DefinitionLine1 = "Creates families of related objects without specifying their concrete classes.",
                DefinitionLine2 = "Ensures that all created items match and work properly together.",
                RealLifeTitle = "Real-Life Example: Matching UI Themes",
                RealLifeAnalogy = "When you switch your phone or laptop between Light theme and Dark theme (or macOS and Windows), all the buttons, checkboxes, and text boxes change together so everything matches nicely.",
                RealLifeProblem = "If you mix and match controls by hand, you might accidentally put a Windows button inside a Mac window, making the app look broken.",
                RealLifeSolution = "Use a factory that produces a whole set of matching controls (Buttons, Checkboxes, TextBoxes) designed for that specific theme or OS.",
                DemoTitle = "UI Theme Factory Simulator",
                DemoDescription = "Select an operating system to manufacture a complete matching set of UI controls.",
                OptionList1 = new List<string> { "macOS Style Theme", "Windows Style Theme", "Linux Style Theme" },
                InputLabel1 = "App Name",
                DefaultInput1 = "My Dashboard",
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Build Matching UI Family", Parameter = "build", Description = "Creates matching controls." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string target = ctx.SelectedOption1;
                    string title = string.IsNullOrWhiteSpace(ctx.Input1) ? "App" : ctx.Input1;

                    IUIFactory factory = target.Contains("macOS") ? new MacUIFactory() :
                                         target.Contains("Windows") ? new WindowsUIFactory() :
                                         new LinuxUIFactory();

                    CrossPlatformApp app = new CrossPlatformApp(factory);
                    return app.RenderSuite(title);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IUIFactory.cs",
                        Role = "Factory Interface",
                        Description = "Defines methods to create a whole family of UI elements.",
                        Code = @"namespace UIDemo;

public interface IButton { string Render(); }
public interface ICheckbox { string Render(); }

public interface IUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}"
                    },
                    new()
                    {
                        FileName = "MacUIFactory.cs",
                        Role = "Mac Factory",
                        Description = "Produces matching Mac buttons and checkboxes.",
                        Code = @"namespace UIDemo;

public class MacButton : IButton { public string Render() => ""[Mac] Rounded Button""; }
public class MacCheckbox : ICheckbox { public string Render() => ""[Mac] Switch Toggle""; }

public class MacUIFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "builder",
                Name = "Builder",
                Category = PatternCategory.Creational,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4M11,6V13.25L16.25,16.4L17,15.15L12.5,12.5V6H11Z",
                DefinitionLine1 = "Constructs complex objects step by step using simple, readable methods.",
                DefinitionLine2 = "Lets you produce different types of an object using the same building process.",
                RealLifeTitle = "Real-Life Example: Custom PC or Burger Order",
                RealLifeAnalogy = "When you order a custom pizza or build a PC, you pick the parts one by one: CPU, graphics card, RAM, and extra cooling. You only choose what you need step by step.",
                RealLifeProblem = "Creating a complex object with 15 optional settings makes constructors long, confusing, and hard to read.",
                RealLifeSolution = "Use a Builder with simple methods like SetCPU() and SetRAM(), and finish by calling Build() when you are done.",
                DemoTitle = "Custom Computer Builder",
                DemoDescription = "Pick components step-by-step and let the Builder assemble the computer and calculate price and power.",
                OptionList1 = new List<string> { "Intel Core i9 (High Performance)", "AMD Ryzen 9 (Gaming & Work)", "Intel Core i5 (Daily Work)" },
                OptionList2 = new List<string> { "NVIDIA RTX 4090 (Top Tier)", "AMD Radeon 7900 (High Tier)", "NVIDIA RTX 4070 (Mid Tier)", "Basic Built-in Graphics" },
                ToggleList = new List<string> { "Extra 64GB RAM", "Extra 2TB Fast Storage", "Liquid Cooling Fan", "RGB Lighting Lights" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Assemble Computer", Parameter = "build", Description = "Runs Builder steps." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string cpu = ctx.SelectedOption1;
                    string gpu = ctx.SelectedOption2;
                    bool highRam = ctx.ActiveToggles.Contains("Extra 64GB RAM");
                    bool highStorage = ctx.ActiveToggles.Contains("Extra 2TB Fast Storage");
                    bool liquidCooling = ctx.ActiveToggles.Contains("Liquid Cooling Fan");
                    bool rgb = ctx.ActiveToggles.Contains("RGB Lighting Lights");

                    double cpuPrice = cpu.Contains("i9") ? 52000 : cpu.Contains("Ryzen") ? 58000 : 21000;
                    int cpuWatts = cpu.Contains("i9") ? 253 : cpu.Contains("Ryzen") ? 162 : 65;

                    double gpuPrice = gpu.Contains("4090") ? 165000 : gpu.Contains("7900") ? 89000 : gpu.Contains("4070") ? 74000 : 0;
                    int gpuWatts = gpu.Contains("4090") ? 450 : gpu.Contains("7900") ? 355 : gpu.Contains("4070") ? 285 : 15;

                    var builder = new CustomComputerBuilder();
                    builder.SetMotherboard("Standard Gaming Board")
                           .SetCPU(cpu, cpuPrice, cpuWatts)
                           .SetGPU(gpu, gpuPrice, gpuWatts)
                           .SetRAM(highRam ? 64 : 16, highRam ? 18500 : 5500)
                           .SetStorage(highStorage ? 2000 : 512, highStorage ? 14000 : 4000)
                           .SetCooling(liquidCooling, liquidCooling ? 15500 : 3200, liquidCooling ? 30 : 10)
                           .SetRGB(rgb, rgb ? 6500 : 0);

                    Computer pc = builder.Build();
                    return pc.GetSummary();
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "Computer.cs",
                        Role = "Product Class",
                        Description = "The computer being built.",
                        Code = @"namespace PCBuilder;

public class Computer
{
    public string CPU { get; set; } = string.Empty;
    public string GPU { get; set; } = string.Empty;
    public int RAM { get; set; }
}"
                    },
                    new()
                    {
                        FileName = "ComputerBuilder.cs",
                        Role = "Builder Class",
                        Description = "Builds the computer step by step.",
                        Code = @"namespace PCBuilder;

public class ComputerBuilder
{
    private Computer _pc = new();

    public ComputerBuilder SetCPU(string cpu) { _pc.CPU = cpu; return this; }
    public ComputerBuilder SetRAM(int ram) { _pc.RAM = ram; return this; }
    public Computer Build() => _pc;
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "prototype",
                Name = "Prototype",
                Category = PatternCategory.Creational,
                AccentColor = "#FFFFFF",
                IconGeometry = "M19,3H14.82C14.4,1.84 13.3,1 12,1C10.7,1 9.6,1.84 9.18,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M12,3A1,1 0 0,1 13,4A1,1 0 0,1 12,5A1,1 0 0,1 11,4A1,1 0 0,1 12,3M7,7H17V5H19V19H5V5H7V7Z",
                DefinitionLine1 = "Makes exact copies of existing objects without writing duplicate creation code.",
                DefinitionLine2 = "Delegates the cloning process to the object itself.",
                RealLifeTitle = "Real-Life Example: Copying a Document Template",
                RealLifeAnalogy = "Instead of creating a 20-page business document from scratch every time, you duplicate an existing template and just fill in the new customer name and date.",
                RealLifeProblem = "Creating a fresh copy of an object from outside can be difficult if some internal data is private or complex.",
                RealLifeSolution = "Add a Clone() method directly on the object so it can make an exact copy of itself instantly.",
                DemoTitle = "Document Cloner Simulator",
                DemoDescription = "Pick a master template, enter new customer details, and clone it in memory.",
                OptionList1 = new List<string> { "Standard Business Invoice", "Course Certificate", "Monthly Report" },
                InputLabel1 = "Customer Name",
                DefaultInput1 = "Rahul Sharma",
                InputLabel2 = "Reference ID",
                DefaultInput2 = "INV-2026-01",
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Clone Document", Parameter = "clone", Description = "Calls Clone() and updates customer name." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string template = ctx.SelectedOption1;
                    string recipient = string.IsNullOrWhiteSpace(ctx.Input1) ? "Customer" : ctx.Input1;
                    string refCode = string.IsNullOrWhiteSpace(ctx.Input2) ? "REF-001" : ctx.Input2;

                    var master = new DocumentTemplate
                    {
                        Title = template,
                        ThemeColor = "#FFFFFF",
                        Recipient = "Master Template",
                        ReferenceCode = "TEMPLATE-00",
                        Sections = new List<string> { "Header", "Table", "Signature" }
                    };

                    DocumentTemplate clone = master.Clone();
                    clone.Recipient = recipient;
                    clone.ReferenceCode = refCode;

                    return $"[PROTOTYPE CLONING RESULT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Original Template: \"{master.Title}\" (Memory ID: #{master.GetHashCode():X})\n" +
                           $"• Cloned Document:   \"{clone.Title}\" (Memory ID: #{clone.GetHashCode():X})\n" +
                           $"• Assigned To:       {clone.Recipient}\n" +
                           $"• Reference ID:      {clone.ReferenceCode}\n" +
                           $"• Cloned Sections:   {clone.Sections.Count} sections copied instantly\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: The document was cloned in memory without rebuilding from scratch.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IPrototype.cs",
                        Role = "Clone Interface",
                        Description = "Defines the Clone method.",
                        Code = @"namespace CloneDemo;

public interface IPrototype<T>
{
    T Clone();
}"
                    },
                    new()
                    {
                        FileName = "InvoiceTemplate.cs",
                        Role = "Clonable Class",
                        Description = "Creates copies of itself.",
                        Code = @"namespace CloneDemo;

public class InvoiceTemplate : IPrototype<InvoiceTemplate>
{
    public string Title { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;

    public InvoiceTemplate Clone() => new InvoiceTemplate
    {
        Title = this.Title,
        Customer = this.Customer
    };
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "singleton",
                Name = "Singleton",
                Category = PatternCategory.Creational,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4M12,6A6,6 0 0,0 6,12A6,6 0 0,0 12,18A6,6 0 0,0 18,12A6,6 0 0,0 12,6Z",
                DefinitionLine1 = "Ensures a class has only one single instance across the entire application.",
                DefinitionLine2 = "Provides a single shared way for everyone to access that instance.",
                RealLifeTitle = "Real-Life Example: Single Office Printer or Global Settings",
                RealLifeAnalogy = "In an office, all workers send their print jobs to one shared printer. You don't buy a separate printer for every single employee. Everyone shares the same one.",
                RealLifeProblem = "If different parts of your app create their own separate settings objects, changing a setting in one screen will not update other screens.",
                RealLifeSolution = "Make the constructor private and provide a single static property 'Instance' so the entire app shares the exact same object.",
                DemoTitle = "Global Settings Singleton Simulator",
                DemoDescription = "Change settings in Module A and verify that Module B sees the exact same updated object in memory.",
                OptionList1 = new List<string> { "Environment: Live", "Environment: Testing", "Environment: Local" },
                InputLabel1 = "Max Connections",
                DefaultInput1 = "50",
                ToggleList = new List<string> { "Enable Fast Cache", "Enable Activity Logging" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Save in Module A", Parameter = "save", Description = "Updates AppSettings.Instance." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string env = ctx.SelectedOption1;
                    if (!int.TryParse(ctx.Input1, out int pool)) pool = 50;
                    bool redis = ctx.ActiveToggles.Contains("Enable Fast Cache");
                    bool audit = ctx.ActiveToggles.Contains("Enable Activity Logging");

                    AppConfiguration moduleA = AppConfiguration.Instance;
                    moduleA.Environment = env;
                    moduleA.MaxConnections = pool;
                    moduleA.RedisCacheEnabled = redis;
                    moduleA.AuditLoggingEnabled = audit;

                    AppConfiguration moduleB = AppConfiguration.Instance;
                    bool sameInstance = object.ReferenceEquals(moduleA, moduleB);

                    return $"[SINGLETON INSTANCE CHECK]\n" +
                           $"------------------------------------------------------------\n" +
                           $"MODULE A (Saved Settings):\n" +
                           $"• Memory ID:       #{moduleA.GetHashCode():X}\n" +
                           $"• Environment:     {moduleA.Environment}\n" +
                           $"• Max Connections: {moduleA.MaxConnections}\n\n" +
                           $"MODULE B (Read Settings):\n" +
                           $"• Memory ID:       #{moduleB.GetHashCode():X} (Same instance!)\n" +
                           $"• Environment:     {moduleB.Environment}\n" +
                           $"• Both Match:      {sameInstance}\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: All modules share the exact same configuration object.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "AppSettings.cs",
                        Role = "Singleton Class",
                        Description = "Ensures only one copy exists.",
                        Code = @"namespace SettingsDemo;

public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _instance = 
        new(() => new AppSettings());

    public static AppSettings Instance => _instance.Value;

    private AppSettings() { }
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "adapter",
                Name = "Adapter",
                Category = PatternCategory.Structural,
                AccentColor = "#FFFFFF",
                IconGeometry = "M4,2A2,2 0 0,0 2,4V20A2,2 0 0,0 4,22H20A2,2 0 0,0 22,20V4A2,2 0 0,0 20,2H4M4,4H20V20H4V4M6,6V10H10V6H6M14,6V10H18V6H14M6,14V18H10V14H6M14,14V18H18V14H14Z",
                DefinitionLine1 = "Allows classes with incompatible interfaces to work together.",
                DefinitionLine2 = "Converts calls from one format into the format expected by another system.",
                RealLifeTitle = "Real-Life Example: Travel Plug Adapter",
                RealLifeAnalogy = "When you travel to another country, your phone charger plug doesn't fit the wall socket. You don't buy a new phone; you plug it into an adapter that connects your charger to the wall.",
                RealLifeProblem = "Your modern app uses simple JSON format, but an older bank system only accepts old XML format.",
                RealLifeSolution = "Write an Adapter class that takes your simple JSON data, converts it into the XML format the bank wants, and sends it.",
                DemoTitle = "Payment Adapter Simulator",
                DemoDescription = "Pay using modern JSON format or pass through an Adapter for older XML bank systems.",
                InputLabel1 = "Payment Amount (₹)",
                DefaultInput1 = "2500.00",
                InputLabel2 = "Account ID",
                DefaultInput2 = "ACC-9921",
                OptionList1 = new List<string> { "Modern Gateway (JSON)", "Old Bank System (XML Adapter)" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Make Payment", Parameter = "pay", Description = "Runs payment through adapter." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string target = ctx.SelectedOption1;
                    if (!decimal.TryParse(ctx.Input1, out decimal amount)) amount = 1000m;
                    string cust = string.IsNullOrWhiteSpace(ctx.Input2) ? "ACC-01" : ctx.Input2;

                    IPaymentProcessor processor = target.Contains("Modern")
                        ? new ModernStripeGateway()
                        : new LegacyBankAdapter(new LegacyBankSoapSdk());

                    return processor.ProcessPayment(amount, "INR", cust);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IPaymentProcessor.cs",
                        Role = "Standard Interface",
                        Description = "The standard payment format your app uses.",
                        Code = @"namespace PaymentApp;

public interface IPaymentProcessor
{
    string ProcessPayment(decimal amount, string currency, string customerId);
}"
                    },
                    new()
                    {
                        FileName = "OldBankAdapter.cs",
                        Role = "Adapter Class",
                        Description = "Translates payment requests into the old format.",
                        Code = @"namespace PaymentApp;

public class OldBankAdapter : IPaymentProcessor
{
    private readonly OldBankService _oldBank = new();

    public string ProcessPayment(decimal amount, string currency, string customerId)
    {
        string xml = $""<Payment><Amount>{amount}</Amount></Payment>"";
        return _oldBank.SendXml(xml);
    }
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "bridge",
                Name = "Bridge",
                Category = PatternCategory.Structural,
                AccentColor = "#FFFFFF",
                IconGeometry = "M2,18H4V6H2V4H8V6H6V18H18V6H16V4H22V6H20V18H22V20H2V18M11,4H13V18H11V4Z",
                DefinitionLine1 = "Splits controls (what you see) from devices (how things work) into two separate parts.",
                DefinitionLine2 = "Allows both the controls and devices to be changed independently.",
                RealLifeTitle = "Real-Life Example: TV and Remote Control",
                RealLifeAnalogy = "A remote control has buttons like Power and Volume. Any TV brand (Sony, LG, Samsung) can respond to those buttons. You can upgrade your remote without changing your TV, and vice versa.",
                RealLifeProblem = "Creating classes like SonyBasicRemote, SonyAdvancedRemote, LGBasicRemote, LGAdvancedRemote creates too many duplicate classes.",
                RealLifeSolution = "Separate Remotes from Devices. The Remote simply holds a link to any Device and tells it what to do.",
                DemoTitle = "Universal Remote Bridge Simulator",
                DemoDescription = "Pair any remote control with any appliance and send commands.",
                OptionList1 = new List<string> { "Smart Remote Control", "Basic Button Remote" },
                OptionList2 = new List<string> { "Living Room TV", "Home Audio Receiver", "Air Conditioner" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Power Button", Parameter = "power", Description = "Toggles power." },
                    new() { Title = "Volume Up (+10%)", Parameter = "vol_up", Description = "Increases volume." },
                    new() { Title = "Mute Audio", Parameter = "mute", Description = "Mutes device." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string remoteType = ctx.SelectedOption1;
                    string deviceType = ctx.SelectedOption2;
                    string action = ctx.ActionCommand;

                    IDevice device = deviceType.Contains("TV") ? new SonyBraviaTv() :
                                     deviceType.Contains("Audio") ? new YamahaSoundbar() :
                                     new DaikinAirConditioner();

                    RemoteControl remote = remoteType.Contains("Smart")
                        ? new AdvancedRemoteControl(device)
                        : new RemoteControl(device);

                    string reaction = action switch
                    {
                        "vol_up" => remote.VolumeUp(),
                        "mute" when remote is AdvancedRemoteControl adv => adv.Mute(),
                        _ => remote.TogglePower()
                    };

                    return $"[BRIDGE CONTROLLER RESULT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Remote Used:  {remote.GetType().Name}\n" +
                           $"• Device:       {device.GetDeviceName()}\n" +
                           $"• Button:       {action.ToUpper()}\n" +
                           $"• Result:       {reaction}\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: The remote and device work together without being tightly locked together.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IDevice.cs",
                        Role = "Device Interface",
                        Description = "Defines what every device can do.",
                        Code = @"namespace RemoteDemo;

public interface IDevice
{
    void Enable();
    void Disable();
    int Volume { get; set; }
}"
                    },
                    new()
                    {
                        FileName = "RemoteControl.cs",
                        Role = "Remote Abstraction",
                        Description = "The remote that talks to any device.",
                        Code = @"namespace RemoteDemo;

public class RemoteControl
{
    protected readonly IDevice _device;
    public RemoteControl(IDevice device) { _device = device; }
    public void VolumeUp() => _device.Volume += 10;
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "decorator",
                Name = "Decorator",
                Category = PatternCategory.Structural,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4M12,6A6,6 0 0,0 6,12A6,6 0 0,0 12,18A6,6 0 0,0 18,12A6,6 0 0,0 12,6Z",
                DefinitionLine1 = "Adds new features to an object by wrapping it inside topping or wrapper objects.",
                DefinitionLine2 = "Provides a flexible way to add features at runtime without making dozens of subclasses.",
                RealLifeTitle = "Real-Life Example: Coffee and Toppings",
                RealLifeAnalogy = "You order a base coffee (₹250). You can add milk (+₹70), then add caramel (+₹80), and whipped cream (+₹60). Each topping adds to the description and the bill without needing 50 different coffee classes.",
                RealLifeProblem = "Creating a separate class for every drink combination (like CoffeeWithMilk, CoffeeWithMilkAndSugar) creates hundreds of unnecessary classes.",
                RealLifeSolution = "Wrap the coffee inside small topping objects that add their own cost and description on top of the original drink.",
                DemoTitle = "Coffee Customizer Simulator",
                DemoDescription = "Pick a base coffee, check any toppings, and see how the Decorator wraps the drink and calculates the total price.",
                OptionList1 = new List<string> { "Espresso (₹250.00)", "Cold Brew (₹325.00)", "Americano (₹275.00)", "Blonde Roast (₹300.00)" },
                ToggleList = new List<string> { "Steamed Oat Milk (+₹70.00)", "Salted Caramel Drizzle (+₹80.00)", "Madagascar Vanilla Syrup (+₹60.00)", "Whipped Cream Cloud (+₹60.00)", "Extra Double Espresso Shot (+₹120.00)" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Make Drink & Calculate Bill", Parameter = "order", Description = "Wraps coffee in decorators." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string baseDrink = ctx.SelectedOption1;

                    IBeverage beverage = baseDrink.Contains("Cold Brew") ? new ColdBrew() :
                                         baseDrink.Contains("Americano") ? new Americano() :
                                         baseDrink.Contains("Blonde") ? new BlondeRoast() :
                                         new Espresso();

                    if (ctx.ActiveToggles.Contains("Steamed Oat Milk (+₹70.00)"))
                        beverage = new MilkDecorator(beverage);
                    if (ctx.ActiveToggles.Contains("Salted Caramel Drizzle (+₹80.00)"))
                        beverage = new CaramelDecorator(beverage);
                    if (ctx.ActiveToggles.Contains("Madagascar Vanilla Syrup (+₹60.00)"))
                        beverage = new VanillaDecorator(beverage);
                    if (ctx.ActiveToggles.Contains("Whipped Cream Cloud (+₹60.00)"))
                        beverage = new WhippedCreamDecorator(beverage);
                    if (ctx.ActiveToggles.Contains("Extra Double Espresso Shot (+₹120.00)"))
                        beverage = new ExtraShotDecorator(beverage);

                    return $"[COFFEE ORDER RECEIPT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"WRAPPED LAYERS:\n" +
                           string.Join("\n", beverage.GetLayers().Select(l => $"  ↳ {l}")) + "\n\n" +
                           $"ORDER SUMMARY:\n" +
                           $"• Final Item:  {beverage.GetDescription()}\n" +
                           $"• Total Price: ₹{beverage.GetCost():F2}\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: Toppings were added dynamically on top of the base drink.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IBeverage.cs",
                        Role = "Base Interface",
                        Description = "Methods shared by base drinks and toppings.",
                        Code = @"namespace CoffeeShop;

public interface IBeverage
{
    string GetDescription();
    decimal GetCost();
}"
                    },
                    new()
                    {
                        FileName = "MilkDecorator.cs",
                        Role = "Decorator Class",
                        Description = "Wraps a drink and adds milk price and name.",
                        Code = @"namespace CoffeeShop;

public class MilkDecorator : IBeverage
{
    private readonly IBeverage _drink;
    public MilkDecorator(IBeverage drink) { _drink = drink; }

    public string GetDescription() => _drink.GetDescription() + "", Milk"";
    public decimal GetCost() => _drink.GetCost() + 70.00m;
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "facade",
                Name = "Facade",
                Category = PatternCategory.Structural,
                AccentColor = "#FFFFFF",
                IconGeometry = "M19,4H5A2,2 0 0,0 3,6V18A2,2 0 0,0 5,20H19A2,2 0 0,0 21,18V6A2,2 0 0,0 19,4M19,7L12,11.5L5,7V6L12,10.5L19,6V7Z",
                DefinitionLine1 = "Provides a single, easy-to-use door to a complex group of classes.",
                DefinitionLine2 = "Hides all the complicated setup steps behind one simple method call.",
                RealLifeTitle = "Real-Life Example: One-Button Home Theater",
                RealLifeAnalogy = "Instead of turning on the TV, dimming the lights, switching the sound amplifier, and starting Netflix one by one, you press one 'Movie Night' button that does everything for you.",
                RealLifeProblem = "Managing 5 different devices and remotes in your code requires dozens of complicated setup steps everywhere.",
                RealLifeSolution = "Create a single simple class (the Facade) with clean methods like WatchMovie() that manage all the devices behind the scenes.",
                DemoTitle = "Home Theater Facade Simulator",
                DemoDescription = "Turn on or off 4 different home theater devices with a single button click.",
                InputLabel1 = "Movie Title",
                DefaultInput1 = "Interstellar",
                OptionList1 = new List<string> { "Surround Sound Mode", "Headphone Mode", "TV Speaker Mode" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Start Movie (1-Click)", Parameter = "watch", Description = "Turns on all devices." },
                    new() { Title = "Turn Off All Devices", Parameter = "end", Description = "Turns off all devices." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string movie = string.IsNullOrWhiteSpace(ctx.Input1) ? "Movie" : ctx.Input1;
                    string sound = ctx.SelectedOption1;
                    string action = ctx.ActionCommand;

                    var facade = new HomeTheaterFacade(
                        new LightsSubsystem(),
                        new ProjectorSubsystem(),
                        new AudioSubsystem(),
                        new StreamingPlayerSubsystem());

                    return action == "end" ? facade.EndMovie() : facade.WatchMovie(movie, sound);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "HomeTheaterFacade.cs",
                        Role = "Facade Class",
                        Description = "Simplifies multi-device control into one clean class.",
                        Code = @"namespace SmartHome;

public class HomeTheaterFacade
{
    private readonly Lights _lights = new();
    private readonly TV _tv = new();

    public void WatchMovie()
    {
        _lights.Dim();
        _tv.TurnOn();
    }
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "observer",
                Name = "Observer",
                Category = PatternCategory.Behavioral,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z",
                DefinitionLine1 = "Notifies multiple subscriber objects whenever something happens to the publisher.",
                DefinitionLine2 = "Allows objects to listen for updates without constantly asking or polling.",
                RealLifeTitle = "Real-Life Example: YouTube Channel Subscriptions",
                RealLifeAnalogy = "When you subscribe to a YouTube channel, you don't check YouTube every 5 minutes to see if there is a new video. The channel notifies you automatically as soon as a new video is posted.",
                RealLifeProblem = "Checking repeatedly for updates wastes time, and hardcoding each person into the channel code makes it hard to add new subscribers.",
                RealLifeSolution = "Let subscribers sign up with a Subscribe() method. When a new video is published, the channel loops through the list and sends an alert to everyone.",
                DemoTitle = "YouTube Notification Broadcast Simulator",
                DemoDescription = "Publish a video and see how all registered subscribers receive instant alerts.",
                InputLabel1 = "New Video Title",
                DefaultInput1 = "How Design Patterns Work Simply",
                InputLabel2 = "New Subscriber Name",
                DefaultInput2 = "Rohan Sharma",
                ToggleList = new List<string> { "Send Mobile Notification", "Send Discord Message", "Send Email Alert" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Publish Video & Alert All", Parameter = "broadcast", Description = "Sends alert to subscribers." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string video = string.IsNullOrWhiteSpace(ctx.Input1) ? "New Video" : ctx.Input1;
                    string newSub = string.IsNullOrWhiteSpace(ctx.Input2) ? "Guest_User" : ctx.Input2;
                    bool push = ctx.ActiveToggles.Contains("Send Mobile Notification");
                    bool discord = ctx.ActiveToggles.Contains("Send Discord Message");
                    bool email = ctx.ActiveToggles.Contains("Send Email Alert");

                    var channel = new YouTubeChannel("Tech Talks");
                    channel.Subscribe(new UserSubscriber(newSub));
                    channel.Subscribe(new UserSubscriber("Priya"));
                    channel.Subscribe(new UserSubscriber("Aarav"));

                    if (push) channel.Subscribe(new PushGatewayObserver());
                    if (discord) channel.Subscribe(new DiscordWebhookObserver());
                    if (email) channel.Subscribe(new EmailDigestObserver());

                    var logs = channel.UploadVideo(video);

                    return $"[SUBSCRIBER NOTIFICATION DISPATCH]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Channel:    {channel.ChannelName}\n" +
                           $"• Video:      \"{video}\"\n" +
                           $"• Alerts Sent ({logs.Count} active subscribers):\n" +
                           string.Join("\n", logs) + "\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: All subscribers were notified automatically.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IObserver.cs",
                        Role = "Subscriber Interface",
                        Description = "Defines how subscribers receive alerts.",
                        Code = @"namespace ObserverDemo;

public interface IObserver
{
    string Notify(string videoTitle);
}"
                    },
                    new()
                    {
                        FileName = "Channel.cs",
                        Role = "Publisher Class",
                        Description = "Keeps subscriber list and notifies all on new video.",
                        Code = @"namespace ObserverDemo;

public class Channel
{
    private readonly List<IObserver> _subscribers = new();
    public void Subscribe(IObserver sub) => _subscribers.Add(sub);

    public void UploadVideo(string title)
    {
        foreach (var sub in _subscribers) sub.Notify(title);
    }
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "strategy",
                Name = "Strategy",
                Category = PatternCategory.Behavioral,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,2L4.5,20.29L5.21,21L12,18L18.79,21L19.5,20.29L12,2Z",
                DefinitionLine1 = "Puts different calculation methods into separate classes and lets you switch between them.",
                DefinitionLine2 = "Lets you change the active calculation method at runtime.",
                RealLifeTitle = "Real-Life Example: Travel Routes in Google Maps",
                RealLifeAnalogy = "When you use a map app to go to an airport, you can choose Driving (fastest), Bicycle (healthy), or Bus (cheap). The map stays the same; you simply switch which travel method you want.",
                RealLifeProblem = "Putting driving, walking, and bus route calculations into one big file with giant if-else blocks makes the code messy.",
                RealLifeSolution = "Put each route calculation into its own class and let the map switch between them whenever the user chooses.",
                DemoTitle = "Map Route Strategy Simulator",
                DemoDescription = "Pick origin, destination, and travel method to calculate time, cost, and route.",
                InputLabel1 = "Start Point",
                DefaultInput1 = "City Center",
                InputLabel2 = "Destination Point",
                DefaultInput2 = "Airport Terminal",
                OptionList1 = new List<string> { "Highway Driving Route", "Scenic Bicycle Route", "Public Bus/Train Route" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Calculate Route", Parameter = "nav", Description = "Runs selected route strategy." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string origin = string.IsNullOrWhiteSpace(ctx.Input1) ? "Start" : ctx.Input1;
                    string dest = string.IsNullOrWhiteSpace(ctx.Input2) ? "Destination" : ctx.Input2;
                    string choice = ctx.SelectedOption1;

                    IRouteStrategy strategy = choice.Contains("Highway") ? new HighwayExpressStrategy() :
                                             choice.Contains("Bicycle") ? new BicycleScenicStrategy() :
                                             new PublicTransitStrategy();

                    var navigator = new NavigatorContext(strategy);
                    return navigator.Calculate(origin, dest);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IRouteStrategy.cs",
                        Role = "Strategy Interface",
                        Description = "Defines the route calculation method.",
                        Code = @"namespace MapsDemo;

public interface IRouteStrategy
{
    string CalculateRoute(string from, string to);
}"
                    },
                    new()
                    {
                        FileName = "Navigator.cs",
                        Role = "Context Class",
                        Description = "Holds the chosen route strategy and runs it.",
                        Code = @"namespace MapsDemo;

public class Navigator
{
    private IRouteStrategy _strategy;
    public Navigator(IRouteStrategy strategy) { _strategy = strategy; }
    public void SetStrategy(IRouteStrategy strategy) => _strategy = strategy;
    public string Go(string from, string to) => _strategy.CalculateRoute(from, to);
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "command",
                Name = "Command",
                Category = PatternCategory.Behavioral,
                AccentColor = "#FFFFFF",
                IconGeometry = "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3M19,19H5V5H19V19M17,12H13V8H11V12H7V14H11V18H13V14H17V12Z",
                DefinitionLine1 = "Turns an action into a stand-alone object containing everything needed to perform it.",
                DefinitionLine2 = "Enables easy Undo, Redo, and saving actions in a history list.",
                RealLifeTitle = "Real-Life Example: Restaurant Food Order Slip",
                RealLifeAnalogy = "A waiter writes your food order on a slip and hands it to the kitchen. Any chef can pick up the slip and cook it. If you change your mind, the waiter can simply cancel the order slip (Undo).",
                RealLifeProblem = "Directly running code from buttons makes it hard to support Undo, Redo, or save a history of past actions.",
                RealLifeSolution = "Turn each action into a Command object with Execute() and Undo() methods, and save them in a history stack.",
                DemoTitle = "Text Editor Undo / Redo Simulator",
                DemoDescription = "Type words, run formatting commands, and click Undo to step back in history.",
                InputLabel1 = "Text to Add",
                DefaultInput1 = "Design Patterns",
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Add Text Command", Parameter = "insert", Description = "Runs text insertion and adds to undo stack." },
                    new() { Title = "Convert to Uppercase", Parameter = "upper", Description = "Runs uppercase command." },
                    new() { Title = "Undo Last Action", Parameter = "undo", Description = "Pops latest command and calls Undo()." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string text = string.IsNullOrWhiteSpace(ctx.Input1) ? "Hello" : ctx.Input1;
                    string action = ctx.ActionCommand;

                    var editor = new TextEditor { Buffer = "My Document" };
                    var history = new CommandHistory();

                    var insertCmd = new InsertTextCommand(editor, $": {text}");
                    insertCmd.Execute();
                    history.Push(insertCmd);

                    if (action == "upper")
                    {
                        var upperCmd = new ChangeCaseCommand(editor);
                        upperCmd.Execute();
                        history.Push(upperCmd);

                        return $"[COMMAND EXECUTED: UPPERCASE]\n" +
                               $"------------------------------------------------------------\n" +
                               $"• Command Name:    {upperCmd.GetName()}\n" +
                               $"• Current Content: \"{editor.Buffer}\"\n" +
                               $"• Undo Stack:      2 commands saved\n" +
                               $"------------------------------------------------------------\n" +
                               $"STATUS: Action completed and saved to history.";
                    }
                    else if (action == "undo")
                    {
                        ICommand? popped = history.Pop();
                        popped?.Undo();

                        return $"[COMMAND UNDO RESULT]\n" +
                               $"------------------------------------------------------------\n" +
                               $"• Rolled Back:     {popped?.GetName()}\n" +
                               $"• Restored Text:   \"{editor.Buffer}\"\n" +
                               $"• Remaining Stack: {history.Count} command(s)\n" +
                               $"------------------------------------------------------------\n" +
                               $"STATUS: Action undone cleanly.";
                    }

                    return $"[COMMAND EXECUTED: ADD TEXT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Command Name:    {insertCmd.GetName()}\n" +
                           $"• Current Content: \"{editor.Buffer}\"\n" +
                           $"• Undo Stack:      1 command saved\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: Text added and recorded.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "ICommand.cs",
                        Role = "Command Interface",
                        Description = "Defines Execute and Undo methods.",
                        Code = @"namespace CommandDemo;

public interface ICommand
{
    void Execute();
    void Undo();
}"
                    },
                    new()
                    {
                        FileName = "InsertCommand.cs",
                        Role = "Concrete Command",
                        Description = "Adds text and saves old text for Undo.",
                        Code = @"namespace CommandDemo;

public class InsertCommand : ICommand
{
    private string _oldText = """";
    private readonly TextEditor _editor;
    public InsertCommand(TextEditor ed) { _editor = ed; }

    public void Execute() { _oldText = _editor.Text; _editor.Text += "" New""; }
    public void Undo() { _editor.Text = _oldText; }
}"
                    }
                }
            },

            new PatternItem
            {
                Id = "state",
                Name = "State",
                Category = PatternCategory.Behavioral,
                AccentColor = "#FFFFFF",
                IconGeometry = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4M12,6A6,6 0 0,0 6,12A6,6 0 0,0 12,18A6,6 0 0,0 18,12A6,6 0 0,0 12,6Z",
                DefinitionLine1 = "Allows an object to change its behavior when its internal state changes.",
                DefinitionLine2 = "Makes the object behave as if it changed its class dynamically.",
                RealLifeTitle = "Real-Life Example: Phone Lock Screen & Music Player",
                RealLifeAnalogy = "When your phone is locked, tapping the screen only shows the clock. When it is unlocked, the exact same tap opens apps. The phone changes its behavior based on whether it is Locked or Unlocked.",
                RealLifeProblem = "Writing giant if-else blocks like 'if (isLocked) ... else if (isPlaying) ...' inside every button makes the code messy and easy to break.",
                RealLifeSolution = "Create a separate class for each state (Stopped, Playing, Paused, Locked) and let the player pass button clicks to whichever state is active.",
                DemoTitle = "Music Player State Simulator",
                DemoDescription = "Click buttons to see how the player transitions between Playing, Paused, and Locked states.",
                OptionList1 = new List<string> { "State: Stopped", "State: Playing", "State: Paused", "State: Locked" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "▶ Play Button", Parameter = "play", Description = "Sends Play to current state." },
                    new() { Title = "⏸ Pause Button", Parameter = "pause", Description = "Sends Pause to current state." },
                    new() { Title = "🔒 Lock Button", Parameter = "lock", Description = "Sends Lock to current state." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string action = ctx.ActionCommand;
                    string choice = ctx.SelectedOption1;

                    var context = new AudioPlayerContext();
                    if (choice.Contains("Playing")) context.State = new PlayingState();
                    else if (choice.Contains("Paused")) context.State = new PausedState();
                    else if (choice.Contains("Locked")) context.State = new LockedState(new StoppedState());
                    else context.State = new StoppedState();

                    string prevState = context.State.StateName;
                    string transition = action switch
                    {
                        "pause" => context.State.ClickPause(context),
                        "lock" => context.State.ClickLock(context),
                        _ => context.State.ClickPlay(context)
                    };

                    return $"[PLAYER STATE TRANSITION RESULT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Previous State: {prevState}\n" +
                           $"• Button Clicked: {action.ToUpper()}\n" +
                           $"• Action Result:  {transition}\n" +
                           $"• Current State:  {context.State.StateName}\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: The player changed its response based on its active state.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IPlayerState.cs",
                        Role = "State Interface",
                        Description = "Defines actions that behave differently per state.",
                        Code = @"namespace StateDemo;

public interface IPlayerState
{
    string Play(PlayerContext context);
    string Pause(PlayerContext context);
}"
                    },
                    new()
                    {
                        FileName = "PlayingState.cs",
                        Role = "Playing State",
                        Description = "Handles button clicks when music is playing.",
                        Code = @"namespace StateDemo;

public class PlayingState : IPlayerState
{
    public string Play(PlayerContext context) => ""Already playing music."";
    public string Pause(PlayerContext context)
    {
        context.State = new PausedState();
        return ""Music paused."";
    }
}"
                    }
                }
            }
        };
    }
}
