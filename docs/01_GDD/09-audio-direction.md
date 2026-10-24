---
type: gdd-audio-direction
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Audio Direction

**Version:** 1.2 | **Last Updated:** 2026-10-25

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

โค้ดที่เกี่ยวข้อง: `AudioManager.Play3DSFX(Vector2 pos, SoundEffectName name, float innerRadius = 10, float outerRadius = 80, float volume = 1)`, `AudioSource.UpdateSpatialAudio(Player player)`

## ขั้นตอนการหาและตัดเสียง

1. **ค้นหาจาก Pixabay** — พิมพ์คีย์เวิร์ดตามอารมณ์ที่ต้องการ
   - ตัวอย่างจริง: หาเสียงของ Scopophobia → ใช้คีย์เวิร์ด `creepy atmosphere` (ต้องการเสียงที่ฟังดูน่าขนลุก)
2. **ตัดเสียงด้วย CapCut**
   - ตัดให้เล่นวนซ้ำได้อย่างราบรื่น (loop ไม่สะดุด)
   - ปรับความดังให้ **เท่ากันทุกเสียง** — สำคัญมาก เพราะเสียงที่ดังไม่เท่ากันจะทำให้ผู้เล่นสับสน
3. **นำเข้า** ตามขั้นตอนใน [Asset Pipeline Flow](06-asset-pipeline-flow.md)

## รายการเสียงปัจจุบัน

ไฟล์ทั้งหมดใน `Content/Audio/` เป็นนามสกุล `.mp3` และลงทะเบียนใน `Content.mgcb` ครบ 39 ไฟล์ (ดูตารางเต็มใน [Asset List](05-asset-list.md#audio--sfx))

| หมวด | ไฟล์ | จำนวน |
|---|---|---|
| Ambience | `amb_sewer.mp3`, `amb_electricroom.mp3` | 2 |
| Music (BGM) | `bgm_mainmenu_screen.mp3`, `bgm_death_screen.mp3`, `bgm_win_screen.mp3` | 3 |
| Pipe repair | `sfx_pipe_repairing/success/failed/complete.mp3` | 4 |
| Electric repair | `sfx_electric_repairing/success/failed/complete.mp3` | 4 |
| Door | `sfx_door_open/close/stuck.mp3` | 3 |
| Locker | `sfx_locker_open/close.mp3` | 2 |
| Phobia feedback | `sfx_scopophobia_indicator.mp3`, `sfx_phobias_hit.mp3` | 2 |
| Sanity | `sfx_heartbeat.mp3`, `sfx_panting.mp3` | 2 |
| Light / darkness | `sfx_light_break.mp3`, `sfx_darkness.mp3`, `sfx_light_flicker_long.mp3`, `sfx_light_flicker_short_01..03.mp3` | 6 |
| Screen (blackout) | `sfx_screen_off.mp3`, `sfx_screen_on.mp3` | 2 |
| Flashlight | `sfx_flashlight_toggle.mp3` | 1 |
| Footsteps | `sfx_step_01..08.mp3` | 8 |
| **รวม** | | **39** |

> ⚠️ `sfx_autophobia_indicator.mp3` (ที่เอกสารและค่า enum `AutoIndicator` เคยอ้าง) **ไม่มีใน source แล้ว**

รายการเต็มพร้อมที่มา: [Asset List](05-asset-list.md#audio--sfx)

## Music (BGM)

| Asset | เล่นเมื่อ | สถานะ |
|---|---|---|
| `bgm_mainmenu_screen.mp3` | เมนูหลัก | ✅ ใช้งาน (`BackgroundSound.BgmMainMenu`) |
| `bgm_death_screen.mp3` | จอตาย | ✅ ใช้งาน (`BackgroundSound.BgmDeathScreen`) |
| `bgm_win_screen.mp3` | จอชนะ | 🔲 ผูกใน enum/dictionary แล้ว แต่ยังไม่มีหน้าจอไหนเรียก |
| `dark frequency` (freetouse.com) | — | ❌ เอกสารเดิมอ้างไว้ แต่ไม่พบไฟล์นี้ใน `Content/Audio/` |

> ระดับเสียง default ของ BGM/ambience = `AudioManager.BgsVolume` (0.2) และปรับได้จากสไลเดอร์ "Ambience Volume" ใน Settings / pause overlay

> **TBD:** ยังไม่ได้ตัดสินใจว่าเกมจะมีเพลงระหว่างเล่น (in-game) ไหม — ปัจจุบันระหว่างเล่นมีแค่ ambience

## งานที่ต้องทำ (จากผล Playtest)

- [ ] เพิ่ม Visual Indicator ตอน monster เกิด (สำหรับผู้เล่นที่ไม่ได้ใส่หูฟัง)
- [ ] ตรวจสอบว่าเสียงทุกไฟล์ดังเท่ากันจริง
- [ ] หาเสียง Jumpscare (อยู่ในแผน Sprint 3)
- [ ] เพิ่มเสียงสำหรับ phobia ที่ยังไม่มี indicator (Nyctophobia, Claustrophobia)
- [ ] **Entamaphobia** — เสียงพึมพำ (mumbling) อยู่หน้าประตูตอนมันเกิด · เสียงตอน stage เลื่อนขึ้น · เสียงตอนถูกไฟฉายส่องจน stage ลด · เสียงจอพัลส์ดำ (ดู [Mechanics](03-mechanics.md#entamaphobia--รายละเอียดคอนเซปต์))

## Related Documents

- Asset list: [Asset List](05-asset-list.md)
- Pipeline: [Asset Pipeline Flow](06-asset-pipeline-flow.md)
- Art: [Art Direction](08-art-direction.md)
- Playtest: [Report Backlog](../04_Agile/05-report-backlog.md)
