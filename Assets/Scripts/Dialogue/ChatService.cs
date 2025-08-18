using System.Threading.Tasks;

public class ChatService<T> {
  private readonly FunctionsClient<T> client;
  
  public static bool isDebug = false;

  public T testData;

  public ChatService(string functionName, string region = "us-central1") {
    this.client = new FunctionsClient<T>(region, functionName);
  }

  public Task<T> Send(string req) {
    if (isDebug) return Task.FromResult(testData);
    return RetryPolicy.Execute(() => client.Chat(req));
  }
}

