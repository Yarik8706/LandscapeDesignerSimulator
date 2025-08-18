using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Functions;
using Google.MiniJSON;
using UnityEngine;

public class FunctionsClient<T> {
  private readonly FirebaseFunctions functions;
  private readonly string functionName;

  public FunctionsClient(string region, string functionName) {
    this.functions = FirebaseFunctions.GetInstance(region);
    this.functionName = functionName;
  }

  public async Task<T> Chat(string req) {
    var callable = functions.GetHttpsCallable(functionName);
    var result = await callable.CallAsync(req);
    
    string json = Json.Serialize(result.Data);
    return JsonUtility.FromJson<T>(json);
  }
}

