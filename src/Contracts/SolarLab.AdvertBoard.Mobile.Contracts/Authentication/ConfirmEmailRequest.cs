using System.Windows.Input;

namespace SolarLab.AdvertBoard.Mobile.Contracts.Authentication
{
    /// <summary>
    /// DTO для запроса подтверждения email пользователя.
    /// </summary>
    /// <param name="UserId">Идентификатор пользователя.</param>
    /// <param name="EncodedToken">Закодированный токен подтверждения email.</param>

    public record ConfirmEmailRequest(string UserId, string EncodedToken);
}
