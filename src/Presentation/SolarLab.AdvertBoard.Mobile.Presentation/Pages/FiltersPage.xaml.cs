namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class FiltersPage : ContentPage
{
    public FiltersPage(FiltersPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
