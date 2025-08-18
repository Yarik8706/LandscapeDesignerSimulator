using System;
using System.Threading.Tasks;

public static class RetryPolicy {
  public static async Task<T> Execute<T>(Func<Task<T>> action, int retries = 3) {
    int delay = 1000;
    for (int i = 0; i < retries; i++) {
      try {
        return await action();
      } catch {
        if (i == retries - 1) throw;
        await Task.Delay(delay);
        delay *= 2;
      }
    }
    return await action();
  }
}

