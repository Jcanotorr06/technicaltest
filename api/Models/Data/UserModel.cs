namespace api.Models.Data
{
  public class UserModel
  {
    public UserModel() { }
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = "User";
    public string Email { get; set; } = "user@example.com";
  }
}