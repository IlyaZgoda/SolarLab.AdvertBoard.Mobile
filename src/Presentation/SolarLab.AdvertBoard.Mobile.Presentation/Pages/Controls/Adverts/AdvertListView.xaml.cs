using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using System.Collections.ObjectModel;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages.Controls.Adverts;

public partial class AdvertListView : ContentView
{
    public AdvertListView()
    {
        InitializeComponent();
    }

    // Коллекция объявлений
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(ObservableCollection<PublishedAdvertItem>),
            typeof(AdvertListView),
            new ObservableCollection<PublishedAdvertItem>());

    public ObservableCollection<PublishedAdvertItem> Items
    {
        get => (ObservableCollection<PublishedAdvertItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    // Команда подгрузки следующей страницы
    public static readonly BindableProperty LoadNextPageCommandProperty =
        BindableProperty.Create(
            nameof(LoadNextPageCommand),
            typeof(IAsyncRelayCommand),
            typeof(AdvertListView));

    public IAsyncRelayCommand LoadNextPageCommand
    {
        get => (IAsyncRelayCommand)GetValue(LoadNextPageCommandProperty);
        set => SetValue(LoadNextPageCommandProperty, value);
    }

    // Выбранный элемент
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(PublishedAdvertItem),
            typeof(AdvertListView),
            null,
            BindingMode.TwoWay);

    public PublishedAdvertItem SelectedItem
    {
        get => (PublishedAdvertItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}