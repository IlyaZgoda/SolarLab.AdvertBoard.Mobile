using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages.Controls.Adverts;

public partial class DraftListView : ContentView
{
    public DraftListView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(IEnumerable<AdvertDraftItem>), typeof(DraftListView), null);

    public IEnumerable<AdvertDraftItem> Items
    {
        get => (IEnumerable<AdvertDraftItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(AdvertDraftItem), typeof(DraftListView), null, BindingMode.TwoWay);

    public AdvertDraftItem SelectedItem
    {
        get => (AdvertDraftItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty LoadNextPageCommandProperty =
        BindableProperty.Create(nameof(LoadNextPageCommand), typeof(IAsyncRelayCommand), typeof(DraftListView), null);

    public IAsyncRelayCommand LoadNextPageCommand
    {
        get => (IAsyncRelayCommand)GetValue(LoadNextPageCommandProperty);
        set => SetValue(LoadNextPageCommandProperty, value);
    }
}
