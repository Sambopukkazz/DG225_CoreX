---
type: gdd-scope
version: 2.0
date: 2026-11-02
---
# Imaginophobia — Scope & Feature List

**Version:** 2.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

## Unique Selling Point (USP)

1. **ศัตรูไม่ใช่ของจริง** — ทุกอย่างที่ไล่ล่าผู้เล่นคือภาพหลอนจากความกลัวของตัวละครเอง ผู้เล่นจะค่อย ๆ รู้ตัวว่าไม่มีอะไรให้หนีตั้งแต่แรก (twist mechanic)
2. **ความกลัวกลายเป็นกลไกเกม** — แต่ละ phobia ถูกแปลงจาก "คำอธิบายของความกลัว" เป็น mechanic ที่เล่นได้จริง (เช่น กลัวการถูกมอง → ต้องหลบในตู้ / ทำลายแสง)
3. **พื้นที่จำกัดแต่เหตุการณ์หลากหลาย** — ใช้ข้อจำกัดของพื้นที่ให้คุ้มค่า ทำให้ต้นทุนการสร้างต่ำแต่เล่นซ้ำได้หลายรอบ (Low-Budget, High-Impact)

## In Scope — สิ่งที่อยู่ในเกม (Release 1.0.0)

| #  | Feature                                                                                                       | Priority | สถานะในโค้ด                                                                                                                                                                                                                                                                                              |
| -- | ------------------------------------------------------------------------------------------------------------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | Player เดิน/เคลื่อนที่ + อนิเมชั่น                                                     | Must     | มีแล้ว — `Player` เดินแกน X (`MoveSpeed = 250`), animation `walk-forward` (9 เฟรม) / `sneak-forward` / `sneak-backward` / `idle` (12 เฟรม) / `back` / `check`                                                       |
| 2  | ระบบ Collision + Scene ที่สร้างจาก tilemap                                                     | Must     | มีแล้ว — `CollisionManager` 4 layer (`walls` / `triggers` / `enemies` / `lights`) + `SceneBuilder` อ่าน `.ldtk` 3 ฉาก                                                                                                                                        |
| 3  | Manager หลัก (Game / Audio / Input / Screen / Scene)                                                     | Must     | มีแล้ว — `GameManager`, `AudioManager` (singleton), `InputManager`, `ModifiedScreenManager`, `SceneManager`                                                                                                                          |
| 4  | 3D / spatial audio + ambience                                                                                 | Must     | มีแล้ว — `AudioManager.Play3DSFX` / `PlayStepsSFX` + `BackgroundSound` 5 ค่า (ambience 2 + BGM 3)                                                                                                                            |
| 5  | มินิเกม Skill Check (ซ่อมท่อ)                                                                   | Must     | **ไม่มีในโค้ด** — ไม่มีคลาส `SkillCheckScreen` แล้ว ถูกแทนที่ด้วย Spin The Valve                                                                                                                                                                     |
| 6  | มินิเกม Connect The Dots (ซ่อมไฟ)                                                                | Must     | มีแล้ว — กระดาน 6×6, ซ่อม 5 รอบ (`_maxRepairStage`), ปลดล็อกไฟทั้งฉากเมื่อเสร็จ                                                                                                                                    |
| 7  | มินิเกม Spin The Valve                                                                                 | Should   | มีแล้ว — เป้าหมาย progress 200, เข็มสุ่มความเร็วทุก 5 วินาที                                                                                                                                                                                                    |
| 8  | ศัตรู: Autophobia, Scopophobia                                                                           | Must     | มีแล้ว — spawn เฉพาะฉาก `tilemap_sewer`, chance ตามระดับสติ                                                                                                                                                                        |
| 9  | การซ่อน (Hide / Claustrophobia timing)                                                                 | Must     | มีแล้ว — ซ่อนได้ 10 วินาที (`Player.HideTime`) + cooldown 3 วินาที (`Player.HideCooldown`) + แถบเวลาเหนือ prompt                                                                             |
| 10 | ค่า Sanity + Battery และผลต่อเกม                                                                | Must     | มีแล้ว — 3 ระดับ (80 / 40 / 0) เปลี่ยน vignette + สีแถบ + เสียง (heartbeat/panting) · ชน Autophobia ทำให้ทั้งจอสีซีด (`Saturation = 0`) · Nyctophobia ทำให้สติหมด → จอตาย · HUD มีแถบ Sanity + แถบ Battery |
| 11 | แสง + shader เอฟเฟกต์กระพริบตา / ถูกความมืดกลืน                             | Must     | มีแล้ว — Penumbra + `Vignette` / `BottomFade` / `Silhouette` / `HighlightComposite` / `Saturation`                                                                                                                          |
| 12 | UI: เมนู, Settings, HUD                                                                                   | Should   | มีแล้ว — `MainMenuScreen` (มี EyeField) / `SettingsScreen` / `CreditsScreen` (Gum) + pause overlay + สไลเดอร์ Ambience/SFX volume                                                                                                   |
| 13 | ระบบข้อความในเกม (Tutorial / HUD text)                                                                   | Should   | มีแล้ว — `TextManager` + `Datas/tutorial.json` (10 ขั้น)                                                                                                                                                                                                                                            |
| 14 | ระบบ Save game                                                                                            | Should   | มีแล้ว — `SaveManager` บันทึก progress ของ trigger + สถานะไฟต่อฉากที่ `%APPDATA%/Imaginophobia/save.json` (ไม่บันทึกตำแหน่ง/สติ/แบต)                                                                                                                                                                        |
| 15 | Tutorial / Guide ก่อนเริ่มมินิเกม                                                             | Should   | **เพิ่มจากผล Playtest** — มีแล้ว (tutorial 10 ขั้นตาม objective)                                                                                                                                                |
| 16 | บันทึก progress ของ task (ออกกลางคันกลับมาไม่เริ่มใหม่)                  | Should   | **เพิ่มจากผล Playtest** — มีแล้ว (`SaveManager.SaveScene` / `ApplyToScene`)                                                                                                                                                                                                                                     |
| 17 | Redesign monster + jumpscare                                                                                  | Should   | **เพิ่มจากผล Playtest** — มีแล้ว — `JumpScareScreen` + `spritesheet_entamaphobia` + เสียง `EntamaJumpscare`                                                                                                                                    |
| 18 | บทพูดของตัวละครเบื้องต้น                                                              | Could    | ไม่มีในโค้ด — เรื่องราวเล่าผ่านคลิปบอร์ดแทน                                                                                                                                                                                                                              |
| 19 | Entamaphobia (Fear of Doors) — ศัตรูใหม่: แอบมองใต้ช่องประตู + ส่องไฟฉาย | Should   | **ข้อเสนอใหม่หลัง Playtest** — มีแล้ว — spawn นอกฉาก sewer, 4 stage, jumpscare เมื่อเปิดประตูตอนมัน active (ดู [Mechanics](03-mechanics.md#entamaphobia)) |
| 20 | จอจบเกม (Death / Win) + ทางกลับเมนู                                                         | Should   | มีแล้ว — `DeathScreen` (tip สุ่ม 9 ข้อความ) · `WinScreen` (แสดงเวลาที่เหลือ) · ทั้งคู่มีปุ่มกลับเมนู                                                                                                                                           |

## Out of Scope — สิ่งที่ "จะไม่ทำ"

| สิ่งที่ตัดออก                       | เหตุผล                                                                                                                                            |
| ------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| เนื้อหาแนว Lovecraft                   | นำออกเพื่อปรับให้เป็นแนว Survival Horror ที่ชัดเจนขึ้น (ไม่พึ่งจักรวาล Mythos)                       |
| ศัตรูเหนือ supernatural จริง ๆ  | ขัดกับ twist หลัก — ทุกอย่างต้องเป็นภาพหลอน                                                                           |
| แผนที่แบบ metroidvania ขนาดใหญ่ | แผนที่ตอน Game Jam ซับซ้อนเกินไปและกินเวลาเดินมากเกินไป → ลดขนาดลง เน้น encounter และ task |
| ห้องไฟที่มี entity ได้             | เปลี่ยนแนวคิด — ห้องไฟต้องเป็น safe zone เพื่อให้ผู้เล่นมีจุดพัก                                     |
| งานภาพวาดมือ 2D                      | ทีมใช้ pixel art + asset ที่หา/ปรับจากแหล่งที่มา ไม่วาดใหม่ทั้งหมด                                         |
| แผนที่/ด่านใหม่เพิ่มเติม  | โฟกัสที่การขัด polish ของที่มีอยู่                                                                                            |

## ประเด็นจาก Playtest (25/09/2569) และสถานะในโค้ดปัจจุบัน

| ประเด็นที่ผู้ทดสอบรายงาน                                 | คะแนน / จำนวนคน                     | สถานะในโค้ด Release 1.0.0                                                                                                                                                                                                       |
| ------------------------------------------------------------------------- | ------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ความน่ากลัวของ Monster                                             | 2.5 / 5                                      | มี 4 phobia (`Autophobia` · `Scopophobia` · `Nyctophobia` · `Entamaphobia`) และมี `JumpScareScreen` เมื่อเปิดประตูขณะ Entamaphobia active                                                                  |
| เกมพึ่งเสียงมากเกินไป (คนไม่ใส่หูฟังรับรู้ monster ไม่ได้) | 1 / 6 (ความเห็นเพิ่มเติม)      | `Scopophobia` มีสัญญาณที่ไม่ใช้เสียง: จอดำ 1 วินาที (`UIManager.BlackScreen`) + ไฟทั้งฉากเปลี่ยนเป็นสีแดง (`ScopoAlarmAll`) · `Nyctophobia` มีไฟกระพริบ 3 วินาทีแล้วดับ · `Autophobia` ยังมีแค่เสียงฝีเท้า (`PlayStepsSFX`) |
| ค่า Sanity ไม่ส่งผลต่อ gameplay และไม่รู้ว่ามีอยู่         | 2 / 6                                       | Sanity ส่งผลต่อ vignette (3 ระดับ), สีของแถบ HUD, เสียง heartbeat/panting, ความเร็วการลดสติในโถงอุโมง และการตายเมื่อถึง 0 · HUD วาดแถบ Sanity (440px) + แถบ Battery  |
| Minigame ยากเกินไป                                          | 4 / 5 (ยากแต่ท้าทายดี)                    | มี tutorial 10 ขั้น + highlight วัตถุเป้าหมาย + ตัวนับวาล์วบน HUD + บันทึก progress และกด `Q` ออกได้ทุกจอ                                                       |
| Objective ไม่ชัดเจน                                                | 2 / 6                                       | tutorial ขั้นต่อขั้นผ่าน `ObjectiveManager` + ป้าย Spacebar prompt + ไฮไลต์วัตถุที่โต้ตอบได้ + ตัวนับ `Valves repaired: n/5`                                     |
| ประตู/ทางเดินกลืนกับพื้นหลัง                               | 2 / 6                                       | ประตูและบันไดมี highlight group ของตัวเอง (`Doors` / `Ladder`, outline `#e0e8e0`) และมีป้าย `Exit_sign` / `sewer_indicator_red` ในฉาก                     |
| ออกจาก Task แล้วค้าง                                      | 1 / 6                                       | ทุกหน้าจอ task ออกได้ด้วย `Q` (Spin The Valve / Connect The Dots / Door Peeking / Clipboard) และปิดผ่าน `ModifiedScreenManager.CloseScreen`      |
| ยังไม่มีเนื้อเรื่องในเกม                                        | 1 / 6                                       | คลิปบอร์ด 4 หน้าต่อห้องไฟฟ้า (er1 / er2) + หน้าลับ 1 หน้า เปิดอ่านด้วย `Space`                                    |
| ตัวละครแยกจากพื้นหลังชัดเจน                              | 4.75 / 5                                    | คงเดิม                                                                                                                                                                                                                          |
| แยกห้อง/ทางเดินชัดเจน                                       | 3.75 / 5                                    | —                                                                                                                                                                                                                                    |
| สัญญาณเตือนก่อนเจอ phobia                                    | 4.0 / 5                                     | —                                                                                                                                                                                                                                    |

> ที่มาของคะแนนทั้งหมด: [Playtesting Report](../04_Agile/reports/playtest-2026-09-25.md) (25/09/2569)

## Related Documents

- Concept: [Game Concept](00-concept.md)
- Core Loop: [Core Loop](01-core-loop.md)
- Backlog: [Product Backlog](../04_Agile/01-product-backlog.md)
- Reports: [Report Backlog](../04_Agile/05-report-backlog.md)
