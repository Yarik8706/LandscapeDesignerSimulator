import { ai } from "../llm";
import { googleAI } from "@genkit-ai/googleai";
import { z } from "genkit";

export const ModelOutput = z.object({
    reply: z.string(),              // ответ ИИ игроку
    topics: z.array(z.string()).default([]) ,// произвольное число тем,
    allTopicsCount: z.number(),     // всего тем
});

export const FlowOutput = z.object({
    reply: z.string(),
    learnedSummary: z.string(),     // "X/Y"
    learnedText: z.string()        // готовая строка со списком подтверждённых пунктов
});

export const clientCallFlow = ai.defineFlow({
    name: "clientCall",
    inputSchema: z.string(),    // текущий диалог/сообщение (или весь диалог как строка)
    outputSchema: FlowOutput,
}, async (message) => {
    const { output } = await ai.generate({
        model: googleAI.model("gemini-2.5-flash"),
        prompt:
            message + `\n
1) Дай ответ игроку в поле "reply".
2) Верни массив "allTopicsCount": общее количество требований.
3) Верни массив "topics": требования, которые ты выдал в диалоге, опиши их предложением.
Не выдумывай.
`,
        output: { schema: ModelOutput }
    });

    if (!output) throw new Error("Response doesn't satisfy schema.");

    const total = output.topics.length;
    const learnedSummary = `${total}/${output.allTopicsCount}`;
    const learnedText = output.topics.join("\n");

    return {
        reply: output.reply,
        learnedSummary,      
        learnedText
    };
});
