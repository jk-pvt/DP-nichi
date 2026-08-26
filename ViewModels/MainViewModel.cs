using CommunityToolkit.Mvvm.ComponentModel;
using DesignPatternCatalog.Models;

namespace DesignPatternCatalog.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    private readonly PatternListViewModel _listViewModel;

    public MainViewModel()
    {
        _listViewModel = new PatternListViewModel(NavigateToDetail);
        _currentViewModel = _listViewModel;
    }

    public void NavigateToDetail(PatternItem pattern)
    {
        CurrentViewModel = new PatternDetailViewModel(pattern, NavigateToList);
    }

    public void NavigateToList()
    {
        CurrentViewModel = _listViewModel;
    }
}
