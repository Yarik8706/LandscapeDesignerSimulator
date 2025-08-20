import { ai } from "../llm";
import { z } from "genkit";

export const aiCallFlow = ai.defineFlow({
        name: "aiCall",
        inputSchema: z.string(),
        outputSchema: z.string(), 
    }, async (message, {sendChunk}) => {
        const {text} 
            = await ai.generate(
            message);

        return text;
    },
);