namespace SolarLab.AdvertBoard.Mobile.Contracts.Comments
{
    /// <summary>
    /// DTO для запроса на обновление комментария.
    /// </summary>
    /// <param name="Text">Новый текст комментария.</param>
    public record UpdateCommentRequest(string Text);
}
