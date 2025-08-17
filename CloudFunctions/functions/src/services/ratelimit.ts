import { getFirestore } from "firebase-admin/firestore";
const db = getFirestore();

export async function rateLimit(userId: string, maxPerHour = 120) {
  const windowKey = new Date().toISOString().slice(0,13); // YYYY-MM-DDTHH
  const ref = db.collection("usage").doc(`${userId}_${windowKey}`);
  await db.runTransaction(async tr => {
    const snap = await tr.get(ref);
    const data = snap.exists ? snap.data()! : { count: 0, windowKey };
    if (data.count >= maxPerHour) throw new Error("quota");
    data.count++;
    tr.set(ref, data);
  });
}

