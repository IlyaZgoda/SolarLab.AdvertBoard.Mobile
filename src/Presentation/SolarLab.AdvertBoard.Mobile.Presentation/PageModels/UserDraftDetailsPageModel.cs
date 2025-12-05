using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Categories;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    // ============================
    // Конвертеры
    // ============================
    public class EqualsZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int count && count == 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int count && count > 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // ============================
    // Модель фото
    // ============================
    public class PhotoViewModel
    {
        public Guid Id { get; set; }
        public ImageSource ImageSource { get; set; } = default!;
    }

    // ============================
    // Основная PageModel
    // ============================
    public partial class UserDraftDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _advertClient;
        private readonly IAuthService _auth;
        private readonly IImageApiClient _imageClient;
        private readonly ICategoryStore _categoryStore;

        public UserDraftDetailsPageModel(IAdvertApiClient client, IAuthService auth, IImageApiClient imageClient, ICategoryStore categoryStore)
        {
            _advertClient = client;
            _auth = auth;
            _imageClient = imageClient;
            _categoryStore = categoryStore;

            AdvertImages = new ObservableCollection<PhotoViewModel>();

            // Обновляем листовые категории сразу
            UpdateLeafCategories();

            // Подписка на изменения
            _categoryStore.Categories.CollectionChanged += (_, __) => UpdateLeafCategories();
        }


        [ObservableProperty]
        private AdvertDraftDetailsResponse? _advert;

        [ObservableProperty]
        private ObservableCollection<PhotoViewModel> _advertImages;

        [ObservableProperty]
        private CategoryNode? _selectedCategory;

        [ObservableProperty]
        private ObservableCollection<CategoryNode> _leafCategories = new();

        public ObservableCollection<CategoryNode> Categories => _categoryStore.Categories;

        // ============================
        // Навигация
        // ============================
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) && Guid.TryParse(idObj?.ToString(), out var advertId))
            {
                _ = LoadDraftDetailsAsync(advertId);
            }
        }

        // ============================
        // Загрузка черновика
        // ============================
        private async Task LoadDraftDetailsAsync(Guid advertId)
        {
            try
            {
                Advert = await _advertClient.GetDraftDetailsAsync(advertId, _auth.Jwt!);
                await LoadImagesAsync();

                // Устанавливаем выбранную категорию по Id после загрузки
                SelectedCategory = LeafCategories.FirstOrDefault(c => c.Id == Advert.CategoryId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex}");
            }
        }

        // ============================
        // Загрузка изображений
        // ============================
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

                    var mem = new MemoryStream(bytes);
                    AdvertImages.Add(new PhotoViewModel
                    {
                        Id = id,
                        ImageSource = ImageSource.FromStream(() => mem)
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения {id}: {ex}");
                }
            }
        }

        // ============================
        // Добавление фото
        // ============================
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
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Сервер не вернул ID изображения", "OK");
                    return;
                }

                Advert = await _advertClient.GetDraftDetailsAsync(Advert.Id, _auth.Jwt!);
                await LoadImagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        // ============================
        // Удаление фото
        // ============================
        [RelayCommand]
        private async Task DeletePhotoAsync(Guid imageId)
        {
            if (Advert == null)
                return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Удалить фото?",
                "Действие нельзя отменить.",
                "Удалить", "Отмена");

            if (!confirm)
                return;

            try
            {
                await _imageClient.DeleteDraftImageAsync(Advert.Id, imageId, _auth.Jwt!);

                Advert = await _advertClient.GetDraftDetailsAsync(Advert.Id, _auth.Jwt!);
                await LoadImagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        // ============================
        // Сохранение черновика
        // ============================
        [RelayCommand]
        private async Task SaveDraftAsync()
        {
            if (Advert == null)
                return;

            try
            {
                var request = new UpdateAdvertDraftRequest(
                    CategoryId: SelectedCategory?.Id ?? Advert.CategoryId,
                    Title: string.IsNullOrWhiteSpace(Advert.Title) ? null : Advert.Title,
                    Description: string.IsNullOrWhiteSpace(Advert.Description) ? null : Advert.Description,
                    Price: Advert.Price
                );

                var json = JsonSerializer.Serialize(request);
                System.Diagnostics.Debug.WriteLine($"PATCH /draft request JSON: {json}");

                await _advertClient.UpdateDraftAsync(Advert.Id, request, _auth.Jwt!);

                Advert = await _advertClient.GetDraftDetailsAsync(Advert.Id, _auth.Jwt!);

                await Application.Current.MainPage.DisplayAlert("Успех", "Черновик сохранён", "OK");
            }
            catch (HttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка сервера", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        // ============================
        // Листовые категории
        // ============================
        private void UpdateLeafCategories()
        {
            LeafCategories.Clear();
            foreach (var leaf in GetLeafCategories(Categories))
                LeafCategories.Add(leaf);
        }

        private IEnumerable<CategoryNode> GetLeafCategories(IEnumerable<CategoryNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Children == null || node.Children.Count == 0)
                    yield return node;
                else
                {
                    foreach (var leaf in GetLeafCategories(node.Children))
                        yield return leaf;
                }
            }
        }

        [RelayCommand]
        private async Task PublishDraftAsync()
        {
            if (Advert == null)
                return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Публикация",
                "Вы уверены, что хотите опубликовать этот черновик?",
                "Опубликовать", "Отмена");

            if (!confirm)
                return;

            try
            {
                // Отправка на сервер
                await _advertClient.PublishDraftAsync(Advert.Id, _auth.Jwt!);

                // Обновляем локально (по желанию, можно пропустить)
               // Advert = await _advertClient.GetDraftDetailsAsync(Advert.Id, _auth.Jwt!);

                await Application.Current.MainPage.DisplayAlert("Успех", "Объявление опубликовано", "OK");

                // Возврат на предыдущую страницу
                await Shell.Current.GoToAsync("profile");
            }
            catch (HttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка сервера", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteDraftAsync()
        {
            if (Advert == null)
                return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Удаление черновика",
                "Вы уверены, что хотите удалить этот черновик? Действие нельзя отменить.",
                "Удалить", "Отмена");

            if (!confirm)
                return;

            try
            {
                // Отправка запроса на сервер
                await _advertClient.DeleteDraftAsync(Advert.Id, _auth.Jwt!);

                await Application.Current.MainPage.DisplayAlert("Успех", "Черновик удалён", "OK");

                // Возврат на предыдущую страницу
                await Shell.Current.GoToAsync("..");
            }
            catch (HttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка сервера", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }


    }
}
