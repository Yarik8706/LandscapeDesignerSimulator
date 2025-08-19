using Firebase;
using Firebase.Extensions;
using Firebase.Functions;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseInitializer : MonoBehaviour {
  
  public static UnityEvent OnInitialized = new UnityEvent();
  
  private void Awake() {
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
      if (task.Result != DependencyStatus.Available) {
        Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
      }
      else {
        AuthService.SignInAnonymously().ContinueWithOnMainThread(task => {
          if (task.IsFaulted || task.IsCanceled) {
            Debug.LogError($"Could not sign in: {task.Exception}");
          }
        });
        OnInitialized.Invoke();
      }
    });
  }
}

