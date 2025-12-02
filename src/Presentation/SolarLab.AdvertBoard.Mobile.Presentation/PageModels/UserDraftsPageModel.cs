using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class UserDraftsPageModel : ObservableObject
    {
        private readonly IAdvertApiClient _client;
        private readonly IAuthService _auth;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<AdvertDraftItem> Adverts { get; } = new();

        private int _currentPage = 1;
        private int _totalPages = 1;
        private const int PageSize = 20;

        public UserDraftsPageModel(IAdvertApiClient client, IAuthService auth)
        {
            _client = client;
            _auth = auth;

            LoadNextPageCommand = new AsyncRelayCommand(LoadNextPageAsync);

            _ = ReloadAsync();
        }

        public IAsyncRelayCommand LoadNextPageCommand { get; }

        [ObservableProperty]
        private AdvertDraftItem? _selectedAdvert;

        partial void OnSelectedAdvertChanged(AdvertDraftItem? value)
        {
            if (value == null) return;

            _ = Shell.Current.GoToAsync($"draft-details?AdvertId={value.Id}");

            SelectedAdvert = null;
        }

        private async Task ReloadAsync()
        {
            Adverts.Clear();
            _currentPage = 1;
            _totalPages = 1;

            await LoadNextPageAsync();
        }

        private async Task LoadNextPageAsync()
        {
            if (IsLoading) return;
            if (_currentPage > _totalPages) return;

            try
            {
                IsLoading = true;

                var request = new GetUserAdvertDraftsRequest(_currentPage, PageSize);
                var response = await _client.GetUserDraftsAsync(request, _auth.Jwt!);

                foreach (var item in response.Items)
                    Adverts.Add(item);

                _totalPages = response.TotalPages;
                _currentPage++;
            }
            finally
            {
                IsLoading = false;
            }
        }


    }
}
