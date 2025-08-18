using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
  [SerializeField] private ProgressTracker tracker;
  [SerializeField] private TextMeshProUGUI statusText;
  [SerializeField] private TMP_Text assistantBubblePrefab;
  [SerializeField] private TMP_InputField inputField;
  [SerializeField] private Transform _messageContainer;
  [SerializeField] private Sprite[] loadSprites;
  [SerializeField] private Image loadImage;
  
  private int _currentSpriteIndex = 0;
  private float _animationTimer = 0f;
  private const float FRAME_RATE = 0.1f;
  private bool _isLoading = false;
  
  private string _currentMessage;
  private ChatService chatService;

  private void Start()
  {
    if (inputField != null)
    {
      inputField.onValueChanged.AddListener(OnInputChanged);
    }

    FirebaseInitializer.OnInitialized.AddListener(async () =>
    {
      chatService = new ChatService(new FunctionsClient("us-central1", "aiCall"));
      var res = await chatService.Send(new ChatRequest("Привет!"));
      AppendBubble(assistantBubblePrefab, res.reply);
    });
  }

  private void Update()
  {
    if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
        ) && _currentMessage.Length > 0)
    {
      Send();
    }
    
    if (_isLoading)
    {
      _animationTimer += Time.deltaTime;
      if (_animationTimer >= FRAME_RATE)
      {
        _animationTimer = 0f;
        UpdateLoadingAnimation();
      }
    }
  }
  
  private void UpdateLoadingAnimation()
  {
    if (loadImage == null || loadSprites == null || loadSprites.Length == 0) return;
    
    _currentSpriteIndex = (_currentSpriteIndex + 1) % loadSprites.Length;
    loadImage.sprite = loadSprites[_currentSpriteIndex];
  }
  
  private void OnInputChanged(string newValue) {
    _currentMessage = newValue;
  }
  
  public async void Send() {
    if (string.IsNullOrEmpty(_currentMessage)) return;
    
    // Start loading
    StartLoading();
    
    AppendBubble(assistantBubblePrefab, _currentMessage, true);
    
    try {
      var req = new ChatRequest(_currentMessage);
      var resp = await chatService.Send(req);
      AppendBubble(assistantBubblePrefab, resp.reply);
      // tracker?.UpdateFromResponse(resp.progress);
      if (resp.lowConfidence) statusText.text = "⚠ Низкая уверенность ответа";
    }
    finally {
      // Stop loading in any case (success or error)
      StopLoading();
    }
    
    if (inputField != null) {
      inputField.text = string.Empty;
      _currentMessage = string.Empty;
    }
  }

  private void AppendBubble(TMP_Text prefab, string text, bool isPlayer = false)
  {
    var bubble = Instantiate(prefab, _messageContainer);
    bubble.text = text;
    bubble.color = isPlayer ? new Color(0.4f, 0.8f, 1f) : Color.white;
  }
  
  private void StartLoading()
  {
    _isLoading = true;
    _currentSpriteIndex = 0;
    _animationTimer = 0f;
    
    if (loadImage != null)
    {
      loadImage.gameObject.SetActive(true);
      if (loadSprites != null && loadSprites.Length > 0)
      {
        loadImage.sprite = loadSprites[0];
      }
    }
  }
  
  private void StopLoading()
  {
    _isLoading = false;
    
    if (loadImage != null)
    {
      loadImage.gameObject.SetActive(false);
    }
  }
}

