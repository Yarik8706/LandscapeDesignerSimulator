import * as admin from "firebase-admin";
import { onCallGenkit } from "firebase-functions/https";
import { aiCallFlow } from "./flows/aiCallFlow";
import { clientCallFlow } from "./flows/clientCallFlow";
import {defineSecret} from "firebase-functions/params";

export const apiKey = defineSecret("GOOGLE_GENAI_API_KEY");

admin.initializeApp();

export const aiCall = onCallGenkit({
        secrets: [apiKey],
    },
    aiCallFlow,
);

export const clientCall = onCallGenkit({
        secrets: [apiKey],
    },
    clientCallFlow,
);