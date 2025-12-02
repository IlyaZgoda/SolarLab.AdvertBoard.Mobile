namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}