namespace SolarLab.AdvertBoard.Mobile.Contracts.Comments
{
    /// <summary>
    /// DTO для детального представления комментария.
    /// </summary>
    /// <param name="Id">Идентификатор комментария.</param>
    /// <param name="AdvertId">Идентификатор объявления, к которому относится комментарий.</param>
    /// <param name="AuthorId">Идентификатор автора комментария.</param>
    /// <param name="Text">Текст комментария.</param>
    /// <param name="CreatedAt">Дата и время создания комментария.</param>
    /// <param name="UpdatedAt">Дата и время последнего обновления комментария (null если не редактировался).</param>
    public record CommentResponse(Guid Id, Guid AdvertId, Guid AuthorId, string Text, DateTime CreatedAt, DateTime? UpdatedAt);
}
