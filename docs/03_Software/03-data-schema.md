---
type: software-data-schema
version: 3.0
date: 2026-10-25
---

# Imaginophobia — Data Schema

**Version:** 3.0 | **Last Updated:** 2026-10-25

> เอกสารนี้อธิบายโครงสร้างข้อมูลที่เกมใช้ ณ ปัจจุบัน — ตรวจจากโค้ดและไฟล์ใน `Content/` จริง (commit `cf024ef`, 2026-10-24)
> ไม่นับ `bin/` และ `obj/` ที่เป็น build artifact
> อ้างอิง: [System Design](01-system-design.md) · [Class Diagram](02-class-diagram.md)

## 1. Enums (ค่าคงที่ในเกม)

ค่าคงที่ทั้งหมดประกาศเป็น `enum` ใน `Scripts/Enums/` (ยกเว้น `CharacterAction` ที่อยู่ที่ `Scripts/CharacterAction.cs`)

### `SanityState` — ระดับสติ (`Scripts/Enums/SanityState.cs`)

| ชื่อ | ค่า | ความหมาย |
|---|---|---|
| `Normal` | 85 | ปกติ |
| `Anxious` | 40 | เริ่มกังวล |
| `Insane` | 0 | สติแตกระดับวิกฤต |

> ค่า enum ถูกใช้เป็น **threshold** ใน `Player.CheckSanityStage()` — Sanity เริ่มที่ 100 แล้วลดลงผ่าน 85 → 40 → 0
> ⚠️ ค่าเหล่านี้เพิ่งถูกปรับ (เดิม 70/30/0) — เอกสารและค่าในโค้ดต้องอัปเดตพร้อมกันเสมอ

### `CharacterAction` — สถานะการกระทำ (`Scripts/CharacterAction.cs`)

`Repairing` · `Checking` · `Peeking` · `ReadyToHide` · `Hiding` · `Walking` · `Idle`

> `Sneaking` ถูกคอมเมนต์ออกแล้ว — การซุ่มเดิน (กด `Left Ctrl`) ไม่ได้เปลี่ยน state แต่ลดความเร็ว ÷1.7 และล็อกทิศการหันหน้า โดย `CurrentAction` ยังเป็น `Walking`
> `Peeking` เพิ่มมาพร้อมฟีเจอร์แอบมองประตู (`Player.TogglePeek`, ปุ่ม `E`)

### `Scene` — ฉากที่โหลดได้

| ค่า | ไฟล์ |
|---|---|
| `tilemap_electrical_room` | `Content/Tilemaps/tilemap_electrical_room.ldtk` |
| `tilemap_electrical_room_2` | `Content/Tilemaps/tilemap_electrical_room_2.ldtk` |
| `tilemap_sewer` | `Content/Tilemaps/tilemap_sewer.ldtk` |

### `SoundEffectName` — SFX ที่เล่นได้ (`Scripts/Enums/SoundEffectName.cs`)

26 ค่า: `DoorClose` · `DoorOpen` · `DoorStuck` · `LockerClose` · `LockerOpen` · `PipeComplete` · `PipeFail` · `PipeRepairing` · `PipeSuccess` · `ElecComplete` · `ElecFail` · `ElecRepairing` · `ElecSuccess` · `ScopoIndicator` · `PhobiaHit` · `HeartBeat` · `Painting` · `LightBreak` · `Darkness` · `ScreenOff` · `ScreenOn` · `FlashlightToggle` · `LightFlickerLong` · `LightFlickerShort1` · `LightFlickerShort2` · `LightFlickerShort3`

> ทุกค่าถูก map กับไฟล์จริงใน `AudioManager.SfxByName` — enum ใช้ชื่อ `PipeFail` / `ElecFail` แต่ไฟล์จริงชื่อ `sfx_pipe_failed.mp3` / `sfx_electric_failed.mp3` (map ถูกต้อง) · `Painting` map กับ `sfx_panting.mp3` (ชื่อไฟล์พิมพ์ผิด)
> เปลี่ยนชื่อจาก `Sound` → **`SoundEffectName`** แล้ว และค่าที่เคยมีคือ `AutoIndicator` ถูกถอดออก (ไม่มีไฟล์ `sfx_autophobia_indicator` ใน source แล้ว)

### `BackgroundSound` — ambience / BGM (`Scripts/Enums/BackgroundSound.cs`)

