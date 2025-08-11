using api.Models.Data;
using api.Models.Dto.Tag;

namespace api.Models.Dto.Task
{
  public class ReadTaskDto
  {
    public ReadTaskDto()
    { }
    public ReadTaskDto(TaskModel task)
    {
      Id = task.Id;
      Title = task.Title;
      Description = task.Description;
      DueDate = task.DueDate;
      Status = task._Status?.StatusName ?? "Pending";
      StatusId = task.Status;
      ListId = task.ListId;
      ListName = task._List?.Name ?? string.Empty;
      CreatedBy = task.CreatedBy;
      AssignedTo = task.AssignedTo;
      Order = task.Order;
      Tags = task.Tags?.Select(tag => new ReadTagDto(tag)).ToList() ?? new List<ReadTagDto>();
    }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public DateTime? DueDate { get; set; } = null;
    public string Status { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public Guid ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<ReadTagDto> Tags { get; set; } = new List<ReadTagDto>();
  }
}