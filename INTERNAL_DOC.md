## Functions
- `chatWithClient`: callable function wrapping `clientChatFlow`; enforces per-user rate limiting.
- `summarizeSession`: callable for manual invocation of `summarizeFlow`.
- `clientChatFlow`: orchestrates dialogue with preferences passed via `clientBrief`; handles caching, escalation, session persistence, periodic summarization, and returns per-topic progress flags.
- `summarizeFlow`: compresses accumulated dialogue into a summary.
- `withCache`: caches LLM responses in Firestore with TTL.
- `loadSession` / `saveSession`: manage session summaries and recent turns in Firestore.
- `rateLimit`: simple per-user hourly quota using Firestore.
- `BuildElementsGenerator.Generate`: editor menu that creates or updates `BuildElementData` assets in `Assets/GameData/BuildElements` and автоматом формирует поле описания.

## Types
- `Turn`: `{ role: "user" | "assistant"; text: string }` stored in session documents.
- `Progress`: `{budget,deadline,weights,must,bans,climate,risks,bonus}` booleans returned each turn.
- `BuildElementData`: ScriptableObject defining id, display name, description, category, cost, build time, scoring delta, stability (tolerance, failMod, aura), placement constraints (terrain, climate, proximity, allowedBase), adjacency bonuses, and terraform overlay rules.

## Data Flow
1. Client calls `chatWithClient` with `{sessionId,userId,message,persona,clientBrief}`.
2. Function loads session data from `sessions/{sessionId}`.
3. LLM is prompted with system prompt embedding `clientBrief`, previous summary, and last K turns.
4. Response includes `progress` flags; low confidence (<0.4) triggers escalation to a stronger model with caching.
5. Session history updated; every N=6 turns `summarizeFlow` updates summary.
6. Session stored back to Firestore: `{summary,lastK,turns,updatedAt}`.
7. `BuildElementsGenerator.Generate` iterates specs, creates assets, links `allowedBase`, and saves to disk.

## Notes
- History keeps only summary plus last 6 messages; summarization runs every 6 turns.
- Rate limiter allows 120 calls per user per hour.
- Progress flags are derived from summary, recent turns, and the current message each request.
- Build element assets are idempotently regenerated; rerunning the generator updates existing assets.
- Все `BuildElementData` ассеты теперь имеют русские названия в поле `displayName`.
- `GroundElementData` заполнены значениями `replaceCost`, `replaceTime`, `overlayCost`, `overlayTime`.

Ниже — полная задача для Codex по созданию каталога строительных элементов в формате текущего ScriptableObject `BuildElementData` (Unity). Цель — сгенерировать **ассеты** со значениями полей и вспомогательный редакторский код для автоматического создания/связывания. Категория **Embankment** служит для подсыпки (чернозём или гравий+песок), на которой стоят другие объекты для их укрепления.

---

# 1) Цель

* Создать каталог элементов (ScriptableObject-ассеты) согласно текущему формату `BuildElementData`.
* Обеспечить **двухфазное наполнение**: сначала создать все ассеты, затем проставить `allowedBase` (ссылки на Embankment/базовые элементы), чтобы корректно работали зависимости.
* Упростить настройку: все ассеты кладём в `Assets/GameData/BuildElements/`.

# 2) Что у нас есть (структура данных)

* Класс `BuildElementData` с полями:

* `id:int`, `displayName:string`, `description:string`, `category:Category`, `icon:Sprite`.
    * `cost:int`, `buildTime:float`.
    * `delta:F/A/S` (вклады в функциональность/эстетику/устойчивость зоны).
    * `stability` (tolerance по climate, failMod, aura).
    * `constraints` (terrainAllowed, climate, proximity, **allowedBase: List**).
    * `adjacency` (бонусы рядом с объектами).
    * `terraform.overlayOn: List<TerrainType>` — список поверхностей, над которыми объект кладётся как «оверлей» без удаления базы (например, мост над водой).
* `Category`: **Embankment**, **Decoration**.
* `TerrainType`: **Pound** (пруд), **Swamp**, **Forest**, **Steppe**, **Mountain**, **All**, **None**.

> Важно: *усиление от подсыпки* реализуем так: если объект размещается на тайле, где в слоте «база» стоит элемент категории **Embankment**, движок при установке добавляет к объекту временный бонус `S += 2` (или значение из ауры Embankment, если указано `s_local`). В этой задаче Codex только проставляет `allowedBase` и (по желанию) `stability.aura` у Embankment; сам игровой код усиления реализуется отдельно.

