using UnityEngine;
using UnityEngine.Events;

public class FirebaseInitializer : MonoBehaviour
{
  public static UnityEvent OnInitialized = new UnityEvent();

  [SerializeField]
  private bool initializeOnAwake = true;

  private void Awake()
  {
    if (initializeOnAwake)
    {
      OnInitialized.Invoke();
    }
  }
}

