using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignPatternCatalog.Models;
using DesignPatternCatalog.Services;

namespace DesignPatternCatalog.ViewModels;

public partial class PatternListViewModel : ViewModelBase
{
    private readonly Action<PatternItem> _onPatternSelected;
    private readonly List<PatternItem> _allPatterns;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "All";

    public ObservableCollection<PatternItem> FilteredPatterns { get; } = new();
    public ObservableCollection<PatternItem> CreationalPatterns { get; } = new();
    public ObservableCollection<PatternItem> StructuralPatterns { get; } = new();
    public ObservableCollection<PatternItem> BehavioralPatterns { get; } = new();

    public int TotalPatternsCount => _allPatterns.Count;

    public PatternListViewModel(Action<PatternItem> onPatternSelected)
    {
        _onPatternSelected = onPatternSelected;
        _allPatterns = PatternRepository.Instance.GetAllPatterns().ToList();

        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void SetCategory(string category)
    {
        SelectedCategory = category;
    }

    [RelayCommand]
    private void SelectPattern(PatternItem pattern)
    {
        if (pattern != null)
        {
            _onPatternSelected(pattern);
        }
    }

    private void ApplyFilter()
    {
        FilteredPatterns.Clear();
        CreationalPatterns.Clear();
        StructuralPatterns.Clear();
        BehavioralPatterns.Clear();

        var query = _allPatterns.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLowerInvariant().Contains(search) ||
                p.DefinitionLine1.ToLowerInvariant().Contains(search) ||
                p.DefinitionLine2.ToLowerInvariant().Contains(search) ||
                p.RealLifeAnalogy.ToLowerInvariant().Contains(search) ||
                p.CategoryName.ToLowerInvariant().Contains(search));
        }

        if (SelectedCategory != "All")
        {
            query = query.Where(p => string.Equals(p.CategoryName, SelectedCategory, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.ToList();
        foreach (var p in list)
        {
            FilteredPatterns.Add(p);
            if (p.Category == PatternCategory.Creational) CreationalPatterns.Add(p);
            else if (p.Category == PatternCategory.Structural) StructuralPatterns.Add(p);
            else if (p.Category == PatternCategory.Behavioral) BehavioralPatterns.Add(p);
        }
    }
}
