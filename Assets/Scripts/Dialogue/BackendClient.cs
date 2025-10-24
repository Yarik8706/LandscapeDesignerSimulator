using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BackendClient<T>
{
  public const string DefaultBaseUrl = "https://landscape-designer-simulator-nextjs.vercel.app";

  private readonly string baseUrl;
  private readonly string endpoint;

  public BackendClient(string endpoint, string baseUrl = DefaultBaseUrl)
  {
    if (string.IsNullOrEmpty(endpoint))
    {
      throw new ArgumentException("Не указан путь до эндпоинта", nameof(endpoint));
    }

    this.baseUrl = string.IsNullOrEmpty(baseUrl) ? DefaultBaseUrl : baseUrl.TrimEnd('/');
    this.endpoint = endpoint.StartsWith("/") ? endpoint : $"/{endpoint}";
  }

  public async Task<T> Send(string message)
  {
    var payload = JsonUtility.ToJson(new BackendRequest { message = message });
    using var request = new UnityWebRequest($"{baseUrl}{endpoint}", UnityWebRequest.kHttpVerbPOST);
    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    var operation = request.SendWebRequest();
    while (!operation.isDone)
    {
      await Task.Yield();
    }

#if UNITY_2020_2_OR_NEWER
    if (request.result != UnityWebRequest.Result.Success)
#else
    if (request.isNetworkError || request.isHttpError)
#endif
    {
      var errorText = string.IsNullOrEmpty(request.downloadHandler.text)
        ? request.error
        : $"{request.error}\n{request.downloadHandler.text}";
      throw new Exception($"Ошибка запроса к серверу: {errorText}");
    }

    return JsonUtility.FromJson<T>(request.downloadHandler.text);
  }

  [Serializable]
  private struct BackendRequest
  {
    public string message;
  }
}

