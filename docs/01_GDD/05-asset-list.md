---
type: asset-list
version: 2.0
date: 2026-10-24
---
# Asset List — Imaginophobia

**Version:** 2.0 | **Last Updated:** 2026-10-24

> ตารางนี้อิงจากไฟล์จริงใน `src/Imaginophonia/Imaginophonia/Content/` (ไม่นับ `bin/` และ `obj/` ที่เป็น build artifact) และจาก `Content/Content.mgcb`
> Asset ที่ยังไม่มีให้ใส่ใน `docs/02_Assets/_candidates/` ก่อน แล้วค่อยย้ายเข้า `Content/` ตาม [Asset Pipeline Flow](06-asset-pipeline-flow.md)
> คอลัมน์ **mgcb** = ลงทะเบียนใน `Content.mgcb` แล้วหรือยัง (ถ้าไม่ลงทะเบียน เกมจะโหลดไม่ได้)

## Sprites / Textures

| Asset Name | คำอธิบาย | mgcb | โหลดโดย | รับผิดชอบ |
|---|---|---|---|---|
| `Character/spritesheet_player-texture.png` + `.json` | Spritesheet ตัวละครหลัก (960×360, เฟรม 120×120) + animation data | ✅ | `Player`, `EnemyManager` (Autophobia ย้อมดำ) | นาถวัฒน์ |
| `Character/New_version/spitesheet_player.*` | ตัวละครเวอร์ชัน redesign | ❌ | — (ยังไม่ถูกโหลด) | นาถวัฒน์ |
| `Character/Old_version/spitesheet_player.*` | ตัวละครเวอร์ชันก่อน redesign (เก็บไว้เทียบ) | ❌ | — | นาถวัฒน์ |
| `Phobias/sprite_scopophobia.png` | Entity ของ Scopophobia | ✅ | `EnemyManager` | นาถวัฒน์ |
| `UI/sprite_spacebar.png` | ปุ่ม Spacebar บน UI (แจ้งว่าโต้ตอบได้) | ✅ | `UIManager` | พรภวิษย์ |
| `UI/sprite_valve.png` | วาล์วสำหรับมินิเกม Spin The Valve | ✅ | `SpinTheValveScreen` | นาถวัฒน์ |
| `UI/spritesheet_minigame_dot-texture.png` + `.json` | Atlas ของมินิเกม Connect The Dots (dot + line 6 สี + grid) | ✅ | `ConnectTheDotsScreen` | พรภวิษย์ |
| `UI/Needle.png` | เข็มชี้ของมินิเกม Skill Check | ❌ | `SkillCheckScreen` (dead code) | พรภวิษย์ |
| `UI/Radial Circle.png` | วงกลม radial ของ Skill Check | ❌ | `SkillCheckScreen` (dead code) | พรภวิษย์ |
| `Tilemaps/mapassets/spritesheet_background-texture.png` | Tileset พื้นหลังหลัก (1080×1080) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/asset-1-export.png` | Tileset ชุดที่ 1 (1920×1080) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/asset-2 (1)-export.png` | Tileset ชุดที่ 2 (1920×1080) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/asset-3-Recovered (1)-export.png` | Tileset ชุดที่ 3 (1920×1080) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/black_opa-export.png` | พื้นทึบแสงสำหรับ masking (600×120) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/Sprite-bgx400%_3.png` | พื้นหลังขยาย 400% (1200×240) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Tilemaps/mapassets/Cha1-export.png` | Tileset ตัวละคร/ฉาก (240×240, grid 16) | — (ผ่าน `.ldtk`) | `SceneBuilder` | นาถวัฒน์ |
| `Phobias/entamaphobia-*.png` | Entity ของ Entamaphobia (5 stage) | 🔲 **ยังไม่มีไฟล์** | — | — |

> `UI/Needle.png` และ `UI/Radial Circle.png` **ไม่ได้ลงทะเบียนใน `Content.mgcb`** — ถ้านำ `SkillCheckScreen` กลับมาใช้จะโหลดไม่เจอ ต้องลงทะเบียนก่อน

## Audio — SFX

ไฟล์ทั้งหมดอยู่ใน `Content/Audio/` และผูกกับ `enum Sound` ในโค้ด (25 ไฟล์ · ลงทะเบียนใน mgcb ครบ)

