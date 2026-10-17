---
type: gdd-audio-direction
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Audio Direction

**Version:** 1.0 | **Last Updated:** 2026-10-14

## บทบาทของเสียงในเกม

เสียงเป็น **กลไก gameplay ไม่ใช่แค่บรรยากาศ** — มันคือช่องทางหลักที่ผู้เล่นรับรู้ว่า phobia กำลังมา

> ⚠️ **ข้อควรระวัง:** ผล Playtest พบว่าเกมพึ่งเสียงมากเกินไป จน **ผู้เล่นที่ไม่ได้ใส่หูฟังไม่สามารถรับรู้การเคลื่อนไหวของ monster ได้เลย** และไม่สามารถคาดเดาได้ → ต้องเพิ่ม Visual Indicator ควบคู่กับเสียง

## แนวทางเสียง

| ประเภท | แนวทาง |
|---|---|
| **Ambience** | เสียงบรรยากาศต่อเนื่อง สร้างความอึดอัด — น้ำหยด ความชื้น เสียงสไลม์ (ดูตามสภาพแวดล้อม) |
| **SFX** | เสียงตอบสนองการกระทำ (เปิด/ปิดประตู, ซ่อมท่อ, ซ่อมไฟ, เปิด/ปิดตู้, ฝีเท้า) |
| **Indicator** | เสียงเตือนก่อน phobia มา — เป็นสัญญาณ gameplay ที่สำคัญที่สุด |
| **Spatial (3D)** | เสียงดัง/เบาตามระยะห่างจากผู้เล่น และแยกซ้าย-ขวาได้ |
| **BGM** | ยังไม่สมบูรณ์ — ดูหัวข้อ Music |

## Spatial Audio

เกมใช้ระบบ 3D/spatial audio — เสียง phobia จะดังขึ้นเมื่อมันเข้าใกล้ และแยกซ้าย-ขวาได้ ทำให้ผู้เล่นใช้หูระบุตำแหน่งได้

โค้ดที่เกี่ยวข้อง: `AudioManager.Play3DSFX(Vector2 pos, Sound name, float innerRadius = 10, float outerRadius = 80)`, `AudioSource.UpdateSpatialAudio(Player player)`

## ขั้นตอนการหาและตัดเสียง

1. **ค้นหาจาก Pixabay** — พิมพ์คีย์เวิร์ดตามอารมณ์ที่ต้องการ
   - ตัวอย่างจริง: หาเสียงของ Scopophobia → ใช้คีย์เวิร์ด `creepy atmosphere` (ต้องการเสียงที่ฟังดูน่าขนลุก)
2. **ตัดเสียงด้วย CapCut**
   - ตัดให้เล่นวนซ้ำได้อย่างราบรื่น (loop ไม่สะดุด)
   - ปรับความดังให้ **เท่ากันทุกเสียง** — สำคัญมาก เพราะเสียงที่ดังไม่เท่ากันจะทำให้ผู้เล่นสับสน
3. **นำเข้า** ตามขั้นตอนใน [Asset Pipeline Flow](06-asset-pipeline-flow.md)

## รายการเสียงปัจจุบัน

| หมวด | ไฟล์ | จำนวน |
|---|---|---|
| Ambience | `amb_sewer.mp3`, `amb_electricroom.mp3` | 2 |
| Pipe repair | `sfx_pipe_repairing/success/failed/complete.mp3` | 4 |
| Electric repair | `sfx_electric_repairing/success/failed/complete.mp3` | 4 |
| Door | `sfx_door_open/close/stuck.mp3` | 3 |
| Locker | `sfx_locker_open/close.mp3` | 2 |
| Phobia indicator | `sfx_scopophobia_indicator.mp3`, `sfx_autophobia_indicator.mp3` | 2 |
| Footsteps | `sfx_step_01..08.mp3` | 8 |
| **รวม** | | **25** |

รายการเต็มพร้อมที่มา: [Asset List](05-asset-list.md#audio--sfx)

## Music (BGM)

| Asset | แหล่งที่มา | สถานะ |
|---|---|---|
| `dark frequency` | https://freetouse.com/music/aylex/dark-frequency | 🔲 ระบุไว้ในเอกสารแล้ว แต่ **ยังไม่มีไฟล์ใน `Content/Audio/`** |

> **TBD:** ยังไม่ได้ตัดสินใจว่าเกมจะมี BGM กี่แทร็ก และจะเล่นตอนไหน (เมนู / ระหว่างเล่น / ช่วงหนี)

## งานที่ต้องทำ (จากผล Playtest)

- [ ] เพิ่ม Visual Indicator ตอน monster เกิด (สำหรับผู้เล่นที่ไม่ได้ใส่หูฟัง)
- [ ] ตรวจสอบว่าเสียงทุกไฟล์ดังเท่ากันจริง
- [ ] หาเสียง Jumpscare (อยู่ในแผน Sprint 3)
- [ ] เพิ่มเสียงสำหรับ phobia ที่ยังไม่มี indicator (Nyctophobia, Claustrophobia)

## Related Documents

- Asset list: [Asset List](05-asset-list.md)
- Pipeline: [Asset Pipeline Flow](06-asset-pipeline-flow.md)
- Art: [Art Direction](08-art-direction.md)
- Playtest: [Report Backlog](../04_Agile/05-report-backlog.md)
