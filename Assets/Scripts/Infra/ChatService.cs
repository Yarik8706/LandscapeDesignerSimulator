using System.Threading.Tasks;

public class ChatService {
  private readonly FunctionsClient client;

  public ChatService(FunctionsClient client) {
    this.client = client;
  }

  public Task<ChatResponse> Send(ChatRequest req) {
    // return RetryPolicy.Execute(() => client.Chat(req));
    return client.Chat(req);
  }
}

