using Microsoft.AspNetCore.WebUtilities;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class AdvertApiClient(IHttpClientFactory factory) : IAdvertApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<PublishedAdvertDetailsResponse> GetAdvertDetailsAsync(Guid id)
        {
            var url = $"/api/adverts/{id}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PublishedAdvertDetailsResponse>();
        }

        public async Task<AdvertDraftDetailsResponse> GetDraftDetailsAsync(Guid id, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var url = $"/api/adverts/drafts/{id}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AdvertDraftDetailsResponse>();
        }

        public async Task<PaginationCollection<PublishedAdvertItem>> GetPublishedAsync(AdvertFilterRequest filter)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["Page"] = filter.Page.ToString(),
                ["PageSize"] = filter.PageSize.ToString(),
                ["CategoryId"] = filter.CategoryId.ToString(),
                ["MinPrice"] = filter.MinPrice.ToString(),
                ["MaxPrice"] = filter.MaxPrice.ToString(),
                ["SortBy"] = filter.SortBy,
                ["SortDescending"] = filter.SortDescending.ToString(),
                ["SearchText"] = filter.SearchText,
            };

            var url = QueryHelpers.AddQueryString("/api/adverts", parameters);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginationCollection<PublishedAdvertItem>>();
        }

        public async Task<PaginationCollection<AdvertDraftItem>> GetUserDraftsAsync(GetUserAdvertDraftsRequest request, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var parameters = new Dictionary<string, string?>
            {
                ["Page"] = request.Page.ToString(),
                ["PageSize"] = request.PageSize.ToString(),
            };

            var url = QueryHelpers.AddQueryString("/api/adverts/my/drafts", parameters);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginationCollection<AdvertDraftItem>>();
        }

        public async Task<PaginationCollection<PublishedAdvertItem>> GetUserPublishedAdvertsAsync(
            GetUserPublishedAdvertsRequest request, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var parameters = new Dictionary<string, string?>
            {
                ["Page"] = request.Page.ToString(),
                ["PageSize"] = request.PageSize.ToString(),
            };

            var url = QueryHelpers.AddQueryString("/api/adverts/my/published", parameters);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginationCollection<PublishedAdvertItem>>();
        }

        public async Task UpdateDraftAsync(Guid advertId, UpdateAdvertDraftRequest request, string jwt)
        {
            // Новый маршрут POST для обхода проблем PATCH
            var url = $"/drafts/{advertId}/update";

            try
            {
                // Сериализация JSON с camelCase, все поля включены
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                    WriteIndented = false
                });

                // Логируем JSON
                System.Diagnostics.Debug.WriteLine($"POST /draft/update request JSON: {json}");

                using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                // Отправляем POST-запрос
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"POST /draft/update succeeded for advert {advertId}");
                    return;
                }
                else
                {
                    // Попытка десериализовать ProblemDetails, если есть
                    ProblemDetails? problem = null;
                    try
                    {
                        problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                    }
                    catch { }

                    var messageText = problem?.Detail ?? $"Ошибка сервера: {response.StatusCode}";
                    System.Diagnostics.Debug.WriteLine($"POST /draft/update failed: {messageText}");
                    throw new InvalidOperationException(messageText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateDraftAsync: {ex}");
                throw;
            }
        }




    }
}