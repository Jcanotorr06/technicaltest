namespace api.Models.Requests
{
  public class SortPagination
  {
    public SortPagination()
    { }
    public SortPagination(int limit, int offset, string sortBy, string sortOrder)
    {
      Limit = limit > 0 ? limit : 10;
      Offset = offset >= 0 ? offset : 0;
      SortBy = !string.IsNullOrWhiteSpace(sortBy) ? sortBy : string.Empty;
      SortOrder = sortOrder.ToLower() == "desc" ? "desc" : "asc";
    }

    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Page => Limit > 0 ? (Offset / Limit) + 1 : 1;
    public string SortBy { get; set; } = string.Empty;
    public string SortOrder { get; set; } = "asc";
  }
}