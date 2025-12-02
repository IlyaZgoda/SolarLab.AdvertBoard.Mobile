using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Images;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class ImageApiClient(IHttpClientFactory factory) : IImageApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task DeleteDraftImageAsync(Guid advertId, Guid imageId, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var url = $"api/adverts/drafts/{advertId}/images/{imageId}";

            var result = await _httpClient.DeleteAsync(url);

            result.EnsureSuccessStatusCode();

            return;
        }

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

        public Task<string> GetUrlForDraftImage(Guid imageId, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var url = $"api/images/{imageId}/download";
            var requestUri = new Uri(_httpClient.BaseAddress!, url);
            return Task.FromResult(requestUri.ToString());
        }

        public async Task<ImageIdResponse> UploadDraftImageAsync(Guid advertId, Stream imageStream, string jwt)
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(imageStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // или определяй по расширению

            // !!! ключевое имя поля — "file"
            content.Add(fileContent, "file", "photo.jpg");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            var url = $"api/adverts/drafts/{advertId}/images";

            var result = await _httpClient.PostAsync(url, content);

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<ImageIdResponse>()
                   ?? throw new InvalidOperationException("Failed to deserialize ImageIdResponse.");
        }

    }
}