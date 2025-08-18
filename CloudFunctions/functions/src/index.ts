import * as admin from "firebase-admin";
import { onCallGenkit } from "firebase-functions/https";
import { aiCall as aiCallFlow } from "./flows/aiCall";
import {defineSecret} from "firebase-functions/params";

export const apiKey = defineSecret("GOOGLE_GENAI_API_KEY");

admin.initializeApp();

export const aiCall = onCallGenkit({
        secrets: [apiKey],
    },
    aiCallFlow,
);