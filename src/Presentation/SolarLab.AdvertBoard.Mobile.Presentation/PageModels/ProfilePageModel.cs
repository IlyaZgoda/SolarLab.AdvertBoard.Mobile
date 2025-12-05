using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class ProfilePageModel : ObservableObject
    {
        public ICommand ShowPublishedCommand { get; }
        public ICommand ShowDraftsCommand { get; }




        // Профильные свойства
        [ObservableProperty]
        private string _firstName;

        partial void OnFirstNameChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(FullName));
        }

        [ObservableProperty]
        private string _middleName;

        partial void OnMiddleNameChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(FullName));
        }

        [ObservableProperty]
        private string _lastName;

        partial void OnLastNameChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(FullName));
        }

        // Вычисляемое свойство
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();

        [ObservableProperty] private string _contactEmail = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;

        // Ошибки валидации (пока пусто, позже будем использовать)
        [ObservableProperty] private string _firstNameError = string.Empty;
        [ObservableProperty] private string _lastNameError = string.Empty;
        [ObservableProperty] private string _middleNameError = string.Empty;
        [ObservableProperty] private string _contactEmailError = string.Empty;
        [ObservableProperty] private string _phoneError = string.Empty;

        public ICommand EditProfileCommand { get; }

        private readonly IUsersApiClient _usersClient;
        private readonly IAuthService _authService;
        private int _selectedThemeIndex;
        public int SelectedThemeIndex
        {
            get => _selectedThemeIndex;
            set
            {
                if (SetProperty(ref _selectedThemeIndex, value))
                {
                    Application.Current.UserAppTheme = value == 0 ? AppTheme.Light : AppTheme.Dark;
                }
            }
        }

        // В конструкторе или методе инициализации
        public ProfilePageModel(IUsersApiClient usersClient, IAuthService authService)
        {
            _usersClient = usersClient;
            _authService = authService;
            // Синхронизация с текущей темой
            _selectedThemeIndex = (Application.Current.UserAppTheme == AppTheme.Dark ||
                       (Application.Current.UserAppTheme == AppTheme.Unspecified && Application.Current.RequestedTheme == AppTheme.Dark))
                       ? 1 : 0;

            EditProfileCommand = new AsyncRelayCommand(OnEditProfileAsync);

            ShowPublishedCommand = new AsyncRelayCommand(OnShowPublishedAsync);
            ShowDraftsCommand = new AsyncRelayCommand(OnShowDraftsAsync);

            _ = LoadProfileAsync();
        }

        private async Task OnEditProfileAsync()
        {
            var query = new Dictionary<string, object>
            {
                { "FirstName", FirstName },
                { "LastName", LastName },
                { "MiddleName", MiddleName },
                { "ContactEmail", ContactEmail },
                { "Phone", Phone }
            };

            await Shell.Current.GoToAsync("edit-profile", query);
        }


        private async Task OnShowPublishedAsync()
        {
            await Shell.Current.GoToAsync("published-user-adverts");
        }

        private async Task OnShowDraftsAsync()
        {
            await Shell.Current.GoToAsync("user-drafts");
        }

        private async Task LoadProfileAsync()
        {
            if (!_authService.IsAuthenticated)
                return;

            try
            {
                // Передаем JWT в заголовке
                var user = await _usersClient.GetUserByIdentityIdAsync(_authService.Jwt!);

                // Заполняем свойства
                FirstName = user.FirstName;
                LastName = user.LastName;
                MiddleName = user.MiddleName ?? string.Empty;
                ContactEmail = user.ContactEmail ?? string.Empty;
                Phone = user.PhoneNumber ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Можно показать сообщение об ошибке или лог
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки профиля: {ex}");
            }
        }

        [RelayCommand]
        private async Task CreateAdvertAsync()
        {
            // Переход на страницу создания черновика
            await Shell.Current.GoToAsync("create-draft");
        }

    }
}
