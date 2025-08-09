using System.ComponentModel.DataAnnotations;

namespace api.Models.Dto.List
{
  /// <summary>
  /// Data Transfer Object for creating a list.
  /// This DTO is used to receive data from the client when creating a new list.
  /// </summary>
  public class CreateListDto
  {
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
    public string Name { get; set; } = string.Empty;
  }
}