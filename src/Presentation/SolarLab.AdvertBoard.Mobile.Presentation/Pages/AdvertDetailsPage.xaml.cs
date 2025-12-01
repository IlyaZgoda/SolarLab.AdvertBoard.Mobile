using System.ComponentModel;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages;

public partial class AdvertDetailsPage : ContentPage
{
	public AdvertDetailsPage(AdvertDetailsPageModel model)
	{
		InitializeComponent();
		BindingContext = model;

        model.CommentsOpened += async () => await AnimateOpenAsync();
        model.CommentsClosed += async () => await AnimateCloseAsync();
    }


    private async Task AnimateOpenAsync()
    {
        CommentsContainer.IsVisible = true;

        // Анимация: выезжает вниз и плавно проявляется
        await Task.WhenAll(
            CommentsContainer.FadeTo(1, 250, Easing.CubicOut),
            CommentsContainer.ScaleYTo(1, 250, Easing.CubicOut)
        );
    }

    private async Task AnimateCloseAsync()
    {
        await Task.WhenAll(
            CommentsContainer.FadeTo(0, 200, Easing.CubicIn),
            CommentsContainer.ScaleYTo(0, 200, Easing.CubicIn)
        );

        CommentsContainer.IsVisible = false;
    }
}