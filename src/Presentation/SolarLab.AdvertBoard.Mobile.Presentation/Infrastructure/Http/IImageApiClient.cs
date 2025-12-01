using SolarLab.AdvertBoard.Mobile.Contracts.Images;
using System.Net.Http.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public interface IImageApiClient
    {
        Task<ImageResponse> GetImageAsync(Guid imageId);
    }

    public class ImageApiClient(IHttpClientFactory factory) : IImageApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<ImageResponse> GetImageAsync(Guid imageId)
        {
            var url = $"api/images/{imageId}/download";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            // Определяем MIME-тип и имя файла при необходимости
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = $"{imageId}.jpg"; // или получать из headers если сервер присылает Content-Disposition

            return new ImageResponse(contentBytes, contentType, fileName);
        }

    }
}