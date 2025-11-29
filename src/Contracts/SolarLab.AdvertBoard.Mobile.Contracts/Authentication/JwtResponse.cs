namespace SolarLab.AdvertBoard.Mobile.Contracts.Authentication
{
    /// <summary>
    /// DTO для ответа с JWT токеном аутентификации.
    /// </summary>
    /// <param name="Token">JWT токен для доступа к защищенным ресурсам.</param>
    public record JwtResponse(string Token);
}
