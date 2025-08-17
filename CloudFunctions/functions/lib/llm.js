import { genkit } from "@genkit-ai/core";
import { googleAI } from "@genkit-ai/googleai";
import { firebase as genkitFirebase } from "@genkit-ai/firebase";
export const ai = genkit({
    plugins: [
        googleAI({ apiKey: process.env.GOOGLE_API_KEY }),
        genkitFirebase()
    ],
});
