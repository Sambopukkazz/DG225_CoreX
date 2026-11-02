---
type: software-data-schema
version: 4.0
date: 2026-11-02
---
# Imaginophobia — Data Schema

**Version:** 4.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

> เอกสารนี้อธิบายโครงสร้างข้อมูลที่เกมใช้ ณ ปัจจุบัน — ตรวจจากโค้ดและไฟล์ใน `Content/` จริง (commit `3a300ec`, 2026-11-02)
> ไม่นับ `bin/` และ `obj/` ที่เป็น build artifact
> อ้างอิง: [System Design](01-system-design.md) · [Class Diagram](02-class-diagram.md)

## 1. Enums (ค่าคงที่ในเกม)

ค่าคงที่ทั้งหมดประกาศเป็น `enum` ใน `Scripts/Enums/` (ยกเว้น `CharacterAction` ที่อยู่ที่ `Scripts/Enums/CharacterAction.cs` เช่นกัน)

### `SanityState` — ระดับสติ (`Scripts/Enums/SanityState.cs`)

| ชื่อ    | ค่า | ความหมาย                 |
| ----------- | ------ | -------------------------------- |
| `Normal`  | 80     | ปกติ                         |
| `Anxious` | 40     | เริ่มกังวล             |
| `Insane`  | 0      | สติแตกระดับวิกฤต |

> ค่า enum ถูกใช้เป็น **threshold** ใน `Player.CheckSanityStage()` — Sanity เริ่มที่ 100 แล้วลดลงผ่าน 80 → 40 → 0 · การเทียบใช้ `<=` ที่ขอบบน (`Sanity <= Normal` → Anxious) และถ้าสติเพิ่มกลับเกิน 80 จะกลับเป็น `Normal`

### `CharacterAction` — สถานะการกระทำ (`Scripts/Enums/CharacterAction.cs`)

`Repairing` · `Checking` · `Reading` · `Peeking` · `Hiding` · `Walking` · `Idle`

> enum ไม่มีค่า `Sneaking` และไม่มี `ReadyToHide` — การซุ่มเดิน (กด `Left Ctrl`) ไม่ได้เปลี่ยน state แต่ลดความเร็ว ÷1.7 และล็อกทิศการหันหน้า โดย `CurrentAction` ยังเป็น `Walking`
> `Reading` เพิ่มมาพร้อมคลิปบอร์ด (`Player.ToggleReading`, trigger tag `report`)

### `Scene` — ฉากที่โหลดได้

| ค่า                        | ไฟล์                                            |
| ----------------------------- | --------------------------------------------------- |
| `tilemap_electrical_room`   | `Content/Tilemaps/tilemap_electrical_room.ldtk`   |
| `tilemap_electrical_room_2` | `Content/Tilemaps/tilemap_electrical_room_2.ldtk` |
| `tilemap_sewer`             | `Content/Tilemaps/tilemap_sewer.ldtk`             |

### `SoundEffectName` — SFX ที่เล่นได้ (`Scripts/Enums/SoundEffectName.cs`)

**33 ค่า:** `DoorClose` · `DoorOpen` · `DoorStuck` · `LockerClose` · `LockerOpen` · `PipeComplete` · `PipeFail` · `PipeRepairing` · `PipeSuccess` · `ElecComplete` · `ElecFail` · `ElecRepairing` · `ElecSuccess` · `ScopoIndicator` · `EntamaAdvance` · `EntamaDecrease` · `EntamaPresence` · `EntamaJumpscare` · `PhobiaHit` · `HeartBeat` · `Painting` · `LightBreak` · `Darkness` · `ScreenOff` · `ScreenOn` · `FlashlightToggle` · `LightFlickerLong` · `LightFlickerShort1` · `LightFlickerShort2` · `LightFlickerShort3` · `ClipboardPageAdvance` · `ClipboardPageReverse` · `ClipboardCheck`

> ทุกค่าถูก map กับไฟล์จริงใน `AudioManager.SfxByName` — enum ใช้ชื่อ `PipeFail` / `ElecFail` แต่ไฟล์จริงชื่อ `sfx_pipe_failed.mp3` / `sfx_electric_failed.mp3` (map ถูกต้อง) · `Painting` map กับ `sfx_panting.mp3` (ชื่อไฟล์พิมพ์ผิด)
> enum ชื่อ `SoundEffectName` (เดิมชื่อ `Sound`) · ค่า `AutoIndicator` ถูกถอดออกแล้ว — ไม่มีไฟล์ `sfx_autophobia_indicator` ใน source

### `BackgroundSound` — ambience / BGM (`Scripts/Enums/BackgroundSound.cs`)

