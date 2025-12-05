namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class AddDraftPage : ContentPage
{
	public AddDraftPage(AddDraftPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}