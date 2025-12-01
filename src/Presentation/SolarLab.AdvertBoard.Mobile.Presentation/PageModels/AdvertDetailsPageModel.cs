using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using SolarLab.AdvertBoard.Mobile.Contracts.Comments;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class AdvertDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _advertClient;
        private readonly ICommentsApiClient _commentsClient;

        public AdvertDetailsPageModel(IAdvertApiClient advertClient, ICommentsApiClient commentsClient)
        {
            _advertClient = advertClient;
            _commentsClient = commentsClient;

            AdvertImages = new ObservableCollection<ImageSource>();
            Comments = new ObservableCollection<CommentItem>();

            ToggleCommentsButtonText = "Показать комментарии";
        }

        [ObservableProperty]
        private PublishedAdvertDetailsResponse? advert;

        [ObservableProperty]
        private ObservableCollection<ImageSource> advertImages;

        [ObservableProperty]
        private ObservableCollection<CommentItem> comments;

        [ObservableProperty]
        private bool commentsVisible;

        [ObservableProperty]
        private bool showToggleCommentsButton;

        [ObservableProperty]
        private string toggleCommentsButtonText;

        [ObservableProperty]
        private bool addCommentVisible;

        private bool commentsLoaded = false;

        [ObservableProperty]
        private int commentsPage = 1;

        [ObservableProperty]
        private int commentsPageSize = 10;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) &&
                idObj is string idStr &&
                Guid.TryParse(idStr, out var id))
            {
                _ = LoadAdvertAsync(id);
            }
        }

        private async Task LoadAdvertAsync(Guid id)
        {
            Advert = await _advertClient.GetAdvertDetailsAsync(id);

            if (Advert != null)
                await LoadImagesAsync();

            // Показ кнопки "Показать комментарии", если есть комментарии
            ShowToggleCommentsButton = Advert?.CommentsCount > 0;

            // Кнопка "Добавить комментарий"
            AddCommentVisible = Advert != null && (Advert.CommentsCount > 0 ? CommentsVisible : true);
        }

        private Task LoadImagesAsync()
        {
            AdvertImages.Clear();

            if (Advert?.ImageIds == null || Advert.ImageIds.Count == 0)
                return Task.CompletedTask;

            foreach (var id in Advert.ImageIds)
            {
                try
                {
                    var url = $"http://10.0.2.2:8083/api/images/{id}/download";
                    var imageSource = ImageSource.FromUri(new Uri(url));
                    AdvertImages.Add(imageSource);
                }
                catch { /* игнорируем ошибки */ }
            }

            return Task.CompletedTask;
        }

        public event Func<Task>? CommentsOpened;
        public event Func<Task>? CommentsClosed;

        [RelayCommand]
        private async Task ToggleCommentsAsync()
        {
            if (CommentsVisible)
            {
                CommentsVisible = false;
                ToggleCommentsButtonText = "Показать комментарии";
                AddCommentVisible = false;

                if (CommentsClosed != null) await CommentsClosed.Invoke();
            }
            else
            {
                if (!commentsLoaded)
                {
                    await LoadCommentsAsync();
                    commentsLoaded = true;
                }

                CommentsVisible = true;
                ToggleCommentsButtonText = "Скрыть комментарии";
                AddCommentVisible = true;

                if (CommentsOpened != null) await CommentsOpened.Invoke();
            }
        }

        [RelayCommand]
        private async Task LoadCommentsAsync()
        {
            if (Advert == null) return;

            try
            {
                var request = new GetCommentsByAdvertIdRequest(Page: CommentsPage, PageSize: CommentsPageSize);
                var result = await _commentsClient.GetCommentsByAdvertIdAsync(Advert.Id, request);

                Comments.Clear();
                foreach (var comment in result.Items)
                    Comments.Add(comment);

                CommentsVisible = true;
                AddCommentVisible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки комментариев: {ex.Message}");
            }
        }
    }
}
