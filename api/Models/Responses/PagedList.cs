using Microsoft.EntityFrameworkCore;

namespace api.Models.Responses
{
  public class PagedList<T> where T : class
  {
    public PagedList(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
      CurrentPage = currentPage;
      PageSize = pageSize;
      TotalCount = totalCount;
      Items.AddRange(items);
    }
    public PagedList()
    {
      Items = new List<T>();
    }
    public PagedList(IEnumerable<T> items)
    {
      Items = items.ToList();
    }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Items { get; } = new List<T>();

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
      var totalCount = await source.CountAsync(cancellationToken);
      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

      return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
  }
}