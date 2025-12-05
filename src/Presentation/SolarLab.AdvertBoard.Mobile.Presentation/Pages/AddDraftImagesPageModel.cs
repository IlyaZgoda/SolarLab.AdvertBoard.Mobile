using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Images;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts; // Добавлено

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    // Модель данных для элемента в CollectionView.
    public partial class AdvertImageViewModel : ObservableObject
    {
        public Guid Id { get; set; }
        public ImageSource ImageSource { get; set; }
        // Добавлено для сохранения информации о выбранном файле перед загрузкой на сервер.
        public FileResult? FileInfo { get; set; }
    }

    public partial class AddDraftImagesPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IImageApiClient _imageClient;
        private readonly IAuthService _authService;
        private readonly IAdvertApiClient _advertClient; // !!! Добавлен клиент для работы с объявлениями

        // ID черновика, который мы получим при навигации
        private Guid _draftId;

        public AddDraftImagesPageModel(
            IImageApiClient imageClient,
            IAuthService authService,
            IAdvertApiClient advertClient) // !!! IAdvertApiClient добавлен в конструктор
        {
            _imageClient = imageClient;
            _authService = authService;
            _advertClient = advertClient; // Инициализация

            // Инициализация команд
            DeletePhotoCommand = new AsyncRelayCommand<Guid>(DeletePhotoAsync);
            AddPhotoCommand = new AsyncRelayCommand(AddPhotoAsync);
            SaveDraftCommand = new AsyncRelayCommand(SaveDraftAsync);
            PublishDraftCommand = new AsyncRelayCommand(PublishDraftAsync); // !!! Теперь вызывает реальный метод
        }

        // Коллекция изображений
        [ObservableProperty]
        private ObservableCollection<AdvertImageViewModel> advertImages = new();

        // Команды
        public ICommand DeletePhotoCommand { get; }
        public ICommand AddPhotoCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand PublishDraftCommand { get; }


        // ------------------------------
        // ПРИЁМ ПАРАМЕТРОВ ИЗ NAVIGATION
        // ------------------------------
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("DraftId", out var idObj) &&
                Guid.TryParse(idObj.ToString(), out var parsed))
            {
                _draftId = parsed;
            }
        }

        // ------------------------------
        // ДОБАВЛЕНИЕ ФОТО
        // ------------------------------
        private async Task AddPhotoAsync()
        {
            try
            {
                var pickOptions = new PickOptions
                {
                    PickerTitle = "Выберите фотографию",
                    FileTypes = FilePickerFileType.Images
                };

                var result = await FilePicker.PickAsync(pickOptions);

                if (result != null)
                {
                    // Создаем ImageSource из потока выбранного файла
                    var stream = await result.OpenReadAsync();
                    var newImageSource = ImageSource.FromStream(() => stream);

                    // Добавляем FileInfo для последующей загрузки на сервер
                    AdvertImages.Add(new AdvertImageViewModel
                    {
                        Id = Guid.NewGuid(),
                        ImageSource = newImageSource,
                        FileInfo = result // Сохраняем FileResult для загрузки
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка при выборе фото: {ex.Message}", "OK");
            }
        }

        // ------------------------------
        // УДАЛЕНИЕ ФОТО
        // ------------------------------
        private Task DeletePhotoAsync(Guid imageId)
        {
            var imageToDelete = AdvertImages.FirstOrDefault(i => i.Id == imageId);
            if (imageToDelete != null)
            {
                AdvertImages.Remove(imageToDelete);
            }
            return Task.CompletedTask;
        }

        // ------------------------------
        // СОХРАНЕНИЕ ФОТОК НА СЕРВЕР
        // (Вызывается также перед публикацией)
        // ------------------------------
        private async Task SaveDraftAsync()
        {
            if (_draftId == Guid.Empty)
            {
                // Не показываем ошибку здесь, чтобы не прерывать Publish,
                // если проблема с ID всплывет позже.
                return;
            }

            if (!_authService.IsAuthenticated || _authService.Jwt == null)
            {
                throw new InvalidOperationException("Пользователь не авторизован.");
            }

            var imagesToUpload = AdvertImages.Where(i => i.FileInfo != null).ToList();

            if (imagesToUpload.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var image in imagesToUpload)
                {
                    // Открываем поток для загрузки
                    using var stream = await image.FileInfo!.OpenReadAsync();

                    await _imageClient.UploadDraftImageAsync(
                        _draftId,
                        stream,
                        _authService.Jwt
                    );

                    // Очищаем FileInfo после успешной загрузки
                    image.FileInfo = null;
                }
            }
            catch (Exception ex)
            {
                // Для "Сохранить" показываем простое сообщение.
                await Application.Current.MainPage.DisplayAlert("Ошибка сохранения фото",
                    $"Не удалось сохранить все фотографии: {ex.Message}", "OK");
                throw; // Пробрасываем ошибку дальше, если вызывались из PublishDraftAsync
            }
        }

        // ------------------------------
        // ПУБЛИКАЦИЯ ЧЕРНОВИКА
        // ------------------------------
        private async Task PublishDraftAsync()
        {
            // 1. Сначала пытаемся сохранить все не загруженные фотографии
            try
            {
                await SaveDraftAsync();
            }
            catch (Exception)
            {
                // SaveDraftAsync уже показал сообщение об ошибке, прекращаем публикацию.
                return;
            }

            // 2. Проверка ID и авторизации
            if (_draftId == Guid.Empty)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    "Не удалось получить ID черновика для публикации.", "OK");
                return;
            }

            if (!_authService.IsAuthenticated || _authService.Jwt == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    "Пользователь не авторизован для публикации.", "OK");
                return;
            }

            // Проверка, что есть хотя бы одно фото. Сервер может вернуть 400, но лучше проверить заранее.
            if (!AdvertImages.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                   "Для публикации необходимо добавить хотя бы одну фотографию.", "OK");
                return;
            }

            // 3. Вызов API для публикации
            try
            {
                await _advertClient.PublishDraftAsync(_draftId, _authService.Jwt);

                await Application.Current.MainPage.DisplayAlert("Успех",
                    "Объявление успешно опубликовано!", "OK");

                // 4. Навигация: возвращаемся на две страницы назад (Images -> Draft -> Main/List)
                await Shell.Current.GoToAsync("../..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка публикации",
                    $"Не удалось опубликовать объявление: {ex.Message}", "OK");
            }
        }
    }
}