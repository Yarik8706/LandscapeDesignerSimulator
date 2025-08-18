"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.rateLimit = rateLimit;
const firestore_1 = require("firebase-admin/firestore");
const db = (0, firestore_1.getFirestore)();
async function rateLimit(userId, maxPerHour = 120) {
    const windowKey = new Date().toISOString().slice(0, 13); // YYYY-MM-DDTHH
    const ref = db.collection("usage").doc(`${userId}_${windowKey}`);
    await db.runTransaction(async (tr) => {
        const snap = await tr.get(ref);
        const data = snap.exists ? snap.data() : { count: 0, windowKey };
        if (data.count >= maxPerHour)
            throw new Error("quota");
        data.count++;
        tr.set(ref, data);
    });
}
//# sourceMappingURL=ratelimit.js.map