---

# 3) Список элементов для ассетов

## 3.1. Embankment (подсыпки)

1. **1** — Topsoil Fill (Чернозём)

    * `category: Embankment`, `cost: 8`, `buildTime: 0.2`
    * `delta: {F:0,A:0,S:1}` (небольшой вклад в устойчивость)
    * `stability.tolerance: {temp:2, wind:0, hum:2}`, `failMod: 0.95`
    * `stability.aura: radius 0, shape: Square, effects: [{key:"s_local", value: +2}]`
    * `constraints.terrainAllowed: [All]`
    * `terraform.overlayOn: []`

2. **2** — Gravel-Sand Fill (Гравий+Песок)

    * `category: Embankment`, `cost: 10`, `buildTime: 0.25`
    * `delta: {F:0,A:0,S:2}`
    * `stability.tolerance: {temp:2, wind:1, hum:1}`, `failMod: 0.9`
    * `stability.aura: radius 0, shape: Square, effects: [{key:"s_local", value: +3}]`
    * `constraints.terrainAllowed: [All]`
    * `terraform.overlayOn: []`

> Оба Embankment-элемента будут использованы как **база** для других объектов через `constraints.allowedBase`.

## 3.2. Decoration — Покрытия/дорожки/настилы

3. **3** — Paver Tile (Плитка)

    * `cost: 12`, `buildTime: 0.4`, `delta:{F:1,A:1,S:1}`
    * `constraints.terrainAllowed:[All]`
    * `constraints.allowedBase:[1,2]`
    * `terraform.overlayOn: []`

4. **4** — Gravel Path (Гравийная дорожка)

    * `cost: 6`, `buildTime: 0.2`, `delta:{F:1,A:0,S:1}`
    * `constraints.terrainAllowed:[All]`
    * `constraints.allowedBase:[2]`
    * `terraform.overlayOn: [Steppe, Swamp]`

5. **5** — Wood Deck (Деревянный настил)

    * `cost: 18`, `buildTime: 0.6`, `delta:{F:1,A:1,S:0}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[2]`
    * `terraform.overlayOn: [Swamp]`

6. **6** — Asphalt (Асфальт)

    * `cost: 15`, `buildTime: 0.5`, `delta:{F:2,A:0,S:1}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[2]`
    * `terraform.overlayOn: []`

## 3.3. Decoration — Растительность

7. **7** — Lawn (Газон)

    * `cost: 4`, `buildTime: 0.1`, `delta:{F:0,A:1,S:0}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[1]`

8. **8** — Flowerbed (Клумба)

    * `cost: 8`, `buildTime: 0.2`, `delta:{F:0,A:2,S:0}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[1]`

9. **9** — Shrub (Кустарник)

    * `cost: 10`, `buildTime: 0.3`, `delta:{F:0,A:1,S:1}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[1]`

