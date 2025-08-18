"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.withCache = withCache;
const firestore_1 = require("firebase-admin/firestore");
const crypto_1 = __importDefault(require("crypto"));
const db = (0, firestore_1.getFirestore)();
async function withCache(key, ttlSec, fn) {
    const id = crypto_1.default.createHash("sha256").update(JSON.stringify(key)).digest("hex");
    const ref = db.collection("aiCache").doc(id);
    const snap = await ref.get();
    const now = firestore_1.Timestamp.now();
    if (snap.exists) {
        const d = snap.data();
        if (d.expiresAt.toMillis() > now.toMillis())
            return d.output;
    }
    const out = await fn();
    await ref.set({
        key,
        output: out,
        createdAt: now,
        expiresAt: firestore_1.Timestamp.fromMillis(now.toMillis() + ttlSec * 1000),
    }, { merge: true });
    return out;
}
//# sourceMappingURL=cache.js.map