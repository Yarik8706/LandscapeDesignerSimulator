using System;

[Serializable]
public class ChatRequest {
  public string sessionId;
  public string userId;
  public string message;
  public string persona;
  public string clientBrief;
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
  public ProgressData progress;
}

