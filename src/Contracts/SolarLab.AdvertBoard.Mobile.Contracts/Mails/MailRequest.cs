namespace SolarLab.AdvertBoard.Mobile.Contracts.Mails
{
    /// <summary>
    /// DTO для запроса на отправку почты.
    /// </summary>
    /// <param name="To">Кому.</param>
    /// <param name="Subject">От кого.</param>
    /// <param name="Body">Сообщение.</param>
    public record MailRequest(string To, string Subject, string Body);
}