| ค่า             | ไฟล์                    | เล่นเมื่อ                                                            |
| ------------------ | --------------------------- | ----------------------------------------------------------------------------- |
| `AmbElectric`    | `amb_electricroom.mp3`    | โหลดฉากที่ไม่ใช่ sewer + ตอน `GameplayScreen.Initialize`      |
| `AmbSewer`       | `amb_sewer.mp3`           | โหลดฉาก `tilemap_sewer`                                              |
| `BgmMainMenu`    | `bgm_mainmenu_screen.mp3` | `MainMenuScreen`                                                            |
| `BgmDeathScreen` | `bgm_death_screen.mp3`    | `DeathScreen`                                                               |
| `BgmWinScreen`   | `bgm_win_screen.mp3`      | `WinScreen`                                                                 |

### `GameState` — สถานะเกม (`Scripts/Enums/GameState.cs`)

`Menu` · `Gameplay` · `Lose` · `Win` · `Pause` · `Jumpscare`

> ถูกใช้จริงเป็น static `MainGame.GameState` — `Player.Update` หยุดเมื่อเป็น `Jumpscare`, `GameManager.Update` `return` เมื่อเป็น `Pause`, `CollisionManager` ไม่รับ interaction เมื่อเป็น `Jumpscare` (ดู [System Design § 1](01-system-design.md#gamestate-สถานะเกมระดับ-static))

### `VignettePhase` — เฟสของ Nyctophobia

`StopNyctophobia` · `StartNyctophobia`

> enum `BlinkPhase` ถูกลบออกจากโปรเจกต์แล้ว

## 2. ข้อมูลระดับ (Level Data)

### LDtk Tilemaps

ไฟล์ `.ldtk` 3 ไฟล์ใน `Content/Tilemaps/` เป็นแหล่งข้อมูลหลักของฉาก — แต่ละไฟล์มี 1 level ชื่อ `Level_0`
ขนาด level: ห้องไฟฟ้าทั้งสอง **1920×1080** (1 จอ) · โถงอุโมง **6960×1080** (ยาว ~3.6 จอ)

`SceneBuilder` อ่านไฟล์เหล่านี้แล้วสร้าง:

- **TileMap** — ชั้นภาพและข้อมูล tile (ส่งให้ `TilemapSpriteBatchRenderer`)
- **Collider** — จาก object layer `Collision` (ส่งให้ `CollisionManager`)
- **Light** — จาก object layer `Lighting` (ส่งให้ `LightManager`)
- **PlayerSpawnPoints** — จุดเกิดผู้เล่น (`SpawnPoint { Name, Position }` โดย `Name` = property `Origin`)
- **Triggers** — trigger ที่ใช้ทำ highlight + interact (`Trigger { Id, Name, Tag, Interactable, Progress, TileLayer, Tiles }`)

### Object Layer `Collision`

`SceneBuilder` แยกประเภท entity ด้วย **`entity.Class`** ซึ่ง MonoGame.Extended map มาจาก **LDtk tag ตัวแรก** ของ entity (`__tags[0]`) ไม่ใช่ชื่อ entity

| `entity.Class` (tag) | กลายเป็น                                              | ชื่อ entity ที่ใช้จริง                                           | Properties                           |
| ---------------------- | ------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------------------------------------ |
| `wall`               | `Wall` (layer `walls`)                                    | `InvisWall`                                                                  | —                                   |
| `skillcheck`         | `Trigger` (layer `triggers`)                              | `Valve`                                                                      | —                                   |
| `dots`               | `Trigger` (layer `triggers`)                              | `ElectricalPanel`                                                            | —                                   |
| `hideout`            | `Trigger` (layer `triggers`)                              | `Locker`                                                                     | —                                   |
| `door`               | `Trigger` (layer `triggers`) + ชื่อ = `Destination` | `LoadNextScene`                                                              | `Destination`                      |
| `report`             | `Trigger` (layer `triggers`) + ชื่อ = `Detail`      | `Clipboard`                                                                  | `Detail` (`Story1` / `Story2`) |
| `spawn`              | `SpawnPoint` + ชื่อ = `Origin`                        | `PlayerSpawnPoint` (ห้องไฟฟ้า) · `SpawnPoint` (โถงอุโมง) | `Origin`                           |

### Object Layer `Lighting`

`SceneBuilder` ตรวจ `entity.Class == "light"` แล้วแยกตาม **`entity.Name`** (ชื่อ entity ใน LDtk)

| `entity.Name` | กลายเป็น                                           | Properties ที่อ่าน                                      | ค่าที่ใช้อยู่                                                                        |
| --------------- | ---------------------------------------------------------- | -------------------------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| `PointLight`  | `PointLight` + `LightArea` (collider layer `lights`) | `Intensity`, `Scale`, `Color`                            | intensity 0.5 · scale 500 ·`#9CEFFF` (sewer)                                                  |
| `DoorLight`   | `PointLight` เฉย ๆ (ไม่มี collider)             | `Intensity`, `Scale`, `Color`                            | intensity 0.1 · scale 500 ·`#FFF2A5`                                                          |
| `SpotLight`   | `Spotlight` (ไม่มี collider)                        | `Intensity`, `ScaleX`, `ScaleY`, `Rotation`, `Color` | intensity 0.5 · rotation 90° · 750×1250 (ห้องละ 4 ตัว และมี 1 ตัวที่เป็น 750×5000) ·`#FFF2A5` |

### จำนวน entity จริงในแต่ละฉาก

| ฉาก                        | wall | skillcheck  | dots | hideout | door | report | spawn | แสง                                   |
| ----------------------------- | ---- | ----------- | ---- | ------- | ---- | ------ | ----- | ---------------------------------------- |
| `tilemap_electrical_room`   | 2    | —          | 1    | 1       | 2    | 1      | 2     | `SpotLight` ×5                        |
| `tilemap_electrical_room_2` | 2    | —          | 1    | 1       | 1    | 1      | 1     | `SpotLight` ×5                        |
| `tilemap_sewer`             | 3    | **5** | —   | 4       | 2    | —     | 2     | `PointLight` ×12 · `DoorLight` ×2 |

> วาล์ว 5 จุดในโถงอุโมงตรงกับ `ObjectiveManager.TargetValves = 5` พอดี
> ห้องไฟฟ้าใช้ `SpotLight` (ไม่มี `LightArea`) ทั้งหมด — จึง**ไม่**ทำให้ผู้เล่นติด `Nyctophobia` และไม่ถูก Scopophobia ดับไฟ (ระบบไฟที่ถูกจับตาคือ `PointLight` ของโถงอุโมง + แสงของห้องที่ถูก `TurnOff` ตอน Scopophobia มา)
> คลิปบอร์ดในห้องไฟฟ้า 1 ใช้ `Detail: Story1` และห้องไฟฟ้า 2 ใช้ `Detail: Story2`

**Destination ของประตู**

| ฉาก                        | ประตู | Destination                                                  |
| ----------------------------- | ---------- | ------------------------------------------------------------ |
| `tilemap_electrical_room`   | 2          | `tilemap_sewer` · `end`                                 |
| `tilemap_electrical_room_2` | 1          | `tilemap_sewer`                                            |
| `tilemap_sewer`             | 2          | `tilemap_electrical_room_2` · `tilemap_electrical_room` |

**Origin ของจุดเกิด** (ใช้เลือกจุดวางตัวเมื่อเข้าฉาก)

| ฉาก                        | spawn | Origin                                                       |
| ----------------------------- | ----- | ------------------------------------------------------------ |
| `tilemap_electrical_room`   | 2     | `Start` · `tilemap_sewer`                               |
| `tilemap_electrical_room_2` | 1     | `tilemap_sewer`                                            |
| `tilemap_sewer`             | 2     | `tilemap_electrical_room` · `tilemap_electrical_room_2` |

### Tile Layers (ชั้นภาพ)

| ฉาก                        | ชั้นภาพ                                                                                                                                                                                 |
| ----------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `tilemap_electrical_room`   | `Shadow` · `ElectricalPanel` · **`Ladder`** · `Doors` · `Lockers` · **`Clipboard`** · `Decoration2` · `Decoration1` · `SewerIndicator` · `Floor` · `Background` |
| `tilemap_electrical_room_2` | `Shadow` · `Decoration` · `Decoration2` · `Doors` · `SewerIndicator` · `Clipboard` · `Lockers` · `ElectricalPanel` · `Floor` · `Background`                                    |
| `tilemap_sewer`             | `ShadowBoundary` · `Decoration3` · `Decoration2` · `Decoration` · `Lights` · `Doors` · `Lockers` · `Valves` · `Pipes` · `ExitSign` · `Floor` · `Background`                 |

ชั้นที่ระบบ highlight ใช้: `Valves` · `Lockers` · `ElectricalPanel` · `Doors` · `Clipboard` · `Ladder` — `SceneBuilder.FindHighlightLayer` จับคู่ entity กับ layer ด้วยชื่อ (ตัด `s`/`S` ตัวท้ายออกทั้งสองฝั่ง) ยกเว้น entity `LoadNextScene` ที่ใช้ layer `Doors` และถ้า `Destination == "end"` จะใช้ layer `Ladder`

### Tilesets (`mapassets/`)

มี 10 ไฟล์ในโฟลเดอร์ — 9 ไฟล์ถูกอ้างจาก `.ldtk` จริง:

| ไฟล์                               | ใช้ใน                       | อ้างโดย layer |
| -------------------------------------- | -------------------------------- | ------------ |
| `spritesheet_background-texture.png` | ห้องไฟฟ้าทั้งสอง      | — (มีใน tileset list ของ `.ldtk`) |
| `spritesheet_er_floor.png`           | ทั้ง 3 ฉาก                | `Floor` |
| `spritesheet_er_tiles.png`           | ทั้ง 3 ฉาก                | `ElectricalPanel` · `Lockers` · `Decoration` · `Lights` · `Doors` · `Background` |
| `spritesheet_general_decoration.png` | ทั้ง 3 ฉาก                | `Ladder` · `Doors` · `Decoration1` · `Decoration2` · `Decoration3` · `Floor` |
| `spritesheet_sewer_tiles.png`        | โถงอุโมง                 | `Valves` · `Pipes` · `Decoration2` · `Background` |
| `spritesheet_shadow.png`             | ทั้ง 3 ฉาก                | `Shadow` · `ShadowBoundary` |
| `Clip_board.png`                     | ห้องไฟฟ้าทั้งสอง | `Clipboard` |
| `sewer_indicator_red.png`            | ห้องไฟฟ้าทั้งสอง | `SewerIndicator` |
| `Exit_sign.png`                      | โถงอุโมง                 | `ExitSign` |
| `sewer_indicator.png`                | —                            | **ไม่มี `.ldtk` ไฟล์ไหนอ้างถึง** |

> ไฟล์ tileset ไม่ต้องลงทะเบียนใน `Content.mgcb` แยก — `LDtkTilemapImporter` โหลดให้จาก path ใน `.ldtk` (ดังนั้น tileset ที่ชื่อใน `.ldtk` ไม่ตรงกับไฟล์ที่มีอยู่ จะพังตอน build/รัน ไม่ใช่ตอน compile)

## 3. Animation Data

### ตัวละครผู้เล่น — `Content/Character/spritesheet_player.json`

TexturePacker JSON (dataformat `monogame-extended`) คู่กับ `spritesheet_player-texture.png` (960×360, **22 เฟรม** เฟรมละ 120×120, pivot กลาง)

| กลุ่มเฟรม                       | จำนวน | ใช้ทำอะไร                    |
| ---------------------------------------- | ---------- | ------------------------------------- |
| `sprite_back`                          | 1          | ท่าหันหลัง (ตอนซ่อม / อ่านคลิปบอร์ด) |
| `sprite_idle_01` … `sprite_idle_12` | 12         | ท่า idle                           |
| `sprite_walk_01` … `sprite_walk_09` | 9          | ท่าเดิน / ซุ่ม             |

`Player.SetUpAnimation()` ประกอบเป็น animation 6 ตัว:

| Animation          | เฟรม                                  | Loop | ความยาว/เฟรม           |
| ------------------ | ----------------------------------------- | ---- | --------------------------------- |
| `walk-forward`   | `sprite_walk_01..09`                    | ✅   | 0.15s (เฟรมสุดท้าย 0s) |
| `sneak-forward`  | `sprite_walk_01..09`                    | ✅   | 0.2s                              |
| `sneak-backward` | `sprite_walk_09..01` (ย้อนกลับ) | ✅   | 0.2s                              |
| `idle`           | `sprite_idle_01..12`                    | ✅   | 0.4s                              |
| `back`           | `sprite_back`                           | ❌   | —                                |
| `check`          | `sprite_idle_01`                        | ❌   | —                                |

`EnemyManager` ประกอบ animation ของ Autophobia จาก atlas **ตัวเดียวกัน** (`Character/spritesheet_player`) แล้วย้อมสีดำ — `walk-forward` (9 เฟรม, 0.12s) และ `idle` (1 เฟรม, ไม่ loop)

### Entamaphobia — `Content/Phobias/spritesheet_entamaphobia.json`

TexturePacker atlas (4096×4096) **6 เฟรม** — `entama_stage_1` · `entama_stage_2` · `entama_stage_3` · `entama_stage_4` · `entama_hand` · `entama_jumpscare`

- `DoorPeekingScreen` วาด `entama_stage_{Trigger.Progress}` (stage ที่ตัว Entamaphobia ตัวนั้นเขียนกลับลง trigger ทุกเฟรม) และวาด `entama_hand` ทับเมื่อ `Progress > 2`
- `JumpScareScreen` วาด `entama_jumpscare` เต็มจอ

### UI atlas

| Atlas | จำนวนเฟรม | ชื่อเฟรม |
| --- | --- | --- |
| `UI/spritesheet_buttons.json` (450×460) | 6 | `sprite_spacebar` · `sprite_button_exit` · `sprite_button_left` · `sprite_button_right` · `sprite_button_light` · `sprite_button_peek` |
| `UI/spritesheet_clipboard.json` (4096×2048) | 9 | `sprite_clipboard_er1_page1..4` · `sprite_clipboard_er2_page1..4` · `sprite_clipboard_secret` |
| `UI/spritesheet_minigame_dot.json` (771×736) | 13 | `minigame_dot_grid` · `sprite_dot_{blue,cyan,green,purple,red,yellow}` · `sprite_line_{blue,cyan,green,purple,red,yellow}` |

## 4. Tutorial Data

`Content/Datas/tutorial.json` — อ่านด้วย `TutorialManager.Load()` ผ่าน `File.ReadAllText` จาก `AppContext.BaseDirectory/Content/Datas/tutorial.json` (ไฟล์ถูก copy ไป output ด้วย `CopyToOutputDirectory=PreserveNewest` ใน `Imaginophobia.csproj` — **ไม่ได้** ผ่าน `Content.mgcb`)

| ฟิลด์                  | ชนิด               | ความหมาย                                                                    |
| --------------------------- | ---------------------- | ----------------------------------------------------------------------------------- |
| `Steps`                   | `List<TutorialStep>` | ลำดับข้อความ tutorial                                                   |
| `TutorialStep.Text`       | `string`             | ข้อความที่แสดงบนจอ                                                |
| `TutorialStep.CompleteOn` | `string`             | objective id ที่ต้องสำเร็จก่อนจะเลื่อนไป step ถัดไป |

ลำดับ step ปัจจุบัน (10 ขั้น ตรงกับไฟล์จริง):

| #  | Text (เต็ม)                                           | CompleteOn          |
| -- | ------------------------------------------------------- | ------------------- |
| 1  | Press A/D to walk around. F to use flashlight           | `move`            |
| 2  | Press space to interact with objects                    | `interact`        |
| 3  | Head to the electrical panel to do a task               | `repair`          |
| 4  | Connect all the dots.\nPress A/D to look left and right, Q to leave | `dots-done`       |
| 5  | You can peek under the door to check the outside        | `peek`            |
| 6  | Leave the room on the left side, look for a valve       | `door-open`       |
| 7  | Find a valve to repair                                  | `valve-start`     |
| 8  | Click and spin the valve. Stay on the green gauge. \nPress A/D to look left and right, F to use flashlight, Q to leave | `valve-done`      |
| 9  | Repair all five valves                                  | `valves-all`      |
| 10 | Head to the ladder in the right electric room and exit. | `exit-ladder`     |

`TutorialManager.Advance()` จะข้าม step ที่ `CompleteOn` ทำเสร็จไปแล้ว — มันถูกเรียกใน constructor และทุกครั้งที่ `ObjectiveManager.Completed` ยิงออกมา

**Objective id ที่ใช้จริง** (เรียกผ่าน `ObjectiveManager.Complete(id)`)

| id                     | เรียกจาก                                                                                        |
| ---------------------- | ------------------------------------------------------------------------------------------------------- |
| `move`               | `UIManager.Update` (กด `A` + `D` + `F` ครบทั้งสามปุ่ม)                                     |
| `interact`           | `CollisionManager` (ตู้ / แผงไฟ / คลิปบอร์ด / ใช้ประตู)                                     |
| `repair`             | `CollisionManager` (เปิดมินิเกม Connect The Dots)                                          |
| `dots-done`          | `ConnectTheDotsScreen` (ซ่อมแผงไฟสำเร็จ)                                               |
| `peek`               | `DoorPeekingScreen` (กด `Q` ออกจากจอแอบมอง)                                              |
| `door-open`          | `CollisionManager` (ใช้ประตูด้วย `Space`)                                                          |
| `valve-start`        | `SpinTheValveScreen` constructor (เปิดมินิเกมวาล์ว)                                   |
| `valve-done`         | `SpinTheValveScreen` (หมุนวาล์วสำเร็จ)                                                 |
| `valves-all`         | `ObjectiveManager.RepairValve` (ครบ 5 วาล์ว)                                                  |
| `entered-sewer`      | `GameManager.OnSceneLoaded` (เข้าฉาก sewer) — ใช้เป็นเงื่อนไขแสดงตัวนับวาล์วบน HUD           |
| `exit-ladder`        | `GameManager.OnDoorInteracted` (ประตู `Destination == "end"` และเงื่อนไขครบ)           |

## 5. Save Game

`SaveManager` เป็น static class ที่เขียน JSON ลง **`%APPDATA%\Imaginophobia\save.json`** (`Environment.SpecialFolder.ApplicationData`)

```json
{
  "Scenes": {
    "tilemap_sewer": {
      "Triggers": [ { "Id": 12, "Progress": 200, "Interactable": false } ],
      "Lights":   [ { "Id": 3,  "On": true } ]
    }
  }
}
```

| คลาส        | ฟิลด์                                                  | ความหมาย                                                            |
| --------------- | ----------------------------------------------------------- | --------------------------------------------------------------------------- |
| `GameData`    | `Dictionary<string, SceneData> Scenes`                    | key = ชื่อฉาก                                                        |
| `SceneData`   | `List<TriggerData> Triggers`, `List<LightData> Lights`  | สถานะของฉากนั้น                                              |
| `TriggerData` | `Id` (int), `Progress` (float), `Interactable` (bool) | ความคืบหน้าของมินิเกม + ยังโต้ตอบได้ไหม |
| `LightData`   | `Id` (int), `On` (bool)                                 | ไฟเปิด/ปิด                                                         |

**รอบการทำงาน**

| เมธอด                                        | เรียกเมื่อ                                                                                                            |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| `SaveScene(sceneName, triggers, lightAreas)`    | ก่อนออกจากฉาก (`SceneManager.LoadScene` → `UnloadScene`) — ข้าม trigger ที่ tag เป็น `door`          |
| `ApplyToScene(sceneName, triggers, lightAreas)` | หลังสร้างฉากใหม่ (`SceneBuilder` สร้างเสร็จ)                                                        |
| `TurnAllLightsOn()`                             | ซ่อมแผงไฟสำเร็จ (`ConnectTheDotsScreen`)                                                                       |
| `AllSewerLightsAreOn()`                         | เงื่อนไขชนะ — คืน `true` เฉพาะเมื่อ **มี** entry ของฉาก sewer และไฟทุกดวงใน entry นั้นติด |
| `ResetScene(sceneName)`                         | Nyctophobia หรือ Scopophobia เกิด (`GameManager.OnEnemySpawned` รีเซ็ตห้องไฟฟ้าทั้งสอง)         |
| `Delete()`                                      | `GameManager` constructor (เริ่มรอบใหม่) — และเรียกอีกครั้งตอนชนะที่ประตู `end` |

> **ยังไม่บันทึก:** ตำแหน่งผู้เล่น · Sanity · Battery · ฉากปัจจุบัน · objective ที่ทำสำเร็จ
> เกมเริ่มจาก `MainMenuScreen` แล้วสร้าง `GameManager` ใหม่เสมอ (โหลด `tilemap_electrical_room`) → save มีอายุแค่ภายในรอบเล่นเดียว ใช้จำ progress ของ task ระหว่างสลับฉากเท่านั้น

## 6. ค่าปรับแต่งเกม (Tuning Values)

> ไม่มีไฟล์ config/data กลาง — ค่าปรับแต่งทั้งหมด hardcode อยู่ในโค้ด (มีแค่ `Datas/tutorial.json` ที่เป็น data จริง)

### Player

| ค่า                                                     | ค่าปัจจุบัน                                                       | ตำแหน่ง                                  |
| ---------------------------------------------------------- | ---------------------------------------------------------------------------- | ----------------------------------------------- |
| Player speed                                               | 250                                                                          | `Player.MoveSpeed`                            |
| Player sneak                                               | ÷ 1.7 (+ ล็อกทิศหันหน้า)                                      | `Player.Update`                               |
| Player size                                                | 100 × 240 (scale 3)                                                         | `Player._size`                                |
| Sanity เริ่มต้น / threshold                        | 100 · 80 / 40 / 0                                                           | `Player.Sanity`, `SanityState`              |
| Battery เริ่มต้น / อัตราการลด            | 100 · 1 ต่อวินาที                                                | `Player.Battery`, `Player.Update`           |
| แสงรอบตัว (scotopic)                              | scale 500×500 · intensity 0.5 ·`Color.FromHSV(150, 0.5, 0.5)`           | `Player` constructor                          |
| ไฟฉาย                                                 | scale 600×1250 · intensity 1 · offset เริ่มต้น (85, −12) แล้วขยับตามเฟรม animation | `Player` constructor + `Player.Animate`                          |
| ระยะสายตา (`EyeSight`) ตอนเปิดไฟฉาย | 400 px                                                                       | `Player.Update`                               |
| Sanity ลดในโถงอุโมง                            | 0.5 / 0.2 / 0 ต่อวินาที (Normal/Anxious/Insane)                     | `Player._availableSanityRedectionSpeed`       |
| Sanity ที่ลดเมื่อชน Autophobia                 | −10 +`ScreenManager.Saturation = 0`                                            | `CollisionManager`                            |
| Sanity ที่ลดเมื่อชน Scopophobia                | −25                                                                         | `CollisionManager`                            |
| Hide duration / cooldown                                   | 10 วินาที · 3 วินาที                                            | `Player.HideTime`, `Player.HideCooldown` |
| FocusLevel (ซูมกล้อง)                              | 1 ปกติ · 1.25 ซ่อน/ซ่อม/อ่าน · 1.75 แอบมอง                    | `Player`                                      |
| Saturation recovery                                        | +0.3 ต่อวินาที (จาก 0 กลับเป็น 1 ใน ~3.3 วินาที) | `UIManager.Update`                            |

### Enemies

| ค่า                                         | ค่าปัจจุบัน                                                                                                      | ตำแหน่ง                                       |
| ---------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------- |
| Autophobia speed                               | 400                                                                                                                         | `Autophobia.MoveSpeed`                             |
| Autophobia อายุ                            | 28 วินาที                                                                                                             | `Autophobia` constructor                           |
| Autophobia spawn interval                      | 4.96 วินาที (timer `AutoSpawnTimer`)                                                                                | `EnemyManager`                                     |
| Scopophobia speed                              | 5000                                                                                                                        | `Scopophobia.MoveSpeed`                            |
| Scopophobia เวลาก่อนพุ่ง           | 8 วินาที                                                                                                              | `Scopophobia` constructor                          |
| Scopophobia อายุ                           | 16 วินาที (`SetActive(false)`)                                                                                            | `Scopophobia` constructor                          |
| Scopophobia spawn interval                     | 4.98 วินาที (timer `ScopoSpawnTimer`)                                                                               | `EnemyManager`                                     |
| Scopophobia ถือว่า active เมื่อ     | `LightPassedCount < 12`                                                                                                    | `EnemyManager.HasActiveScopophobia`                |
| Scopophobia เกิดแบบบังคับ       | เมื่อซ่อมวาล์วครบ (`ForceSpawnScopophobia` ทำงานครั้งเดียวต่อรอบ)                                      | `EnemyManager.Update`                              |
| Nyctophobia spawn interval                     | 29.95 วินาที (timer `NyctoSpawnTimer`)                                                                                 | `EnemyManager`                                     |
| Entamaphobia spawn interval                    | 3.97 วินาที (timer `EntamaSpawnTimer` — ทำงานเฉพาะนอกฉาก sewer)                                                                     | `EnemyManager`                                     |
| Entamaphobia: ความเร็วเพิ่ม stage      | 60 ต่อวินาที (×1.2 เมื่อเกิดตอนสติ `Insane`)                                                                     | `Entamaphobia._increaseSpeed`                      |
| Entamaphobia: เป้าหมายต่อ stage     | stage 1 → 343–492 · stage 2 → 521–678 · stage 3 → 60–1500 · stage 4 → หยุด | `Entamaphobia` constructor / `Update`              |
| Entamaphobia: ความเร็วลด stage         | 1 ต่อวินาที (เพดานการลด: 4→3 = 4 · 3→2 = 4 · 2→1 = 3 · 1→0 = 3)                                                      | `Entamaphobia._decreaseSpeed`                      |
| Entamaphobia: cooldown หลังหาย          | 10 วินาที (timer `EntamaCooldown`)                                                                                     | `Entamaphobia.OnDestroyed`                         |
| Entamaphobia: Sanity ที่ลดตาม stage | 25 / 30 / 35 / 40 (stage 1–4)                                                                                            | `Entamaphobia.OnDestroyed`                         |
| Enemy spawn chance (Normal / Anxious / Insane) | Autophobia `20 / 40 / 55` · Scopophobia `5 / 12 / 20` · Nyctophobia `35 / 50 / 65` · Entamaphobia `15 / 20 / 25` | `EnemyManager` (สลับผ่าน `OnStageChanged`) |
| Enemy spawn distance จาก world bound        | 100–500 px (Scopophobia เพิ่ม ±1920/−10000 อีกชั้น)                                                                  | `EnemyManager.Spawn*`                              |
| Spawn timer เปิด/ปิดตามฉาก        | Scopophobia / Autophobia / Nyctophobia ทำงานเฉพาะฉาก `tilemap_sewer` · Entamaphobia ทำงานเฉพาะฉากอื่น                                          | `EnemyManager.Update`                              |

### Systems

| ค่า                                       | ค่าปัจจุบัน                                                                                                                                                                     | ตำแหน่ง                                  |
| -------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------- |
| ตัวจับเวลาหลัก                 | 721 วินาที → `Player.Dead()` (Sanity = 0 → `DeathScreen`)                                                                                                                                                 | `GameManager` constructor (`"MainTimer"`)   |
| จำนวนวาล์วเป้าหมาย         | 5                                                                                                                                                                                          | `ObjectiveManager.TargetValves`               |
| Spin The Valve: progress เป้าหมาย    | 200                                                                                                                                                                                        | `SpinTheValveScreen`                          |
| Spin The Valve: โซนสำเร็จ / โทษ  | 255°–285° (+20/วินาที) · 285°–313° (−35/วินาที) · 225°–255° (−15/วินาที)                                                                                    | `SpinTheValveScreen`                          |
| Spin The Valve: ความเร็วเข็ม     | สุ่ม ±65/10 องศา/วินาที (ไม่เอา ±20) ทุก 5 วินาที · clamp 225°–313°                                                                                     | `SpinTheValveScreen`                          |
| Connect The Dots: ขนาดกระดาน       | 6 × 6 (ช่อง 100 px)                                                                                                                                                                   | `ConnectTheDotsScreen`                        |
| Connect The Dots: จำนวนรอบซ่อม   | 5 (`_maxRepairStage`, รอบละ +40 progress)                                                                                                                                           | `ConnectTheDotsScreen`                        |
| Connect The Dots: จำนวนสีสูงสุด | 5 (`_maxDots`) จาก palette 6 สี                                                                                                                                                     | `DotsGenerator`                               |
| Connect The Dots: เกณฑ์ level           | path ยาว ≥ 5 ช่อง · ปลายทางห่าง ≥ 3 (ระยะแมนฮัตตัน) · ลองสูงสุด 500 ครั้ง แล้วผ่อนเกณฑ์ลงถ้าหาไม่ได้ | `DotsGenerator`                               |
| Clipboard: จำนวนหน้า                  | 4 หน้าต่อห้อง (`MaxPage = 4`) + หน้าลับ (โอกาส 3% เมื่อพลิกไปหน้าที่ 4)                                                                                            | `ClipboardScreen`                             |
| Clipboard: แว่นขยาย                    | วงกลม 256 px (ขอบ 8 px) · ซูม 1.5×–8× ด้วย scroll                                                                                                                 | `ClipboardScreen`                             |
| Jump scare                                   | 1.6 วินาที แล้วปิดตัวเอง                                                                                                                                                                | `JumpScareScreen`                             |
| ระยะ spatial audio                       | default inner 10 / outer 80 · Scopophobia indicator 5000/26500 · Entamaphobia 60/1920 · ไฟแตก 50/1800 · ไฟกระพริบ 50/1800 · ฝีเท้าผู้เล่น 10/150 · ฝีเท้า Autophobia 10/1200 | `AudioManager` / `LightArea`                |
| ระดับเสียง default                 | `BgsVolume = 0.2` · flicker SFX 0.15 · light break 0.4 · `EntamaJumpscare` 0.2 · `EntamaPresence` 0.5 → 0.75 · เสียง `Painting` 0.025 / 0.065 ตามระดับสติ · SFX คลิปบอร์ด 0.15 / 0.3        | `AudioManager` / `Player` / `Entamaphobia` / `ClipboardScreen` |
| LightArea ขนาดพื้นที่             | 600 × 600 (5 tiles)                                                                                                                                                                       | `LightArea._areaSize`                         |
| LightArea เวลากระพริบ             | 1.5 วินาที (Autophobia) · 3 วินาทีแล้วดับถาวร (Nyctophobia alarm)                                                                                                  | `LightArea`                                   |
| ระยะเวลา black screen                | 1 วินาที                                                                                                                                                                             | `UIManager.BlackScreen`                       |
| Penumbra ambient color                       | `#242323` (ปกติ) · `#615D5D` (จอแอบมองประตู)                                                                                                                                                                                | `LightManager` / `DoorPeekingScreen`                                |
| Vignette scale                               | ปกติ 0.9×0.5 · Anxious 0.55×0.4 · Insane 0.45×0.35 · ซ่อนตัว 0.375×0.175 · Nyctophobia 0.9×0.5 → 0 (25 วินาที)                                                                    | `UIManager`                                   |
| Nyctophobia: ความเร็วหด/ขยาย         | หด 25 วินาที (`ExponentialOut`) · เปิดกลับ 10 วินาที (`SineOut`)                                                                                                        | `UIManager.NyctoClose` / `NyctoOpen`          |
| ข้อความ Death screen tips             | 9 ข้อความ                                                                                                                                                                           | `DeathScreen._tips`                           |
| ข้อความ "ยังมีงานค้าง"                | "I still have work to do." เป็นเวลา 2 วินาที                                                                                                                      | `GameManager.OnDoorInteracted`                |

## Related Documents

- System Design: [System Design](01-system-design.md)
- Class Diagram: [Class Diagram](02-class-diagram.md)
- Asset List: [Asset List](../01_GDD/04-asset-list.md)
- Asset Pipeline: [Asset Pipeline Flow](../01_GDD/05-asset-pipeline-flow.md)
- Mechanics: [Mechanics](../01_GDD/03-mechanics.md)