| Asset Name | คำอธิบาย | enum `Sound` | แหล่งที่มา | License | รับผิดชอบ |
|---|---|---|---|---|---|
| `sfx_pipe_repairing.mp3` | เสียงระหว่างซ่อมท่อ | `PipeRepairing` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_pipe_success.mp3` | ซ่อมท่อสำเร็จ | `PipeSuccess` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_pipe_failed.mp3` | ซ่อมท่อล้มเหลว | `PipeFail` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_pipe_complete.mp3` | ซ่อมท่อครบทั้งหมด | `PipeComplete` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_electric_repairing.mp3` | เสียงระหว่างซ่อมไฟ | `ElecRepairing` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_electric_success.mp3` | ซ่อมไฟสำเร็จ | `ElecSuccess` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_electric_failed.mp3` | ซ่อมไฟล้มเหลว | `ElecFail` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_electric_complete.mp3` | ซ่อมไฟครบทั้งหมด | `ElecComplete` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_door_open.mp3` | เปิดประตู | `DoorOpen` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_door_close.mp3` | ปิดประตู | `DoorClose` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_door_stuck.mp3` | ประตูติด/เปิดไม่ได้ (ตอน Scopophobia มา) | `DoorStuck` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_locker_open.mp3` | เปิดตู้ | `LockerOpen` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_locker_close.mp3` | ปิดตู้ | `LockerClose` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_scopophobia_indicator.mp3` | เสียงเตือนก่อน Scopophobia มา (spatial) | `ScopoIndicator` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_autophobia_indicator.MP3` | เสียงเตือนก่อน Autophobia มา | `AutoIndicator` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `sfx_step_01..08.mp3` | เสียงฝีเท้า 8 แบบ (ไล่ตามลำดับ) | *(โหลดแยกเป็น array)* | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |

## Audio — Ambience

| Asset Name | คำอธิบาย | เล่นเมื่อ | แหล่งที่มา | License | รับผิดชอบ |
|---|---|---|---|---|---|
| `amb_sewer.mp3` | บรรยากาศโถงอุโมง (น้ำ dripping, ชื้น) | ฉาก `tilemap_sewer` | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |
| `amb_electricroom.mp3` | บรรยากาศห้องไฟฟ้า (ใช้เป็นค่า default ด้วย) | ฉากห้องไฟฟ้าทั้งสอง | Pixabay | [ต้องยืนยัน] | พรภวิษย์ |

## Audio — Music (BGM)

| Asset Name | คำอธิบาย | สถานะ |
|---|---|---|
| *(ยังไม่มี)* | ยังไม่มี BGM ในโปรเจกต์ — `AudioManager` มีแค่ `Song` 2 ตัวที่เป็น ambience | 🔲 **ยังไม่มีไฟล์** · เอกสารเดิมอ้าง `dark frequency` (freetouse.com) แต่ไม่พบไฟล์ใน `Content/Audio/` |

