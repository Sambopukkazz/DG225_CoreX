---
type: gdd-mechanics
version: 2.0
date: 2026-10-24
---

# Imaginophobia — Mechanic Design

**Version:** 2.0 | **Last Updated:** 2026-10-24

> เอกสารนี้อัปเดตจากโค้ดจริงใน `src/Imaginophonia/Imaginophonia/Scripts/`
> อ้างอิง: [System Design](../03_Software/01-system-design.md) · [Data Schema](../03_Software/03-data-schema.md)

## State Diagram

สถานะของผู้เล่นตรงกับ `enum CharacterAction` ใน `Scripts/CharacterAction.cs`

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Walking : กด A / D
    Walking --> Idle : ปล่อยปุ่ม
    Idle --> Sneaking : กด Left Ctrl + ทิศทาง
    Sneaking --> Idle : ปล่อยปุ่ม
    Idle --> ReadyToHide : เข้าใกล้ตู้ (locker)
    ReadyToHide --> Hiding : กด Space
    Hiding --> Idle : กด Space ออก / ครบเวลาซ่อน (บังคับออก)
    Idle --> Repairing : กด Space ที่จุดซ่อม
    Repairing --> Checking : กด A / D เพื่อมองซ้าย-ขวา
    Checking --> Repairing : ปล่อยปุ่ม
    Repairing --> Idle : กด Q ออก / ซ่อมสำเร็จ
```

## Rules

| State | เข้าเงื่อนไข | ออกเงื่อนไข | Note |
| ----- | ----------------------------------------------- | ------------------------------------------------------------------- | --------------- |
| `Idle` | เริ่มเกม / หยุดเคลื่อนที่ | กด input ใด ๆ | Animation loop `idle` (12 เฟรม) |
| `Walking` | กด A / D | ปล่อยปุ่ม | `MoveSpeed = 250` |
| `Sneaking` | กด Left Ctrl ขณะเดิน | ปล่อย Left Ctrl | ความเร็ว ÷ 1.7 · ล็อกทิศทางการหันหน้า |
| `ReadyToHide` | เข้าใกล้ collider tag `hideout` | ออกจากพื้นที่ / กด Space | ยังไม่เข้าไปในตู้ |
| `Hiding` | กด Space ขณะ `ReadyToHide` | กด Space ออก / ครบ 8 วินาที | บังคับออกอัตโนมัติ (Claustrophobia) · cooldown 3 วินาทีก่อนซ่อนใหม่ |
| `Repairing` | กด Space ที่ collider tag `skillcheck` / `dots` | กด Q / ซ่อมสำเร็จ | หยุดการเคลื่อนที่ + ปิดไฟฉาย |
| `Checking` | กด A / D ขณะ `Repairing` | ปล่อยปุ่ม | เลื่อนกล้องมองซ้าย-ขวา 400px |

> **TBD:** ตารางปุ่มจริงยังไม่ครบ — `InputManager` ผูกแค่ `A` / `D` (ทิศทาง) ส่วน `Left Ctrl` (ซุ่ม), `F` (ไฟฉาย), `Space` (interact), `Q` (ออกมินิเกม) อ่านตรงใน `Player.cs` / `CollisionManager.cs` / หน้าจอมินิเกม ควรย้ายมาที่ `InputManager` ให้เป็นที่เดียว

## Sanity (SanityState)

ค่า Sanity เป็นทรัพยากรหลักที่ผูกกับ "ความกลัว" ของตัวละคร เริ่มต้นที่ **100** และแบ่งเป็น 3 ระดับตาม `enum SanityState` (`Scripts/Enums/SanityState.cs`)

| ระดับ | ค่า threshold | ความหมาย | ผลที่เห็น |
|---|---|---|---|
| `Normal` | 70 | ปกติ | vignette ปกติ (`Scale 0.9 × 0.5`, สีดำ) |
| `Anxious` | 30 | เริ่มกังวล | vignette หด (`0.55 × 0.4`) |
| `Insane` | 0 | สติแตกระดับวิกฤต | vignette หดสุด (`0.45 × 0.35`) + เปลี่ยนสีเป็น `#2e0202` |

