import { ai } from "../llm";
import { z } from "zod";
import { withCache } from "../services/cache";
import { loadSession, saveSession } from "../services/sessions";
import { summarizeFlow } from "./summarize";
export const clientChatFlow = ai.flow("clientChat", {
    inputSchema: z.object({
        sessionId: z.string(),
        userId: z.string(),
        message: z.string(),
        persona: z.string().default("pragmatic"),
        clientBrief: z.string()
    }),
    outputSchema: z.object({
        reply: z.string(),
        lowConfidence: z.boolean().optional(),
        progress: z.object({
            budget: z.boolean().default(false),
            deadline: z.boolean().default(false),
            weights: z.boolean().default(false),
            must: z.boolean().default(false),
            bans: z.boolean().default(false),
            climate: z.boolean().default(false),
            risks: z.boolean().default(false),
            bonus: z.boolean().default(false)
        })
    })
}, async ({ input, llm }) => {
    const sess = await loadSession(input.sessionId);
    const historyForModel = {
        summary: sess.summary,
        lastK: sess.lastK
    };
    const systemPrompt = `Ты играешь роль клиента ландшафтного дизайна. Общайся кратко и по делу.
У тебя есть предпочтения и требования (ниже одним блоком), ты можешь их корректировать при переговорах.
Факты, которые следует учитывать и подтверждать в разговоре: бюджет, срок, must/nice/bans, приоритеты F/A/S, климат/риски, бонусы.
Если игрок указывает объективные риски участка, допускай пересмотр.
Оффтоп мягко перенаправляй. При токсичности/злоупотреблении — прекращай сделку.
Требование: верни JSON с полями reply, confidence (0..1) и progress (булевы флаги тем: budget, deadline, weights, must, bans, climate, risks, bonus).
Ставь флаг true только если тема ОДНОЗНАЧНО подтверждена фактами из истории (summary + последние реплики) и текущего сообщения.
Твои предпочтения и требования:
${input.clientBrief}
Используй историю (summary) и последние реплики для контекста.`;
    const msgs = [
        { role: "system", content: systemPrompt },
        { role: "user", content: `SUMMARY:\n${historyForModel.summary}` },
        ...historyForModel.lastK.map(t => ({ role: t.role, content: t.text })),
        { role: "user", content: input.message }
    ];
    const run = async (model) => llm.generate({
        model,
        messages: msgs,
        json: true,
        schema: {
            type: "object",
            properties: {
                reply: { type: "string" },
                confidence: { type: "number" },
                progress: {
                    type: "object",
                    properties: {
                        budget: { type: "boolean" },
                        deadline: { type: "boolean" },
                        weights: { type: "boolean" },
                        must: { type: "boolean" },
                        bans: { type: "boolean" },
                        climate: { type: "boolean" },
                        risks: { type: "boolean" },
                        bonus: { type: "boolean" }
                    },
                    required: ["budget", "deadline", "weights", "must", "bans", "climate", "risks", "bonus"]
                }
            },
            required: ["reply", "progress"]
        }
    });
    const key = { m: "lite", msgs: msgs.map(m => ({ r: m.role, c: typeof m.content === "string" ? m.content : "" })) };
    const outLite = await withCache(key, 30, async () => (await run("gemini-2.0-flash-lite")).output);
    let final = outLite;
    let low = typeof outLite?.confidence === "number" && outLite.confidence < 0.4;
    if (low) {
        const key2 = { m: "flash", msgs: msgs.map(m => ({ r: m.role, c: typeof m.content === "string" ? m.content : "" })) };
        final = await withCache(key2, 15, async () => (await run("gemini-2.0-flash")).output);
    }
    const newTurns = [
        ...historyForModel.lastK,
        { role: "user", text: input.message },
        { role: "assistant", text: final.reply }
    ];
    const K = 6;
    const trimmed = newTurns.slice(-K);
    const turns = (sess.turns ?? 0) + 1;
    const N = 6;
    let summary = sess.summary;
    if (turns % N === 0) {
        const sum = await summarizeFlow.run({ summary, turns: trimmed });
        summary = sum.summary;
    }
    await saveSession(input.sessionId, summary, trimmed, turns);
    return { reply: final.reply, lowConfidence: low, progress: final.progress };
});