10. **10** — Tree Sapling (Саженец дерева)

    * `cost: 12`, `buildTime: 0.3`, `delta:{F:0,A:1,S:2}`
    * `stability.aura: radius 1, shape: Circle, effects:[{key:"wind", value:-1}]`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[1]`

## 3.4. Decoration — Вода и декор

11. **11** — Pond Water (Зеркало воды)

    * `cost: 20`, `buildTime: 0.8`, `delta:{F:0,A:2,S:0}`
    * `constraints.terrainAllowed:[All]`
    * `terraform.overlayOn: [None]`

12. **12** — Fountain (Фонтан)

    * `cost: 40`, `buildTime: 1.2`, `delta:{F:0,A:3,S:0}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[3]`

13. **13** — Rock Group (Каменная композиция)

    * `cost: 14`, `buildTime: 0.4`, `delta:{F:0,A:1,S:1}`
    * `constraints.terrainAllowed:[All]`, `allowedBase:[2,3]`

14. **14** — Bench (Лавка)

    * `cost: 6`, `buildTime: 0.1`, `delta:{F:1,A:0,S:0}`
    * `constraints.allowedBase:[3,5,6]`
    * `adjacency: needId="4" radius=1 bonus:{F:+1,A:0,S:0}`

15. **15** — Bin (Урна)

    * `cost: 5`, `buildTime: 0.1`, `delta:{F:1,A:0,S:0}`
    * `constraints.allowedBase:[3,5,6]`

16. **16** — Lamp (Фонарь)

    * `cost: 20`, `buildTime: 0.5`, `delta:{F:1,A:0,S:1}`
    * `stability.aura: radius 2, shape: Square, effects:[{key:"s_local", value:+2}]`
    * `constraints.allowedBase:[3,5,6]`

## 3.5. Decoration — Функциональные

17. **17** — Playground (Детская площадка)

    * `cost: 80`, `buildTime: 2.0`, `delta:{F:3,A:1,S:1}`
    * `constraints.allowedBase:[3,6]`

18. **18** — Sports Ground (Спортплощадка)

    * `cost: 90`, `buildTime: 2.2`, `delta:{F:3,A:0,S:1}`
    * `constraints.allowedBase:[3,6]`

19. **19** — Parking (Парковка)

    * `cost: 60`, `buildTime: 1.5`, `delta:{F:3,A:0,S:1}`
    * `constraints.allowedBase:[6]`

20. **20** — Gazebo (Беседка)

    * `cost: 70`, `buildTime: 1.6`, `delta:{F:2,A:2,S:1}`
    * `constraints.allowedBase:[3,5]`

21. **21** — BBQ Zone (Мангал/BBQ)

    * `cost: 25`, `buildTime: 0.5`, `delta:{F:2,A:0,S:0}`
    * `constraints.allowedBase:[3,6]`

22. **22** — Small Bridge (Мостик)

    * `cost: 50`, `buildTime: 1.4`, `delta:{F:2,A:1,S:1}`
    * `constraints.allowedBase:[5,2]`
    * `terraform.overlayOn:[Pound]`

---

# 4) Идентификаторы и зависимости

* Id фиксируем, как указано (от 1 до 22). Это упростит ссылки в Proximity/adjacency.
* Для `adjacency.needId` используем **строковые идентификаторы** выше (например, "4" = Gravel Path). Если нужен строгий id, задать соответствие в комментариях к коду.
* `constraints.allowedBase` заполняется **во второй фазе** после создания всех ассетов (см. ниже Editor-утилиту).

---

# 5) Editor-утилита (двухфазное создание)

1. Скрипт `Assets/Editor/BuildElementsGenerator.cs`:

    * Создаёт все `BuildElementData` ассеты с полями **кроме** `allowedBase`.
    * Сохраняет карту `id -> asset`.
    * Вторым проходом проставляет `allowedBase` по указанным связям (например, 3.allowedBase = \[1,2]).
    * Ставит `terraform.overlayOn` согласно списку выше.
    * Сохраняет ассеты, делает `AssetDatabase.SaveAssets()`.
2. Скрипт должен быть идемпотентным: при повторном запуске обновляет значения, не создавая дублей.
3. Иконки: поле `icon` оставляем пустым (или ищем по имени из `Assets/Textures` при наличии совпадений).

Пример API (эскиз):

```csharp
[MenuItem("Tools/LD/Generate Build Elements")]
static void Generate() {
  // 1) create or load assets by id
  // 2) fill base fields (displayName, category, cost, delta, stability, constraints.climate, terraform)
  // 3) link allowedBase by finding assets by numeric id (1, 2, 3 и т.д.)
  // 4) save
}
```

---

# 6) Мини‑правила заполнения полей

* `constraints.terrainAllowed`: если нет спец‑ограничений — `[All]`.
* `constraints.climate`: оставить дефолтные окна (-25..45°C, hum 0..95, windMax 25).
* `proximity.need/avoid`: пока пусто (кроме примера для Bench → `needId:4` в `adjacency`).
* `stability.failMod`: покрытия 0.9–1.0, функциональные 0.85–0.95, растительность 0.95–1.0.
* `stability.aura`: у деревьев/фонарей/embankment допускается локальный эффект (`s_local:+2/3`). Радиус небольшой (0–2).

---

# 7) Acceptance Criteria

1. В папке `Assets/GameData/BuildElements/` сгенерированы 22 ассета с id от 1 до 22.
2. Для всех элементов второй фазой корректно заполнено `constraints.allowedBase`.
3. Для `Small Bridge` проставлен `terraform.overlayOn:[Pound]`.
4. Для Embankment элементов выставлены `stability.aura.effects` с ключом `s_local` и связаны как база у покрытий/объектов.
5. Генератор можно запускать повторно без создания дублей; он обновляет существующие ассеты.
6. Все ассеты собираются в билд; отсутствуют сериализационные ошибки в инспекторе.

---

# 8) Примечания

* Если движок пока не применяет бонус `s_local` от Embankment к стоящему сверху объекту, оставить как задел (не ломает текущую логику).
* При желании можно добавить дополнительные покрытия (например, "Road"), но в рамках данной задачи ограничиться перечисленными 22 элементами.

