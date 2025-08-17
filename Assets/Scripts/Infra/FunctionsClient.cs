using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;

public class FunctionsClient {
  private readonly FirebaseFunctions functions;
  private readonly string functionName;

  public FunctionsClient(string region, string functionName) {
    this.functions = FirebaseFunctions.GetInstance(region);
    this.functionName = functionName;
  }

  public async Task<ChatResponse> Chat(ChatRequest req) {
    var callable = functions.GetHttpsCallable(functionName);
    var data = new Dictionary<string, object> {
      {"sessionId", req.sessionId},
      {"userId", req.userId},
      {"message", req.message},
      {"persona", req.persona},
      {"clientBrief", req.clientBrief}
    };
    var result = await callable.CallAsync(data);
    return JsonUtility.FromJson<ChatResponse>(result.Data.ToString());
  }
}

