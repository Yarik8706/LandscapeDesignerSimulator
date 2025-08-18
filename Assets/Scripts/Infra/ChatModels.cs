using System;

[Serializable]
public class ChatRequest {
  public string sessionId;
  public string userId;
  public string message;
  public string persona = "Прагматичный собеседник";
  public string clientBrief;
  
  public ChatRequest(string sessionId, string userId, string message, string persona, string clientBrief) {
    this.sessionId = sessionId;
    this.userId = userId;
    this.message = message;
    this.persona = persona;
    this.clientBrief = clientBrief;
  }

  public ChatRequest(string message)
  {
    this.message = message;
  }
}

[Serializable]
public class ProgressData {
  public bool budget;
  public bool deadline;
  public bool weights;
  public bool must;
  public bool bans;
  public bool climate;
  public bool risks;
  public bool bonus;
}

[Serializable]
public class ChatResponse {
  public string reply;
  public bool lowConfidence;
  // public ProgressData progress;
}

