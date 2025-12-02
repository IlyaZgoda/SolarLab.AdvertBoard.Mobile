using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public class EqualsZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count == 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class UserDraftDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _client;
        private readonly IAuthService _auth;
        private readonly IImageApiClient _imageClient;

        public UserDraftDetailsPageModel(IAdvertApiClient client, IAuthService auth, IImageApiClient imageClient)
        {
            _client = client;
            _auth = auth;
            _imageClient = imageClient;

            AdvertImages = new ObservableCollection<ImageSource>();
        }

        [ObservableProperty]
        private AdvertDraftDetailsResponse? _advert;

        [ObservableProperty]
        private ObservableCollection<ImageSource> _advertImages;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) && Guid.TryParse(idObj?.ToString(), out var advertId))
            {
                _ = LoadDraftDetailsAsync(advertId);
            }
        }

        private async Task LoadDraftDetailsAsync(Guid advertId)
        {
            try
            {
                Advert = await _client.GetDraftDetailsAsync(advertId, _auth.Jwt!);
                await LoadImagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки черновика: {ex}");
            }
        }

        private async Task LoadImagesAsync()
        {
            AdvertImages.Clear();

            if (Advert?.ImageIds == null || Advert.ImageIds.Count == 0)
                return;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.Jwt!);

            foreach (var id in Advert.ImageIds)
            {
                try
                {
                    var url = $"http://10.0.2.2:8083/api/images/{id}/download";
                    var bytes = await client.GetByteArrayAsync(url);

                    // Создаём ImageSource из byte[]
                    var stream = new MemoryStream(bytes);
                    AdvertImages.Add(ImageSource.FromStream(() => stream));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения {id}: {ex}");
                }
            }
        }

        [RelayCommand]
        private async Task AddPhotoAsync()
        {
            if (Advert == null)
                return;

            try
            {
                var pick = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Выберите изображение"
                });

                if (pick == null)
                    return;

                await using var stream = await pick.OpenReadAsync();

                var upload = await _imageClient.UploadDraftImageAsync(Advert.Id, stream, _auth.Jwt!);

                if (upload == null || upload.Id == Guid.Empty)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Сервер не вернул ID изображения.", "ОК");
                    return;
                }

                // ⛔ НЕ трогаем Advert.ImageIds — оно init-only!

                // ✔ Загружаем обновлённый черновик
                Advert = await _client.GetDraftDetailsAsync(Advert.Id, _auth.Jwt!);

                // ✔ Загружаем изображения
                await LoadImagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления фото: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }





    }
}

