namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class PublishedUserAdvertsPage : ContentPage
{
	public PublishedUserAdvertsPage(PublishedUserAdvertsPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}