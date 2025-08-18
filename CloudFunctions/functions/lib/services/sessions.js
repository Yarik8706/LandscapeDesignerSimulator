"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.loadSession = loadSession;
exports.saveSession = saveSession;
const firestore_1 = require("firebase-admin/firestore");
const db = (0, firestore_1.getFirestore)();
async function loadSession(sessionId) {
    var _a, _b, _c;
    const ref = db.collection("sessions").doc(sessionId);
    const snap = await ref.get();
    if (!snap.exists)
        return { summary: "", lastK: [], turns: 0 };
    const d = snap.data();
    return { summary: (_a = d.summary) !== null && _a !== void 0 ? _a : "", lastK: (_b = d.lastK) !== null && _b !== void 0 ? _b : [], turns: (_c = d.turns) !== null && _c !== void 0 ? _c : 0 };
}
async function saveSession(sessionId, summary, lastK, turns) {
    const ref = db.collection("sessions").doc(sessionId);
    await ref.set({ summary, lastK, turns, updatedAt: firestore_1.Timestamp.now() }, { merge: true });
}
//# sourceMappingURL=sessions.js.map