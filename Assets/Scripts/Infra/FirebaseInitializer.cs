using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour {
  private void Awake() {
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
      if (task.Result != DependencyStatus.Available) {
        Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
      }
    });
  }
}

