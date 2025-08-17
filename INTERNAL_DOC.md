## Functions
- `chatWithClient`: callable function wrapping `clientChatFlow`; enforces per-user rate limiting.
- `summarizeSession`: callable for manual invocation of `summarizeFlow`.
- `clientChatFlow`: orchestrates dialogue with preferences passed via `clientBrief`; handles caching, escalation, session persistence, and periodic summarization.
- `summarizeFlow`: compresses accumulated dialogue into a summary.
- `withCache`: caches LLM responses in Firestore with TTL.
- `loadSession` / `saveSession`: manage session summaries and recent turns in Firestore.
- `rateLimit`: simple per-user hourly quota using Firestore.

## Types
- `Turn`: `{ role: "user" | "assistant"; text: string }` stored in session documents.

## Data Flow
1. Client calls `chatWithClient` with `{sessionId,userId,message,persona,clientBrief}`.
2. Function loads session data from `sessions/{sessionId}`.
3. LLM is prompted with system prompt embedding `clientBrief`, previous summary, and last K turns.
4. Response is cached via `withCache`; low confidence escalates to higher model.
5. Session history updated; every N=6 turns `summarizeFlow` updates summary.
6. Session stored back to Firestore: `{summary,lastK,turns,updatedAt}`.

## Notes
- History keeps only summary plus last 6 messages; summarization runs every 6 turns.
- Rate limiter allows 120 calls per user per hour.

