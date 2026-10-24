---
type: asset-list
version: 3.0
date: 2026-10-25
---
# Asset List — Imaginophobia

**Version:** 3.0 | **Last Updated:** 2026-10-25

> ตารางนี้อิงจากไฟล์จริงใน `src/Imaginophonia/Imaginophonia/Content/` (ไม่นับ `bin/` และ `obj/` ที่เป็น build artifact) และจาก `Content/Content.mgcb` (commit `cf024ef`, 2026-10-24)
> Asset ที่ยังไม่มีให้ใส่ใน `docs/02_Assets/_candidates/` ก่อน แล้วค่อยย้ายเข้า `Content/` ตาม [Asset Pipeline Flow](06-asset-pipeline-flow.md)
> คอลัมน์ **mgcb** = ลงทะเบียนใน `Content.mgcb` แล้วหรือยัง (ถ้าไม่ลงทะเบียน เกมจะโหลดไม่ได้)

## Sprites / Textures

| Asset Name                                                | คำอธิบาย                                                                  | mgcb                                                     | โหลดโดย                                                           | รับผิดชอบ |
| --------------------------------------------------------- | --------------------------------------------------------------------------------- | -------------------------------------------------------- | ------------------------------------------------------------------------ | ------------------ |
| `Character/spritesheet_player-texture.png` + `.json`  | Spritesheet ตัวละครหลัก (960×360, เฟรม 120×120) + animation data | ✅                                                       | `Player`, `EnemyManager` (Autophobia ย้อมดำ)                   | นาถวัฒน์   |
| `Character/New_version/spitesheet_player.*`             | ตัวละครเวอร์ชัน redesign                                           | ❌                                                       | — (ยังไม่ถูกโหลด)                                          | นาถวัฒน์   |
| `Character/Old_version/spitesheet_player.*`             | ตัวละครเวอร์ชันก่อน redesign (เก็บไว้เทียบ)        | ❌                                                       | —                                                                       | นาถวัฒน์   |
| `Phobias/sprite_scopophobia.png`                        | Entity ของ Scopophobia                                                         | ✅                                                       | `EnemyManager`                                                         | นาถวัฒน์   |
| `Phobias/entamaphobia-*.png`                            | Entity ของ Entamaphobia (5 stage)                                              | 🔲**ยังไม่มีไฟล์**                     | —                                                                       | —                 |
| `UI/Imaginophobia_bg_placerholder.png`                  | พื้นหลังเมนูหลัก / หน้าตั้งค่า                         | ✅                                                       | `MainMenuScreen`, `SettingsScreen`                                   | นาถวัฒน์   |
| `UI/death_screen.png`                                   | พื้นหลังจอตาย                                                        | ✅                                                       | `DeathScreen`                                                          | นาถวัฒน์   |
| `UI/win_screen.png`                                     | พื้นหลังจอชนะ                                                        | ✅                                                       | `WinScreen`                                                            | นาถวัฒน์   |
| `UI/pause_button.png`                                   | ไอคอนปุ่ม pause มุมขวาบนของ gameplay                          | ✅                                                       | `GumUIHelper.CreatePauseButton`                                        | นาถวัฒน์   |
| `UI/slider_placeholder.png`                             | รางของสไลเดอร์ volume                                               | ✅                                                       | `GumUIHelper.CreateSlider`                                             | นาถวัฒน์   |
| `UI/slider_thumbinstance_placeholder.png`               | หัวจับของสไลเดอร์ volume                                         | ✅                                                       | `GumUIHelper.CreateSlider`                                             | นาถวัฒน์   |
| `UI/sprite_spacebar.png`                                | ปุ่ม Spacebar บน UI (แจ้งว่าโต้ตอบได้)                      | ✅                                                       | `UIManager`                                                            | เตชินท์     |
| `UI/sprite_valve.png`                                   | วาล์วสำหรับมินิเกม Spin The Valve                               | ✅                                                       | `SpinTheValveScreen`                                                   | นาถวัฒน์   |
| `UI/spritesheet_minigame_dot-texture.png` + `.json`   | Atlas ของมินิเกม Connect The Dots (dot + line 6 สี + grid)            | ✅                                                       | `ConnectTheDotsScreen`                                                 | พรภวิษย์   |
| `UI/Needle.png`                                         | เข็มชี้ของมินิเกม Skill Check                                    | 🔲**ไม่มีไฟล์นี้ในโปรเจกต์** | *(เคยอ้างโดย `SkillCheckScreen` ที่เป็น dead code)* | พรภวิษย์   |
| `UI/Radial Circle.png`                                  | วงกลม radial ของ Skill Check                                              | 🔲**ไม่มีไฟล์นี้ในโปรเจกต์** | *(เคยอ้างโดย `SkillCheckScreen`)*                          | พรภวิษย์   |
| `Tilemaps/mapassets/spritesheet_background-texture.png` | Tileset พื้นหลังหลัก (1080×1080)                                     | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_er_floor.png`           | Tileset พื้นห้องไฟฟ้า (1920×1080)                                   | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_er_tiles.png`           | Tileset ผนัง/ของในห้องไฟฟ้า (1920×1080)                        | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_general_decoration.png` | Tileset ของตกแต่งทั่วไป (1920×1080)                               | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_sewer_tiles.png`        | Tileset ท่อ/โถงอุโมง (1200×240)                                       | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_shadow.png`             | Tileset เงา (600×120)                                                         | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/Clip_board.png`                     | คลิปบอร์ด (ใช้กับ entity `Clipboard` / tag `report`)           | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/sewer_indicator_red.png`            | ป้ายชี้ทางสีแดง (480×360)                                         | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/sewer_indicator.png`                | ป้ายชี้ทาง (ไม่ได้ถูกอ้างใน `.ldtk` แล้ว)          | ❌ (ไม่มีฉากไหนอ้าง)                      | —                                                                       | นาถวัฒน์   |
| `Tilemaps/mapassets/Exit_sign.png`                      | ป้ายทางออก (360×120) ใช้ในโถงอุโมง                        | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |

> ไฟล์ tileset ทั้งหมดถูกอ้างจาก `.ldtk` โดยตรง (`defs.tilesets[].relPath`) — ชื่อ identifier ใน `.ldtk` ของแต่ละไฟล์ไม่เหมือนกัน (`Spritesheet_er_floor` / `Er_floor`) แต่ชี้ไปไฟล์ภาพเดียวกัน ถ้าเปลี่ยนชื่อไฟล์ภาพต้องแก้ใน `.ldtk` ทุกไฟล์ที่อ้าง

## Audio — SFX

ไฟล์ทั้งหมดอยู่ใน `Content/Audio/` นามสกุล `.mp3` และผูกกับ `enum SoundEffectName` ในโค้ด — **ลงทะเบียนใน mgcb ครบทั้ง 34 ไฟล์** (รวม ambience + BGM ทั้งโปรเจกต์เป็น 39 ไฟล์)

| Asset Name                             | คำอธิบาย                                                           | enum `SoundEffectName`                                               | แหล่งที่มา | License                | รับผิดชอบ |
| -------------------------------------- | -------------------------------------------------------------------------- | ---------------------------------------------------------------------- | -------------------- | ---------------------- | ------------------ |
| `sfx_pipe_repairing.mp3`             | เสียงระหว่างซ่อมท่อ                                     | `PipeRepairing`                                                      | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_pipe_success.mp3`               | ซ่อมท่อสำเร็จ                                                 | `PipeSuccess` *(โหลดแล้วแต่ยังไม่ถูกเล่น)* | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_pipe_failed.mp3`                | ซ่อมท่อล้มเหลว                                               | `PipeFail`                                                           | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_pipe_complete.mp3`              | ซ่อมวาล์วสำเร็จ                                             | `PipeComplete`                                                       | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_electric_repairing.mp3`         | เสียงระหว่างซ่อมไฟ                                       | `ElecRepairing`                                                      | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_electric_success.mp3`           | ต่อจุดสำเร็จ (Connect The Dots)                                | `ElecSuccess`                                                        | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_electric_failed.mp3`            | เริ่ม/ล้างเส้นใน Connect The Dots                           | `ElecFail`                                                           | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_electric_complete.mp3`          | ซ่อมแผงไฟสำเร็จ                                             | `ElecComplete`                                                       | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_door_open.mp3`                  | เปิดประตู                                                         | `DoorOpen` *(โหลดแล้วแต่ยังไม่ถูกเล่น)*    | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_door_close.mp3`                 | ปิดประตู (ตอนเข้าฉากใหม่)                            | `DoorClose`                                                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_door_stuck.mp3`                 | ประตูติด/เปิดไม่ได้ (ตอน Scopophobia มา)            | `DoorStuck`                                                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_locker_open.mp3`                | เปิดตู้                                                             | `LockerOpen`                                                         | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_locker_close.mp3`               | ปิดตู้ (เข้าไปซ่อน)                                        | `LockerClose`                                                        | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_scopophobia_indicator.mp3`      | เสียงเตือนก่อน Scopophobia มา (spatial 600/15000)          | `ScopoIndicator`                                                     | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_phobias_hit.mp3`                | เสียงตอนโดน phobia ชน                                         | `PhobiaHit`                                                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_heartbeat.mp3`                  | เสียงหัวใจเต้น (ระดับสติ Insane)                     | `HeartBeat`                                                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_panting.mp3`                    | เสียงหอบหายใจ                                                 | `Painting`                                                           | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_light_break.mp3`                | ไฟแตก (Scopophobia ชนไฟ)                                          | `LightBreak`                                                         | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_darkness.mp3`                   | ความมืดกลืน (เข้า LightArea ที่ดับ)                   | `Darkness`                                                           | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_screen_off.mp3`                 | จอดำ (Scopophobia เกิด)                                            | `ScreenOff`                                                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_screen_on.mp3`                  | จอกลับมา                                                           | `ScreenOn`                                                           | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_flashlight_toggle.mp3`          | เปิด/ปิดไฟฉาย                                                  | `FlashlightToggle`                                                   | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_light_flicker_long.mp3`         | ไฟกระพริบยาว (ตอน Nyctophobia เกิด)                     | `LightFlickerLong`                                                   | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_light_flicker_short_01..03.mp3` | ไฟกระพริบสั้น 3 แบบ (สุ่มตอน Autophobia แตะไฟ) | `LightFlickerShort1..3`                                              | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |
| `sfx_step_01..08.mp3`                | เสียงฝีเท้า 8 แบบ (ไล่ตามลำดับ)                   | *(โหลดแยกเป็น array `_steps`)*                          | Pixabay              | [ต้องยืนยัน] | พรภวิษย์   |

## Audio — Ambience & Music (BGM)

| Asset Name                  | คำอธิบาย                                                           | enum `BackgroundSound` | เล่นเมื่อ                                                                           |
| --------------------------- | -------------------------------------------------------------------------- | ------------------------ | -------------------------------------------------------------------------------------------- |
| `amb_sewer.mp3`           | บรรยากาศโถงอุโมง (น้ำ dripping, ชื้น)               | `AmbSewer`             | โหลดฉาก `tilemap_sewer`                                                             |
| `amb_electricroom.mp3`    | บรรยากาศห้องไฟฟ้า (ใช้เป็นค่า default ด้วย) | `AmbElectric`          | ฉากห้องไฟฟ้าทั้งสอง                                                       |
| `bgm_mainmenu_screen.mp3` | เพลงเมนูหลัก                                                   | `BgmMainMenu`          | `MainMenuScreen`                                                                           |
| `bgm_death_screen.mp3`    | เพลงจอตาย                                                         | `BgmDeathScreen`       | `DeathScreen`                                                                              |
| `bgm_win_screen.mp3`      | เพลงจอชนะ                                                         | `BgmWinScreen`         | *(ผูกใน enum/dictionary แล้ว แต่ยังไม่มีหน้าจอไหนเรียก)* |

> ⚠️ เดิมเอกสารระบุว่า "ยังไม่มี BGM ในโปรเจกต์" — **ไม่จริงแล้ว** มี BGM 3 แทร็กและถูกเล่นในเมนู/จอตาย/จอชนะเรียบร้อย (`AudioManager.BgsVolume` default 0.2)
> ⚠️ เอกสารเดิมอ้าง `sfx_autophobia_indicator.MP3` — ไฟล์นี้ **ไม่มีใน source** แล้ว และค่า enum `AutoIndicator` ก็ถูกถอดออกด้วย (Autophobia ไม่มีเสียงเตือนในโค้ดปัจจุบัน)

## Fonts

| Asset Name                                                          | คำอธิบาย                                                                                                                   | mgcb | โหลดโดย                                                | รับผิดชอบ |
| ------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- | ---- | ------------------------------------------------------------- | ------------------ |
| `Fonts/GenerationFonting.fnt` + `Fonts/GenerationFonting_0.png` | Bitmap font หลักสำหรับ HUD (โฟลเดอร์`Fonts/` พหูพจน์)                                                   | ✅   | `TextManager` (`Fonts/GenerationFonting`)                 | พรภวิษย์   |
| `Fonts/TrueTypeFont/GenerationFonting.ttf`                        | ฟอนต์ที่ Gum สร้างตอน runtime ผ่าน `KernSmithFontCreator` (ลงทะเบียนชื่อ `generationfonting`) | ✅   | `MainGame.Initialize` → ใช้โดยทุกหน้าจอ Gum | พรภวิษย์   |
| ~~`notepen`~~                                                    | ชื่อ font ที่เคยระบุไว้ในเอกสารเดิม —**ไม่พบไฟล์นี้ในโปรเจกต์**          | ❌   | —                                                            | —                 |

> `SceneManager.Debug()` ยังโหลด `"Font/GenerationFonting"` (โฟลเดอร์เอกพจน์) ในโค้ดที่คอมเมนต์ไว้ — ถ้าเปิดใช้จะโหลดไม่เจอ ต้องแก้เป็น `Fonts/`

## Shaders

| Asset Name                        | คำอธิบาย                                                                                                         | mgcb | โหลดโดย                            |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------ | ---- | ----------------------------------------- |
| `Shaders/Vignette.fx`           | เอฟเฟกต์ขอบมืด (เทคนิค `EllipseMask`) — sanity / blink / Nyctophobia                              | ✅   | `VignetteOverlay`                       |
| `Shaders/BottomFade.fx`         | เฟดด้านล่างของจอ                                                                                         | ✅   | `BottomFadeOverlay`                     |
| `Shaders/Silhouette.fx`         | เขียน mask ของ tile ที่จะ highlight (เทคนิค `HighlightMask`)                                        | ✅   | `TileHighlightRenderer`                 |
| `Shaders/HighlightComposite.fx` | รวม mask เป็น outline / scan / pulse (เทคนิค `Highlight`)                                                 | ✅   | `TileHighlightRenderer`                 |
| `Shaders/Saturation.fx`         | เอฟเฟกต์ความอิ่มสีทั้งเฟรม (เทคนิค `ApplySaturation`) — ใช้ตอนโดน Autophobia | ✅   | `MainGame.LoadContent` / `DrawGraded` |

## Data / Config

| Asset Name              | คำอธิบาย                                                     | mgcb | โหลดโดย      |
| ----------------------- | -------------------------------------------------------------------- | ---- | ------------------- |
| `Datas/tutorial.json` | ข้อมูล tutorial 10 ขั้น (`Steps[] { Text, CompleteOn }`) | ✅   | `TutorialManager` |

> `Datas/` สะกดผิด (ควรเป็น `Data/`) — ถ้าแก้ต้องแก้ทั้งโฟลเดอร์, `Content.mgcb` และ `TutorialManager` (`Content.Load<TutorialData>("Datas/tutorial")`)

## Tilemaps

| Asset Name                                  | คำอธิบาย                                                                                                                                                           | mgcb |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---- |
| `Tilemaps/tilemap_electrical_room.ldtk`   | ฉากห้องไฟฟ้า 1 — 1920×1080 (2 walls · 1 panel · 1 locker · 1 clipboard · 2 doors [sewer/end] · 2 spawns · 5 SpotLight)                                 | ✅   |
| `Tilemaps/tilemap_electrical_room_2.ldtk` | ฉากห้องไฟฟ้า 2 — 1920×1080 (2 walls · 1 panel · 1 locker · 1 clipboard · 1 door [sewer] · 1 spawn · 5 SpotLight)                                       | ✅   |
| `Tilemaps/tilemap_sewer.ldtk`             | ฉากโถงอุโมง —**6960×1080** (3 walls · **5 valves** · 4 lockers · 2 doors [ห้องไฟฟ้า 1/2] · 2 spawns · 12 PointLight · 2 DoorLight) | ✅   |

รายละเอียด entity / property / tile layer / tileset / destination / origin: [Data Schema § Level Data](../03_Software/03-data-schema.md#2-ข้อมูลระดับ-level-data)

## ⚠️ ปัญหาการตั้งชื่อที่ต้องแก้

ตาม [Asset Naming Convention](README.md#-naming-convention) ใน README ของ GDD:

| ปัญหา                                             | ตัวอย่าง                                                                                   | ที่ถูกต้อง                                                                  |
| ------------------------------------------------------ | -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| สะกดผิด                                         | `spitesheet_player`                                                                              | `spritesheet_player` (ขาด `r`)                                                 |
| สะกดผิด (ชื่อโฟลเดอร์)              | `Datas/`                                                                                         | `Data/`                                                                             |
| สะกดผิด (ชื่อคลาส/ไฟล์)             | `TurotialData.cs`, `TurorialDataReader.cs`                                                     | `TutorialData.cs`, `TutorialDataReader.cs`                                        |
| ชื่อ enum ไม่ตรงกับไฟล์               | `SoundEffectName.Painting` → `sfx_panting.mp3`                                                | ให้ตรงกัน (`Painting` หรือแก้ไฟล์เป็น `sfx_painting.mp3`) |
| ชื่อไฟล์สะกดผิด                         | `Imaginophobia_bg_placerholder.png`                                                              | `placeholder`                                                                       |
| ชื่อโฟลเดอร์ vs ชื่อ project           | โฟลเดอร์`Imaginophonia` แต่ project `Imaginophobia`                                 | ตกลงให้ตรงกัน                                                            |
| ชื่อ identifier ใน `.ldtk` ไม่ตรงกัน  | `Spritesheet_er_floor` vs `Er_floor` ชี้ไฟล์เดียวกัน                            | ตั้งชื่อ identifier ให้ตรงกันทุกไฟล์                          |
| ชื่อ entity ทำหน้าที่เดียวกัน     | `PlayerSpawnPoint` vs `SpawnPoint` (ต่างกันแค่ tag `spawn` / property `Origin`)  | ตกลงชื่อเดียว                                                            |
| นามสกุลไฟล์ไม่ consistent                | `bin/` มี `.ogg`/`.xnb` ค้างจากชุดเก่า ขณะที่ source เป็น `.mp3` | ลบ artifact และใช้`.mp3` อย่างเดียว                               |
| ชื่อไฟล์เสียงมีช่องว่างเกิน | `sfx_light_flickering_short_01 .ogg` (พบเฉพาะใน `Content/bin/`)                       | ลบ artifact                                                                         |

## Related Documents

- Pipeline: [Asset Pipeline Flow](06-asset-pipeline-flow.md)
- Naming convention: [GDD README](README.md)
- Staging area: [02_Assets](../02_Assets/)
- Art direction: [Art Direction](08-art-direction.md) · Audio: [Audio Direction](09-audio-direction.md)
- Data schema (level / animation data): [Data Schema](../03_Software/03-data-schema.md)
