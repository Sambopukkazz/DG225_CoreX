---
type: gdd-scope
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Scope & Feature List

**Version:** 1.2 | **Last Updated:** 2026-10-25

## Unique Selling Point (USP)

1. **ศัตรูไม่ใช่ของจริง** — ทุกอย่างที่ไล่ล่าผู้เล่นคือภาพหลอนจากความกลัวของตัวละครเอง ผู้เล่นจะค่อย ๆ รู้ตัวว่าไม่มีอะไรให้หนีตั้งแต่แรก (twist mechanic)
2. **ความกลัวกลายเป็นกลไกเกม** — แต่ละ phobia ถูกแปลงจาก "คำอธิบายของความกลัว" เป็น mechanic ที่เล่นได้จริง (เช่น กลัวการถูกมอง → ต้องหลบในตู้ / ทำลายแสง)
3. **พื้นที่จำกัดแต่เหตุการณ์หลากหลาย** — ใช้ข้อจำกัดของพื้นที่ให้คุ้มค่า ทำให้ต้นทุนการสร้างต่ำแต่เล่นซ้ำได้หลายรอบ (Low-Budget, High-Impact)

## In Scope — สิ่งที่ "จะทำ" ในภาคการศึกษานี้

| # | Feature | Priority | หมายเหตุ |
|---|---|---|---|
| 1 | Player เดิน/เคลื่อนที่ + อนิเมชั่น | Must | Sprint 1 — เสร็จแล้ว |
| 2 | ระบบ Collision + Scene ที่สร้างจาก tilemap | Must | Sprint 1–2 |
| 3 | Manager หลัก (Game / Audio / Input) | Must | Sprint 1 — เสร็จแล้ว |
| 4 | 3D / spatial audio + ambience | Must | Sprint 1–2 — เสร็จแล้ว |
| 5 | มินิเกม Skill Check (ซ่อมท่อ) | Must | Sprint 2 |
| 6 | มินิเกม Connect The Dots (ซ่อมไฟ) | Must | Sprint 3 |
| 7 | มินิเกม Spin The Valve | Should | Sprint 2 (ปรับจาก Skill Check) |
| 8 | ศัตรู: Autophobia, Scopophobia | Must | Sprint 2 |
| 9 | การซ่อน (Hide / Claustrophobia timing) | Must | Sprint 2–3 |
| 10 | ค่า Sanity + Battery และผลต่อเกม | Must | Sprint 3 — **ส่งผลต่อ gameplay แล้วบางส่วน**: ระดับสติเปลี่ยน vignette + เสียง (heartbeat/panting) · ชน Autophobia ทำให้ทั้งจอสีซีด · Nyctophobia ทำให้สติหมด → จอตาย · ยังขาด visual indicator ตัวเลขบน HUD |
| 11 | แสง + shader เอฟเฟกต์กระพริบตา / ถูกความมืดกลืน | Must | Sprint 3 |
| 12 | UI: เมนู, Settings, HUD | Should | **เสร็จแล้ว** — `MainMenuScreen` / `SettingsScreen` / `CreditsScreen` (Gum) + pause overlay + สไลเดอร์ volume |
| 13 | ระบบข้อความในเกม (Dialogue) | Should | Sprint 3 — `TextManager` + `Datas/tutorial.json` (10 ขั้น) |
| 14 | ระบบ Save game | Should | Sprint 3 (draft) — บันทึก progress ของ trigger + สถานะไฟต่อฉาก (ไม่บันทึกตำแหน่ง/สติ/แบต) |
| 15 | Tutorial / Guide ก่อนเริ่มมินิเกม | Should | **เพิ่มจากผล Playtest** — ทำแล้ว (tutorial 10 ขั้นตาม objective) |
| 16 | บันทึก progress ของ task (ออกกลางคันกลับมาไม่เริ่มใหม่) | Should | **เพิ่มจากผล Playtest** — ทำแล้ว (`SaveManager`) |
| 17 | Redesign monster + jumpscare | Should | **เพิ่มจากผล Playtest** — ยังไม่มี jumpscare ในโค้ด |
| 18 | บทพูดของตัวละครเบื้องต้น | Could | Sprint 3 (draft) |
| 19 | Entamaphobia (Fear of Doors) — ศัตรูใหม่: แอบมองใต้ช่องประตู + ส่องไฟฉาย | Should | **ข้อเสนอใหม่หลัง Playtest** — มีคลาส stub (`Enemies/Entamaphobia.cs`) + ปุ่ม `E` แอบมองประตูผูกแล้ว แต่ยังไม่ spawn และยังไม่มี asset/mechanic (ดู [Mechanics](03-mechanics.md#entamaphobia--รายละเอียดคอนเซปต์)) |
| 20 | จอจบเกม (Death / Win) + ทางกลับเมนู | Should | **ทำแล้ว** — `DeathScreen` (มี tip สุ่ม 9 ข้อความ) และ `WinScreen` |

## Out of Scope — สิ่งที่ "จะไม่ทำ"

| สิ่งที่ตัดออก | เหตุผล |
|---|---|
| เนื้อหาแนว Lovecraft | นำออกเพื่อปรับให้เป็นแนว Survival Horror ที่ชัดเจนขึ้น (ไม่พึ่งจักรวาล Mythos) |
| ศัตรูเหนือธรรมชาติจริง ๆ | ขัดกับ twist หลัก — ทุกอย่างต้องเป็นภาพหลอน |
| แผนที่แบบ metroidvania ขนาดใหญ่ | แผนที่ตอน Game Jam ซับซ้อนเกินไปและกินเวลาเดินมากเกินไป → ลดขนาดลง เน้น encounter และ task |
| ห้องไฟที่มี entity ได้ | เปลี่ยนแนวคิด — ห้องไฟต้องเป็น safe zone เพื่อให้ผู้เล่นมีจุดพัก |
| งานภาพวาดมือ 2D | ทีมใช้ pixel art + asset ที่หา/ปรับจากแหล่งที่มา ไม่วาดใหม่ทั้งหมด |
| แผนที่/ด่านใหม่เพิ่มเติม | โฟกัสที่การขัด polish ของที่มีอยู่ |

## Risks & Assumptions

| ความเสี่ยง | ผลกระทบ | แผนรับมือ |
|---|---|---|
| Monster ยังไม่น่ากลัวพอ (ผู้ทดสอบให้ 2.5/5) | เกมไม่น่ากลัว = เสียจุดขายหลัก | Redesign monster, เพิ่ม jumpscare, เพิ่มจำนวน/ความหลากหลายของมอน |
| เกมพึ่งเสียงมากเกินไป (คนไม่ใส่หูฟังเล่นไม่ได้) | ผู้เล่นกลุ่มใหญ่รับรู้ monster ไม่ได้ | เพิ่ม Visual Indicator ตอน monster เกิด |
| ค่า Sanity ไม่ส่งผลต่อ gameplay | ระบบที่ลงทุนไปกลายเป็นแค่ตัวเลข | ทำให้ Sanity ส่งผลจริง (ภาพบิดเบี้ยว/เอฟเฟกต์) ไม่ใช่แค่แสดงตัวเลข — **คืบหน้าแล้ว:** vignette + สีจอ + เสียงเปลี่ยนตามระดับสติ และสติหมด = จอตาย |
| Minigame ยากเกินไป (ผู้ทดสอบให้ 4/5) | ผู้เล่นใหม่ค้าง ไม่ไปต่อ | ทำง่ายขึ้น + มี guide วิธีเล่น + บันทึก progress |
| Objective ไม่ชัดเจน | ผู้เล่นหลง ไม่รู้ต้องทำอะไรต่อ | เพิ่ม hint / marker ระหว่างเล่น + ป้ายประกาศ |
| ประตู/ทางเดินกลืนกับพื้นหลัง | ผู้เล่นหาประตูไม่เจอ | ปรับสี/contrast + ทำ asset ประตูให้ต่างจาก locker |
| ออกจาก Task แล้วค้าง | เกมดูพัง | แก้ input การออกจาก task |
| ยังไม่มีเนื้อเรื่องในเกม | ผู้เล่นไม่เข้าใจบริบท | เพิ่มเนื้อเรื่องให้เข้าใจบริบทของเกม |

> ที่มาของความเสี่ยงทั้งหมด: [Playtesting Report](../04_Agile/05-report-backlog.md) (25/09/2569)

## Related Documents

- Concept: [Game Concept](00-concept.md)
- Core Loop: [Core Loop](01-core-loop.md)
- Backlog: [Product Backlog](../04_Agile/01-product-backlog.md)
- Reports: [Report Backlog](../04_Agile/05-report-backlog.md)
