namespace CareWorkOps.Web.Infrastructure.Api;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];

    public MetaData MetaData { get; set; } = new();
}