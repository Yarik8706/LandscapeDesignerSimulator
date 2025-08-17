import { getFirestore, Timestamp } from "firebase-admin/firestore";
const db = getFirestore();

export type Turn = { role: "user"|"assistant"; text: string };

export async function loadSession(sessionId: string) {
  const ref = db.collection("sessions").doc(sessionId);
  const snap = await ref.get();
  if (!snap.exists) return { summary: "", lastK: [] as Turn[], turns: 0 };
  const d = snap.data()!;
  return { summary: d.summary ?? "", lastK: d.lastK ?? [], turns: d.turns ?? 0 };
}

export async function saveSession(sessionId: string, summary: string, lastK: Turn[], turns: number) {
  const ref = db.collection("sessions").doc(sessionId);
  await ref.set({ summary, lastK, turns, updatedAt: Timestamp.now() }, { merge: true });
}

