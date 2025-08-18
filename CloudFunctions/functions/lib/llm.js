"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ai = exports.apiKey = void 0;
const params_1 = require("firebase-functions/params");
const googleai_1 = require("@genkit-ai/googleai");
const genkit_1 = require("genkit");
exports.apiKey = (0, params_1.defineSecret)("GOOGLE_GENAI_API_KEY");
exports.ai = (0, genkit_1.genkit)({
    plugins: [
        (0, googleai_1.googleAI)()
    ],
    model: googleai_1.gemini15Flash
});
//# sourceMappingURL=llm.js.map