| ค่า | ไฟล์ | เล่นเมื่อ |
|---|---|---|
| `AmbElectric` | `amb_electricroom.mp3` | โหลดฉากที่ไม่ใช่ sewer |
| `AmbSewer` | `amb_sewer.mp3` | โหลดฉาก `tilemap_sewer` |
| `BgmMainMenu` | `bgm_mainmenu_screen.mp3` | `MainMenuScreen` |
| `BgmDeathScreen` | `bgm_death_screen.mp3` | `DeathScreen` |
| `BgmWinScreen` | `bgm_win_screen.mp3` | *(ผูกไว้แล้ว แต่ยังไม่มีหน้าจอไหนเรียก)* |

### `GameState` — *(ยังไม่ถูกใช้)*

`Menu` · `Gameplay` · `Lose` · `Win` · `Pause` — สถานะเกมตอนนี้สื่อผ่านหน้าจอที่ active ใน `ScreenManager` แทน (enum นี้ยังไม่มีที่อ้างอิง)

### `BlinkPhase` — เฟสการกระพริบตา

`Open` · `Closing` · `Closed` · `Opening` — ประกาศใน namespace `Imaginophobia.Scripts.Enums`

### `VignettePhase` — เฟสของ Nyctophobia

`StopNyctophobia` · `StartNyctophobia`

## 2. ข้อมูลระดับ (Level Data)

### LDtk Tilemaps

ไฟล์ `.ldtk` 3 ไฟล์ใน `Content/Tilemaps/` เป็นแหล่งข้อมูลหลักของฉาก — แต่ละไฟล์มี 1 level ชื่อ `Level_0` (default grid 16px, tile grid 120px)
ขนาด level: ห้องไฟฟ้าทั้งสอง **1920×1080** (1 จอ) · โถงอุโมง **6960×1080** (ยาว ~3.6 จอ)

`SceneBuilder` อ่านไฟล์เหล่านี้แล้วสร้าง:

- **TileMap** — ชั้นภาพและข้อมูล tile (ส่งให้ `TilemapSpriteBatchRenderer`)
- **Collider** — จาก object layer `Collision` (ส่งให้ `CollisionManager`)
- **Light** — จาก object layer `Lighting` (ส่งให้ `LightManager`)
- **PlayerSpawnPoints** — จุดเกิดผู้เล่น (`SpawnPoint { Name, Origin }`)
- **Triggers** — trigger ที่ใช้ทำ highlight + interact (`Trigger { Id, Name, Tag, Tiles }`)

### Object Layer `Collision`

`SceneBuilder` แยกประเภท entity ด้วย **`entity.Class`** ซึ่ง MonoGame.Extended map มาจาก **LDtk tag ตัวแรก** ของ entity (`__tags[0]`) ไม่ใช่ชื่อ entity

| `entity.Class` (tag) | กลายเป็น | ชื่อ entity ที่ใช้จริง | Properties |
|---|---|---|---|
| `wall` | `Wall` (layer `walls`) | `InvisWall` | — |
| `skillcheck` | `Trigger` (layer `triggers`) | `Valve` | — |
| `dots` | `Trigger` (layer `triggers`) | `ElectricalPanel` | — |
| `hideout` | `Trigger` (layer `triggers`) | `Locker` | — |
| `door` | `Trigger` (layer `triggers`) + ชื่อ = `Destination` | `LoadNextScene` | `Destination` |
| `report` | `Trigger` (layer `triggers`) + ชื่อ = `Detail` | `Clipboard` | `Detail` (`Story1` / `Story2`) |
| `spawn` | `SpawnPoint` + ชื่อ = `Origin` | `PlayerSpawnPoint` (ห้องไฟฟ้า) · `SpawnPoint` (โถงอุโมง) | `Origin` |

> `report` เป็นของใหม่ — คลิปบอร์ดที่วางตามห้องเพื่อเล่าเรื่อง (`Detail: Story1` / `Story2`) ยังมีแค่ trigger + highlight ยังไม่ผูก interaction

### Object Layer `Lighting`

`SceneBuilder` ตรวจ `entity.Class == "light"` แล้วแยกตาม **`entity.Name`** (ชื่อ entity ใน LDtk)

