using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Categories;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using SolarLab.AdvertBoard.Mobile.Presentation.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    // Application Layer / Services
    public interface ICategoryStore
    {
        ObservableCollection<CategoryNode> Categories { get; }
    }

    public class CategoryStore : ICategoryStore
    {
        public ObservableCollection<CategoryNode> Categories { get; } = new();
    }

    public partial class MainPageModel : ObservableObject, IQueryAttributable
    {
        private bool _isNavigatedTo;
        private bool _categoriesLoaded;
        public bool HasActiveFilters => _appliedFilters != null &&
    (_appliedFilters.MinPrice.HasValue ||
     _appliedFilters.MaxPrice.HasValue ||
     _appliedFilters.CategoryId != null ||
     !string.IsNullOrEmpty(_appliedFilters.SearchText) ||
     (_appliedFilters.SortBy != null && _appliedFilters.SortDescending != true));
        public MainPageModel(
        ModalErrorHandler errorHandler,
        ICategoryApiClient categoryApiClient,
        IAdvertApiClient advertApiClient,
        ICategoryStore categoryStore,
        FiltersPageModel filters)
        {
            this.errorHandler = errorHandler;
            this.categoryApiClient = categoryApiClient;
            this.advertApiClient = advertApiClient;
            this.categoryStore = categoryStore;
            this.filters = filters;
        }

        [ObservableProperty]
        bool _isRefreshing;

        [ObservableProperty]
        private string? searchQuery;


        public ObservableCollection<PublishedAdvertItem> Adverts { get; } = [];

        [ObservableProperty] private bool isLoadingNextPage;
        private int _page = 1;
        private bool _hasMore = true;

        public ObservableCollection<CategoryNode> Categories => categoryStore.Categories;
        private async Task LoadFirstPageAsync()
        {
            _page = 1;

            _hasMore = true;

            Adverts.Clear();

            await LoadNextPageAsync();
        }

        [RelayCommand]
        private async Task ExecuteSearch()
        {
            // Создаём новый объект фильтра, не изменяя рекорд напрямую
            _appliedFilters = _appliedFilters is not null
                ? _appliedFilters with { SearchText = SearchQuery, Page = 1 }
                : new AdvertFilterRequest { SearchText = SearchQuery, Page = 1 };

            await LoadFirstPageAsync(); // Загружаем первую страницу с новым фильтром
        }


        [RelayCommand]
        public async Task LoadNextPageAsync()
        {
            if (!_hasMore || IsLoadingNextPage)
                return;

            try
            {
                IsLoadingNextPage = true;

                var filter = _appliedFilters is not null
           ? _appliedFilters with { SearchText = SearchQuery, Page = 1 }
           : new AdvertFilterRequest { SearchText = SearchQuery, Page = 1 };


                var response = await advertApiClient.GetPublishedAsync(filter);

                foreach (var item in response.Items)
                    Adverts.Add(item);

                _page++;
                _hasMore = _page <= response.TotalPages;
                Console.WriteLine($"Paginated {response.Items.Count} entries");
            }
            finally
            {
                IsLoadingNextPage = false;
            }
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                var fullTree = await categoryApiClient.GetCategoryTreeAsync();
                if (fullTree?.Categories != null)
                {
                    categoryStore.Categories.Clear();
                    foreach (var node in fullTree.Categories)
                        categoryStore.Categories.Add(node);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;

                await LoadFirstPageAsync();
            }
            catch (Exception e)
            {
                errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void NavigatedTo() =>
            _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() =>
            _isNavigatedTo = false;

        [RelayCommand]
        private async Task Appearing()
        {
            if (!_categoriesLoaded)
            {
                await LoadCategoriesAsync();
                
                _categoriesLoaded = true;

                await Refresh();
            }
            // This means we are being navigated to
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        [RelayCommand]
        private Task OpenFilters() =>
            Shell.Current.GoToAsync("filters");

        private AdvertFilterRequest? _appliedFilters;
        private ModalErrorHandler errorHandler;
        private ICategoryApiClient categoryApiClient;
        private IAdvertApiClient advertApiClient;
        private ICategoryStore categoryStore;
        private FiltersPageModel filters;

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AppliedFilters", out var f))
            {
                var filters = (AdvertFilterRequest)f;
                _appliedFilters = filters;
                OnPropertyChanged(nameof(HasActiveFilters));
                // перезагрузка списка
                await LoadFirstPageAsync();
            }
        }

        [RelayCommand]
        private async Task OpenFiltersWithCategory(CategoryNode category)
        {
            var filters = new AdvertFilterRequest
            {
                CategoryId = category.Id
            };

            await Shell.Current.GoToAsync("filters", new Dictionary<string, object>
    {
        { "AppliedFilters", filters }
    });
        }

        [ObservableProperty]
        private CategoryNode? selectedCategory;

        partial void OnSelectedCategoryChanged(CategoryNode? value)
        {
            if (value == null) return;

            // Навигация асинхронно
            _ = OpenFiltersWithCategoryCommand.ExecuteAsync(value);

            // Сбрасываем SelectedItem
            SelectedCategory = null;
        }



    }
}