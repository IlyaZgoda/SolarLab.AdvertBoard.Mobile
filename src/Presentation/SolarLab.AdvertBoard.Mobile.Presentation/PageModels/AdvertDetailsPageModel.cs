using Android.Graphics;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{

    public partial class AdvertDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _advertClient;
        private readonly IImageApiClient _imageClient;

        public AdvertDetailsPageModel(IAdvertApiClient advertClient, IImageApiClient imageClient)
        {
            _advertClient = advertClient;
            _imageClient = imageClient;
        }

        [ObservableProperty]
        private PublishedAdvertDetailsResponse? advert;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) && idObj is string idStr && Guid.TryParse(idStr, out var id))
            {
                // Запускаем асинхронно, но без async void
                _ = LoadAdvertAsync(id);
            }
        }

        private async Task LoadAdvertAsync(Guid id)
        {
            Advert = await _advertClient.GetAdvertDetailsAsync(id);
        }

        [ObservableProperty]
        private ObservableCollection<ImageSource> advertImages = new();


        private async Task LoadImagesAsync()
        {

            if (Advert?.ImageIds == null || Advert.ImageIds.Count == 0)
            {
                Console.WriteLine("Advert has no images.");
                return;
            }

            Console.WriteLine($"Loading {Advert.ImageIds.Count} images...");

            AdvertImages.Clear();

            foreach (var id in Advert.ImageIds)
            {
                try
                {
                    Console.WriteLine($"Requesting image {id}...");
                    var imgResponse = await _imageClient.GetImageAsync(id);

                    var info = SKBitmap.DecodeBounds(imgResponse.Content);

                    System.Diagnostics.Debug.WriteLine($"IMAGE SIZE: {info.Width}x{info.Height}");
                    if (imgResponse?.Content == null || imgResponse.Content.Length == 0)
                    {
                        Console.WriteLine($"Image {id} returned empty content.");
                        continue;
                    }

                    Console.WriteLine($"Image {id} downloaded, {imgResponse.Content.Length} bytes. Adding to collection...");
                    var bytes = imgResponse.Content;
                    var header = BitConverter.ToString(bytes.Take(20).ToArray());
                    System.Diagnostics.Debug.WriteLine($"IMAGE HEADER: {header}");

                    var imageSource = ConvertToImageSource(imgResponse.Content);

                    // Обновление коллекции на UI потоке
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AdvertImages.Add(imageSource);
                        Console.WriteLine($"Image {id} added to AdvertImages.");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image {id}: {ex}");
                }
            }

            Console.WriteLine("Finished loading images.");
        }

        private ImageSource ConvertToImageSource(byte[] bytes)
        {
            // Создаём копию массива для каждого запроса потока
            return ImageSource.FromStream(() => new MemoryStream((byte[])bytes.Clone()));
        }





        partial void OnAdvertChanged(PublishedAdvertDetailsResponse? value)
        {
            if (value == null) return;
            _ = LoadImagesAsync(); // подгружаем изображения после того, как объявление загружено
        }


    }


}
