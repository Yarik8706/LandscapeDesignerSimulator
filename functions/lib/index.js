import * as admin from "firebase-admin";
import { onCallGenkit } from "firebase-functions/genkit";
import { clientChatFlow } from "./flows/clientChat";
import { summarizeFlow } from "./flows/summarize";
import { rateLimit } from "./services/ratelimit";
admin.initializeApp();
export const chatWithClient = onCallGenkit(clientChatFlow, {
    before: [async (ctx) => {
            const uid = ctx.auth?.uid || "anon";
            await rateLimit(uid, 120);
        }]
});
export const summarizeSession = onCallGenkit(summarizeFlow);