> **TBD:** ยังไม่ได้ตัดสินใจว่าเกมจะมี BGM กี่แทร็ก และจะเล่นตอนไหน (ดู [Audio Direction](09-audio-direction.md#music-bgm))

## Fonts

| Asset Name | คำอธิบาย | mgcb | โหลดโดย | รับผิดชอบ |
|---|---|---|---|---|
| `Font/GenerationFonting.fnt` + `GenerationFonting_0.png` | Bitmap font หลักสำหรับ UI (`Font/` เอกพจน์) | ✅ | `TextManager`, `SceneManager` | พรภวิษย์ |
| ~~`notepen`~~ | ชื่อ font ที่เคยระบุไว้ในเอกสารเดิม — **ไม่พบไฟล์นี้ในโปรเจกต์** | ❌ | — | — |

## Shaders

| Asset Name | คำอธิบาย | mgcb | โหลดโดย |
|---|---|---|---|
| `Shaders/Vignette.fx` | เอฟเฟกต์ขอบมืด (เทคนิค `EllipseMask`) — sanity / blink / Nyctophobia | ✅ | `VignetteOverlay` |
| `Shaders/BottomFade.fx` | เฟดด้านล่างของจอ | ✅ | `BottomFadeOverlay` |
| `Shaders/Silhouette.fx` | เขียน mask ของ tile ที่จะ highlight (เทคนิค `HighlightMask`) | ✅ | `TileHighlightRenderer` |
| `Shaders/HighlightComposite.fx` | รวม mask เป็น outline / scan / pulse (เทคนิค `Highlight`) | ✅ | `TileHighlightRenderer` |
| `Shaders/Saturation.fx` | *(ไม่มีในโปรเจกต์แล้ว — พบเฉพาะใน `Content/bin/` ที่ล้าสมัย)* | ❌ | — |

## Data / Config

| Asset Name | คำอธิบาย | mgcb | โหลดโดย |
|---|---|---|---|
| `Datas/tutorial.json` | ข้อมูล tutorial 10 ขั้น (`Steps[] { Text, CompleteOn }`) | ✅ | `TutorialManager` |

> `Datas/` สะกดผิด (ควรเป็น `Data/`) — ถ้าแก้ต้องแก้ทั้งโฟลเดอร์, `Content.mgcb` และ `TutorialManager` (`Content.Load<TutorialData>("Datas/tutorial")`)

## Tilemaps

| Asset Name | คำอธิบาย | mgcb |
|---|---|---|
| `Tilemaps/tilemap_electrical_room.ldtk` | ฉากห้องไฟฟ้า 1 (2 walls · 1 panel · 1 locker · 2 doors · 2 spawns · 2 lights) | ✅ |
| `Tilemaps/tilemap_electrical_room_2.ldtk` | ฉากห้องไฟฟ้า 2 (2 walls · 1 panel · 1 locker · 1 door · 1 spawn · 2 lights) | ✅ |
| `Tilemaps/tilemap_sewer.ldtk` | ฉากโถงอุโมง (3 walls · **5 valves** · 4 lockers · 2 doors · 2 spawns · 14 lights) | ✅ |

รายละเอียด entity / tile layer / tileset: [Data Schema § Level Data](../03_Software/03-data-schema.md#2-ข้อมูลระดับ-level-data)

## ⚠️ ปัญหาการตั้งชื่อที่ต้องแก้

ตาม [Asset Naming Convention](README.md#-naming-convention) ใน README ของ GDD:

| ปัญหา | ตัวอย่าง | ที่ถูกต้อง |
|---|---|---|
| สะกดผิด | `spitesheet_player` | `spritesheet_player` (ขาด `r`) |
| สะกดผิด (ชื่อโฟลเดอร์) | `Datas/` | `Data/` |
| สะกดผิด (ชื่อคลาส) | `TurotialData.cs`, `TurorialDataReader.cs` | `TutorialData.cs`, `TutorialDataReader.cs` |
| นามสกุลไฟล์ไม่ consistent | `.MP3` ปนกับ `.mp3` (`sfx_autophobia_indicator.MP3`, `amb_electricroom.MP3` ใน mgcb) | เลือกแบบเดียว (แนะนำตัวพิมพ์เล็ก) |
| มีช่องว่างในชื่อไฟล์ | `Radial Circle.png`, `asset-2 (1)-export.png` | ใช้ `_` แทน |
| ชื่อไฟล์มี `(1)` จากการโหลดซ้ำ | `asset-2 (1)-export.png`, `asset-3-Recovered (1)-export.png` | ตั้งชื่อใหม่ให้สื่อความหมาย |
| ชื่อไฟล์มี `%` | `Sprite-bgx400%_3.png` | เลี่ยงอักขระพิเศษ |
| Prefix ไม่ตรงกับ convention | ไฟล์ภาพไม่มี prefix, เอกสารระบุ `spr_` แต่ README ระบุ `sprite_` | ตกลงให้ตรงกัน |
| ชื่อโฟลเดอร์ vs ชื่อ project | โฟลเดอร์ `Imaginophonia` แต่ project `Imaginophobia` | ตกลงให้ตรงกัน |

## Related Documents

- Pipeline: [Asset Pipeline Flow](06-asset-pipeline-flow.md)
- Naming convention: [GDD README](README.md)
- Staging area: [02_Assets](../02_Assets/)
- Art direction: [Art Direction](08-art-direction.md) · Audio: [Audio Direction](09-audio-direction.md)
- Data schema (level / animation data): [Data Schema](../03_Software/03-data-schema.md)
