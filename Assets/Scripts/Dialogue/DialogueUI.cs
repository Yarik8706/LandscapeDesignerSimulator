using DefaultNamespace;
using Infra;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
  [SerializeField] private ClientsData clientsData;
  [SerializeField] private TMP_Text assistantBubblePrefab;
  [SerializeField] private TMP_InputField inputField;
  [SerializeField] private Transform _messageContainer;
  [SerializeField] private Sprite[] loadSprites;
  [SerializeField] private Image loadImage;
  
  [SerializeField] private TMP_Text clientName;
  [SerializeField] private Image clientImage;
  [SerializeField] private TMP_Text tracker;
  [SerializeField] private TMP_Text gottenPlayerTasks;
  [SerializeField] private TMP_Text canAskCountText;
  
  private int _currentSpriteIndex = 0;
  private float _animationTimer = 0f;
  private const float FRAME_RATE = 0.1f;
  private bool _isLoading = false;
  
  private int _currentAskCount = 0;
  
  private string _currentMessage;
  private ChatService<ChatResponse> _clientChatService;
  private ChatService<string> _simpleChatService;
  
  private DialogueData _currentDialogue = new DialogueData();
  
  private GameData _gameData => GameDataManager.Instance.gameData;

  private void Awake()
  {
    GameDataManager.OnGameStageChanged.AddListener(OnGameStageChanged);
    FirebaseInitializer.OnInitialized.AddListener(() =>
    {
      _clientChatService = new ChatService<ChatResponse>("clientCall");
      _clientChatService.testData = new ChatResponse() { reply = "Test reply message from server response", learnedSummary = "0/0", learnedText = "Test reply message from server response" };
      _simpleChatService = new ChatService<string>("aiCall");
      _simpleChatService.testData = "Test reply message from server response";
      ChatService<ChatResponse>.isDebug = true;
      ChatService<string>.isDebug = true;
    });
  }

  private void Start()
  {
    if (inputField != null)
    {
      inputField.onValueChanged.AddListener(OnInputChanged);
    }
  }
  
  private void OnGameStageChanged() {
    var stage = GameDataManager.Instance.gameData.stage;

    if (stage == GameStage.FirstDialog)
    {
      _currentAskCount = _gameData.firstDialogQuestionCount;
      _currentDialogue = new DialogueData();
      clientName.text = clientsData.clientsNames[_gameData.activeClientIndex]; 
      clientImage.sprite = clientsData.clientsAvatars[_gameData.activeClientIndex];
      SetClientContext(clientsData.personas[_gameData.activeClientIndex]);
      SetClientContext(clientsData.clientsBriefs[_gameData.activeClientBrief]);
      SetClientContext(clientsData.generalContext);
      SystemSendMessage(clientsData.firstSystemMessage);
    }
  }

  private void Update()
  {
    if(GameDataManager.Instance.gameData.stage != GameStage.FinalDialog
       && GameDataManager.Instance.gameData.stage != GameStage.FirstDialog) return;
    if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
        ) && _currentMessage.Length > 0)
    {
      PlayerSendMessage();
    }
    loadImage.transform.parent.gameObject.SetActive(_isLoading);
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
  
  public async void PlayerSendMessage() {
    if (string.IsNullOrEmpty(_currentMessage) || _currentAskCount <= 0) return;
    
    StartLoading();
    
    AppendBubble(assistantBubblePrefab, _currentMessage, true);
    
    try
    {
      _currentAskCount--;
      var req = new ChatRequest(_currentMessage, _currentDialogue);
      var resp = await _clientChatService.Send(req.message);
      AppendBubble(assistantBubblePrefab, resp.reply);
      RecheckTracker(resp.learnedSummary);
      RecheckGottenTasks(resp.learnedText);
      canAskCountText.text = "Вопросов еще можно спросить: " + _currentAskCount;
    }
    finally 
    {
      StopLoading();
    }
    
    if (inputField != null) {
      inputField.text = string.Empty;
      _currentMessage = string.Empty;
    }
  }

  public void SetClientContext(string context)
  {
    _currentDialogue.AppendMessage(DialogueRole.Client, context.Trim() + "\n");
  }
  
  public async void SystemSendMessage(string message) {
    StartLoading();
    
    try {
      var req = new ChatRequest(message, _currentDialogue, DialogueRole.System);
      var resp = await _clientChatService.Send(req.message);
      AppendBubble(assistantBubblePrefab, resp.reply);
      RecheckTracker(resp.learnedSummary);
      RecheckGottenTasks(resp.learnedText);
    }
    finally {
      StopLoading();
    }
  }
  
  private void RecheckTracker(string req) {
    tracker.text = "Количество требований клиент вам рассказал: " + req;
  }

  private void RecheckGottenTasks(string req)
  {
    gottenPlayerTasks.text = "Список требований, который вы узнали: \n" + req;
  }
  

  private void AppendBubble(TMP_Text prefab, string text, bool isPlayer = false)
  {
    _currentDialogue.AppendMessage(isPlayer ? DialogueRole.Player : DialogueRole.Client, text);
    
    var bubble = Instantiate(prefab, _messageContainer);
    bubble.text = text;
    bubble.color = isPlayer ? new Color(0.4f, 0.8f, 1f) : Color.white;
    bubble.alignment = isPlayer ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
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

