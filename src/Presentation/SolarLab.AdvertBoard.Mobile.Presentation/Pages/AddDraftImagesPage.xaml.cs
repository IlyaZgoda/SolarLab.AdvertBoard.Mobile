namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class AddDraftImagesPage : ContentPage
{
	public AddDraftImagesPage(AddDraftImagesPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}