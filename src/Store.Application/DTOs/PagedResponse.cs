namespace Store.Application.DTOs;

public class PagedResponse<T>
{
    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();
}
