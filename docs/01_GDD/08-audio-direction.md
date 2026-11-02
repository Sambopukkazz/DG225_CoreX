---
type: gdd-audio-direction
version: 2.0
date: 2026-11-02
---
# Imaginophobia — Audio Direction

**Version:** 2.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

## บทบาทของเสียงในเกม

เป็นสัญญาณหลักที่บอกให้ผู้เล่นรู้ว่า phobia กำลังมา (ยกเว้น Autophobia ซึ่งมีแค่เสียงฝีเท้า)

## แนวทางเสียง

| ประเภท           | แนวทาง                                                                                                                                                         |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Ambience**     | เสียงบรรยากาศต่อเนื่อง สร้างความอึดอัด — น้ำหยด ความชื้น เสียงสไลม์ (ดูตามสภาพแวดล้อม) · สลับอัตโนมัติตามฉาก (`AmbSewer` / `AmbElectric`) |
| **SFX**          | เสียงตอบสนองการกระทำ (เปิด/ปิดประตู, ซ่อมท่อ, ซ่อมไฟ, เปิด/ปิดตู้, ฝีเท้า, พลิกหน้าคลิปบอร์ด)                              |
| **Indicator**    | เสียงเตือนก่อน phobia มา — `ScopoIndicator` (Spatial 5000/26500) · `EntamaAdvance` / `EntamaPresence` สำหรับ Entamaphobia |
| **Spatial (3D)** | เสียงดัง/เบาตามระยะห่างจากผู้เล่น และแยกซ้าย-ขวาได้                                                                  |
| **BGM**          | ครบทั้ง 3 แทร็ก (เมนูหลัก / จอตาย / จอชนะ) — เล่นวนซ้ำผ่าน `MediaPlayer`                                                                                 |

## Spatial Audio

เกมใช้ระบบ 3D/spatial audio — เสียง phobia จะดังขึ้นเมื่อมันเข้าใกล้ และแยกซ้าย-ขวาได้ ทำให้ผู้เล่นใช้หูระบุตำแหน่งได้

โค้ดที่เกี่ยวข้อง: `AudioManager.Play3DSFX(Vector2 pos, SoundEffectName name, float innerRadius = 10, float outerRadius = 80, float volume = 1)`, `AudioSource.UpdateSpatialAudio(Player player)` (คำนวณ volume จากระยะ, `Pan` จากแกน X, `Pitch` จาก Doppler — `SPEED_OF_SOUND = 343.5`)

ระยะที่ตั้งไว้จริงใน Release 1.0.0:

| เสียง | inner / outer |
| --- | --- |
| Scopophobia indicator | 5000 / 26500 |
| Entamaphobia advance / decrease | 60 / 1920 |
| ไฟแตก (`LightBreak`) | 50 / 1800 |
| ฝีเท้าผู้เล่น | 10 / 150 |
| ฝีเท้า Autophobia | 10 / 1200 |
| ไฟกระพริบ (`PlayLightFlickerSFX`) | 50 / 1800 |

## ขั้นตอนการหาและตัดเสียง

1. **ค้นหาจาก Pixabay** — พิมพ์คีย์เวิร์ดตามอารมณ์ที่ต้องการ
   - ตัวอย่างจริง: หาเสียงของ Scopophobia → ใช้คีย์เวิร์ด `creepy atmosphere` (ต้องการเสียงที่ฟังดูน่าขนลุก)
2. **ตัดเสียงด้วย CapCut**
   - ตัดให้เล่นวนซ้ำได้เนียนไม่สะดุด
   - ปรับความดังให้ **เท่ากันทุกเสียง** — สำคัญมาก เพราะเสียงที่ดังไม่เท่ากันจะทำให้ผู้เล่นสับสน
3. **นำเข้า** ตามขั้นตอนใน [Asset Pipeline Flow](05-asset-pipeline-flow.md)

## รายการเสียงปัจจุบัน

