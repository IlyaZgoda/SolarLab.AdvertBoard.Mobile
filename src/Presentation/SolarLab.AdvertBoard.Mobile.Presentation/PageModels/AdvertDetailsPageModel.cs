using CommunityToolkit.Mvvm.ComponentModel;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System.Collections.ObjectModel;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class AdvertDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _advertClient;

        public AdvertDetailsPageModel(IAdvertApiClient advertClient)
        {
            _advertClient = advertClient;
            AdvertImages = new ObservableCollection<ImageSource>();
        }

        [ObservableProperty]
        private PublishedAdvertDetailsResponse? advert;

        [ObservableProperty]
        private ObservableCollection<ImageSource> advertImages;

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
                    // Формируем рабочий URL
                    var url = $"http://10.0.2.2:8083/api/images/{id}/download";

                    // Создаем ImageSource из URL
                    var imageSource = ImageSource.FromUri(new Uri(url));
                    AdvertImages.Add(imageSource);

                    Console.WriteLine($"Added image from URL: {url}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add image {id} from URL: {ex}");
                }
            }

            Console.WriteLine($"Total images added: {AdvertImages.Count}");
            return Task.CompletedTask;
        }
    }
}