| `entity.Name` | กลายเป็น | Properties ที่อ่าน | ค่าที่ใช้อยู่ |
|---|---|---|---|
| `PointLight` | `PointLight` + `LightArea` (collider layer `lights`) | `Intensity`, `Scale`, `Color` | intensity 0.5 · scale 500 · `#9CEFFF` (sewer) |
| `DoorLight` | `PointLight` เฉย ๆ (ไม่มี collider) | `Intensity`, `Scale`, `Color` | intensity 0.1 · scale 500 · `#FFF2A5` |
| `SpotLight` | `Spotlight` (ไม่มี collider) | `Intensity`, `ScaleX`, `ScaleY`, `Rotation`, `Color` | intensity 0.5 · rotation 90° · 750×1250 (มี 1 ตัวที่เป็น 750×5000) · `#FFF2A5` |

### จำนวน entity จริงในแต่ละฉาก

| ฉาก | wall | skillcheck | dots | hideout | door | report | spawn | แสง |
|---|---|---|---|---|---|---|---|---|
| `tilemap_electrical_room` | 2 | — | 1 | 1 | 2 | 1 | 2 | `SpotLight` ×5 |
| `tilemap_electrical_room_2` | 2 | — | 1 | 1 | 1 | 1 | 1 | `SpotLight` ×5 |
| `tilemap_sewer` | 3 | **5** | — | 4 | 2 | — | 2 | `PointLight` ×12 · `DoorLight` ×2 |

> วาล์ว 5 จุดในโถงอุโมงตรงกับ `ObjectiveManager.TargetValves = 5` พอดี
> ⚠️ ห้องไฟฟ้าใช้ `SpotLight` (ไม่มี `LightArea`) ทั้งหมด — จึง**ไม่**ทำให้ผู้เล่นติด `Nyctophobia` และไม่ถูก Scopophobia ดับไฟ (ระบบไฟที่ถูกจับตาคือ `PointLight` ของโถงอุโมง + แสงของห้องที่ถูก `TurnOff` ตอน Scopophobia มา)

**Destination ของประตู**

| ฉาก | ประตู | Destination |
|---|---|---|
| `tilemap_electrical_room` | 2 | `tilemap_sewer` · `end` |
| `tilemap_electrical_room_2` | 1 | `tilemap_sewer` |
| `tilemap_sewer` | 2 | `tilemap_electrical_room` · `tilemap_electrical_room_2` |

**Origin ของจุดเกิด** (ใช้เลือกจุดวางตัวเมื่อเข้าฉาก)

| ฉาก | spawn | Origin |
|---|---|---|
| `tilemap_electrical_room` | 2 | `Start` · `tilemap_sewer` |
| `tilemap_electrical_room_2` | 1 | `tilemap_sewer` |
| `tilemap_sewer` | 2 | `tilemap_electrical_room` · `tilemap_electrical_room_2` |

### Tile Layers (ชั้นภาพ)

| ฉาก | ชั้นภาพ |
|---|---|
| `tilemap_electrical_room` | `Shadow` · `ElectricalPanel` · **`Ladder`** · `Lockers` · **`Clipboard`** · `Decoration2` · `Decoration1` · `SewerIndicator` · `Floor` · `Background` |
| `tilemap_electrical_room_2` | `Shadow` · `Decoration` · `Decoration2` · `SewerIndicator` · `Clipboard` · `Lockers` · `ElectricalPanel` · `Floor` · `Background` |
| `tilemap_sewer` | `ShadowBoundary` · `Decoration3` · `Decoration2` · `Lights` · `Doors` · `Lockers` · `Valves` · `Pipes` · `Decoration` · `Floor` · `Background` |

ชั้นที่ระบบ highlight ใช้: `Valves` · `Lockers` · `ElectricalPanel` · `Doors` · `Clipboard` · `Ladder` — `SceneBuilder.FindHighlightLayer` จับคู่ entity กับ layer ด้วยชื่อ (ตัด `s`/`S` ตัวท้ายออกทั้งสองฝั่ง) ยกเว้น entity `LoadNextScene` ที่ใช้ layer `Doors` และถ้า `Destination == "end"` จะใช้ layer `Ladder`

### Tilesets (`mapassets/`)

ชื่อ identifier ต่างกันในแต่ละไฟล์ `.ldtk` แต่ชี้ไปไฟล์ภาพเดียวกัน:

| ไฟล์ | ใช้ใน | grid | ขนาด |
|---|---|---|---|
| `spritesheet_background-texture.png` | ทั้ง 3 ฉาก | 120 | 1080×1080 |
| `spritesheet_er_floor.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `spritesheet_er_tiles.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `spritesheet_general_decoration.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `spritesheet_sewer_tiles.png` | ทั้ง 3 ฉาก | 120 | 1200×240 |
| `spritesheet_shadow.png` | ทั้ง 3 ฉาก | 120 | 600×120 |
| `Clip_board.png` | ห้องไฟฟ้าทั้งสอง | 120 | 120×120 |
| `sewer_indicator_red.png` | ห้องไฟฟ้าทั้งสอง | 120 | 480×360 |
| `Exit_sign.png` | โถงอุโมง | 120 | 360×120 |

> ไฟล์ tileset ไม่ต้องลงทะเบียนใน `Content.mgcb` แยก — `LDtkTilemapImporter` โหลดให้จาก path ใน `.ldtk` (ดังนั้น tileset ที่ชื่อใน `.ldtk` ไม่ตรงกับไฟล์ที่มีอยู่ จะพังตอน build/รัน ไม่ใช่ตอน compile)

## 3. Animation Data

`Content/Character/spritesheet_player.json` — TexturePacker JSON (dataformat `monogame-extended`) คู่กับ `spritesheet_player-texture.png` (960×360, เฟรมละ 120×120, pivot กลาง)

| กลุ่มเฟรม | จำนวน | ใช้ทำอะไร |
|---|---|---|
| `sprite_back` | 1 | ท่าหันหลัง (ตอนซ่อม) |
| `sprite_idle_01` … `sprite_idle_12` | 12 | ท่า idle |
| `sprite_walk_01` … `sprite_walk_09` | 9 | ท่าเดิน / ซุ่ม |

`Player.SetUpAnimation()` ประกอบเป็น animation 6 ตัว:

| Animation | เฟรม | Loop | ความยาว/เฟรม |
|---|---|---|---|
| `walk-forward` | `sprite_walk_01..09` | ✅ | 0.15s (เฟรมสุดท้าย 0s) |
| `sneak-forward` | `sprite_walk_01..09` | ✅ | 0.2s |
| `sneak-backward` | `sprite_walk_09..01` (ย้อนกลับ) | ✅ | 0.2s |
| `idle` | `sprite_idle_01..12` | ✅ | 0.4s |
| `back` | `sprite_back` | ❌ | — |
| `check` | `sprite_idle_01` | ❌ | — |

`EnemyManager` ประกอบ animation ของ Autophobia จาก atlas **ตัวเดียวกัน** (`Character/spritesheet_player`) แล้วย้อมสีดำ — `walk-forward` (9 เฟรม, 0.12s) และ `idle` (1 เฟรม, ไม่ loop)

> ไฟล์ `Character/New_version/` และ `Character/Old_version/` เป็น spritesheet เวอร์ชันเก่า/redesign ที่ยังไม่ถูกโหลดในโค้ด (ไม่ได้ลงทะเบียนใน `Content.mgcb`)

## 4. Tutorial Data

`Content/Datas/tutorial.json` — deserialize ผ่าน `TutorialDataReader : JsonContentTypeReader<TutorialData>` (ลงทะเบียนใน `Content.mgcb` เป็น `ContentType=Imaginophobia.TutorialDataReader, Imaginophobia`)

| ฟิลด์ | ชนิด | ความหมาย |
|---|---|---|
| `Steps` | `List<TutorialStep>` | ลำดับข้อความ tutorial |
| `TutorialStep.Text` | `string` | ข้อความที่แสดงบนจอ |
| `TutorialStep.CompleteOn` | `string` | objective id ที่ต้องสำเร็จก่อนจะเลื่อนไป step ถัดไป |

ลำดับ step ปัจจุบัน (10 ขั้น):

| # | Text (ย่อ) | CompleteOn |
|---|---|---|
| 1 | Press A/D to walk around. F to use flashlight | `move` |
| 2 | Head to the electrical panel to do a task | `repair` |
| 3 | Connect all the dots. (A/D, Q) | `dots-done` |
| 4 | Enter the locker to hide | `hide` |
| 5 | Leave the room on the left side, look for a valve | `door` |
| 6 | Find a valve to repair | `valve-open` |
| 7 | Click and spin the valve. Stay on the green gauge! | `valve-completed` |
| 8 | Repair all five valves | `valves-all` |
| 9 | Head to the ladder in the right electric room and exit. | `exit-ladder` |
| 10 | Thank you for playing. | `end` |

**Objective id ที่ใช้จริง** (เรียกผ่าน `ObjectiveManager.Complete(id)`)

| id | เรียกจาก |
|---|---|
| `move` | `UIManager` (กด A + D + F พร้อมกัน — ชั่วคราว) |
| `repair` | `CollisionManager` (เปิดมินิเกม Connect The Dots) |
| `dots-done` | `ConnectTheDotsScreen` (ซ่อมแผงไฟสำเร็จ) |
| `fixBlackout` | `ConnectTheDotsScreen` (ซ่อมแผงไฟสำเร็จครั้งที่ 2 เป็นต้นไป) |
| `hide` | `Player.ToggleHide` (ออกจากตู้) |
| `door` | `CollisionManager` (ใช้ประตู) |
| `valve-open` | `SpinTheValveScreen` constructor (เปิดมินิเกมวาล์ว) |
| `valve-completed` | `SpinTheValveScreen` (หมุนวาล์วสำเร็จ) |
| `valves-all` | `ObjectiveManager.RepairValve` (ครบ 5 วาล์ว) |
| `entered-sewer` | `GameManager.OnSceneLoaded` (เข้าฉาก sewer) |
| `exit-ladder` | `GameManager.OnDoorInteracted` (ประตู `Destination == "end"` และวาล์วครบ) |
| `collidedAutophobia` | `GameManager.OnEnemyRemoved` (Autophobia หายหลังเดินชน) |
| `end` | *(ยังไม่มีที่เรียก — step สุดท้ายของ tutorial จึงแสดงไม่ได้)* |

> `GameManager.OnEnemySpawned`/`OnEnemyRemoved` ใช้ `IsComplete("fixBlackout")` และ `IsComplete("collidedAutophobia")` เพื่อเลือกข้อความ hint ที่จะโชว์

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

| คลาส | ฟิลด์ | ความหมาย |
|---|---|---|
| `GameData` | `Dictionary<string, SceneData> Scenes` | key = ชื่อฉาก |
| `SceneData` | `List<TriggerData> Triggers`, `List<LightData> Lights` | สถานะของฉากนั้น |
| `TriggerData` | `Id` (int), `Progress` (float), `Interactable` (bool) | ความคืบหน้าของมินิเกม + ยังโต้ตอบได้ไหม |
| `LightData` | `Id` (int), `On` (bool) | ไฟเปิด/ปิด |

**รอบการทำงาน**

| เมธอด | เรียกเมื่อ |
|---|---|
| `SaveScene(sceneName, triggers, lightAreas)` | ก่อนออกจากฉาก (`SceneManager.LoadScene` → `UnloadScene`) — ข้าม trigger ที่เป็น `door` |
| `ApplyToScene(sceneName, triggers, lightAreas)` | หลังสร้างฉากใหม่ (`SceneBuilder` สร้างเสร็จ) |
| `TurnAllLightsOn()` | ซ่อมแผงไฟสำเร็จ (`ConnectTheDotsScreen`) |
| `ResetScene(sceneName)` | Nyctophobia หรือ Scopophobia เกิด (`GameManager.OnEnemySpawned` รีเซ็ตห้องไฟฟ้าทั้งสอง) |
| `Delete()` | `GameManager` constructor (เริ่มรอบใหม่) — และใน `Restart()` ที่ตอนนี้ไม่มีที่เรียก |

> ⚠️ **ยังไม่บันทึก:** ตำแหน่งผู้เล่น · Sanity · Battery · ฉากปัจจุบัน · objective ที่ทำสำเร็จ
> เกมเริ่มจาก `MainMenuScreen` แล้วสร้าง `GameManager` ใหม่เสมอ (โหลด `tilemap_electrical_room`) → save มีอายุแค่ภายในรอบเล่นเดียว ใช้จำ progress ของ task ระหว่างสลับฉากเท่านั้น

## 6. ค่าปรับแต่งเกม (Tuning Values)

> ⚠️ **ช่องว่างที่ใหญ่ที่สุดของโปรเจกต์นี้:** ปัจจุบัน **ยังไม่มีไฟล์ config/data กลาง** — ค่าปรับแต่งทั้งหมด hardcode อยู่ในโค้ด (มีแค่ `Datas/tutorial.json` ที่เป็น data จริง)
> Product backlog ระบุความต้องการไว้ว่า *"enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling"*

### Player

| ค่า | ค่าปัจจุบัน | ตำแหน่ง | สถานะ |
|---|---|---|---|
| Player speed | 250 | `Player.MoveSpeed` | ⚠️ hardcode |
| Player sneak | ÷ 1.7 (+ ล็อกทิศหันหน้า) | `Player.Update` | ⚠️ hardcode |
| Player size | 100 × 240 (scale 3) | `Player._size` | ⚠️ hardcode |
| Sanity เริ่มต้น / threshold | 100 · 85 / 40 / 0 | `Player.Sanity`, `SanityState` | ⚠️ hardcode |
| Battery เริ่มต้น / อัตราการลด | 100 · 3.5 ต่อวินาที | `Player.Battery`, `Player.Update` | ⚠️ hardcode |
| แสงรอบตัว (scotopic) | scale 500×500 · intensity 0.5 · `Color.FromHSV(150, 0.5, 0.5)` | `Player` constructor | ⚠️ hardcode |
| ไฟฉาย | scale 600×1250 · intensity 1 · offset (85, −12) | `Player` constructor | ⚠️ hardcode |
| ระยะสายตา (`EyeSight`) ตอนเปิดไฟฉาย | 400 px | `Player.Update` | ⚠️ hardcode |
| Sanity ลดในโถงอุโมง | 0.5 / 0.2 / 0 ต่อวินาที (Normal/Anxious/Insane) | `Player._availableSanityRedectionSpeed` | ⚠️ hardcode |
| Sanity ที่ลดเมื่อชน Autophobia | −10 + `MainGame.Saturation = 0` | `CollisionManager` | ⚠️ hardcode |
| Sanity ที่ลดเมื่อชน Scopophobia | −25 | `CollisionManager` | ⚠️ hardcode |
| Hide duration / cooldown | 10 วินาที · 3 วินาที | `Player.ToggleHide`, `Player._hideCoolDown` | ⚠️ hardcode |
| FocusLevel (ซูมกล้อง) | 1 ปกติ · 1.25 ซ่อน/ซ่อม · 1.75 แอบมอง | `Player` | ⚠️ hardcode |
| Saturation recovery | +0.3 ต่อวินาที (จาก 0 กลับเป็น 1 ใน ~3.3 วินาที) | `UIManager.Update` | ⚠️ hardcode |

### Enemies

| ค่า | ค่าปัจจุบัน | ตำแหน่ง | สถานะ |
|---|---|---|---|
| Autophobia speed | 400 | `Autophobia.MoveSpeed` | ⚠️ hardcode |
| Autophobia อายุ | 28 วินาที | `Autophobia` constructor | ⚠️ hardcode |
| Autophobia spawn interval | 4.96 วินาที (timer `AutoSpawnTimer`) | `EnemyManager` | ⚠️ hardcode |
| Scopophobia speed | 2600 | `Scopophobia.MoveSpeed` | ⚠️ hardcode |
| Scopophobia เวลาก่อนพุ่ง | 8 วินาที | `Scopophobia` constructor | ⚠️ hardcode |
| Scopophobia อายุ | 15 วินาที | `Scopophobia` constructor | ⚠️ hardcode |
| Scopophobia spawn interval | 4.98 วินาที (timer `ScopoSpawnTimer`) | `EnemyManager` | ⚠️ hardcode |
| Scopophobia ถือว่า active เมื่อ | `LightBrokeCount < 12` | `EnemyManager.HasActiveScopophobia` | ⚠️ hardcode |
| Nyctophobia spawn interval | 30 วินาที (timer `NyctoSpawnTimer`) | `EnemyManager` | ⚠️ hardcode |
| Entamaphobia spawn interval | 5 วินาที (**ยังไม่ถูกเรียก**) | `EnemyManager.SpawnEntamaphobia` | ⚠️ hardcode |
| Enemy spawn chance (Normal / Anxious / Insane) | Autophobia `20 / 40 / 55` · Scopophobia `5 / 12 / 20` · Nyctophobia `35 / 50 / 65` · Entamaphobia `10 / 50 / 65` | `EnemyManager` (แก้ผ่าน `OnStageChanged`) | ⚠️ hardcode |
| Enemy spawn distance จาก world bound | 100–500 px (Scopophobia เพิ่ม ±1920 อีกชั้น) | `EnemyManager.Spawn*` | ⚠️ hardcode |
| Spawn timer เปิด/ปิดตามฉาก | ทำงานเฉพาะฉาก `tilemap_sewer` (ฉากอื่น pause ทั้งหมด) | `EnemyManager.Update` | ⚠️ hardcode |

### Systems

| ค่า | ค่าปัจจุบัน | ตำแหน่ง | สถานะ |
|---|---|---|---|
| ตัวจับเวลาหลัก | 481 วินาที → เปิด `WinScreen` | `GameManager` constructor (`"MainTimer"`) | ⚠️ hardcode + ค่าสงสัย (ดู System Design § 14) |
| จำนวนวาล์วเป้าหมาย | 5 | `ObjectiveManager.TargetValves` | ⚠️ hardcode |
| Spin The Valve: progress เป้าหมาย | 200 | `SpinTheValveScreen` | ⚠️ hardcode |
| Spin The Valve: โซนสำเร็จ / โทษ | 255°–285° (+20/วินาที) · 285°–313° (−35/วินาที) · 225°–255° (−15/วินาที) | `SpinTheValveScreen` | ⚠️ hardcode |
| Spin The Valve: ความเร็วเข็ม | สุ่ม ±65/10 องศา/วินาที (ไม่เอา ±20) ทุก 5 วินาที · clamp 225°–313° | `SpinTheValveScreen` | ⚠️ hardcode |
| Connect The Dots: ขนาดกระดาน | 6 × 6 (ช่อง 100 px) | `ConnectTheDotsScreen` | ⚠️ hardcode |
| Connect The Dots: จำนวนรอบซ่อม | 5 (`_maxRepairStage`, รอบละ +40 progress) | `ConnectTheDotsScreen` | ⚠️ hardcode |
| Connect The Dots: จำนวนสีสูงสุด | 5 (`_maxDots`) จาก palette 6 สี | `DotsGenerator` | ⚠️ hardcode |
| Connect The Dots: เกณฑ์ level | path ยาว ≥ 6 ช่อง · ปลายทางห่าง ≥ 4 (ระยะแมนฮัตตัน) · ลองสูงสุด 500 ครั้ง | `DotsGenerator` | ⚠️ hardcode |
| ระยะ spatial audio | default inner 10 / outer 80 · ฝีเท้าผู้เล่น 10/150 · ฝีเท้า Autophobia 10/1200 · ไฟกระพริบ 50/1800 · ไฟแตก 50/1800 · Scopophobia indicator 600/15000 | `AudioManager` / `LightArea` | ⚠️ hardcode |
| ระดับเสียง default | `BgsVolume = 0.2` · flicker SFX 0.15 · light break 0.4 · `LightFlickerLong` ตอน Nyctophobia เกิด 0.02 · เสียง `Painting` 0.025 / 0.065 ตามระดับสติ | `AudioManager` / `GameManager` / `Player` | ⚠️ hardcode |
| LightArea ขนาดพื้นที่ | 600 × 600 (5 tiles) | `LightArea._areaSize` | ⚠️ hardcode |
| LightArea เวลากระพริบ | 1.5 วินาที (Autophobia) · 3 วินาทีแล้วดับถาวร (Nyctophobia alarm) | `LightArea` | ⚠️ hardcode |
| ระยะเวลา black screen | 1 วินาที | `UIManager.BlackScreen` | ⚠️ hardcode |
| Penumbra ambient color | `#242323` | `LightManager` | ⚠️ hardcode |
| Vignette scale | ปกติ 0.9×0.5 · Anxious 0.55×0.4 · Insane 0.45×0.35 · ซ่อนตัว 0.375×0.175 · Nyctophobia 0.9×0.5 → 0 | `UIManager` | ⚠️ hardcode |
| ข้อความ Death screen tips | 9 ข้อความ | `DeathScreen._tips` | ⚠️ hardcode |

**ข้อเสนอ:** ย้ายค่าทั้งหมดไปไว้ใน `Content/Datas/*.json` แล้วโหลดตอนเริ่มเกม — จะทำให้บาลานซ์เกมได้โดยไม่ต้อง build ใหม่ (ตรงกับความต้องการใน backlog และเป็นรูปแบบเดียวกับ `Datas/tutorial.json` ที่ทำไว้แล้ว)

## Related Documents

- System Design: [System Design](01-system-design.md)
- Class Diagram: [Class Diagram](02-class-diagram.md)
- Asset List: [Asset List](../01_GDD/05-asset-list.md)
- Asset Pipeline: [Asset Pipeline Flow](../01_GDD/06-asset-pipeline-flow.md)
- Mechanics: [Mechanics](../01_GDD/03-mechanics.md)
