namespace SolarLab.AdvertBoard.Mobile.Contracts.Authentication
{
    /// <summary>
    /// DTO для запроса аутентификации пользователя.
    /// </summary>
    /// <param name="Email">Email пользователя.</param>
    /// <param name="Password">Пароль пользователя.</param>
    public record LoginUserRequest(string Email, string Password);
}
