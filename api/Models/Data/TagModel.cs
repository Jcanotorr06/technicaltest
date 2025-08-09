using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Data
{
  [Table("Tags")]
  public class TagModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;

    public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
  }
}