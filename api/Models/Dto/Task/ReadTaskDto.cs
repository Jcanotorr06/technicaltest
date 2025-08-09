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
      Status = task._Status.StatusName;
      StatusId = task.Status;
      ListId = task.ListId;
      CreatedBy = task.CreatedBy;
      AssignedTo = task.AssignedTo;
      Tags = task.Tags.Select(tag => new ReadTagDto(tag)).ToList();
    }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public DateTime? DueDate { get; set; } = null;
    public string Status { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public Guid ListId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public List<ReadTagDto> Tags { get; set; } = new List<ReadTagDto>();
  }
}