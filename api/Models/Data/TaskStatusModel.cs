using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models.Data
{
  [Table("TaskStatus")]
  public class TaskStatusModel
  {
    public int Id { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [JsonIgnore]
    public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
  }
}