namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class UserDraftDetailsPage : ContentPage
{
	public UserDraftDetailsPage(UserDraftDetailsPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}