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
    public partial class PublishedUserAdvertsPageModel : ObservableObject
    {
        private readonly IAdvertApiClient _client;
        private readonly IAuthService _auth;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<PublishedAdvertItem> Adverts { get; } = new();

        private int _currentPage = 1;
        private int _totalPages = 1;
        private const int PageSize = 20;

        public PublishedUserAdvertsPageModel(IAdvertApiClient client, IAuthService auth)
        {
            _client = client;
            _auth = auth;

            LoadNextPageCommand = new AsyncRelayCommand(LoadNextPageAsync);

            _ = ReloadAsync();
        }

        public IAsyncRelayCommand LoadNextPageCommand { get; }

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
            if (_currentPage > _totalPages) return; // всё загружено

            try
            {
                IsLoading = true;

                var request = new GetUserPublishedAdvertsRequest(
                    Page: _currentPage,
                    PageSize: PageSize
                );

                var response = await _client.GetUserPublishedAdvertsAsync(request, _auth.Jwt!);

                foreach (var item in response.Items)
                    Adverts.Add(item);

                _totalPages = response.TotalPages;
                _currentPage++;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки объявлений: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [ObservableProperty]
        private PublishedAdvertItem? selectedAdvert;

        partial void OnSelectedAdvertChanged(PublishedAdvertItem? value)
        {
            if (value == null) return;

            _ = Shell.Current.GoToAsync($"advert-details?AdvertId={value.Id}");

            // сброс, чтобы можно было выбрать снова
            SelectedAdvert = null;
        }

    }
}
