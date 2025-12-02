using SolarLab.AdvertBoard.Mobile.Contracts.Authentication;
using SolarLab.AdvertBoard.Mobile.Contracts.Images;
using SolarLab.AdvertBoard.Mobile.Contracts.Users;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public interface IImageApiClient
    {
        Task<ImageResponse> GetImageAsync(Guid imageId);
        Task<string> GetUrlForDraftImage(Guid imageId, string jwt);
        Task<ImageIdResponse> UploadDraftImageAsync(Guid advertId, Stream imageStream, string jwt);
    }

    public interface IUsersApiClient
    {
        Task<JwtResponse> LoginAsync(LoginUserRequest request);
        Task<UserResponse> GetUserByIdentityIdAsync(string jwt);
    }

    public class UsersApiClient(IHttpClientFactory factory) : IUsersApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<UserResponse> GetUserByIdentityIdAsync(string jwt)
        {
            var url = $"/api/users/me";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();

            return user ?? throw new InvalidOperationException("User is null");
        }

        public async Task<JwtResponse> LoginAsync(LoginUserRequest request)
        {
            var url = $"/api/users/login";
            var response = await _httpClient.PostAsJsonAsync(url, request);
            if (response.IsSuccessStatusCode)
            {
                // Успешный ответ
                var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                return jwt ?? throw new InvalidOperationException("Token is null");
            }
            else
            {
                // Ошибка — десериализуем ProblemDetails
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                var message = problem?.Detail ?? "Ошибка аутентификации";
                throw new InvalidOperationException(message);
            }
        }
    }

    public record ProblemDetails(
      string? Type,
      string? Title,
      int? Status,
      string? Detail,
      string? Instance
  );

    public interface IAuthService
    {
        string? Jwt { get; set; }
        bool IsAuthenticated { get; }
    }

    public class AuthService : IAuthService, INotifyPropertyChanged
    {
        private string? _jwt;

        public string? Jwt
        {
            get => _jwt;
            set
            {
                if (_jwt != value)
                {
                    _jwt = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAuthenticated)); 
                }
            }
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Jwt);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}