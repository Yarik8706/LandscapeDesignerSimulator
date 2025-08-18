import { ai } from "../llm";
import { z } from "genkit";

export const aiCallFlow = ai.defineFlow({
        name: "aiCall",
        inputSchema: z.string(),
        outputSchema: z.string(), 
    }, async (message, {sendChunk}) => {
        const {response: aiResponse} 
            = ai.generateStream(
            message);
        
        const response = await aiResponse;

        return response.text;
    },
);