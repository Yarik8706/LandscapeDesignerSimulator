using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatService<T>
{
  private static readonly Dictionary<string, string> EndpointMap = new Dictionary<string, string>
  {
    { "clientCall", "/api/client-call" },
    { "aiCall", "/api/ai-call" }
  };

  private readonly BackendClient<T> client;

  public static bool isDebug = false;

  public T testData;

  public ChatService(string endpointKey, string baseUrl = BackendClient<T>.DefaultBaseUrl)
  {
    var endpoint = EndpointMap.TryGetValue(endpointKey, out var path) ? path : endpointKey;
    client = new BackendClient<T>(endpoint, baseUrl);
  }

  public Task<T> Send(string req)
  {
    if (isDebug) return Task.FromResult(testData);
    return RetryPolicy.Execute(() => client.Send(req));
  }
}

