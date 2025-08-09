using api.Models.Data;

namespace api.Models.Dto.Tag
{
  public class ReadTagDto
  {
    public ReadTagDto()
    { }
    public ReadTagDto(TagModel tag)
    {
      Id = tag.Id;
      Name = tag.Name;
      Color = tag.Color;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
  }
}