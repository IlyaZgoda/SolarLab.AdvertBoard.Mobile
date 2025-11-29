namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages.Controls;

public partial class MainPageHeaderView : ContentView
{
	public MainPageHeaderView()
	{
		InitializeComponent();
	}

    private async void OnFilterTapped(object sender, TappedEventArgs e)
    {
        var view = (View)sender;

        await view.ScaleToAsync(0.85, 80, Easing.CubicOut);
        await view.ScaleToAsync(1.0, 80, Easing.CubicIn);
    }

    private async void OnSearchFocused(object sender, FocusEventArgs e)
    {
        if (SearchIcon == null)
            return;

        await SearchIcon.ScaleToAsync(0.85, 80, Easing.CubicOut);
        await SearchIcon.ScaleToAsync(1.0, 80, Easing.CubicIn);
    }

    private async void OnSearchUnfocused(object sender, FocusEventArgs e)
    {
        if (SearchIcon == null)
            return;

        await SearchIcon.ScaleToAsync(0.95, 80, Easing.CubicOut);
        await SearchIcon.ScaleToAsync(1.0, 80, Easing.CubicIn);
    }

    private void OnBackgroundTapped(object sender, EventArgs e)
    {
        SearchEntry?.Unfocus();
    }
}