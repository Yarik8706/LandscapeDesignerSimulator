using System.Threading.Tasks;

public class ChatService<T> {
  private readonly FunctionsClient<T> client;

  public ChatService(string functionName, string region = "us-central1") {
    this.client = new FunctionsClient<T>(region, functionName);
  }

  public Task<T> Send(string req) {
    return RetryPolicy.Execute(() => client.Chat(req));
  }
}

