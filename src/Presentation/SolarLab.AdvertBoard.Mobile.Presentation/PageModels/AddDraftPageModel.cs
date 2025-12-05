using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Categories;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System.Collections.ObjectModel;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class AddDraftPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _advertClient;
        private readonly ICategoryStore _categoryStore;
        private readonly IAuthService _authService;

        public AddDraftPageModel(
            IAdvertApiClient advertClient,
            ICategoryStore categoryStore,
            IAuthService authService)
        {
            _advertClient = advertClient;
            _categoryStore = categoryStore;
            _authService = authService;

            Categories = new ObservableCollection<CategoryNode>(
                GetLeafCategories(_categoryStore.Categories)
            );
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private decimal _price;
        [ObservableProperty] private CategoryNode? _selectedCategory;

        public ObservableCollection<CategoryNode> Categories { get; }

        private Guid? _draftId;

        // ------------------------------
        // ПРИЁМ ПАРАМЕТРОВ ИЗ NAVIGATION
        // ------------------------------
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) &&
                Guid.TryParse(idObj.ToString(), out var parsed))
            {
                _draftId = parsed;
            }
        }

        // ------------------------------
        // СОЗДАНИЕ ЧЕРНОВИКА
        // ------------------------------
        [RelayCommand]
        private async Task CreateDraftAsync()
        {
            if (SelectedCategory == null || string.IsNullOrWhiteSpace(Title))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    "Заполните все поля", "OK");
                return;
            }

            try
            {
                var request = new CreateAdvertDraftRequest(
                    CategoryId: SelectedCategory.Id,
                    Title: Title.Trim(),
                    Description: (Description ?? string.Empty).Trim(),
                    Price: Price
                );

                var response = await _advertClient.CreateDraftAsync(
                    request,
                    _authService.Jwt!
                );

                // Проверка успеха
                if (response == null || response.Id == Guid.Empty)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Сервер вернул пустой ID", "OK");
                    return;
                }

                // Переход к странице изображений
                await Shell.Current.GoToAsync(
                    "create-draft-images",
                    new Dictionary<string, object>
                    {
                        { "DraftId", response.Id.ToString() }
                    });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        // ------------------------------
        // Получение листовых категорий
        // ------------------------------
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
    }
}