ไฟล์ทั้งหมดใน `Content/Audio/` เป็นนามสกุล `.mp3` และลงทะเบียนใน `Content.mgcb` ครบ **46 ไฟล์** (ดูตารางเต็มพร้อมที่มาใน [Asset List](04-asset-list.md#audio--sfx))

| หมวด          | ไฟล์                                                                                                              | จำนวน   |
| ----------------- | --------------------------------------------------------------------------------------------------------------------- | ------------ |
| Ambience          | `amb_sewer.mp3`, `amb_electricroom.mp3`                                                                           | 2            |
| Music (BGM)       | `bgm_mainmenu_screen.mp3`, `bgm_death_screen.mp3`, `bgm_win_screen.mp3`                                         | 3            |
| Pipe repair       | `sfx_pipe_repairing/success/failed/complete.mp3`                                                                    | 4            |
| Electric repair   | `sfx_electric_repairing/success/failed/complete.mp3`                                                                | 4            |
| Door              | `sfx_door_open/close/stuck.mp3`                                                                                     | 3            |
| Locker            | `sfx_locker_open/close.mp3`                                                                                         | 2            |
| Phobia feedback   | `sfx_scopophobia_indicator.mp3`, `sfx_phobias_hit.mp3`                                                            | 2            |
| Entamaphobia      | `sfx_entama_advance.mp3`, `sfx_entama_decrease.mp3`, `sfx_entama_presence.mp3`, `sfx_entama_jumpscare.mp3`      | 4            |
| Clipboard         | `sfx_clipboard_pageturn.mp3`, `sfx_clipboard_pageturnback.mp3`, `sfx_clipboard_check.mp3`                       | 3            |
| Sanity            | `sfx_heartbeat.mp3`, `sfx_panting.mp3`                                                                            | 2            |
| Light / darkness  | `sfx_light_break.mp3`, `sfx_darkness.mp3`, `sfx_light_flicker_long.mp3`, `sfx_light_flicker_short_01..03.mp3` | 6            |
| Screen (blackout) | `sfx_screen_off.mp3`, `sfx_screen_on.mp3`                                                                         | 2            |
| Flashlight        | `sfx_flashlight_toggle.mp3`                                                                                         | 1            |
| Footsteps         | `sfx_step_01..08.mp3`                                                                                               | 8            |
| **รวม**  |                                                                                                                       | **46** |

**ไฟล์ที่โหลดไว้แต่ไม่มีจุดเรียกเล่นในโค้ด:** `sfx_pipe_success.mp3` (`PipeSuccess`) และ `sfx_door_open.mp3` (`DoorOpen`) — ทั้งคู่ยังอยู่ใน `SfxByName`

รายการเต็มพร้อมที่มา: [Asset List](04-asset-list.md#audio--sfx)

## Music (BGM)

| Asset                              | เล่นเมื่อ | สถานะ                                                                                  |
| ---------------------------------- | ------------------ | ------------------------------------------------------------------------------------------- |
| `bgm_mainmenu_screen.mp3`        | เมนูหลัก   | ใช้งาน (`BackgroundSound.BgmMainMenu`)                                           |
| `bgm_death_screen.mp3`           | จอตาย         | ใช้งาน (`BackgroundSound.BgmDeathScreen`)                                        |
| `bgm_win_screen.mp3`             | จอชนะ         | ใช้งาน (`BackgroundSound.BgmWinScreen` — เล่นใน `WinScreen.Initialize`)         |

> ระดับเสียง default ของ BGM/ambience = `AudioManager.BgsVolume` (0.2) และปรับได้จากสไลเดอร์ "Ambience Volume" ใน Settings / pause overlay
> ระหว่างอ่านคลิปบอร์ดและตอนแอบมองประตู `AudioManager.ChangeMediaVolume` จะหรี่เสียง ambience/BGM ชั่วคราว (×0.5) แล้วคืนค่าเดิมเมื่อออก

## สถานะงานเสียงเทียบกับผล Playtest (25/09/2569)

| ประเด็น                                                       | สถานะในโค้ด Release 1.0.0                                                                                          |
| -------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Visual Indicator ตอน monster เกิด (สำหรับคนไม่ใส่หูฟัง) | Scopophobia: จอดำ 1 วินาที + ไฟทั้งฉากเป็นสีแดง · Nyctophobia: ไฟกระพริบ 3 วินาทีแล้วดับ · Autophobia: ไม่มี indicator |
| เสียงทุกไฟล์ดังเท่ากันจริงหรือไม่              | —                                                                                                                               |
| เสียง Jumpscare                                                  | มีแล้ว — `sfx_entama_jumpscare.mp3` (volume 0.2)                                                                  |
| เสียงสำหรับ phobia ที่ยังไม่มี indicator            | Nyctophobia มี `LightFlickerLong` ตอนเกิด · Claustrophobia ไม่มีเสียงของตัวเอง                              |
| Entamaphobia — เสียงพึมพำ / เลื่อน stage / ลด stage       | มีแล้ว — `EntamaPresence` (วนตั้งแต่ stage 3) · `EntamaAdvance` · `EntamaDecrease`                                |

## Related Documents

- Asset list: [Asset List](04-asset-list.md)
- Pipeline: [Asset Pipeline Flow](05-asset-pipeline-flow.md)
- Art: [Art Direction](07-art-direction.md)
- Playtest: [Report Backlog](../04_Agile/05-report-backlog.md)
