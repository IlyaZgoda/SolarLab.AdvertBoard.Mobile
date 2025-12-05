using System.ComponentModel;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class AdvertDetailsPage : ContentPage
{
	public AdvertDetailsPage(AdvertDetailsPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
    }



}