using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Categories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public enum SortOption
    {
        Default,
        Newest,
        Oldest,
        Cheaper,
        Expensive
    }
    public partial class EnumEqualsConverter : IValueConverter
    {
        // value: current SelectedSortOption (enum)
        // parameter: конкретное значение (enum) из XAML
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.Equals(parameter);
        }

        // ConvertBack вызывается когда пользователь кликает RadioButton.
        // Если значение true — возвращаем parameter (enum). Если false — ничего не делаем.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return parameter; // возвращаем enum (тип parameter должен совпадать с SelectedSortOption)
            return Binding.DoNothing;
        }
    }
    public partial class FiltersPageModel : ObservableObject, IQueryAttributable
    {
        private (string? sortField, bool desc) BuildSort()
        {
            return SelectedSortOption switch
            {
                SortOption.Cheaper => ("price", false),
                SortOption.Expensive => ("price", true),

                SortOption.Newest => ("createdAt", true),
                SortOption.Oldest => ("createdAt", false),

                _ => (null, true)
            };
        }


        private SortOption _selectedSortOption = SortOption.Default;
        public SortOption SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                SetProperty(ref _selectedSortOption, value);
                OnPropertyChanged(nameof(HasActiveFilters));
            }
        }


        [ObservableProperty]
        private ObservableCollection<CategoryNode> allCategories = [];

        [ObservableProperty]
        private ObservableCollection<CategoryNode> breadcrumb = new();

        [ObservableProperty]
        private CategoryNode? selectedCategory;
        private ICategoryStore _categoryStore;

        [ObservableProperty]
        private ObservableCollection<CategoryNode> currentLevelCategories = new();




        [ObservableProperty] private double priceRangeMin = 0;
        [ObservableProperty] private double priceRangeMax = 100000; // любое максимальное значение

        [ObservableProperty]
        private decimal? priceFrom;

        [ObservableProperty]
        private decimal? priceTo;
        private AdvertFilterRequest _appliedFilters;

        [RelayCommand]
        private void ClearPriceFrom()
        {
            PriceFrom = null;
        }

        [RelayCommand]
        private void ClearPriceTo()
        {
            PriceTo = null;
        }



        public FiltersPageModel(ICategoryStore categoryStore)
        {
            _categoryStore = categoryStore;
            AllCategories = categoryStore.Categories;

            // по умолчанию показываем топ-уровень категорий
            CurrentLevelCategories = new ObservableCollection<CategoryNode>(
                AllCategories.Where(c => c.ParentId == null)
            );

            UpdateBreadcrumb(null);

        }

        partial void OnSelectedCategoryChanged(CategoryNode? value)
        {
            // обновляем путь в breadcrumb
            UpdateBreadcrumb(value);

            // обновляем текущий уровень категорий на дочерние
            if (value != null && value.Children.Any())
                CurrentLevelCategories = new ObservableCollection<CategoryNode>(value.Children);
            else
                CurrentLevelCategories.Clear();

            OnPropertyChanged(nameof(HasActiveFilters));
        }

        private void UpdateBreadcrumb(CategoryNode? node)
        {
            Breadcrumb.Clear();
            Breadcrumb.Add(new CategoryNode(Guid.Empty, null, "Все категории", new())); // всегда первый элемент

            if (node == null)
                return;

            var path = new Stack<CategoryNode>();
            var current = node;

            while (current != null)
            {
                path.Push(current);
                current = AllCategories.FirstOrDefault(c => c.Id == current.ParentId);
            }

            foreach (var n in path)
                Breadcrumb.Add(n);
        }

        // Выбор категории из UI
        [RelayCommand]
        private void SelectCategory(CategoryNode category)
        {
            SelectedCategory = category;
        }

        [RelayCommand]
        private void ResetFilters()
        {
            Console.WriteLine("RESET FIRED!");

            // 1) Сброс категории
            SelectedCategory = null;

            CurrentLevelCategories = new ObservableCollection<CategoryNode>(
                AllCategories.Where(c => c.ParentId == null)
            );

            UpdateBreadcrumb(null);

            // 2) Сброс цен
            PriceFrom = null;
            PriceTo = null;

            // 3) Сброс сортировки
            SelectedSortOption = SortOption.Default;

            // 4) Уведомляем об изменении HasActiveFilters
            OnPropertyChanged(nameof(HasActiveFilters));
        }

        public bool HasActiveFilters =>
            PriceFrom != null ||
            PriceTo != null ||
            SelectedCategory != null ||
            SelectedSortOption != SortOption.Default;


        partial void OnPriceFromChanged(decimal? value)
        {
            OnPropertyChanged(nameof(HasActiveFilters));
        }

        partial void OnPriceToChanged(decimal? value)
        {
            OnPropertyChanged(nameof(HasActiveFilters));
        }

        [RelayCommand]
        private async Task Apply()
        {
            var (sortField, desc) = BuildSort();

            var filters = new AdvertFilterRequest
            {
                MinPrice = PriceFrom,
                MaxPrice = PriceTo,
                CategoryId = SelectedCategory?.Id,
                SortBy = sortField,
                SortDescending = desc
            };

            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
    {
        { "AppliedFilters", filters }
    });
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AppliedFilters", out var f))
            {
                var filters = (AdvertFilterRequest)f;
                _appliedFilters = filters;

                // Если пришла категория, выбираем её
                if (filters.CategoryId.HasValue)
                {
                    SelectedCategory = AllCategories.FirstOrDefault(c => c.Id == filters.CategoryId.Value);
                    // Это вызовет OnSelectedCategoryChanged и построит хлебные крошки + дочерние категории
                }

                // Если пришёл диапазон цен
                PriceFrom = filters.MinPrice;
                PriceTo = filters.MaxPrice;

                // Можно обработать сортировку
                if (!string.IsNullOrEmpty(filters.SortBy))
                {
                    SelectedSortOption = filters.SortBy switch
                    {
                        "price" => filters.SortDescending ? SortOption.Expensive : SortOption.Cheaper,
                        "createdAt" => filters.SortDescending ? SortOption.Newest : SortOption.Oldest,
                        _ => SortOption.Default
                    };
                }

                // Обновляем UI (например, кнопка фильтров)
                OnPropertyChanged(nameof(HasActiveFilters));
            }
        }


    }

}

