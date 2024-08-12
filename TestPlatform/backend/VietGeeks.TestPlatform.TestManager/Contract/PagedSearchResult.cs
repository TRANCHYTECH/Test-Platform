namespace VietGeeks.TestPlatform.TestManager.Contract;

public class PagedSearchResult<T> {
    public IEnumerable<T> Results { get; set; } = default!;
    public long TotalCount { get; set; }
    public int PageCount { get; set; }
}