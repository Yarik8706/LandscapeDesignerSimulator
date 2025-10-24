# Использование backend'a в Unity

Этот документ описывает, как Unity‑клиенту взаимодействовать с backend'ом ландшафтного симулятора. Backend построен на Next.js и предоставляет REST API поверх cloud‑функций, которые вызывают модели Google Gemini через [Genkit](https://github.com/google/genkit). Ниже приведены шаги по локальному запуску, настройке окружения и интеграции с игрой.

## 1. Общая архитектура

- API развёрнуто в приложении Next.js и предоставляет два HTTP‑эндпоинта: `/api/ai-call` и `/api/client-call`.
- Оба эндпоинта принимают POST‑запрос с телом `{ "message": "..." }` и используют библиотеку Genkit для обращения к моделям Gemini.
- `ai-call` возвращает свободный ответ модели.
- `client-call` возвращает структурированный ответ с прогрессом игрока: текст ответа, список согласованных тем, сводку прогресса и количество некорректных сообщений.

## 2. Переменные окружения

Backend требует ключа Google Generative AI.

```bash
export GOOGLE_GENAI_API_KEY="<ваш_ключ>"
```

Ключ должен быть доступен как на Next.js сервере (`lib/ai.ts`), так и в Firebase Cloud Functions (`CloudFunctions/functions/src/index.ts`). Без него инициализация клиента Genkit завершится ошибкой.

## 3. Запуск backend'а локально

1. Установите зависимости: `npm install` в корне проекта и `npm install` в `CloudFunctions/functions` при необходимости.
2. Создайте файл `.env.local` и добавьте переменную `GOOGLE_GENAI_API_KEY`.
3. Запустите dev‑сервер: `npm run dev`. API будет доступен по адресу `http://localhost:3000`.

## 4. API справочник

| Эндпоинт | Метод | Тело запроса | Успешный ответ | Описание |
|---------|-------|--------------|----------------|----------|
| `/api/ai-call` | POST | `{ "message": string }` | `{ "text": string }` | Простой ответ модели на входное сообщение. |
| `/api/client-call` | POST | `{ "message": string }` | `{ "reply": string, "learnedSummary": string, "learnedText": string, "incorrectMessages": number }` | Структурированный ответ для клиента‑NPC. |

### 4.1 `/api/ai-call`

- **Запрос:** `POST http://<host>/api/ai-call`
- **Тело:**
  ```json
  { "message": "Привет!" }
  ```
- **Ответ 200:**
  ```json
  { "text": "..." }
  ```
- **Ошибки:** `400` при неверном JSON, `500` при ошибке модели.

### 4.2 `/api/client-call`

- **Запрос:** `POST http://<host>/api/client-call`
- **Тело:**
  ```json
  { "message": "Диалог игрока" }
  ```
- **Ответ 200:**
  ```json
  {
    "reply": "Текст ответа NPC",
    "learnedSummary": "<количество_подтверждённых_тем>/<общее_число_требований>",
    "learnedText": "Список подтверждённых тем построчно",
    "incorrectMessages": 0
  }
  ```
- **Ошибки:** `400` при неверном JSON, `500` при ошибке модели или нарушении схемы.

## 5. Пример запроса из Unity

```csharp
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BackendClient : MonoBehaviour
{
    private const string BaseUrl = "http://localhost:3000"; // замените на URL сервера

    public IEnumerator SendClientCall(string message)
    {
        var payload = Encoding.UTF8.GetBytes($"{{\"message\":\"{message}\"}}");
        using var request = new UnityWebRequest($"{BaseUrl}/api/client-call", UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(payload),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Ошибка: {request.error}\n{request.downloadHandler.text}");
            yield break;
        }

        Debug.Log($"Ответ: {request.downloadHandler.text}");
        // Распарсите JSON в структуру ClientCallResult
    }
}
```

## 6. Обработка ответов в Unity

Рекомендуется создать C#‑структуры, повторяющие JSON.

```csharp
[System.Serializable]
public struct ClientCallResult
{
    public string reply;
    public string learnedSummary;
    public string learnedText;
    public int incorrectMessages;
}
```

Используйте `JsonUtility.FromJson<ClientCallResult>(json)` или JSON‑парсер по вашему выбору.

## 7. Продакшн и Firebase

Firebase Cloud Functions экспортируют те же потоки `aiCall` и `clientCall`. Если вы разворачиваете backend через Firebase, настройте секрет `GOOGLE_GENAI_API_KEY` в Firebase и вызывайте функции через созданные HTTPS‑эндпоинты. Структура запроса и ответа совпадает.

---

Эти инструкции позволят Unity‑клиенту безопасно обращаться к backend'у, отслеживать прогресс диалога и обрабатывать ошибки.
