using DefaultNamespace;
using Firebase.Functions;
using Infra;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
  [SerializeField] private ClientsData clientsData;
  [SerializeField] private TMP_Text gameMessagePrefab;
  [SerializeField] private TMP_Text playerMessagePrefab;
  [SerializeField] private TMP_InputField inputField;
  [SerializeField] private Transform _messageContainer;
  [SerializeField] private Sprite[] loadSprites;
  [SerializeField] private Image loadImage;
  [SerializeField] private ScrollRect scrollRect; 
  
  [SerializeField] private TMP_Text clientName;
  [SerializeField] private Image clientImage;
  [SerializeField] private TMP_Text tracker;
  [SerializeField] private TMP_Text gottenPlayerTasks;
  [SerializeField] private TMP_Text canAskCountText;
  [SerializeField] private bool isDebugMode = false;
  
  public static DialogueUI Instance { get; private set; }
  
  private int _currentSpriteIndex = 0;
  private float _animationTimer = 0f;
  private const float FRAME_RATE = 0.1f;
  private bool _isLoading = false;

  private int _currentAskCount = 0;
  private int _incorrectMessageCount = 0;
  
  private string _currentMessage;
  private ChatService<ChatResponse> _clientChatService;
  private ChatService<string> _simpleChatService;
  
  private DialogueData _currentDialogue = new DialogueData();
  
  private GameData _gameData => GameDataManager.Instance.gameData;

  private void Awake()
  {
    Instance = this;
    GameDataManager.OnGameStageChanged.AddListener(OnGameStageChanged);
    // FirebaseFunctions.DefaultInstance.UseFunctionsEmulator("http://localhost:5001");
    FirebaseInitializer.OnInitialized.AddListener(() =>
    {
      _clientChatService = new ChatService<ChatResponse>("clientCall");
      _clientChatService.testData = new ChatResponse() { reply = "Test reply message from server response", learnedSummary = "0/0", learnedText = "Test reply message from server response" };
      _simpleChatService = new ChatService<string>("aiCall");
      _simpleChatService.testData = "Test reply message from server response";

#if UNITY_EDITOR
      ChatService<ChatResponse>.isDebug = isDebugMode;
      ChatService<string>.isDebug = isDebugMode;
#endif
     
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

    tracker.gameObject.SetActive(true);
    canAskCountText.gameObject.SetActive(true);
    if (stage == GameStage.FirstDialog)
    {
      ClearMessages();
      _currentAskCount = _gameData.firstDialogQuestionCount;
      _currentDialogue = new DialogueData();
      clientName.text = clientsData.clientsNames[_gameData.activeClientBrief]; 
      clientImage.sprite = clientsData.clientsAvatars[_gameData.activeClientBrief];
      SetClientContext(clientsData.personas[_gameData.activeClientBrief]);
      SetClientContext("Твое имя: " + clientsData.clientsNames[_gameData.activeClientBrief]);
      SetClientContext(clientsData.clientsBriefs[_gameData.activeClientBrief]);
      SetClientContext(clientsData.generalContext);
      SystemSendMessage(clientsData.firstSystemMessage);
    }
    else if (stage == GameStage.SecondDialog)
    {
      _currentAskCount = _gameData.secondDialogQuestionCount;
      SystemSendMessage(clientsData.secondAITask + "\n" + GetPlayerProjectData());
    }
    else if (stage == GameStage.FinalDialog)
    {
      _currentAskCount = 4;
      SystemSendMessage(clientsData.finalAITask + "\n" + GetPlayerProjectData());
    }
    else
    {
      tracker.gameObject.SetActive(false);
      canAskCountText.gameObject.SetActive(false);
    }
  }

  public async void PlayerEndProject()
  {
    if(_isLoading) return;
    StartLoading();
    
    try {
      var req = new ChatRequest(clientsData.setResultAITask, _currentDialogue, DialogueRole.System32);
      var resp = await _clientChatService.Send(req.message);
      ResultWindow.Instance.Show(resp.reply);
    }
    finally {
      StopLoading();
    }
  }

  private void Update()
  {
    if(GameDataManager.Instance.gameData.stage != GameStage.FinalDialog
       && GameDataManager.Instance.gameData.stage != GameStage.SecondDialog
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
    if(_isLoading) return;
    
    StartLoading();
    
    AppendBubble(_currentMessage, true);
    
    try
    {
      _currentAskCount--;
      var req = new ChatRequest(_currentMessage, _currentDialogue);
      var resp = await _clientChatService.Send(req.message);
      AppendBubble(resp.reply);
      RecheckTracker(resp.learnedSummary);
      RecheckGottenTasks(resp.learnedText);
      canAskCountText.text = "Сообщение еще можно отправить: " + _currentAskCount;
      if (resp.incorrectMessages >= 3)
      {
        ResultWindow.Instance.Show("Клиент прервал разговор! Итоговая оценка: 0/10");
        return;
      }
      
      if (_incorrectMessageCount != resp.incorrectMessages)
      {
        _incorrectMessageCount = resp.incorrectMessages;
        NotificationMessageUI.Instance.ShowMessage(
          "Общайтесь с клиентом только по теме, иначе клиент может прервать разговор и сделку!", 7f);
      }
      
    }
    finally 
    {
      StopLoading();
    }

    if (_gameData.stage == GameStage.FinalDialog && _currentAskCount <= 0)
    {
      PlayerEndProject();
    }
    
    if (inputField != null) {
      inputField.text = string.Empty;
      _currentMessage = string.Empty;
    }
  }

  public void SetClientContext(string context)
  {
    _currentDialogue.AppendMessage(DialogueRole.System32, context.Trim() + "\n");
  }
  
  public async void SystemSendMessage(string message) {
    StartLoading();
    
    try {
      var req = new ChatRequest(message, _currentDialogue, DialogueRole.System32);
      var resp = await _clientChatService.Send(req.message);
      AppendBubble(resp.reply);
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
  

  private void AppendBubble(string text, bool isPlayer = false)
  {
    _currentDialogue.AppendMessage(isPlayer ? DialogueRole.Player : DialogueRole.Client, text);
    
    var bubble = Instantiate(isPlayer ? playerMessagePrefab : gameMessagePrefab, _messageContainer);
    bubble.text = text;
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
  
  private void ClearMessages() {
    foreach (Transform child in _messageContainer) {
      Destroy(child.gameObject);
    }
    
    Canvas.ForceUpdateCanvases();
    
    scrollRect.verticalNormalizedPosition = 1f; 
  }

  private string GetPlayerProjectData()
  {
    var data = LevelSelector.Instance.CurrentLevel.Cells;
    var result = "";
    var details = ProjectCalculator.Instance.GetValues();
    result += "Детали проекта игрока. Бюджет " + details.cost + ". Время постройки " + details.time + ".\n"; 
    result += "\nПоказатели (максимальное значение - 40): эстетика - " + details.aesthetics + ", функциональность - " + details.functionality + ".\n"; 
    foreach (var cell in data)
    {
      var cellData = "Клетка. Позиция: " + cell.data.x + " " + cell.data.y;
      if (cell.data.decorations[0] == null
          && cell.data.decorations[1] == null)
      {
        cellData += "\n. Ландшафт: " + cell.data.ground.displayName + ". Игрок ничего не построил.";
      }
      else
      {
        cellData += "\n. Игрок " + (cell.data.decorations[1] ? "построил: " + cell.data.decorations[1].displayName 
                      : " засыпал клетку: " + cell.data.decorations[0].displayName);
      }
      result += cellData + "\n";
    }
    
    return result;
  }
}

