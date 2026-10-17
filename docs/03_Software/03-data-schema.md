---
type: software-data-schema
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Data Schema

**Version:** 1.0 | **Last Updated:** 2026-10-14

> เอกสารนี้อธิบายโครงสร้างข้อมูลที่เกมใช้ ณ ปัจจุบัน
> อ้างอิง: [System Design](01-system-design.md) · [Class Diagram](02-class-diagram.md)

## 1. Enums (ค่าคงที่ในเกม)

ค่าคงที่ทั้งหมดประกาศเป็น `enum` ใน `Scripts/Enums/` และใช้เป็น key อ้างอิงทั้งในโค้ดและ asset

### `MentalState` — ระดับสติ

| ชื่อ | ค่า | ความหมาย |
|---|---|---|
| `Normal` | 75 | ปกติ |
| `Anxious` | 50 | เริ่มกังวล |
| `Insane` | 25 | สติแตกระดับวิกฤต |

### `CharacterAction` — สถานะการกระทำ

`Repairing` · `Checking` · `ReadyToHide` · `Hiding` · `Walking` · `Sneaking` · `Idle`

### `Scene` — ฉากที่โหลดได้

| ค่า | ไฟล์ |
|---|---|
| `tilemap_electrical_room` | `Content/Tilemaps/tilemap_electrical_room.ldtk` |
| `tilemap_electrical_room_2` | `Content/Tilemaps/tilemap_electrical_room_2.ldtk` |
| `tilemap_sewer` | `Content/Tilemaps/tilemap_sewer.ldtk` |

### `Sound` — เสียงที่เล่นได้

`DoorClose` · `DoorOpen` · `DoorStuck` · `LockerClose` · `LockerOpen` · `PipeComplete` · `PipeFail` · `PipeRepairing` · `PipeSuccess` · `ElecComplete` · `ElecFail` · `ElecRepairing` · `ElecSuccess` · `ScopoIndicator` · `AutoIndicator`

> ⚠️ **ข้อสังเกต:** enum `Sound` ใช้ชื่อ `PipeFail` / `ElecFail` แต่ไฟล์จริงชื่อ `sfx_pipe_failed.mp3` / `sfx_electric_failed.mp3` — ต้องตรวจสอบว่าการ map ชื่อทำงานถูกต้อง

## 2. ข้อมูลระดับ (Level Data)

### LDtk Tilemaps

ไฟล์ `.ldtk` 3 ไฟล์ใน `Content/Tilemaps/` เป็นแหล่งข้อมูลหลักของฉาก

`SceneBuilder` อ่านไฟล์เหล่านี้แล้วสร้าง:
- **TileMap** — ชั้นภาพและข้อมูล tile
- **Collider** — จาก layer ที่กำหนด (ส่งให้ `CollisionManager`)
- **Light** — จาก layer แสง (ส่งให้ `LightManager`)
- **PlayerSpawnPoints** — จุดเกิดผู้เล่น (`SpawnPoint { Name, Position }`)

| Entity ที่คาดว่ามีใน tilemap | ใช้ทำอะไร |
|---|---|
| `pipe_*` | จุดซ่อมท่อ |
| `elec_*` | จุดซ่อมไฟ |
| `door_*` | ประตู |
| `locker_*` | ตู้ซ่อนตัว |
| `spawn_*` | จุดเกิด |
| `light_*` | แหล่งแสง |

> **TBD:** ชื่อ entity จริงในไฟล์ `.ldtk` ยังไม่ได้รับการยืนยัน — ต้องเปิดใน LDtk editor แล้วบันทึกชื่อจริงลงตารางนี้

### Tileset (`mapassets/`)

| ไฟล์ | ใช้ทำอะไร |
|---|---|
| `spritesheet_background-texture.png` | Tileset พื้นหลังหลัก |
| `asset-1-export.png` | Tileset ชุดที่ 1 |
| `asset-2 (1)-export.png` | Tileset ชุดที่ 2 |
| `asset-3-Recovered (1)-export.png` | Tileset ชุดที่ 3 |
| `Cha1-export.png` | Tileset ตัวละคร/ฉาก |
| `Sprite-bgx400%_3.png` | พื้นหลังขยาย 400% |
| `black_opa-export.png` | พื้นทึบแสง (masking) |

