namespace SolarLab.AdvertBoard.Mobile.Contracts.Categories
{
    /// <summary>
    /// DTO для ответа с древовидной структурой категорий.
    /// </summary>
    /// <param name="Categories">Коллекция корневых узлов категорий.</param>
    public record CategoryTreeResponse(IReadOnlyList<CategoryNode> Categories);

    /// <summary>
    /// DTO для узла древовидной структуры категорий.
    /// </summary>
    /// <param name="Id">Идентификатор категории.</param>
    /// <param name="Title">Название категории.</param>
    /// <param name="Children">Коллекция дочерних категорий.</param>
    public record CategoryNode(Guid Id, Guid? ParentId, string Title, List<CategoryNode> Children);
}
