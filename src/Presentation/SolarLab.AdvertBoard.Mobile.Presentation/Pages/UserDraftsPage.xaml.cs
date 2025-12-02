namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class UserDraftsPage : ContentPage
{
	public UserDraftsPage(UserDraftsPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}