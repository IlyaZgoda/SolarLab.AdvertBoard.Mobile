namespace SolarLab.AdvertBoard.Mobile.Contracts.Links
{
    /// <summary>
    /// DTO для запроса на получение ссылки на подтверждение почты.
    /// </summary>
    /// <param name="UserId">Идентификатор пользователя в системе аутентификации.</param>
    /// <param name="Token">Токен подтверждение почты.</param>
    public record ConfirmationUriRequest(string UserId, string Token);
}
