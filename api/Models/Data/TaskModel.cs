using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models.Data
{
  [Table("Tasks")]
  public class TaskModel
  {
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain alphanumeric characters and spaces.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?]+$", ErrorMessage = "Description can only contain alphanumeric characters, spaces, and punctuation.")]
#nullable enable
    public string? Description { get; set; } = null;
    public DateTime? DueDate { get; set; } = null;
#nullable disable
    public int Status { get; set; }
    public Guid ListId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public int Order { get; set; }
    public TaskStatusModel _Status { get; set; }
    public ListModel _List { get; set; }
    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
  }
}