import { ai } from "../llm";
import { z } from "zod";
export const summarizeFlow = ai.flow("summarize", {
    inputSchema: z.object({
        summary: z.string(),
        turns: z.array(z.object({ role: z.string(), text: z.string() }))
    }),
    outputSchema: z.object({ summary: z.string() })
}, async ({ input, llm }) => {
    const sys = `Сожми диалог в краткую выжимку для LLM-контекста. Не теряй факты: бюджет, срок, must/nice/bans, приоритеты (F/A/S), климат/риски, договорённости. Кратко и структурно.`;
    const rsp = await llm.generate({
        model: "gemini-2.0-flash-lite",
        system: sys,
        messages: [
            { role: "user", content: JSON.stringify({ summary: input.summary, newTurns: input.turns }) }
        ],
        json: true,
        schema: {
            type: "object",
            properties: { summary: { type: "string" } },
            required: ["summary"]
        }
    });
    return rsp.output;
});
