namespace SolarLab.AdvertBoard.Mobile.Contracts.Users
{
    /// <summary>
    /// DTO для получения контактной информации пользователя.
    /// </summary>
    /// <param name="FullName">Полное имя пользователя.</param>
    /// <param name="ContactEmail">Контактная почта пользователя.</param>
    /// <param name="PhoneNumber">Контактный номер телефона пользователя.</param>
    public record UserContactInfoResponse(string FullName, string ContactEmail, string? PhoneNumber);
}
