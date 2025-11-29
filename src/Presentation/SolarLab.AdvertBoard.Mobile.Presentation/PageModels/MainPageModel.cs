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
    public partial class MainPageModel : ObservableObject, IProjectTaskPageModel
    {
        private bool _isNavigatedTo;
        private bool _categoriesLoaded;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskRepository _taskRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly SeedDataService _seedDataService;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IAdvertApiClient _advertApiClient;
        [ObservableProperty]
        private List<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private List<ProjectTask> _tasks = [];

        [ObservableProperty]
        private List<Project> _projects = [];

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, MMM d");

        [ObservableProperty]
        private Project? selectedProject;

        public bool HasCompletedTasks
            => Tasks?.Any(t => t.IsCompleted) ?? false;


        [ObservableProperty]
        private ObservableCollection<CategoryNode> categories = new();

        private CategoryTreeResponse? fullTree;

        public ObservableCollection<PublishedAdvertItem> Adverts { get; } = new();

        [ObservableProperty] private bool isLoadingNextPage;
        private int _page = 1;
        private bool _hasMore = true;

        public MainPageModel(SeedDataService seedDataService, ProjectRepository projectRepository,
            TaskRepository taskRepository, CategoryRepository categoryRepository, ModalErrorHandler errorHandler,
            ICategoryApiClient categoryApiClient, IAdvertApiClient advertApiClient)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _errorHandler = errorHandler;
            _seedDataService = seedDataService;
            _categoryApiClient = categoryApiClient;
            _advertApiClient = advertApiClient;
        }

        private async Task LoadFirstPageAsync()
        {
            _page = 1;

            _hasMore = true;

            Adverts.Clear();

            await LoadNextPageAsync();
        }

        [RelayCommand]
        public async Task LoadNextPageAsync()
        {
            if (!_hasMore || IsLoadingNextPage)
                return;

            try
            {
                IsLoadingNextPage = true;

                var filter = new AdvertFilterRequest
                {
                    Page = _page,
                    PageSize = 10,
                };

                var response = await _advertApiClient.GetPublishedAsync(filter);

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
                fullTree = await _categoryApiClient.GetCategoryTreeAsync();

                if (fullTree?.Categories != null)
                {
                    Categories.Clear();
                    foreach (var node in fullTree.Categories)
                    {
                        Categories.Add(node); 
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private async Task LoadData()
        {
            try
            {
                IsBusy = true;

                Projects = await _projectRepository.ListAsync();

                var chartData = new List<CategoryChartData>();
                var chartColors = new List<Brush>();

                var categories = await _categoryRepository.ListAsync();
                foreach (var category in categories)
                {
                    chartColors.Add(category.ColorBrush);

                    var ps = Projects.Where(p => p.CategoryID == category.ID).ToList();
                    int tasksCount = ps.SelectMany(p => p.Tasks).Count();

                    chartData.Add(new(category.Title, tasksCount));
                }

                TodoCategoryData = chartData;
                TodoCategoryColors = chartColors;

                Tasks = await _taskRepository.ListAsync();
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        private async Task InitData(SeedDataService seedDataService)
        {
            bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

            if (!isSeeded)
            {
                await seedDataService.LoadSeedDataAsync();
            }

            Preferences.Default.Set("is_seeded", true);
            await Refresh();
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
                _errorHandler.HandleError(e);
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
        private Task TaskCompleted(ProjectTask task)
        {
            OnPropertyChanged(nameof(HasCompletedTasks));
            return _taskRepository.SaveItemAsync(task);
        }

        [RelayCommand]
        private Task AddTask()
            => Shell.Current.GoToAsync($"task");

        [RelayCommand]
        private Task? NavigateToProject(Project project)
            => project is null ? null : Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        private Task NavigateToTask(ProjectTask task)
            => Shell.Current.GoToAsync($"task?id={task.ID}");

        [RelayCommand]
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                await _taskRepository.DeleteItemAsync(task);
                Tasks.Remove(task);
            }

            OnPropertyChanged(nameof(HasCompletedTasks));
            Tasks = new(Tasks);
            await AppShell.DisplayToastAsync("All cleaned up!");
        }
    }
}