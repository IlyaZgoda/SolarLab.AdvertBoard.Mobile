using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Authentication;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        [ObservableProperty]
        private string email = "";

        [ObservableProperty]
        private string password = "";
        private IUsersApiClient _usersClient;
        private IAuthService _authService;

        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

        public LoginPageModel(IUsersApiClient usersClient, IAuthService authService)
        {
            _usersClient = usersClient;
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                var resposnse = await _usersClient.LoginAsync(new LoginUserRequest(Email, Password));
                _authService.Jwt = resposnse.Token;
                await Shell.Current.GoToAsync("//main");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка входа", ex.Message, "OK");
            }

        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            await Shell.Current.GoToAsync("register");
        }

        partial void OnEmailChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }

        partial void OnPasswordChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }

    }
}
