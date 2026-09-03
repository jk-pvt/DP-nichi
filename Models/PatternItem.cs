using System;
using System.Collections.Generic;

namespace DesignPatternCatalog.Models;

public class DemoActionItem
{
    public string Title { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PatternItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PatternCategory Category { get; set; }
    public string CategoryName => Category.ToString();

    public string DefinitionLine1 { get; set; } = string.Empty;
    public string DefinitionLine2 { get; set; } = string.Empty;
    public string TwoLineDefinition => $"{DefinitionLine1}\n{DefinitionLine2}";

    public string CategorySummary { get; set; } = string.Empty;

    public string RealLifeTitle { get; set; } = string.Empty;
    public string RealLifeAnalogy { get; set; } = string.Empty;
    public string RealLifeProblem { get; set; } = string.Empty;
    public string RealLifeSolution { get; set; } = string.Empty;

    public string IconGeometry { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#FFFFFF";

    public List<CodeFileItem> CodeFiles { get; set; } = new();

    public string DemoTitle { get; set; } = string.Empty;
    public string DemoDescription { get; set; } = string.Empty;
    public List<DemoActionItem> DemoActions { get; set; } = new();
    
    public string InputLabel1 { get; set; } = string.Empty;
    public string DefaultInput1 { get; set; } = string.Empty;
    public string InputLabel2 { get; set; } = string.Empty;
    public string DefaultInput2 { get; set; } = string.Empty;
    public string OptionLabel1 { get; set; } = "Select Option";
    public List<string> OptionList1 { get; set; } = new();
    public string OptionLabel2 { get; set; } = "Select Option";
    public List<string> OptionList2 { get; set; } = new();
    public string ToggleLabel { get; set; } = "Optional Add-ons";
    public List<string> ToggleList { get; set; } = new();

    public Func<PatternPlaygroundContext, string>? AdvancedDemoRunner { get; set; }
    public Func<string, string>? DemoRunner { get; set; }
}

public class PatternPlaygroundContext
{
    public string Input1 { get; set; } = string.Empty;
    public string Input2 { get; set; } = string.Empty;
    public string SelectedOption3 { get; set; } = string.Empty;
    public string SelectedOption2 { get; set; } = string.Empty;
    public HashSet<string> ActiveToggles { get; set; } = new();
    public string ActionCommand { get; set; } = string.Empty;
    public List<string> DynamicItems { get; set; } = new();
}
