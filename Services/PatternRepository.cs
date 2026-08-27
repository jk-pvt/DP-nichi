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
                DemoDescription = "Customize package description, distance, delivery vehicle, base rate, and optional handling features to compute real-time dynamic logistics.",
                InputLabel1 = "Package Cargo & Description",
                DefaultInput1 = "Medical Diagnostics Equipment (450 kg)",
                InputLabel2 = "Distance (km)",
                DefaultInput2 = "1250",
                OptionLabel1 = "Delivery Vehicle Mode",
                OptionList1 = new List<string> { "Highway Truck (Road)", "Cargo Container Ship (Sea)", "Boeing 777 Cargo Jet (Air)", "Autonomous Drone Fleet (Rapid Air)" },
                OptionLabel2 = "Rate per Kilometer (₹)",
                OptionList2 = new List<string> { "Standard Rate (₹120.00/km)", "Economy Rate (₹65.00/km)", "Express Priority Rate (₹280.00/km)", "Heavy Freight Rate (₹450.00/km)" },
                ToggleLabel = "Logistics Surcharges & Handling",
                ToggleList = new List<string> { "Express 24/7 Priority (+15%)", "Climate Controlled Refrigeration (+₹500)", "Hazardous Material Handling (+₹1,200)", "Full Cargo Insurance Coverage (+2.5%)" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Dispatch Vehicle", Parameter = "dispatch", Description = "Calls CreateTransport() and runs delivery." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string cargo = string.IsNullOrWhiteSpace(ctx.Input1) ? "General Goods" : ctx.Input1;
                    if (!double.TryParse(ctx.Input2, out double distance) || distance <= 0) distance = 1000;
                    string mode = ctx.SelectedOption1;
                    string rateChoice = ctx.SelectedOption2;

                    double customRate = rateChoice.Contains("65") ? 65.00 :
                                        rateChoice.Contains("280") ? 280.00 :
                                        rateChoice.Contains("450") ? 450.00 : 120.00;

                    Logistics logistics = mode.Contains("Ship") ? new SeaLogistics() :
                                          mode.Contains("Jet") ? new AirLogistics() :
                                          mode.Contains("Drone") ? new DroneLogistics() :
                                          new RoadLogistics();

                    return logistics.PlanDelivery(cargo, distance, customRate, ctx.ActiveToggles);
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
    string VehicleName { get; }
    string Deliver(string cargo, double distanceKm);
    double CalculateCost(double distanceKm, double ratePerKm);
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
    public string VehicleName => ""Highway Truck (Road)"";
    public string Deliver(string cargo, double distanceKm) => $""[Truck] Carrying '{cargo}' over {distanceKm} km by road."";
    public double CalculateCost(double distanceKm, double ratePerKm) => distanceKm * ratePerKm;
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

    public string PlanDelivery(string cargo, double distanceKm, double ratePerKm)
    {
        ITransport transport = CreateTransport();
        return transport.Deliver(cargo, distanceKm) + $"" Cost: ₹{transport.CalculateCost(distanceKm, ratePerKm):F2}"";
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
                DemoDescription = "Customize window title, button text, operating system theme family, accent colors, and optional UI widgets.",
                InputLabel1 = "App Window Title",
                DefaultInput1 = "Cloud Infrastructure Analytics",
                InputLabel2 = "Action Button Label",
                DefaultInput2 = "Deploy Production Cluster",
                OptionLabel1 = "Theme & OS Family",
                OptionList1 = new List<string> { "macOS Sequoia UI Suite", "Windows 11 Fluent Suite", "Linux GNOME GTK4 Suite", "Cyberpunk Neon Dark Suite" },
                OptionLabel2 = "Theme Accent Color",
                OptionList2 = new List<string> { "Platinum Monochrome (#FFFFFF)", "Emerald Green (#10B981)", "Electric Blue (#3B82F6)", "Neon Amber (#F59E0B)", "Cyber Purple (#A855F7)" },
                ToggleLabel = "UI Component Family Elements",
                ToggleList = new List<string> { "Include Search Bar Widget", "Include Notification Badge", "Include Status Bar Footer", "Include Touch Screen Mode" },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Manufacture Component Family", Parameter = "build", Description = "Creates matching controls." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string target = ctx.SelectedOption1;
                    string accent = ctx.SelectedOption2;
                    string title = string.IsNullOrWhiteSpace(ctx.Input1) ? "Application" : ctx.Input1;
                    string buttonText = string.IsNullOrWhiteSpace(ctx.Input2) ? "Execute" : ctx.Input2;

                    IUIFactory factory = target.Contains("macOS") ? new MacUIFactory() :
                                         target.Contains("Windows") ? new WindowsUIFactory() :
                                         target.Contains("Cyberpunk") ? new CyberpunkUIFactory() :
                                         new LinuxUIFactory();

                    var app = new DynamicUIApp(factory);
                    return app.BuildComponentFamily(title, buttonText, accent, ctx.ActiveToggles);
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "IUIFactory.cs",
                        Role = "Factory Interface",
                        Description = "Defines methods to create a whole family of UI elements.",
                        Code = @"namespace UIDemo;

public interface IButton { string Render(string label, string accent); }
public interface ICheckbox { string Render(string label, string accent); }

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

public class MacButton : IButton 
{ 
    public string Render(string label, string accent) => $""[Mac] {label} (Accent: {accent})""; 
}

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
                DemoDescription = "Set rig name, target budget, processor, graphics card, and hardware upgrades. The Builder dynamically computes total pricing, power draw, and budget status.",
                InputLabel1 = "Custom Rig Name",
                DefaultInput1 = "JK's 4K Video & AI Workstation",
                InputLabel2 = "Target Budget Limit (₹)",
                DefaultInput2 = "250000",
                OptionLabel1 = "Processor (CPU)",
                OptionList1 = new List<string>
                {
                    "Intel Core i9-14900K (24C / 253W - ₹54,000)",
                    "AMD Ryzen 9 7950X (16C / 170W - ₹56,500)",
                    "Intel Core i7-14700K (20C / 190W - ₹38,000)",
                    "AMD Ryzen 5 7600X (6C / 105W - ₹19,500)"
                },
                OptionLabel2 = "Graphics Card (GPU)",
                OptionList2 = new List<string>
                {
                    "NVIDIA RTX 4090 24GB (450W - ₹1,68,000)",
                    "AMD Radeon RX 7900 XTX 24GB (355W - ₹92,000)",
                    "NVIDIA RTX 4070 Ti 16GB (285W - ₹76,000)",
                    "Intel Arc A770 16GB (225W - ₹28,000)",
                    "Integrated CPU Graphics (15W - ₹0)"
                },
                ToggleLabel = "Memory, Storage & Cooling Upgrades",
                ToggleList = new List<string>
                {
                    "64GB DDR5-6000 RAM (+₹18,500 / +15W)",
                    "2TB Gen4 NVMe SSD (+₹14,200 / +10W)",
                    "360mm AIO Liquid Cooler (+₹12,800 / +35W)",
                    "RGB Ambient Sync Kit (+₹4,500 / +20W)",
                    "Platinum 1000W Power Supply (+₹16,000)"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Assemble Computer", Parameter = "build", Description = "Runs Builder steps." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string rigName = string.IsNullOrWhiteSpace(ctx.Input1) ? "Custom Rig" : ctx.Input1;
                    if (!double.TryParse(ctx.Input2, out double budget) || budget <= 0) budget = 200000;

                    string cpu = ctx.SelectedOption1;
                    string gpu = ctx.SelectedOption2;

                    double cpuPrice = cpu.Contains("14900K") ? 54000 : cpu.Contains("7950X") ? 56500 : cpu.Contains("14700K") ? 38000 : 19500;
                    int cpuWatts = cpu.Contains("14900K") ? 253 : cpu.Contains("7950X") ? 170 : cpu.Contains("14700K") ? 190 : 105;

                    double gpuPrice = gpu.Contains("4090") ? 168000 : gpu.Contains("7900") ? 92000 : gpu.Contains("4070") ? 76000 : gpu.Contains("A770") ? 28000 : 0;
                    int gpuWatts = gpu.Contains("4090") ? 450 : gpu.Contains("7900") ? 355 : gpu.Contains("4070") ? 285 : gpu.Contains("A770") ? 225 : 15;

                    bool highRam = ctx.ActiveToggles.Contains("64GB DDR5-6000 RAM (+₹18,500 / +15W)");
                    bool highStorage = ctx.ActiveToggles.Contains("2TB Gen4 NVMe SSD (+₹14,200 / +10W)");
                    bool liquidCooling = ctx.ActiveToggles.Contains("360mm AIO Liquid Cooler (+₹12,800 / +35W)");
                    bool rgb = ctx.ActiveToggles.Contains("RGB Ambient Sync Kit (+₹4,500 / +20W)");
                    bool psu1000 = ctx.ActiveToggles.Contains("Platinum 1000W Power Supply (+₹16,000)");

                    var builder = new CustomComputerBuilder();
                    builder.SetBuildName(rigName)
                           .SetTargetBudget(budget)
                           .SetMotherboard("ASUS ROG Strix Gaming Board", 28000, 75)
                           .SetCPU(cpu, cpuPrice, cpuWatts)
                           .SetGPU(gpu, gpuPrice, gpuWatts)
                           .SetRAM(highRam ? "64GB DDR5 Dual Channel" : "16GB DDR5 Standard", highRam ? 18500 : 5500, highRam ? 15 : 8)
                           .SetStorage(highStorage ? "2TB High-Speed NVMe Gen4" : "512GB Standard SSD", highStorage ? 14200 : 4200, highStorage ? 10 : 5)
                           .SetCooling(liquidCooling ? "360mm AIO Triple-Fan Liquid Cooler" : "Dual-Tower Air Cooler", liquidCooling ? 12800 : 3200, liquidCooling ? 35 : 10)
                           .SetRGB(rgb ? "RGB Dynamic Halo Sync" : "No RGB Lighting", rgb ? 4500 : 0, rgb ? 20 : 0)
                           .SetPSU(psu1000 ? "1000W Platinum 80+ Fully Modular" : "750W Gold 80+ Semi-Modular", psu1000 ? 16000 : 8500);

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
    public string BuildName { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string GPU { get; set; } = string.Empty;
    public double TotalPrice { get; set; }
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

    public ComputerBuilder SetCPU(string cpu, double price) { _pc.CPU = cpu; _pc.TotalPrice += price; return this; }
    public ComputerBuilder SetGPU(string gpu, double price) { _pc.GPU = gpu; _pc.TotalPrice += price; return this; }
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
                DemoDescription = "Select a master template, enter customized recipient and reference ID, select a watermark priority, and toggle optional legal/milestone sections.",
                InputLabel1 = "New Recipient / Client Name",
                DefaultInput1 = "Acme Global Technologies Corp",
                InputLabel2 = "New Invoice / Reference ID",
                DefaultInput2 = "INV-2026-9811",
                OptionLabel1 = "Base Document Template",
                OptionList1 = new List<string>
                {
                    "Enterprise Commercial Contract (4 Sections)",
                    "Freelance Software Project Proposal (5 Sections)",
                    "Tax & Compliance Audit Report (6 Sections)",
                    "Confidential NDA Agreement (3 Sections)"
                },
                OptionLabel2 = "Priority Watermark",
                OptionList2 = new List<string>
                {
                    "Standard Delivery (No Watermark)",
                    "CONFIDENTIAL & PROPRIETARY",
                    "URGENT / EXPEDITE REVIEW",
                    "DRAFT FOR APPROVAL"
                },
                ToggleLabel = "Optional Contract Clauses & Sections",
                ToggleList = new List<string>
                {
                    "Include Payment Milestones Table",
                    "Include Digital Cryptographic Signature",
                    "Include Legal Arbitration Clauses",
                    "Include Scope Deliverables Appendix"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Clone & Mutate Document", Parameter = "clone", Description = "Calls Clone() and updates customer name." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string template = ctx.SelectedOption1;
                    string watermark = ctx.SelectedOption2;
                    string recipient = string.IsNullOrWhiteSpace(ctx.Input1) ? "Client" : ctx.Input1;
                    string refCode = string.IsNullOrWhiteSpace(ctx.Input2) ? "REF-001" : ctx.Input2;

                    var master = new DocumentTemplate
                    {
                        Title = template,
                        Recipient = "Master Template",
                        ReferenceCode = "TEMPLATE-ORIGINAL-00",
                        PriorityWatermark = "None",
                        Sections = template.Contains("Proposal")
                            ? new List<string> { "Project Executive Summary", "Technical Architecture", "Milestone Roadmap", "Team Allocation", "Cost Estimation" }
                            : template.Contains("Audit")
                            ? new List<string> { "Audit Overview", "Compliance Checklist", "Security Vulnerabilities", "Financial Data", "Risk Matrix", "Remediation Steps" }
                            : template.Contains("NDA")
                            ? new List<string> { "Confidential Information Definition", "Non-Disclosure Obligations", "Remedies & Jurisdiction" }
                            : new List<string> { "Contract Scope", "Payment Terms", "Intellectual Property", "Termination Clause" }
                    };

                    DocumentTemplate clone = master.Clone();
                    clone.Recipient = recipient;
                    clone.ReferenceCode = refCode;
                    clone.PriorityWatermark = watermark;

                    if (ctx.ActiveToggles.Contains("Include Payment Milestones Table"))
                        clone.Sections.Add("Addendum: Payment Milestone Schedule (30/40/30)");
                    if (ctx.ActiveToggles.Contains("Include Digital Cryptographic Signature"))
                        clone.Sections.Add("Addendum: Cryptographic SHA-256 Digital Signature");
                    if (ctx.ActiveToggles.Contains("Include Legal Arbitration Clauses"))
                        clone.Sections.Add("Addendum: Binding Legal Arbitration & Jurisdiction");
                    if (ctx.ActiveToggles.Contains("Include Scope Deliverables Appendix"))
                        clone.Sections.Add("Addendum: Detailed Functional Deliverables Spec");

                    bool distinctObjects = !object.ReferenceEquals(master, clone);

                    var sectionLines = clone.Sections.Select((s, i) => $"     [{i + 1}] {s}").ToList();

                    return $"[PROTOTYPE CLONING MANIFEST]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Master Prototype:     \"{master.Title}\" (Memory: #0x{master.GetHashCode():X8})\n" +
                           $"• Clone Operation:      IPrototype<DocumentTemplate>.Clone()\n" +
                           $"• Cloned Instance:      New DocumentTemplate (Memory: #0x{clone.GetHashCode():X8})\n" +
                           $"• Mutated Recipient:    \"{clone.Recipient}\"\n" +
                           $"• Mutated Reference ID: \"{clone.ReferenceCode}\"\n" +
                           $"• Priority Watermark:   \"{clone.PriorityWatermark}\"\n" +
                           $"• Reference Comparison: ReferenceEquals(master, clone) => {!distinctObjects} (Independent objects!)\n\n" +
                           $"CLONED DOCUMENT SECTIONS ({clone.Sections.Count} Active Layers):\n" +
                           string.Join("\n", sectionLines) + "\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: Cloned dynamically in memory in 0ms with all custom sections.";
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
                        FileName = "DocumentTemplate.cs",
                        Role = "Clonable Class",
                        Description = "Creates copies of itself.",
                        Code = @"namespace CloneDemo;

public class DocumentTemplate : IPrototype<DocumentTemplate>
{
    public string Title { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public List<string> Sections { get; set; } = new();

    public DocumentTemplate Clone() => new DocumentTemplate
    {
        Title = this.Title,
        Recipient = this.Recipient,
        Sections = new List<string>(this.Sections)
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
                DemoDescription = "Configure host URI, thread pool, server cluster region, cache TTL, and security flags. Module A saves the settings; Module B and Module C read from the identical memory instance.",
                InputLabel1 = "Database Host / Connection URI",
                DefaultInput1 = "db-primary.asia-south1.gcp.internal:5432",
                InputLabel2 = "Max Worker Thread Pool",
                DefaultInput2 = "128",
                OptionLabel1 = "Active Server Cluster",
                OptionList1 = new List<string>
                {
                    "Production Cluster (Region: Mumbai asia-south1)",
                    "Staging Cluster (Region: Frankfurt europe-west3)",
                    "Disaster Recovery Failover (Region: Virginia us-east1)",
                    "Local Dev Environment (127.0.0.1:8080)"
                },
                OptionLabel2 = "Cache Expiration Policy",
                OptionList2 = new List<string>
                {
                    "Aggressive TTL (60 seconds)",
                    "Standard TTL (15 minutes)",
                    "Long-lived TTL (24 hours)",
                    "Bypass Cache (No Caching)"
                },
                ToggleLabel = "Cluster Architecture & Security",
                ToggleList = new List<string>
                {
                    "Enable Distributed Redis Cluster",
                    "Enable Strict TLS 1.3 Encryption",
                    "Enable Live Audit Telemetry Log",
                    "Enable Automatic Horizontal Auto-Scaling"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Update Global Singleton State", Parameter = "save", Description = "Mutates AppConfiguration.Instance." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string host = string.IsNullOrWhiteSpace(ctx.Input1) ? "localhost:5432" : ctx.Input1;
                    if (!int.TryParse(ctx.Input2, out int threads) || threads <= 0) threads = 64;
                    string env = ctx.SelectedOption1;
                    string cache = ctx.SelectedOption2;

                    bool redis = ctx.ActiveToggles.Contains("Enable Distributed Redis Cluster");
                    bool tls = ctx.ActiveToggles.Contains("Enable Strict TLS 1.3 Encryption");
                    bool audit = ctx.ActiveToggles.Contains("Enable Live Audit Telemetry Log");
                    bool scaling = ctx.ActiveToggles.Contains("Enable Automatic Horizontal Auto-Scaling");

                    AppConfiguration moduleA = AppConfiguration.Instance;
                    moduleA.HostUri = host;
                    moduleA.MaxThreads = threads;
                    moduleA.Environment = env;
                    moduleA.CachePolicy = cache;
                    moduleA.RedisCluster = redis;
                    moduleA.TlsEncryption = tls;
                    moduleA.AuditLogging = audit;
                    moduleA.AutoScaling = scaling;
                    moduleA.RequestCounter += 1;

                    AppConfiguration moduleB = AppConfiguration.Instance;
                    AppConfiguration moduleC = AppConfiguration.Instance;

                    bool matchAB = object.ReferenceEquals(moduleA, moduleB);
                    bool matchBC = object.ReferenceEquals(moduleB, moduleC);

                    return $"[SINGLETON MULTI-MODULE STATE SYNCHRONIZATION]\n" +
                           $"------------------------------------------------------------\n" +
                           $"[MODULE A: Configuration Admin Writer]\n" +
                           $"• Memory Address Hash:  #0x{moduleA.GetHashCode():X8}\n" +
                           $"• Database Host URI:    {moduleA.HostUri}\n" +
                           $"• Active Server Region: {moduleA.Environment}\n" +
                           $"• Thread Pool:          {moduleA.MaxThreads} concurrent workers\n" +
                           $"• Cache Policy:         {moduleA.CachePolicy}\n" +
                           $"• Security Flags:       TLS={moduleA.TlsEncryption}, Redis={moduleA.RedisCluster}, Audit={moduleA.AuditLogging}, AutoScale={moduleA.AutoScaling}\n\n" +
                           $"[MODULE B: API Gateway Router]\n" +
                           $"• Memory Address Hash:  #0x{moduleB.GetHashCode():X8} (IDENTICAL!)\n" +
                           $"• Read Host URI:        {moduleB.HostUri}\n" +
                           $"• Read Thread Pool:     {moduleB.MaxThreads} workers\n" +
                           $"• ReferenceEquals(A,B): {matchAB}\n\n" +
                           $"[MODULE C: Billing & Order Processor]\n" +
                           $"• Memory Address Hash:  #0x{moduleC.GetHashCode():X8} (IDENTICAL!)\n" +
                           $"• Read Security TLS:    {moduleC.TlsEncryption}\n" +
                           $"• Global Request Count: {moduleC.RequestCounter} requests processed\n" +
                           $"• ReferenceEquals(B,C): {matchBC}\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: [PERFECT SYNC] All 3 modules point to the exact same memory instance.";
                },
                CodeFiles = new List<CodeFileItem>
                {
                    new()
                    {
                        FileName = "AppConfiguration.cs",
                        Role = "Singleton Class",
                        Description = "Ensures only one copy exists.",
                        Code = @"namespace SettingsDemo;

public sealed class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> _instance = 
        new(() => new AppConfiguration());

    public static AppConfiguration Instance => _instance.Value;

    public string HostUri { get; set; } = ""localhost:5432"";
    public int MaxThreads { get; set; } = 64;

    private AppConfiguration() { }
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
                RealLifeTitle = "Real-Life Example: Travel Plug Adapter & Payment Converter",
                RealLifeAnalogy = "When you travel to another country, your phone charger plug doesn't fit the wall socket. You don't buy a new phone; you plug it into an adapter that connects your charger to the wall.",
                RealLifeProblem = "Your modern app uses simple JSON format, but an older bank system only accepts old XML format.",
                RealLifeSolution = "Write an Adapter class that takes your simple JSON data, converts it into the XML format the bank wants, and sends it.",
                DemoTitle = "Payment Gateway Adapter Simulator",
                DemoDescription = "Customize customer account, payment amount, integration gateway channel, currency, and security options to translate payloads dynamically.",
                InputLabel1 = "Customer Account ID & Name",
                DefaultInput1 = "ACC-9482 (Rohan Sharma)",
                InputLabel2 = "Payment Amount (₹)",
                DefaultInput2 = "14500.00",
                OptionLabel1 = "Payment Integration Gateway",
                OptionList1 = new List<string>
                {
                    "Modern UPI / REST API (Native JSON Gateway)",
                    "Legacy Core Banking SWIFT / SOAP (XML Adapter)"
                },
                OptionLabel2 = "Transaction Currency & Fee Model",
                OptionList2 = new List<string>
                {
                    "INR (₹ Indian Rupee - 0% Domestic Fee)",
                    "USD ($ US Dollar - 2.5% Forex FX Fee)",
                    "EUR (€ Euro - 2.8% Forex FX Fee)",
                    "GBP (£ British Pound - 3.0% Forex FX Fee)"
                },
                ToggleLabel = "Payment Options & Security Filters",
                ToggleList = new List<string>
                {
                    "Enable 2FA One-Time Passcode (OTP)",
                    "Enable Instant Fraud Risk Scoring Filter",
                    "Add Express Settlement Surcharge (+₹50 flat)",
                    "Generate Itemized GST/Tax Invoice (+18%)"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Process Transaction via Adapter", Parameter = "pay", Description = "Runs payment through adapter." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string target = ctx.SelectedOption1;
                    string currChoice = ctx.SelectedOption2;
                    string currency = currChoice.Contains("USD") ? "USD" : currChoice.Contains("EUR") ? "EUR" : currChoice.Contains("GBP") ? "GBP" : "INR";

                    if (!decimal.TryParse(ctx.Input2, out decimal amount) || amount <= 0) amount = 1000m;
                    string cust = string.IsNullOrWhiteSpace(ctx.Input1) ? "ACC-001" : ctx.Input1;

                    IPaymentProcessor processor = target.Contains("Modern")
                        ? new ModernStripeGateway()
                        : new LegacyBankAdapter(new LegacyBankSoapSdk());

                    return processor.ProcessPayment(amount, currency, cust, ctx.ActiveToggles);
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
                DemoDescription = "Customize zone name, power/volume level, remote control abstraction, appliance implementation, and advanced modes.",
                InputLabel1 = "Smart Home Room / Zone",
                DefaultInput1 = "Master Living Room & Entertainment Center",
                InputLabel2 = "Target Volume / Temp Level (0-100%)",
                DefaultInput2 = "65",
                OptionLabel1 = "Remote Controller Abstraction",
                OptionList1 = new List<string>
                {
                    "AI Voice Assistant Remote (Siri / Alexa)",
                    "Mobile App Remote (iOS & Android Touch)",
                    "Basic Physical Remote"
                },
                OptionLabel2 = "Connected Device Implementation",
                OptionList2 = new List<string>
                {
                    "Sony Bravia 4K OLED Smart TV",
                    "Yamaha Dolby Atmos 7.2 Home Theater Receiver",
                    "Daikin Inverter Climate Air Conditioner",
                    "Philips Hue Smart Ambient Light Strip"
                },
                ToggleLabel = "Special Appliance Features",
                ToggleList = new List<string>
                {
                    "Eco Power Saving Mode",
                    "Spatial Audio Calibration",
                    "Ambient Night Dimmer",
                    "30-Minute Sleep Timer"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Power Toggle", Parameter = "power", Description = "Toggles power." },
                    new() { Title = "Calibrate Level", Parameter = "level", Description = "Sets volume or temperature level." },
                    new() { Title = "Engage Special Mode", Parameter = "special", Description = "Applies custom mode across the bridge." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string zone = string.IsNullOrWhiteSpace(ctx.Input1) ? "Living Room" : ctx.Input1;
                    if (!int.TryParse(ctx.Input2, out int level)) level = 50;
                    string remoteType = ctx.SelectedOption1;
                    string deviceType = ctx.SelectedOption2;
                    string action = ctx.ActionCommand;

                    IDevice device = deviceType.Contains("TV") ? new SonyTvDevice() :
                                     deviceType.Contains("Yamaha") ? new YamahaAudioDevice() :
                                     deviceType.Contains("Daikin") ? new DaikinAcDevice() :
                                     new PhilipsHueDevice();

                    RemoteControl remote = remoteType.Contains("Voice") ? new VoiceRemoteControl(device) :
                                         remoteType.Contains("Mobile") ? new TouchAppRemoteControl(device) :
                                         new RemoteControl(device);

                    string activeMode = ctx.ActiveToggles.Count > 0 ? string.Join(", ", ctx.ActiveToggles) : "Standard Balanced";

                    string reaction = action switch
                    {
                        "level" => remote.SetLevel(level),
                        "special" => remote.SendSpecial(activeMode),
                        _ => remote.Power()
                    };

                    return $"[BRIDGE PATTERN DYNAMIC DISPATCH]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Zone Location:     \"{zone}\"\n" +
                           $"• Abstraction Layer: {remote.RemoteType}\n" +
                           $"• Device Bridge:     {device.DeviceName}\n" +
                           $"• Command Fired:     {action.ToUpper()}\n" +
                           $"• Device Reaction:   {reaction}\n" +
                           $"• Current State:     {remote.CheckStatus()}\n" +
                           $"------------------------------------------------------------\n" +
                           $"BENEFIT: Remote abstraction varies independently from electronic appliance hardware.";
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
    string DeviceName { get; }
    string PowerToggle();
    string SetLevel(int level);
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
    public string SetLevel(int level) => _device.SetLevel(level);
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
                DemoTitle = "Artisan Coffee Customizer Simulator",
                DemoDescription = "Customize customer name, order quantity, base coffee drink, cup size, and artisan toppings to dynamically wrap the object and compute live receipt details.",
                InputLabel1 = "Customer Name / Cup Label",
                DefaultInput1 = "Ananya's Morning Brew",
                InputLabel2 = "Order Quantity (Number of Cups)",
                DefaultInput2 = "2",
                OptionLabel1 = "Base Coffee Beverage",
                OptionList1 = new List<string>
                {
                    "Single-Origin Dark Espresso (Base: ₹220.00)",
                    "Nitro Cold Brew Reserve (Base: ₹310.00)",
                    "Caffe Americano Roast (Base: ₹260.00)",
                    "Velvet Blonde Roast (Base: ₹290.00)",
                    "Matcha Green Tea Latte (Base: ₹340.00)"
                },
                OptionLabel2 = "Cup Size & Temperature",
                OptionList2 = new List<string>
                {
                    "Medium Regular 350ml (1.0x Base)",
                    "Large Grande 475ml (+₹60.00)",
                    "Extra Large Venti 600ml (+₹110.00)",
                    "Iced Over Cold Craft Ice (+₹30.00)"
                },
                ToggleLabel = "Artisan Add-ons & Toppings",
                ToggleList = new List<string>
                {
                    "Steamed Oat Milk (+₹70.00)",
                    "Salted Caramel Drizzle (+₹80.00)",
                    "Madagascar Vanilla Syrup (+₹60.00)",
                    "Whipped Cream Cloud (+₹60.00)",
                    "Extra Double Espresso Shot (+₹120.00)",
                    "Cinnamon Dusting & Hazelnut Crunch (+₹45.00)"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Wrap Beverage & Print Receipt", Parameter = "order", Description = "Wraps coffee in decorators." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string custName = string.IsNullOrWhiteSpace(ctx.Input1) ? "Customer" : ctx.Input1;
                    if (!int.TryParse(ctx.Input2, out int qty) || qty <= 0) qty = 1;

                    string baseChoice = ctx.SelectedOption1;
                    string sizeChoice = ctx.SelectedOption2;

                    IBeverage beverage = baseChoice.Contains("Cold Brew") ? new ColdBrew() :
                                         baseChoice.Contains("Americano") ? new Americano() :
                                         baseChoice.Contains("Blonde") ? new BlondeRoast() :
                                         baseChoice.Contains("Matcha") ? new MatchaLatte() :
                                         new Espresso();

                    if (sizeChoice.Contains("Grande"))
                        beverage = new SizeDecorator(beverage, "Large Grande (475ml)", 60.00m);
                    else if (sizeChoice.Contains("Venti"))
                        beverage = new SizeDecorator(beverage, "Extra Large Venti (600ml)", 110.00m);
                    else if (sizeChoice.Contains("Iced"))
                        beverage = new SizeDecorator(beverage, "Iced Craft Cold (350ml)", 30.00m);

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
                    if (ctx.ActiveToggles.Contains("Cinnamon Dusting & Hazelnut Crunch (+₹45.00)"))
                        beverage = new HazelnutDecorator(beverage);

                    decimal singleCost = beverage.GetCost();
                    decimal totalOrderCost = singleCost * qty;
                    decimal taxGst = totalOrderCost * 0.05m;
                    decimal grandTotal = totalOrderCost + taxGst;

                    return $"[DECORATOR PATTERN COFFEE POS RECEIPT]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Customer Name:   \"{custName}\"\n" +
                           $"• Order Quantity:  {qty} cup(s)\n\n" +
                           $"OBJECT COMPOSITION WRAPPER TRACE (IBeverage):\n" +
                           string.Join("\n", beverage.GetLayers().Select(l => $"  ↳ {l}")) + "\n\n" +
                           $"FINAL RECEIPT:\n" +
                           $"• Item Name:       {beverage.GetDescription()}\n" +
                           $"• Single Cup Cost: ₹{singleCost:F2}\n" +
                           $"• Subtotal ({qty}x):  ₹{totalOrderCost:F2}\n" +
                           $"• GST (5%):        ₹{taxGst:F2}\n" +
                           $"• Total Amount:    ₹{grandTotal:F2}\n" +
                           $"------------------------------------------------------------\n" +
                           $"RESULT: Object decorated dynamically at runtime with {beverage.GetLayers().Count} layered wrappers.";
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
                DemoDescription = "Set movie title, audio volume calibration, surround acoustic profile, lighting mood, and motorized automation to orchestrate all subsystems.",
                InputLabel1 = "Movie / Streaming Title",
                DefaultInput1 = "Oppenheimer (4K IMAX HDR)",
                InputLabel2 = "Audio Volume (0 - 100 dB)",
                DefaultInput2 = "72",
                OptionLabel1 = "Surround Sound & Acoustic Profile",
                OptionList1 = new List<string>
                {
                    "Dolby Atmos 7.1.4 Cinematic Master Profile",
                    "Audiophile Spatial Studio Headphone Profile",
                    "Late Night Dialogue Clarity Profile",
                    "Dynamic Concert Hall Live Stage Profile"
                },
                OptionLabel2 = "Lighting & Ambience Mood",
                OptionList2 = new List<string>
                {
                    "Cinema Midnight (10% Dim Amber Glow)",
                    "Cozy Warm Lounge (30% Candlelight Warmth)",
                    "Neon Sci-Fi (Teal & Magenta Cyberpunk Glow)",
                    "Blackout Total Darkness (0% Pure Dark)"
                },
                ToggleLabel = "Motorized Equipment & Peripheral Automation",
                ToggleList = new List<string>
                {
                    "Lower 130-inch Motorized Acoustic Projector Screen",
                    "Activate Smart Popcorn Machine Pre-heater",
                    "Close Motorized Blackout Window Blinds",
                    "Engage Subwoofer Haptic Bass Shakers"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "🎬 Facade.WatchMovie()", Parameter = "watch", Description = "Orchestrates all subsystems." },
                    new() { Title = "⏹ Facade.EndMovie()", Parameter = "end", Description = "Powers off all equipment." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string movie = string.IsNullOrWhiteSpace(ctx.Input1) ? "Feature Film" : ctx.Input1;
                    if (!int.TryParse(ctx.Input2, out int volume) || volume <= 0) volume = 65;
                    string sound = ctx.SelectedOption1;
                    string lighting = ctx.SelectedOption2;
                    string action = ctx.ActionCommand;

                    var facade = new HomeTheaterFacade();

                    return action == "end"
                        ? facade.EndMovie()
                        : facade.WatchMovie(movie, volume, sound, lighting, ctx.ActiveToggles);
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
    private readonly Audio _audio = new();
    private readonly Projector _projector = new();

    public void WatchMovie(string title, int volume)
    {
        _lights.Dim();
        _audio.SetVolume(volume);
        _projector.PowerOn(title);
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
                DemoDescription = "Customize channel name, video title, subscriber name, event priority, and active notification webhooks to dispatch instant alerts.",
                InputLabel1 = "New Video / Live Stream Title",
                DefaultInput1 = "Mastering System Architecture & Design Patterns",
                InputLabel2 = "New Subscriber Name to Register",
                DefaultInput2 = "Kavya Verma",
                OptionLabel1 = "Publisher Channel Context",
                OptionList1 = new List<string>
                {
                    "Tech Lead Architecture Weekly (280K Subs)",
                    "Daily Stock Market & Crypto News (450K Subs)",
                    "Game Dev & 3D Graphics Academy (120K Subs)",
                    "Enterprise Cloud Dev Podcast (85K Subs)"
                },
                OptionLabel2 = "Broadcast Event Priority",
                OptionList2 = new List<string>
                {
                    "STANDARD: New Video Upload Published",
                    "URGENT: Live Stream Started Now (🔴 LIVE)",
                    "COMMUNITY: Exclusive Milestone Announcement"
                },
                ToggleLabel = "Notification Channels & Webhooks",
                ToggleList = new List<string>
                {
                    "Mobile APNs/FCM Push Alert Gateway",
                    "Discord Community Bot Announcement Webhook",
                    "Email Weekly Newsletter Digest Service",
                    "Telegram VIP Channel Instant Bot"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "📢 Broadcast Event to Subscribers", Parameter = "broadcast", Description = "Sends alert to subscribers." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string channelContext = ctx.SelectedOption1;
                    string priority = ctx.SelectedOption2;
                    string video = string.IsNullOrWhiteSpace(ctx.Input1) ? "New Announcement" : ctx.Input1;
                    string newSub = string.IsNullOrWhiteSpace(ctx.Input2) ? "Guest_User" : ctx.Input2;

                    var channel = new YouTubeChannel(channelContext);
                    channel.Subscribe(new UserSubscriber(newSub));
                    channel.Subscribe(new UserSubscriber("Aarav Sharma"));
                    channel.Subscribe(new UserSubscriber("Priya Patel"));

                    if (ctx.ActiveToggles.Contains("Mobile APNs/FCM Push Alert Gateway"))
                        channel.Subscribe(new PushGatewayObserver());
                    if (ctx.ActiveToggles.Contains("Discord Community Bot Announcement Webhook"))
                        channel.Subscribe(new DiscordWebhookObserver());
                    if (ctx.ActiveToggles.Contains("Email Weekly Newsletter Digest Service"))
                        channel.Subscribe(new EmailDigestObserver());
                    if (ctx.ActiveToggles.Contains("Telegram VIP Channel Instant Bot"))
                        channel.Subscribe(new TelegramBotObserver());

                    var logs = channel.Broadcast(video, priority);

                    return $"[OBSERVER PATTERN BROADCAST DISPATCH]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Subject (Publisher):  {channel.ChannelName}\n" +
                           $"• Event Dispatched:     \"{video}\"\n" +
                           $"• Priority Level:       {priority}\n\n" +
                           $"ACTIVE SUBSCRIBERS NOTIFIED ({logs.Count} Listeners):\n" +
                           string.Join("\n", logs) + "\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: [SUCCESS] All observers notified simultaneously with zero polling overhead.";
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
    string Notify(string videoTitle, string channelName, string priority);
}"
                    },
                    new()
                    {
                        FileName = "YouTubeChannel.cs",
                        Role = "Publisher Class",
                        Description = "Keeps subscriber list and notifies all on new video.",
                        Code = @"namespace ObserverDemo;

public class YouTubeChannel
{
    private readonly List<IObserver> _subscribers = new();
    public void Subscribe(IObserver sub) => _subscribers.Add(sub);

    public void UploadVideo(string title, string priority)
    {
        foreach (var sub in _subscribers) sub.Notify(title, ""Tech"", priority);
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
                DemoDescription = "Set origin, destination, routing algorithm, traffic congestion, and trip optimization flags to compute dynamic travel time, toll fares, and carbon footprint.",
                InputLabel1 = "Trip Departure Point",
                DefaultInput1 = "Bandra Kurla Complex (Mumbai)",
                InputLabel2 = "Destination Point",
                DefaultInput2 = "Pune Tech Park (Hinjawadi)",
                OptionLabel1 = "Navigation Strategy Algorithm",
                OptionList1 = new List<string>
                {
                    "Highway Express Strategy (Fastest / Tolls Included)",
                    "Scenic Greenway Bicycle Route (Zero Carbon / Healthy)",
                    "Metropolitan Public Transit (Metro + Express Train)",
                    "Eco-Optimized Electric Vehicle Route (EV Charger Stops)"
                },
                OptionLabel2 = "Traffic & Congestion Conditions",
                OptionList2 = new List<string>
                {
                    "Normal Flowing Traffic (1.0x Time)",
                    "Peak Rush Hour Congestion (1.45x Time)",
                    "Heavy Monsoon Rains & Slow Traffic (1.8x Time)",
                    "Late Night Empty Highways (0.85x Time)"
                },
                ToggleLabel = "Trip Preferences & Flags",
                ToggleList = new List<string>
                {
                    "Avoid Highway Tolls (Reroute via State Highways)",
                    "Include Live EV Fast Charging Stops (+20 mins)",
                    "Carpool / High-Occupancy Vehicle Lane Enabled",
                    "Include Real-Time Carbon Footprint Offset Analysis"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "Calculate Route via Strategy", Parameter = "nav", Description = "Runs selected route strategy." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string origin = string.IsNullOrWhiteSpace(ctx.Input1) ? "Origin" : ctx.Input1;
                    string dest = string.IsNullOrWhiteSpace(ctx.Input2) ? "Destination" : ctx.Input2;
                    string stratChoice = ctx.SelectedOption1;
                    string trafficChoice = ctx.SelectedOption2;

                    double trafficMultiplier = trafficChoice.Contains("1.45") ? 1.45 :
                                               trafficChoice.Contains("1.8") ? 1.80 :
                                               trafficChoice.Contains("0.85") ? 0.85 : 1.00;

                    IRouteStrategy strategy = stratChoice.Contains("Highway") ? new HighwayExpressStrategy() :
                                             stratChoice.Contains("Bicycle") ? new BicycleScenicStrategy() :
                                             stratChoice.Contains("Transit") ? new PublicTransitStrategy() :
                                             new EvEcoStrategy();

                    var navigator = new NavigatorContext(strategy);
                    return navigator.Calculate(origin, dest, trafficMultiplier, ctx.ActiveToggles);
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
    RouteResult CalculateRoute(string from, string to, double traffic);
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
    public RouteResult Navigate(string from, string to) => _strategy.CalculateRoute(from, to, 1.0);
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
                DemoDescription = "Type custom text, select text transformation commands, and trigger Undo to step backward through the history stack.",
                InputLabel1 = "Custom Text / Snippet to Add",
                DefaultInput1 = "Design patterns produce clean and decoupled architectures.",
                InputLabel2 = "Author / Editor Tag",
                DefaultInput2 = "JK_LeadArchitect",
                OptionLabel1 = "Text Transformation Action",
                OptionList1 = new List<string>
                {
                    "Insert Custom Text Snippet",
                    "Convert Entire Buffer to UPPERCASE",
                    "Convert Entire Buffer to Title Case",
                    "Wrap Buffer in Markdown Quotes (> )",
                    "Clear Entire Buffer (Destructive Action)"
                },
                OptionLabel2 = "History Stack Buffer Size",
                OptionList2 = new List<string>
                {
                    "Standard History Stack (10 Levels)",
                    "Extended History Stack (50 Levels)",
                    "Unlimited Persistent Session History"
                },
                ToggleLabel = "Editor Metadata & Formatting",
                ToggleList = new List<string>
                {
                    "Auto-Append Timestamp Header",
                    "Include Word & Character Count Analytics"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "➕ Execute Selected Command", Parameter = "execute", Description = "Pushes command onto history stack." },
                    new() { Title = "⟲ Undo Last Command", Parameter = "undo", Description = "Pops latest command and calls Undo()." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string text = string.IsNullOrWhiteSpace(ctx.Input1) ? "Clean Code" : ctx.Input1;
                    string author = string.IsNullOrWhiteSpace(ctx.Input2) ? "Author" : ctx.Input2;
                    string actionChoice = ctx.SelectedOption1;
                    string action = ctx.ActionCommand;

                    var editor = new TextEditor { Buffer = "Initial Document Buffer" };
                    var history = new CommandHistory();

                    var initialCmd = new InsertTextCommand(editor, $"[{author}] Start");
                    initialCmd.Execute();
                    history.Push(initialCmd);

                    if (action == "undo")
                    {
                        var popped = history.Pop();
                        popped?.Undo();

                        return $"[COMMAND PATTERN: UNDO OPERATION]\n" +
                               $"------------------------------------------------------------\n" +
                               $"• Popped Command:    {popped?.CommandName}\n" +
                               $"• Invoked:           ICommand.Undo()\n" +
                               $"• Restored Buffer:   \"{editor.Buffer}\"\n" +
                               $"• Remaining Stack:   {history.Count} command(s) in history\n" +
                               $"------------------------------------------------------------\n" +
                               $"STATUS: State restored cleanly via encapsulated Command object.";
                    }

                    ICommand cmdToRun = actionChoice.Contains("Insert")
                        ? new InsertTextCommand(editor, text)
                        : new TransformCaseCommand(editor, actionChoice);

                    cmdToRun.Execute();
                    history.Push(cmdToRun);

                    bool showStats = ctx.ActiveToggles.Contains("Include Word & Character Count Analytics");
                    string wordStats = showStats
                        ? $"\n• Character Count:   {editor.Buffer.Length} chars | Word Count: {editor.Buffer.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length} words"
                        : "";

                    return $"[COMMAND PATTERN: EXECUTED COMMAND]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Executed Action:   {cmdToRun.CommandName}\n" +
                           $"• Active Buffer:     \"{editor.Buffer}\"{wordStats}\n" +
                           $"• Active Undo Stack ({history.Count} items):\n" +
                           string.Join("\n", history.GetHistoryList().Select((c, i) => $"   [{i + 1}] {c}")) + "\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: Command encapsulated as stand-alone object with full Undo state.";
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
    string CommandName { get; }
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
    public string CommandName => ""InsertCommand"";

    public InsertCommand(TextEditor ed) { _editor = ed; }

    public void Execute() { _oldText = _editor.Text; _editor.Text += "" New Text""; }
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
                DemoTitle = "Audio Media Player State Machine",
                DemoDescription = "Configure track name, streaming bitrate, equalizer profile, and starting state to test dynamic state machine transitions.",
                InputLabel1 = "Audio Track Name & Artist",
                DefaultInput1 = "Interstellar Main Theme - Hans Zimmer",
                InputLabel2 = "Audio Playback Bitrate (kbps)",
                DefaultInput2 = "320",
                OptionLabel1 = "Player Initial State",
                OptionList1 = new List<string>
                {
                    "State: Stopped (Ready to Stream)",
                    "State: Playing (Active Stream)",
                    "State: Paused (Frozen Buffer)",
                    "State: Locked (Controls Locked)"
                },
                OptionLabel2 = "Equalizer Acoustic Preset",
                OptionList2 = new List<string>
                {
                    "Dynamic Bass Boost Profile",
                    "Vocal Clarity Acoustic Profile",
                    "Flat Studio Reference Profile",
                    "Dolby Spatial Surround Profile"
                },
                ToggleLabel = "Hi-Res Audio Capabilities",
                ToggleList = new List<string>
                {
                    "Enable Headphone Spatial Audio",
                    "Enable Lossless FLAC Hi-Res Audio",
                    "Enable Crossfade Between Tracks (3s)",
                    "Enable Sleep Timer Auto-Stop"
                },
                DemoActions = new List<DemoActionItem>
                {
                    new() { Title = "▶ Play Button", Parameter = "play", Description = "Sends Play to current state." },
                    new() { Title = "⏸ Pause Button", Parameter = "pause", Description = "Sends Pause to current state." },
                    new() { Title = "🔒 Lock / Unlock Button", Parameter = "lock", Description = "Sends Lock to current state." }
                },
                AdvancedDemoRunner = (ctx) =>
                {
                    string track = string.IsNullOrWhiteSpace(ctx.Input1) ? "Master Audio Track" : ctx.Input1;
                    if (!int.TryParse(ctx.Input2, out int bitrate) || bitrate <= 0) bitrate = 320;

                    string choice = ctx.SelectedOption1;
                    string eq = ctx.SelectedOption2;
                    string action = ctx.ActionCommand;

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
                        _ => context.State.ClickPlay(context, track, bitrate, eq)
                    };

                    bool spatial = ctx.ActiveToggles.Contains("Enable Headphone Spatial Audio");

                    return $"[AUDIO PLAYER STATE MACHINE TRANSITION]\n" +
                           $"------------------------------------------------------------\n" +
                           $"• Audio Track:       \"{track}\"\n" +
                           $"• Bitrate & EQ:      {bitrate} kbps | EQ: [{eq}]\n" +
                           $"• Spatial Audio:     {(spatial ? "Enabled" : "Disabled")}\n" +
                           $"• Previous State:    {prevState}\n" +
                           $"• Button Dispatched: Click{char.ToUpper(action[0])}{action[1..]}()\n" +
                           $"• Execution Log:     {transition}\n" +
                           $"• Active State Now:  {context.State.StateName}\n" +
                           $"------------------------------------------------------------\n" +
                           $"STATUS: Player alters behavior dynamically via encapsulated State object delegation.";
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
    string StateName { get; }
    string ClickPlay(AudioPlayerContext ctx, string track, int bitrate, string eq);
    string ClickPause(AudioPlayerContext ctx);
    string ClickLock(AudioPlayerContext ctx);
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
    public string StateName => ""Playing State"";
    public string ClickPlay(AudioPlayerContext ctx, string track, int bitrate, string eq) => ""Already playing."";
    public string ClickPause(AudioPlayerContext ctx)
    {
        ctx.State = new PausedState();
        return ""Music paused."";
    }
    public string ClickLock(AudioPlayerContext ctx) => ""Locked in playback."";
}"
                    }
                }
            }
        };
    }
}
