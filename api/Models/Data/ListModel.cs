using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Data
{
  [Table("List")]
  public class ListModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsPublic { get; set; }

    public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
  }
}