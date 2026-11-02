---
type: asset-list
version: 4.0
date: 2026-11-02
---
# Asset List — Imaginophobia

**Version:** 4.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

> นับจากไฟล์จริงใน `src/Imaginophobia/Imaginophobia/Content/` (ไม่นับ `bin/` และ `obj/`) — **99 ไฟล์**
> `Content/Content.mgcb` มี `/build` 87 entries (Audio 46 · UI 26 · Shaders 5 · Tilemaps 3 · Phobias 3 · Fonts 2 · Character 2)
> ไฟล์ 2 ตัว **ไม่ผ่าน mgcb** แต่ถูก copy ไป output ผ่าน `Imaginophobia.csproj` (`CopyToOutputDirectory=PreserveNewest`): `Content/Datas/tutorial.json` และ `Content/Fonts/TrueTypeFont/GenerationFonting.ttf`

## Sprites / Textures

| Asset Name                                                | คำอธิบาย                                                                  | mgcb                                                     | โหลดโดย                                                           | รับผิดชอบ |
| --------------------------------------------------------- | --------------------------------------------------------------------------------- | -------------------------------------------------------- | ------------------------------------------------------------------------ | ------------------ |
| `Character/spritesheet_player-texture.png` + `.json`  | Spritesheet ตัวละครหลัก (960×360, 22 เฟรม 120×120: `sprite_back` 1 · `sprite_idle_01..12` · `sprite_walk_01..09`) + animation data | ✅                                                       | `Player`, `EnemyManager` (Autophobia ย้อมดำ)                   | นาถวัฒน์   |
| `Phobias/sprite_scopophobia.png`                        | Entity ของ Scopophobia                                                         | ✅                                                       | `EnemyManager`                                                         | นาถวัฒน์   |
| `Phobias/spritesheet_entamaphobia-texture.png` + `.json` | Atlas ของ Entamaphobia (4096×4096) — 6 เฟรม: `entama_stage_1..4` · `entama_hand` · `entama_jumpscare`                       | ✅                                                       | `DoorPeekingScreen` · `JumpScareScreen`                                  | —                 |
| `UI/background_main.png`                                | พื้นหลังเมนูหลัก                                               | ✅                                                       | `MainMenuScreen` (ใช้เป็นพื้นให้ `EyeField`)                        | นาถวัฒน์   |
| `UI/background_death.png`                               | พื้นหลังจอตาย                                                        | ✅                                                       | `DeathScreen`                                                          | นาถวัฒน์   |
| `UI/background_win.png`                                 | พื้นหลังจอชนะ                                                        | ✅                                                       | `WinScreen`                                                            | นาถวัฒน์   |
| `UI/background_peek_er1.png` · `UI/background_peek_er2.png` | พื้นหลังจอแอบมองประตู (คนละภาพตามห้องไฟฟ้า 1 / 2)              | ✅                                                       | `DoorPeekingScreen`                                                    | —                 |
| `UI/foreground_peek_door.png`                           | ประตูที่วาดทับด้านหน้าจอแอบมอง (สร้างช่องใต้ประตู)             | ✅                                                       | `DoorPeekingScreen`                                                    | —                 |
| `UI/sprite_button_pause.png`                            | ไอคอนปุ่ม pause มุมขวาบนของ gameplay                          | ✅                                                       | `GumUIHelper.CreatePauseButton`                                        | นาถวัฒน์   |
| `UI/sprite_bar_sanity.png`                              | แถบ Sanity บน HUD                                                    | ✅                                                       | `UIManager`                                                            | —                 |
| `UI/slider_placeholder.png`                             | รางของสไลเดอร์ volume                                               | ✅                                                       | `GumUIHelper.AddSettingsPanel` (และใช้เป็นรางให้ ScrollViewer ใน `CreditsScreen`) | นาถวัฒน์   |
| `UI/slider_thumbinstance_placeholder.png`               | หัวจับของสไลเดอร์ volume                                         | ✅                                                       | `GumUIHelper.AddSettingsPanel` (และเป็นหัวเลื่อนของ `CreditsScreen`)                                             | นาถวัฒน์   |
| `UI/spritesheet_minigame_dot-texture.png` + `.json`   | Atlas ของมินิเกม Connect The Dots (771×736) — 13 เฟรม: `minigame_dot_grid` · `sprite_dot_<สี>` ×6 · `sprite_line_<สี>` ×6            | ✅                                                       | `ConnectTheDotsScreen`                                                 | พรภวิษย์   |
| `UI/spritesheet_buttons-texture.png` + `.json`          | Atlas ปุ่มบนจอ (450×460) — 6 เฟรม: `sprite_spacebar` · `sprite_button_exit` · `sprite_button_peek` · `sprite_button_light` · `sprite_button_left` · `sprite_button_right`                       | ✅                                                       | `UIManager` (HUD + ปุ่มออกมินิเกม/พลิกหน้า)                             | เตชินท์     |
| `UI/spritesheet_clipboard-texture.png` + `.json`        | Atlas คลิปบอร์ด (4096×2048) — 9 เฟรม: `sprite_clipboard_er1_page1..4` · `sprite_clipboard_er2_page1..4` · `sprite_clipboard_secret`              | ✅                                                       | `ClipboardScreen`                                                      | —                 |
| `UI/sprite_valve.png`                                   | วาล์วสำหรับมินิเกม Spin The Valve                               | ✅                                                       | `SpinTheValveScreen`                                                   | นาถวัฒน์   |
| `UI/Eyes/sprite_sclera_A/B/C.PNG`                          | ตาขาว 3 แบบ (ใช้สุ่มให้ดวงตาเมนูหน้าตาไม่ซ้ำ)                  | ✅                                                       | `EyeField`                                                             | —                 |
| `UI/Eyes/sprite_pupil_A/B/C.PNG`                           | รูม่านตา 3 แบบ                                                            | ✅                                                       | `EyeField`                                                             | —                 |
| `UI/Eyes/sprite_eyelid_A/B/C.PNG`                          | เปลือกตา 3 แบบ (สำหรับการกระพริบตา)                              | ✅                                                       | `EyeField`                                                             | —                 |
| `Tilemaps/mapassets/spritesheet_background-texture.png` | Tileset พื้นหลังหลัก (1080×1080)                                     | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_er_floor.png`           | Tileset พื้นห้องไฟฟ้า (1920×1080)                                   | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_er_tiles.png`           | Tileset ผนัง/ของในห้องไฟฟ้า (1920×1080)                        | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_general_decoration.png` | Tileset ของตกแต่งทั่วไป (1920×1080)                               | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_sewer_tiles.png`        | Tileset ท่อ/โถงอุโมง (1200×240)                                       | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/spritesheet_shadow.png`             | Tileset เงา (600×120)                                                         | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/Clip_board.png`                     | คลิปบอร์ด (ใช้กับ entity `Clipboard` / tag `report`)           | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/sewer_indicator_red.png`            | ป้ายชี้ทางสีแดง (480×360)                                         | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |
| `Tilemaps/mapassets/sewer_indicator.png`                | ป้ายชี้ทาง (ไม่มี `.ldtk` ไฟล์ไหนอ้างถึง)          | ❌ (ไม่มีฉากไหนอ้าง)                      | —                                                                       | นาถวัฒน์   |
| `Tilemaps/mapassets/Exit_sign.png`                      | ป้ายทางออก (360×120) ใช้ในโถงอุโมง                        | — (ผ่าน `.ldtk`)                                  | `SceneBuilder`                                                         | นาถวัฒน์   |

## Audio — SFX

ไฟล์ทั้งหมดอยู่ใน `Content/Audio/` นามสกุล `.mp3` — **46 ไฟล์** ลงทะเบียนใน mgcb ครบทั้ง 46
ไฟล์เสียง 33 ไฟล์ผูกกับ `enum SoundEffectName` ผ่าน `AudioManager.SfxByName`, อีก 8 ไฟล์ (`sfx_step_01..08`) โหลดเป็น array `_steps`

| Asset Name                             | คำอธิบาย                                                           | enum `SoundEffectName`                                               | แหล่งที่มา | License                                                                                      | รับผิดชอบ |
| -------------------------------------- | -------------------------------------------------------------------------- | ---------------------------------------------------------------------- | -------------------- | -------------------------------------------------------------------------------------------- | ------------------ |
| `sfx_pipe_repairing.mp3`             | เสียงระหว่างซ่อมท่อ (ตอนลากเมาส์หมุนวาล์ว)                                     | `PipeRepairing`                                                      | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_pipe_success.mp3`               | โหลดไว้ใน `SfxByName` แต่ไม่มีจุดไหนเรียกเล่น | `PipeSuccess` | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_pipe_failed.mp3`                | เข็มวาล์วอยู่นอกโซนเขียว (เล่นแบบ `waitExist`)                                               | `PipeFail`                                                           | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_pipe_complete.mp3`              | ซ่อมวาล์วสำเร็จ (progress ครบ 200)                                             | `PipeComplete`                                                       | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_electric_repairing.mp3`         | เปิดมินิเกมซ่อมแผงไฟ                                       | `ElecRepairing`                                                      | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_electric_success.mp3`           | ต่อจุดสำเร็จ (Connect The Dots)                                | `ElecSuccess`                                                        | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_electric_failed.mp3`            | เริ่มลากเส้นใหม่ / ล้างเส้นด้วยคลิกขวา                           | `ElecFail`                                                           | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_electric_complete.mp3`          | ปิดมินิเกมซ่อมแผงไฟ (สำเร็จหรือกดออก)                                             | `ElecComplete`                                                       | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_door_open.mp3`                  | โหลดไว้ใน `SfxByName` แต่ไม่มีจุดไหนเรียกเล่น    | `DoorOpen` | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_door_close.mp3`                 | เข้าฉากใหม่สำเร็จ (`GameManager.OnSceneLoaded`)                            | `DoorClose`                                                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_door_stuck.mp3`                 | ประตูเปิดไม่ได้ (ตอน Scopophobia ยัง active)            | `DoorStuck`                                                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_locker_open.mp3`                | ออกจากตู้                                                             | `LockerOpen`                                                         | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_locker_close.mp3`               | เข้าไปซ่อน (เล่นวนระหว่างซ่อน)                                        | `LockerClose`                                                        | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_scopophobia_indicator.mp3`      | เสียงเตือนก่อน Scopophobia มา (spatial inner 5000 / outer 26500)          | `ScopoIndicator`                                                     | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_entama_advance.mp3`             | Entamaphobia เกิด / เลื่อน stage (spatial 60 / 1920)             | `EntamaAdvance`                                                      | —                    | —                                                                                            | —                 |
| `sfx_entama_decrease.mp3`            | stage ของ Entamaphobia ลดลง (spatial 60 / 1920)                      | `EntamaDecrease`                                                     | —                    | —                                                                                            | —                 |
| `sfx_entama_presence.mp3`            | เสียงพึมพำของ Entamaphobia ตั้งแต่ stage 3 (เล่นวน)              | `EntamaPresence`                                                     | —                    | —                                                                                            | —                 |
| `sfx_entama_jumpscare.mp3`           | จังหวะ jumpscare ตอนเปิดประตูเจอ Entamaphobia            | `EntamaJumpscare`                                                    | —                    | —                                                                                            | —                 |
| `sfx_phobias_hit.mp3`                | เสียงตอนโดน phobia ชน (Autophobia / Scopophobia)                                         | `PhobiaHit`                                                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_heartbeat.mp3`                  | เสียงหัวใจเต้น (ระดับสติ Insane วนทุก 10–16 วินาที)                     | `HeartBeat`                                                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_panting.mp3`                    | เสียงหอบหายใจ (Anxious 0.025 · Insane 0.065)                                                 | `Painting`                                                           | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_light_break.mp3`                | ไฟแตก (Scopophobia วิ่งผ่าน `LightArea` ที่ยังติด)                                          | `LightBreak`                                                         | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_darkness.mp3`                   | ความมืดกลืน (เข้า `LightArea` ที่ดับ — เล่นแบบ `waitExist`)                   | `Darkness`                                                           | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_screen_off.mp3`                 | จอดำ (Scopophobia เกิด)                                            | `ScreenOff`                                                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_screen_on.mp3`                  | จอกลับมา (หลังจอดำ หรือหลังปิดจอแอบมอง)                                                           | `ScreenOn`                                                           | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_flashlight_toggle.mp3`          | เปิด/ปิดไฟฉาย                                                  | `FlashlightToggle`                                                   | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_light_flicker_long.mp3`         | ไฟกระพริบยาว (ตอน Nyctophobia เกิด)                     | `LightFlickerLong`                                                   | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_light_flicker_short_01..03.mp3` | ไฟกระพริบสั้น 3 แบบ (สุ่มตอน Autophobia แตะไฟ) | `LightFlickerShort1..3`                                              | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |
| `sfx_clipboard_pageturn.mp3`         | พลิกหน้าคลิปบอร์ดไปข้างหน้า                                        | `ClipboardPageAdvance`                                               | —                    | —                                                                                            | —                 |
| `sfx_clipboard_pageturnback.mp3`     | พลิกหน้าคลิปบอร์ดย้อนกลับ                                        | `ClipboardPageReverse`                                               | —                    | —                                                                                            | —                 |
| `sfx_clipboard_check.mp3`            | เปิด/ปิดคลิปบอร์ด                                                  | `ClipboardCheck`                                                     | —                    | —                                                                                            | —                 |
| `sfx_step_01..08.mp3`                | เสียงฝีเท้า 8 แบบ (ไล่ตามลำดับ `_stepOrder`)                   | *(โหลดแยกเป็น array `_steps` — ไม่มีค่าใน enum)*                          | Pixabay              | (Free for use under the Pixabay[Content License](https://pixabay.com/service/license-summary/)) | พรภวิษย์   |

## Audio — Ambience & Music (BGM)

| Asset Name                  | คำอธิบาย                                                           | enum `BackgroundSound` | เล่นเมื่อ                                                                           |
| --------------------------- | -------------------------------------------------------------------------- | ------------------------ | -------------------------------------------------------------------------------------------- |
| `amb_sewer.mp3`           | บรรยากาศโถงอุโมง (น้ำ dripping, ชื้น)               | `AmbSewer`             | โหลดฉาก `tilemap_sewer`                                                             |
| `amb_electricroom.mp3`    | บรรยากาศห้องไฟฟ้า (ใช้เป็นค่า default ด้วย) | `AmbElectric`          | ฉากห้องไฟฟ้าทั้งสอง + ตอนสร้าง `GameplayScreen`                                                       |
| `bgm_mainmenu_screen.mp3` | เพลงเมนูหลัก                                                   | `BgmMainMenu`          | `MainMenuScreen`                                                                           |
| `bgm_death_screen.mp3`    | เพลงจอตาย                                                         | `BgmDeathScreen`       | `DeathScreen`                                                                              |
| `bgm_win_screen.mp3`      | เพลงจอชนะ                                                         | `BgmWinScreen`         | `WinScreen` |

> ระดับเสียง default ของ ambience/BGM = `AudioManager.BgsVolume` (0.2) และปรับได้จากสไลเดอร์ "Ambience Volume" ใน Settings / pause overlay

## Fonts

| Asset Name                                                          | คำอธิบาย                                                                                                                   | mgcb | โหลดโดย                                                | รับผิดชอบ |
| ------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- | ---- | ------------------------------------------------------------- | ------------------ |
| `Fonts/GenerationFonting.fnt` + `Fonts/GenerationFonting_0.png` | Bitmap font หลักสำหรับ HUD (โฟลเดอร์`Fonts/` พหูพจน์)                                                   | ✅   | `TextManager` (`Fonts/GenerationFonting`)                 | พรภวิษย์   |
| `Fonts/TrueTypeFont/GenerationFonting.ttf`                        | ฟอนต์ที่ Gum สร้างตอน runtime ผ่าน `KernSmithFontCreator` (ลงทะเบียนชื่อ `generationfonting`) — copy ผ่าน csproj ไม่ใช่ mgcb | csproj | `MainGame.Initialize` → ใช้โดยทุกหน้าจอ Gum | พรภวิษย์   |

> `SceneManager.Debug()` ยังอ้าง path `"Font/GenerationFonting"` (โฟลเดอร์เอกพจน์) ในโค้ดที่คอมเมนต์ไว้ — โค้ดส่วนนั้นไม่ถูกเรียก

## Shaders

| Asset Name                        | คำอธิบาย                                                                                                         | mgcb | โหลดโดย                            |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------ | ---- | ----------------------------------------- |
| `Shaders/Vignette.fx`           | เอฟเฟกต์ขอบมืด (เทคนิค `EllipseMask`) — sanity / blink / Nyctophobia                              | ✅   | `VignetteOverlay`                       |
| `Shaders/BottomFade.fx`         | เฟดด้านล่างของจอ                                                                                         | ✅   | `BottomFadeOverlay`                     |
| `Shaders/Silhouette.fx`         | เขียน mask ของ tile ที่จะ highlight (เทคนิค `HighlightMask`)                                        | ✅   | `TileHighlightRenderer`                 |
| `Shaders/HighlightComposite.fx` | รวม mask เป็น outline / scan / pulse (เทคนิค `Highlight`)                                                 | ✅   | `TileHighlightRenderer`                 |
| `Shaders/Saturation.fx`         | เอฟเฟกต์ความอิ่มสีทั้งเฟรม (เทคนิค `ApplySaturation`) — ใช้ตอนโดน Autophobia | ✅   | `ModifiedScreenManager.LoadContent` / `DrawGraded` |

## Data / Config

| Asset Name              | คำอธิบาย                                                     | mgcb | โหลดโดย      |
| ----------------------- | -------------------------------------------------------------------- | ---- | ------------------- |
| `Datas/tutorial.json` | ข้อมูล tutorial 10 ขั้น (`Steps[] { Text, CompleteOn }`) — อ่านด้วย `File.ReadAllText` จาก `AppContext.BaseDirectory/Content/Datas/tutorial.json` | ❌ (copy ผ่าน csproj) | `TutorialManager.Load` |

> ชื่อโฟลเดอร์ `Datas/` และไฟล์ `TutorialData` สะกดตามที่มีอยู่ในโปรเจกต์จริง (`Scripts/Datas/TurotialData.cs`)

## Tilemaps

| Asset Name                                  | คำอธิบาย                                                                                                                                                           | mgcb |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---- |
| `Tilemaps/tilemap_electrical_room.ldtk`   | ฉากห้องไฟฟ้า 1 — 1920×1080 (2 walls · 1 panel · 1 locker · 1 clipboard · 2 doors [sewer/end] · 2 spawns · 5 SpotLight)                                 | ✅   |
| `Tilemaps/tilemap_electrical_room_2.ldtk` | ฉากห้องไฟฟ้า 2 — 1920×1080 (2 walls · 1 panel · 1 locker · 1 clipboard · 1 door [sewer] · 1 spawn · 5 SpotLight)                                       | ✅   |
| `Tilemaps/tilemap_sewer.ldtk`             | ฉากโถงอุโมง —**6960×1080** (3 walls · **5 valves** · 4 lockers · 2 doors [ห้องไฟฟ้า 1/2] · 2 spawns · 12 PointLight · 2 DoorLight) | ✅   |

รายละเอียด entity / property / tile layer / tileset / destination / origin: [Data Schema § Level Data](../03_Software/03-data-schema.md#2-ข้อมูลระดับ-level-data)

## Related Documents

- Pipeline: [Asset Pipeline Flow](05-asset-pipeline-flow.md)
- Naming convention: [GDD README](README.md)
- Staging area: [02_Assets](../02_Assets/)
- Art direction: [Art Direction](07-art-direction.md) · Audio: [Audio Direction](08-audio-direction.md)
- Data schema (level / animation data): [Data Schema](../03_Software/03-data-schema.md)
