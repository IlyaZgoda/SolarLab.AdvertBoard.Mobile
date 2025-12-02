using Microsoft.AspNetCore.WebUtilities;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using SolarLab.AdvertBoard.Mobile.Contracts.Comments;
using System.Net.Http.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class CommentsApiClient(IHttpClientFactory factory) : ICommentsApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<PaginationCollection<CommentItem>> GetCommentsByAdvertIdAsync(Guid advertId, GetCommentsByAdvertIdRequest request)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["Page"] = request.Page.ToString(),
                ["PageSize"] = request.PageSize.ToString(),
            };

            var url = QueryHelpers.AddQueryString($"/api/adverts/{advertId}/comments", parameters);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginationCollection<CommentItem>>();
        }
    }
}