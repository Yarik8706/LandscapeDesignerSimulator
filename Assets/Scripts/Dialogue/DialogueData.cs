namespace Infra
{
    public enum DialogueRole
    {
        Player,
        Client,
        System
    }
    
    public class DialogueData
    {
        public string DialogueText { get; private set; }
        
        public void AppendMessage(DialogueRole role, string message) => AppendMessage(role.ToString(), message);
        private void AppendMessage(string role, string message) => DialogueText += $"{role}: {message}\n";   
        public static string GetDialoguePart(DialogueRole role, string message) => $"{role}: {message}\n";
        public void Copy(DialogueData other) => DialogueText = other.DialogueText;
    }
}