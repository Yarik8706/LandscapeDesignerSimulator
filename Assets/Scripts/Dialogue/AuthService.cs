using Firebase.Auth;
using System.Threading.Tasks;

public static class AuthService {
  public static async Task<string> SignInAnonymously() {
    var auth = FirebaseAuth.DefaultInstance;
    var user = auth.CurrentUser;
    if (user != null) return user.UserId;
    var result = await auth.SignInAnonymouslyAsync();
    return result.User.UserId;
  }
}

