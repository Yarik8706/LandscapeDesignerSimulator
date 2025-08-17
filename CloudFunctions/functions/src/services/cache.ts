import { getFirestore, Timestamp } from "firebase-admin/firestore";
import crypto from "crypto";
const db = getFirestore();

export async function withCache<T>(key: unknown, ttlSec: number, fn: () => Promise<T>): Promise<T> {
  const id = crypto.createHash("sha256").update(JSON.stringify(key)).digest("hex");
  const ref = db.collection("aiCache").doc(id);
  const snap = await ref.get();
  const now = Timestamp.now();

  if (snap.exists) {
    const d = snap.data()!;
    if (d.expiresAt.toMillis() > now.toMillis()) return d.output as T;
  }
  const out = await fn();
  await ref.set({
    key,
    output: out,
    createdAt: now,
    expiresAt: Timestamp.fromMillis(now.toMillis() + ttlSec * 1000),
  }, { merge: true });
  return out;
}