## 3. Animation Data

`Content/Character/spitesheet_player.json` — ข้อมูล animation คู่กับ `spitesheet_player-texture.png`

โครงสร้างเป็น spritesheet + json ตามรูปแบบของ **MonoGame.Extended** (`SpriteSheet` / `AnimatedSprite`)

เวอร์ชันที่มีในโปรเจกต์:
- `Character/spitesheet_player.*` — เวอร์ชันใช้งานปัจจุบัน
- `Character/New_version/` — เวอร์ชัน redesign ล่าสุด
- `Character/Old_version/` — เวอร์ชันก่อน redesign (เก็บไว้เทียบ)

> **TBD:** รายชื่อ animation ทั้งหมด (idle, walk, sneak, hide, repair, check) ต้องอ่านจากไฟล์ `.json`

## 4. ค่าปรับแต่งเกม (Tuning Values)

> ⚠️ **ช่องว่างที่ใหญ่ที่สุดของโปรเจกต์นี้:** ปัจจุบัน **ยังไม่มีไฟล์ config/data กลาง** — ค่าปรับแต่งทั้งหมด hardcode อยู่ในโค้ด
> Product backlog ระบุความต้องการไว้ว่า *"enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling"*

| ค่า | ตำแหน่งปัจจุบัน | สถานะ |
|---|---|---|
| Player speed | `Player.cs` (`_speed`) | ⚠️ hardcode |
| Autophobia speed (มากกว่าผู้เล่น) | `Autophobia.cs` | ⚠️ hardcode |
| Autophobia ระยะการมองเห็น | `Autophobia.cs` | ⚠️ hardcode (สุ่ม) |
| Sanity อัตราการลด | `Player.cs` / `MentalState` | ⚠️ hardcode |
| Battery อัตราการลด | `Player.cs` | ⚠️ hardcode |
| Enemy spawn rate | `EnemyManager.cs` | ⚠️ hardcode |
| Skill Check ความเร็วเข็ม / ขนาดโซนสำเร็จ | `SkillCheckScreen.cs` | ⚠️ hardcode |
| ระยะ spatial audio (inner 10 / outer 80) | `AudioManager.Play3DSFX` | ⚠️ hardcode (มี default param) |

**ข้อเสนอ:** ย้ายค่าทั้งหมดไปไว้ใน `Content/Data/*.json` แล้วโหลดตอนเริ่มเกม — จะทำให้บาลานซ์เกมได้โดยไม่ต้อง build ใหม่ (ตรงกับความต้องการใน backlog)

## 5. Save Game

**สถานะ: ยังไม่มี** — อยู่ในแผน Sprint 3 (draft)

ผล Playtest ระบุว่าต้องมี *"ระบบบันทึก progress — ถ้าเดินออกกลางคัน กลับมาแล้วไม่ต้องเริ่มใหม่"*

ข้อมูลที่ต้องบันทึก (ข้อเสนอ):

| ข้อมูล | เหตุผล |
|---|---|
| ฉากปัจจุบัน | กลับมาเล่นต่อจากที่เดิม |
| ตำแหน่งผู้เล่น | — |
| รายการ task ที่ซ่อมเสร็จแล้ว | ป้องกันการเริ่มมินิเกมใหม่ |
| ค่า Sanity / Battery | คงสภาพเดิม |
| ข้อความตาย/ชนะที่เก็บได้ | รองรับ Achievement Hunter |

> **TBD:** ยังไม่ตัดสินใจรูปแบบไฟล์ (JSON / binary) และตำแหน่งเก็บ

## Related Documents

- System Design: [System Design](01-system-design.md)
- Class Diagram: [Class Diagram](02-class-diagram.md)
- Asset List: [Asset List](../01_GDD/05-asset-list.md)
- Asset Pipeline: [Asset Pipeline Flow](../01_GDD/06-asset-pipeline-flow.md)
- Mechanics: [Mechanics](../01_GDD/03-mechanics.md)
