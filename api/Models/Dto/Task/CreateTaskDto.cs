using System.ComponentModel.DataAnnotations;

namespace api.Models.Dto.Task
{
  public class CreateTaskDto
  {
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
    public string AssignedTo { get; set; } = string.Empty;

  }
}