using System.Net;
using Newtonsoft.Json;

namespace api.Models.Messages
{
  public class InfoMessage
  {
    public InfoMessage(string title = "An error has ocurred", string message = "", HttpStatusCode status = HttpStatusCode.OK, string traceId = "")
    {
      Title = title;
      Message = message;
      Status = (int)status;
      TraceId = traceId;
    }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("traceId")]
    public string TraceId { get; set; }
  }
}