**อัตราการลด Sanity**

| สาเหตุ | ค่า | ที่มา |
|---|---|---|
| อยู่ในโถงอุโมง (sewer) | 0.5 / วินาที (Normal) · 0.2 (Anxious) · 0 (Insane) | `Player._availableSanityRedectionSpeed` |
| ชน Autophobia | −10 (ครั้งเดียว แล้ว Autophobia หายไป) | `CollisionManager` |
| ถูก Scopophobia พุ่งชน | −25 (ครั้งเดียว แล้ว Scopophobia หายไป) | `CollisionManager` |
| Nyctophobia — อยู่ในพื้นที่ที่ไฟดับ | vignette ปิดลงเรื่อย ๆ จน `Scale` เหลือ 0 → `Sanity = 0` ทันที | `UIManager` |

- Sanity ถึง 0 = แพ้ → `GameManager.Restart()` โหลดฉากแรกใหม่
- **Battery** — พลังงานไฟฉาย เริ่ม 100 ลด 3.5 / วินาทีขณะเปิดไฟฉาย หมดแล้วเปิดไม่ได้ (ต้องอยู่ในความมืด ซึ่งเร่งการลด Sanity)

> ⚠️ **Known issue จาก Playtest:** ผู้ทดสอบบางคนไม่รู้ด้วยซ้ำว่ามีระบบนี้อยู่ — HUD ปัจจุบันเป็นแถบสี (เขียว/เหลือง/แดง) ไม่มีตัวเลข ต้องทำให้ชัดขึ้น (ดู [Scope: Risks](02-scope-features.md#risks--assumptions))

## Phobias (ศัตรู)

แต่ละ phobia ถูกออกแบบโดยนำ "คำอธิบายของความกลัว" มาแปลงเป็นสิ่งที่มองเห็นได้ แล้วใช้ concept นั้นเป็น mechanic โดยตรง — สีดำขาวสไตล์ contrast ให้ความรู้สึกเกินจริง

| Phobia | ความกลัว | ลักษณะที่เห็น | การเกิด | วิธีรับมือ | ไฟล์โค้ด |
|---|---|---|---|---|---|
| **Autophobia** | Fear of Being Alone<br/>(กลัวการอยู่โดดเดี่ยว) | เงาร่างดำ (spritesheet เดียวกับผู้เล่น ย้อมดำ) | สุ่ม spawn ในโถงอุโมงทุก 5 วินาที · เดินตามผู้เล่นด้วย `MoveSpeed = 400` (เร็วกว่าผู้เล่นที่ 250) · หายเองใน 28 วินาที | หันไปมอง (แนวสายตา `EyeSight` ตัดกับตัวมัน) → กระพริบตาแล้วมันหายไป · ขอบเขตการมองเห็นถูกสุ่ม | `Enemies/Autophobia.cs` |
| **Scopophobia** | Fear of Being Watched<br/>(กลัวการถูกมอง) | หนอน/มวลที่มีลูกตาหลายดวง | สุ่ม spawn (ต้องไม่มีตัวอื่นอยู่) · มีเสียงนำ + ไฟทุกดวงในฉากเปลี่ยนเป็นสีแดง (`LightManager.ScopoAlarmAll`) · รอ 8 วินาทีก่อนเริ่มพุ่ง · หายเองใน 15 วินาที | หลบในตู้ให้ทัน · ซ่อมไฟที่ห้องไฟฟ้าเพื่อคืนแสง (ระบบไฟถูกรีเซ็ตเมื่อมันเกิด) | `Enemies/Scopophobia.cs` |
| **Nyctophobia** | Fear of Darkness<br/>(กลัวความมืด) | ความมืดที่ขยายตัวกลืนจอ (vignette) | เมื่อผู้เล่นยืนใน `LightArea` ที่ไฟดับ (`On == false` และไม่กำลัง flicker) | ออกจากความมืด กลับไปที่ที่มีแสง — `Nyctophobia` flag จะถูกล้างทุกเฟรมแล้วคำนวณใหม่ | *(ไม่มีคลาส — ใช้ `LightArea` + `UIManager`)* |
| **Claustrophobia** | Fear of Tight Spaces<br/>(กลัวที่แคบ) | — (เป็นเงื่อนไข ไม่ใช่ศัตรู) | ซ่อนในตู้นานเกิน 8 วินาที | ถูกบังคับออกจากตู้อัตโนมัติ + cooldown 3 วินาทีก่อนซ่อนใหม่ | *(ไม่มีคลาส — อยู่ใน `Player.ToggleHide`)* |
| **Entamaphobia** | Fear of Doors<br/>(กลัวประตู) | ร่างสูงผอมสีดำ หน้าเบี้ยว มีมือและขายาวผิดสัดส่วน | **ข้อเสนอใหม่** หลัง playtest — ยังไม่มีคลาส/asset | "Peek through door" — แอบมองใต้ช่องประตู + ส่องไฟฉาย (ดูหัวข้อถัดไป) | *(ยังไม่มีคลาส)* |

**ผลข้างเคียงจาก phobia**

- Autophobia: ลดขนาดแสงรอบตัวและลด Saturation เมื่อเดินชนขอบแสง · มี animation การหลับตาแล้วจึง reset ขนาดแสงและ Saturation
- Scopophobia: จอสั่น (ใช้ `CameraShaker`) และเสียงดังขึ้นเมื่อมันเข้าใกล้ · ไฟในฉากกลายเป็นสีแดง
- Enemy ทุกตัวชนกับ `LightArea`: Autophobia ทำให้ไฟ **กระพริบ** (`StartFlicker`) ส่วนตัวอื่นทำให้ไฟ **ดับ** (`TurnOff`)

---

### Entamaphobia — รายละเอียดคอนเซปต์

> **ที่มา:** ข้อเสนอใหม่หลังผล playtest (ดู [standup week 14](../04_Agile/standups/standup-week-14.md)) — คอนเซปต์ฉบับเต็มอยู่ในไฟล์ `entamaphobia concept.pdf`
> **สถานะ:** ยังไม่มีคลาสในโค้ด และยังไม่มี asset — เป็นข้อเสนอที่ต้อง implement

**Concept — รูปกาย**

ร่างของความกลัวประตู หน้าตาเหมือน **ผีที่ยืนอยู่หลังตู้เสื้อผ้าที่ปิดอยู่** ตามจินตนาการตอนเด็ก — สูงมาก ผอมผิดปกติ หน้าเบี้ยว มีมือและขาใหญ่ยาว ให้ความรู้สึกว่า "ตัวมันใหญ่จนไม่น่าจะใส่ในที่แคบได้ แต่ก็ใส่ได้" (ตัวใหญ่กว่าผู้เล่นหลายเท่า — ดูภาพ size comparison ในไฟล์คอนเซปต์)

**Behavior**

- จะพยายามตามหาผู้เล่น
- คนที่เป็น Entamaphobia ไม่ได้กลัว *ประตู* แต่กลัว **"สิ่งที่อยู่หลังประตู"** ซึ่งมักเป็นผู้บุกรุกหรือผี
- **กลัวแสง — เข้าห้องที่มีแสงไม่ได้**

**Encounter**

- เกิดในโถงอุโมง (sewer) **ขณะที่ผู้เล่นอยู่ในห้องไฟฟ้าห้องใดห้องหนึ่ง** แล้วรอโจมตี
- มันจะเริ่มพึมพำ (mumbling) อยู่หน้าประตู — ถ้าเปิดประตูออกโดยไม่ตรวจสอบก่อน จะถูกโจมตี

**5 ระยะ (Stage 0–4)** — 0 คือยังไม่เกิด, 1 น้อยที่สุด, 4 อันตรายที่สุด

| Stage | สิ่งที่เห็น | คำอธิบาย |
|---|---|---|
| **0** | ด้านนอกว่างเปล่า | **It** ยังไม่เกิด |
| **1** | เห็นแค่ขา | **It** เกิดแล้ว ยืนอยู่หน้าประตู หันหน้าเข้าหาผู้เล่น เอนตัวลงมาที่ช่องประตู |
| **2** | เห็นมือกดที่ช่องประตู | **It** พยายามดันมือผ่านช่องประตูเข้ามา ใกล้ขึ้นจนมองไม่เห็นเท้าแล้ว |
| **3** | เห็นมือบีบผ่านช่อง | **It** กำลังพยายามบีบตัวผ่านช่องประตู ใกล้ขึ้นอีก |
| **4** | เห็นหน้า | **It** กำลังมองผู้เล่นอยู่ |

**กลไกการเกิดและเลื่อนระยะ**

- มีโอกาสเกิด ~**10% ทุก 5 วินาที**
- เมื่อเกิดแล้ว ค่าหนึ่งจะเพิ่มขึ้นเรื่อย ๆ จนถึงค่าสูงสุดที่สุ่มไว้ — ถ้าถึงค่าสูงสุด **It** จะเลื่อน stage ขึ้น
- stage ที่สูงขึ้นมีค่าสูงสุดที่สุ่มได้สูงกว่า → ใช้เวลานานขึ้นในการเลื่อน stage ถัดไป
- **โอกาสเกิดและค่าสูงสุดได้รับผลจาก sanity state**

**วิธีรับมือ (Solution)**

- **แอบมองใต้ช่องประตู** (ปุ่มที่เสนอ: `E`) แล้ว **ส่องไฟฉาย** — จะรู้ว่ามันอยู่ตรงนั้นหรือไม่
- เมื่อมันหายไปแล้วให้มี **cooldown** เพื่อไม่ให้มันเกิดกลับมาทันทีที่เปิดประตู
- ระหว่างแอบมอง **ค่าการเพิ่ม stage จะหยุดชั่วคราว**
- ถ้าส่องไฟฉายใส่ **It** ค่าจะรีเซ็ตกลับเป็น 0
- ถ้าส่องไฟฉายต่อเนื่องเป็นระยะเวลาหนึ่ง stage จะ **ลดลง** จนถึง 0 → **It** หายไป + เริ่ม cooldown ก่อนจะเกิดใหม่ได้
- ทุกครั้งที่ stage ลดลง **จอจะพัลส์เป็นสีดำ**
- stage ที่สูงกว่าจะใช้เวลาส่องไฟฉายนานขึ้นกว่าจะลด stage — อย่าปล่อยให้มันเลื่อน stage เลย
- **เวลาที่ใช้ส่องไฟฉายไม่ได้รับผลจาก sanity state**

**Inspirations:** Mandela Catalogue · SCP-096 · Kitty Backrooms · จินตนาการของผู้เขียน

> **TBD:** ค่าตัวเลขทั้งหมด (โอกาสเกิด, ช่วงของค่าสูงสุด, cooldown, เวลาส่องไฟฉายต่อ stage) ยังเป็นข้อเสนอ — ต้องตัดสินใจและย้ายไปไว้ใน data file ตาม [Data Schema](../03_Software/03-data-schema.md)
> **TBD:** ปุ่ม `E` ยังไม่ถูกผูกใน `InputManager` — ปัจจุบันมีแค่ `A`/`D`/`F`/`Space`/`Left Ctrl`/`Q`

---

## Minigames

มินิเกมที่ใช้จริงอยู่ใน `Scripts/Screen/` และสืบทอดจาก `Screen` ของ MonoGame.Extended (มี `Update(GameTime)` / `Draw(GameTime)`)

| Minigame | ใช้กับ | Trigger tag | วิธีเล่น | รายละเอียด |
|---|---|---|---|---|
| **Spin The Valve** | ซ่อมวาล์ว/ท่อ (pipe) | `skillcheck` | กดเมาส์ค้างแล้วลากเป็นวงกลมเพื่อหมุนวาล์ว ให้เข็มอยู่ในเส้นสีเขียว | เป้าหมาย 5 วาล์ว (`ObjectiveManager.TargetValves`) · progress 0–200 · เข็มสุ่มความเร็วใหม่ทุก 5 วินาที · `SpinTheValveScreen.cs` |
| **Connect The Dots** | ซ่อมไฟ (electric panel) | `dots` | ลากเมาส์เชื่อมจุดสีเดียวกันให้ครบทุกคู่ | กระดาน **6×6** (`GridCell` / `GeneratedDotsMap` / `DotsGenerator`) · สูงสุด 5 สี · ทำครบ 5 รอบ (`_maxRepairStage`) จึงเต็ม · คลิกขวา = ล้างเส้นที่ลากไว้ · ซ่อมสำเร็จแล้ว `SaveManager.TurnAllLightsOn()` · `ConnectTheDotsScreen.cs` |
| **Skill Check** | *(ไม่ได้ใช้แล้ว)* | — | กด Spacebar เมื่อเข็มชี้ที่แถบสำเร็จ | `SkillCheckScreen.cs` **ไม่มีที่ไหนเรียกใช้** — ถูกแทนที่ด้วย `SpinTheValveScreen` แล้ว (ดู [System Design § Known Issues](../03_Software/01-system-design.md#11-known-issues--technical-debt)) |

> ⚠️ **Known issue จาก Playtest:** มินิเกมได้คะแนนความยาก 4/5 — ยากเกินไป และไม่มี guide สอนวิธีเล่น ทำให้ผู้เล่นใหม่ค้าง (แก้บางส่วนแล้วด้วยระบบ tutorial — ดูหัวข้อ Objectives)

## Hide / Escape

- กด Space ขณะอยู่ใกล้ตู้ (locker) เพื่อเข้าไปซ่อน — ผู้เล่นจะถูกย้ายไปที่กลาง collider ของตู้
- ซ่อนได้นาน 8 วินาที แล้วถูกบังคับออกอัตโนมัติ (Claustrophobia) · ออกแล้วมี cooldown 3 วินาทีก่อนซ่อนใหม่
- ระหว่างซ่อน กล้องซูมเข้า (`FocusLevel = 1.25`) และ vignette หด
- ตู้ซ่อนตัวเป็นทางรับมือหลักของ Scopophobia

## Objectives

- **Repair** — ซ่อมวาล์ว 5 จุดในโถงอุโมง (`ObjectiveManager.TargetValves = 5`) และซ่อมแผงไฟในห้องไฟฟ้า
- **Hide / Escape** — หนีหรือซ่อนจากศัตรู
- **Navigation** — ประตู (`LoadNextScene`) พาไปฉากถัดไป · ประตูทางออก (`Destination == "end"`) ใช้ได้เมื่อซ่อมวาล์วครบเท่านั้น
- **Tutorial** — `TutorialManager` อ่าน `Content/Datas/tutorial.json` แสดงข้อความทีละขั้นตาม objective ที่ทำสำเร็จ (10 ขั้น ตั้งแต่ "Press A/D to walk around" จนถึง "Thank you for playing")
- HUD แสดง: แถบ Sanity · แถบ Battery · ตัวนับวาล์ว (`Valves repaired: n/5`) · ตัวจับเวลา 10 นาที (`MainTimer = 600s`)

**เงื่อนไขการดำเนินเกม**

- จบเกมเมื่อ: หมดเวลา 10 นาที (`Time.AddTimer(Restart, 600)`) / Sanity = 0 / ออกจากประตูทางออกหลังซ่อมวาล์วครบ
- ทุกกรณีเรียก `GameManager.Restart()` → รีเซ็ตผู้เล่น ลบ save และโหลด `tilemap_electrical_room` ใหม่

## Related Documents

- Core Loop: [Core Loop](01-core-loop.md)
- Concept: [Game Concept](00-concept.md)
- Narrative (phobia เป็นตัวละคร): [Narrative](07-narrative.md)
- Art Direction (การออกแบบ phobia): [Art Direction](08-art-direction.md)
- System Design: [System Design](../03_Software/01-system-design.md)
- Class Diagram: [Class Diagram](../03_Software/02-class-diagram.md)
- Data Schema: [Data Schema](../03_Software/03-data-schema.md)
