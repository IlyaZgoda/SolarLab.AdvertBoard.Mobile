namespace SolarLab.AdvertBoard.Mobile.Contracts.Authentication
{
    /// <summary>
    /// DTO для запроса регистрации нового пользователя.
    /// </summary>
    /// <param name="FirstName">Имя пользователя.</param>
    /// <param name="LastName">Фамилия пользователя.</param>
    /// <param name="MiddleName">Отчество пользователя (опционально).</param>
    /// <param name="Email">Email для входа в систему.</param>
    /// <param name="ContactEmail">Контактный email (опционально, если отличается от основного).</param>
    /// <param name="Password">Пароль пользователя.</param>
    /// <param name="PhoneNumber">Номер телефона пользователя (опционально).</param>
    public record RegisterUserRequest(
        string FirstName, 
        string LastName, 
        string? MiddleName, 
        string Email, 
        string? ContactEmail,
        string Password, 
        string? PhoneNumber);
}
