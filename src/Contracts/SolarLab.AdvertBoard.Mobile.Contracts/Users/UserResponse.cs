namespace SolarLab.AdvertBoard.Mobile.Contracts.Users
{
    public record UserResponse(Guid Id, string FirstName, string LastName, string? MiddleName, string? ContactEmail, string? PhoneNumber);
}
