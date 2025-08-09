using System.ComponentModel.DataAnnotations;

namespace api.Models.Dto.List
{
  /// <summary>
  /// Data Transfer Object for updating a list.
  /// </summary>
  public class UpdateListDto
  {
    [Required(ErrorMessage = "List ID is required.")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
    public string Name { get; set; } = string.Empty;
  }
}