using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternCatalog.Models;

namespace DesignPatternCatalog.ViewModels;

public partial class ToggleOptionItem : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;

    public ToggleOptionItem(string name, bool isSelected = false)
    {
        Name = name;
        _isSelected = isSelected;
    }
}

public partial class PatternDetailViewModel : ViewModelBase
{
    private readonly Action _onBack;

    [ObservableProperty]
    private PatternItem _pattern;

    [ObservableProperty]
    private CodeFileItem? _selectedCodeFile;

    [ObservableProperty]
    private string _demoOutput = string.Empty;

    [ObservableProperty]
    private bool _isCopiedToastVisible;

    [ObservableProperty]
    private string _input1 = string.Empty;

    [ObservableProperty]
    private string _input2 = string.Empty;

    [ObservableProperty]
    private string _selectedOption1 = string.Empty;

    [ObservableProperty]
    private string _selectedOption2 = string.Empty;

    public ObservableCollection<CodeFileItem> CodeFiles { get; } = new();
    public ObservableCollection<string> OptionList1 { get; } = new();
    public ObservableCollection<string> OptionList2 { get; } = new();
    public ObservableCollection<ToggleOptionItem> ToggleOptions { get; } = new();

    public bool HasInput1 => !string.IsNullOrEmpty(Pattern.InputLabel1);
    public bool HasInput2 => !string.IsNullOrEmpty(Pattern.InputLabel2);
    public bool HasOptionList1 => Pattern.OptionList1.Count > 0;
    public bool HasOptionList2 => Pattern.OptionList2.Count > 0;
    public bool HasToggles => Pattern.ToggleList.Count > 0;

    public PatternDetailViewModel(PatternItem pattern, Action onBack)
    {
        _pattern = pattern;
        _onBack = onBack;

        foreach (var file in pattern.CodeFiles)
        {
            CodeFiles.Add(file);
        }
        SelectedCodeFile = CodeFiles.FirstOrDefault();

        Input1 = pattern.DefaultInput1;
        Input2 = pattern.DefaultInput2;

        foreach (var opt in pattern.OptionList1) OptionList1.Add(opt);
        SelectedOption1 = OptionList1.FirstOrDefault() ?? string.Empty;

        foreach (var opt in pattern.OptionList2) OptionList2.Add(opt);
        SelectedOption2 = OptionList2.FirstOrDefault() ?? string.Empty;

        foreach (var tog in pattern.ToggleList)
        {
            var item = new ToggleOptionItem(tog, false);
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ToggleOptionItem.IsSelected))
                {
                    RunCurrentSimulation();
                }
            };
            ToggleOptions.Add(item);
        }

        RunCurrentSimulation();
    }

    [RelayCommand]
    private void GoBack()
    {
        _onBack?.Invoke();
    }

    [RelayCommand]
    private void SelectCodeFile(CodeFileItem file)
    {
        SelectedCodeFile = file;
    }

    [RelayCommand]
    private void ExecuteDemoAction(DemoActionItem actionItem)
    {
        RunSimulationWithAction(actionItem.Parameter);
    }

    [RelayCommand]
    private void RunSimulation()
    {
        RunCurrentSimulation();
    }

    partial void OnInput1Changed(string value) => RunCurrentSimulation();
    partial void OnInput2Changed(string value) => RunCurrentSimulation();
    partial void OnSelectedOption1Changed(string value) => RunCurrentSimulation();
    partial void OnSelectedOption2Changed(string value) => RunCurrentSimulation();

    private void RunCurrentSimulation()
    {
        string defaultAction = Pattern.DemoActions.Count > 0 ? Pattern.DemoActions[0].Parameter : "default";
        RunSimulationWithAction(defaultAction);
    }

    private void RunSimulationWithAction(string actionCommand)
    {
        var activeToggles = new HashSet<string>(
            ToggleOptions.Where(t => t.IsSelected).Select(t => t.Name));

        var ctx = new PatternPlaygroundContext
        {
            Input1 = Input1,
            Input2 = Input2,
            SelectedOption1 = SelectedOption1,
            SelectedOption2 = SelectedOption2,
            ActiveToggles = activeToggles,
            ActionCommand = actionCommand
        };

        if (Pattern.AdvancedDemoRunner != null)
        {
            DemoOutput = Pattern.AdvancedDemoRunner(ctx);
        }
        else if (Pattern.DemoRunner != null)
        {
            DemoOutput = Pattern.DemoRunner(actionCommand);
        }
    }

    [RelayCommand]
    private async Task CopyCurrentCodeAsync()
    {
        if (SelectedCodeFile == null || string.IsNullOrWhiteSpace(SelectedCodeFile.Code))
            return;

        try
        {
            var topLevel = Avalonia.Application.Current?.ApplicationLifetime switch
            {
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
                _ => null
            };

            if (topLevel?.Clipboard != null)
            {
                await topLevel.Clipboard.SetTextAsync(SelectedCodeFile.Code);
                IsCopiedToastVisible = true;
                await Task.Delay(2500);
                IsCopiedToastVisible = false;
            }
        }
        catch
        {
        }
    }
}
