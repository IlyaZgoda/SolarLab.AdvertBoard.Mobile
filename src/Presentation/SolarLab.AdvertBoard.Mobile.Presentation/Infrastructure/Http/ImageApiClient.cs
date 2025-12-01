using SolarLab.AdvertBoard.Mobile.Contracts.Images;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class ImageApiClient(IHttpClientFactory factory) : IImageApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<ImageResponse> GetImageAsync(Guid imageId)
        {
            var url = $"api/images/{imageId}/download";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = $"{imageId}.jpg"; 

            return new ImageResponse(contentBytes, contentType, fileName);
        }

    }
}