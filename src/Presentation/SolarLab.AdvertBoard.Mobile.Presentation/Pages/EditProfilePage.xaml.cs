namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage(EditProfilePageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }
}