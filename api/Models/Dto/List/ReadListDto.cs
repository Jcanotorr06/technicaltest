using api.Models.Data;

namespace api.Models.Dto.List
{
  /// <summary>
  /// Data Transfer Object for reading a list.
  /// This DTO is used to return list information to the client.
  /// </summary>
  public class ReadListDto
  {
    public ReadListDto()
    { }

    public ReadListDto(ListModel list)
    {
      Id = list.Id;
      Name = list.Name;
      TaskCount = list.Tasks?.Count ?? 0;
    }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TaskCount { get; set; }
  }
}