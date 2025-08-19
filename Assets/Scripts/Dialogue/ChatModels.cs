using System;
using Infra;

[Serializable]
public class ChatRequest {
  public string message;

  public ChatRequest(string message, DialogueData context = null, DialogueRole role = DialogueRole.Player)
  {
    if (context == null) this.message = message;
    else
    {
      this.message = context.DialogueText + DialogueData.GetDialoguePart(role, message);
    }
  }
}

[Serializable]
public class ChatResponse {
  public string reply;
  public string learnedSummary;
  public string learnedText;
  public int incorrectMessages;
}


// reply: z.string(),
// learnedSummary: z.string(),     // "X/Y"
// learnedText: z.string()  
