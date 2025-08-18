import { ai } from "../llm";
import { z } from "genkit";

export const aiCall = ai.defineFlow({
        name: "aiCall",
        inputSchema: z.object({
            sessionId: z.string().nullish(),
            userId: z.string().nullish(),
            message: z.string(),
            persona: z.string().default("pragmatic"),
            clientBrief: z.string().nullish()
        }),
        outputSchema: z.object({
            reply: z.string(),
            lowConfidence: z.boolean().optional(),
        }), 
        streamSchema: z.string(),
    }, async (message, {sendChunk}) => {
        const {stream, response: aiResponse} = ai.generateStream(
            message.message);
        
        for await (const chunk of stream) {
            sendChunk(chunk.text);
        }

        const response = await aiResponse;

        return {reply: response.text, lowConfidence: false};
    },
);