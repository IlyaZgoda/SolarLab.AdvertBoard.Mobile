namespace SolarLab.AdvertBoard.Mobile.Contracts.Comments
{
    /// <summary>
    /// DTO для запроса на создания комментария.
    /// </summary>
    /// <param name="Text">Текст комментария.</param>
    public record CreateCommentRequest(string Text);
}
