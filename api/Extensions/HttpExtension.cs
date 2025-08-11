using System.Globalization;
using api.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;

namespace api.Extensions
{
  public static class HttpExtension
  {
    public static T Get<T>(this IQueryCollection collection, string key)
    {
      var value = default(T);

      if (collection.TryGetValue(key, out var result))
      {
        try
        {
          string valueToConvert = result.ToString();
          value = (T)Convert.ChangeType(valueToConvert, typeof(T), CultureInfo.InvariantCulture);
        }
        catch (System.Exception)
        {
        }
      }

      return value;
    }
    public static SortPagination GetSortPagination(this IQueryCollection collection)
    {
      var limit = collection.Get<int>("limit");
      var offset = collection.Get<int>("offset");
      var sortBy = collection.Get<string>("sortBy");
      var sortOrder = collection.Get<string>("sortOrder");

      return new SortPagination
      {
        Limit = limit == 0 ? 10 : limit,
        Offset = offset == 0 ? 0 : offset,
        SortBy = sortBy.IsNullOrDefault() ? "Id" : sortBy,
        SortOrder = sortOrder.IsNullOrDefault() ? "asc" : sortOrder
      };
    }
  }
}