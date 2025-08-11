using api.Models.Data;

namespace api.Models.Dto.User
{
  public class ReadUserDto
  {
    public ReadUserDto()
    { }
    public ReadUserDto(UserModel user)
    {
      Name = user.Name;
      Email = user.Email;
    }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
  }
}