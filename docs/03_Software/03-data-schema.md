---
type: software-data-schema
version: 2.0
date: 2026-10-24
---

# Imaginophobia — Data Schema

**Version:** 2.0 | **Last Updated:** 2026-10-24

> เอกสารนี้อธิบายโครงสร้างข้อมูลที่เกมใช้ ณ ปัจจุบัน (ตรวจจากโค้ดและไฟล์ใน `Content/` จริง)
> อ้างอิง: [System Design](01-system-design.md) · [Class Diagram](02-class-diagram.md)

## 1. Enums (ค่าคงที่ในเกม)

ค่าคงที่ทั้งหมดประกาศเป็น `enum` ใน `Scripts/Enums/` (ยกเว้น `CharacterAction` ที่อยู่ที่ `Scripts/CharacterAction.cs`)

### `SanityState` — ระดับสติ (`Scripts/Enums/SanityState.cs`)

| ชื่อ | ค่า | ความหมาย |
|---|---|---|
| `Normal` | 70 | ปกติ |
| `Anxious` | 30 | เริ่มกังวล |
| `Insane` | 0 | สติแตกระดับวิกฤต |

> ค่า enum ถูกใช้เป็น **threshold** ใน `Player.CheckSanityStage()` — Sanity เริ่มที่ 100 แล้วลดลงผ่าน 70 → 30 → 0

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

> ค่าทั้ง 15 ตัวถูก map กับไฟล์ใน `AudioManager.NameSFX` — enum ใช้ชื่อ `PipeFail` / `ElecFail` แต่ไฟล์จริงชื่อ `sfx_pipe_failed.mp3` / `sfx_electric_failed.mp3` (map ถูกต้องแล้ว)

### `BlinkPhase` — เฟสการกระพริบตา

`Open` · `Closing` · `Closed` · `Opening` — ประกาศใน namespace `Imaginophobia.Scripts.Enums`

### `VignettePhase` — เฟสของ Nyctophobia

`StopNyctophobia` · `StartNyctophobia`

## 2. ข้อมูลระดับ (Level Data)

### LDtk Tilemaps

ไฟล์ `.ldtk` 3 ไฟล์ใน `Content/Tilemaps/` เป็นแหล่งข้อมูลหลักของฉาก — แต่ละไฟล์มี 1 level ชื่อ `Level_0` (world layout `Free`, default grid 16px, tile grid 120px)

`SceneBuilder` อ่านไฟล์เหล่านี้แล้วสร้าง:

- **TileMap** — ชั้นภาพและข้อมูล tile (ส่งให้ `TilemapSpriteBatchRenderer`)
- **Collider** — จาก object layer `Collision` (ส่งให้ `CollisionManager`)
- **Light** — จาก object layer `Lighting` (ส่งให้ `LightManager`)
- **PlayerSpawnPoints** — จุดเกิดผู้เล่น (`SpawnPoint { Name, Origin }`)

### Object Layer `Collision`

`SceneBuilder` แยกประเภท entity ด้วย **`entity.Class`** ซึ่ง MonoGame.Extended map มาจาก **LDtk tag ตัวแรก** ของ entity (`__tags[0]`) ไม่ใช่ชื่อ entity

| `entity.Class` (tag) | กลายเป็น | ชื่อ entity ที่ใช้จริง |
|---|---|---|
| `wall` | `Wall` (layer `walls`) | `InvisWall` |
| `skillcheck` | `Trigger` (layer `triggers`) | `Valve` |
| `dots` | `Trigger` (layer `triggers`) | `ElectricalPanel` |
| `hideout` | `Trigger` (layer `triggers`) | `Locker` |
| `door` | `Trigger` (layer `triggers`) + ชื่อ = property `Destination` | `LoadNextScene` |
| `spawn` | `SpawnPoint` + ชื่อ = property `Origin` | `PlayerSpawnPoint` (ห้องไฟฟ้า) · `SpawnPoint` (โถงอุโมง) |

### Object Layer `Lighting`

`SceneBuilder` ตรวจ `entity.Class == "light"` แล้วแยกตาม **`entity.Name`** (ชื่อ entity ใน LDtk)

| `entity.Name` | กลายเป็น | Properties ที่อ่าน |
|---|---|---|
| `PointLight` | `PointLight` + `LightArea` (collider layer `lights`) | `Intensity`, `Scale`, `Color` |
| `DoorLight` | `PointLight` เฉย ๆ (ไม่มี collider) | `Intensity`, `Scale`, `Color` |
| `SpotLight` | `Spotlight` | `Intensity`, `ScaleX`, `ScaleY`, `Rotation`, `Color` |

### จำนวน entity จริงในแต่ละฉาก

| ฉาก | wall | skillcheck | dots | hideout | door | spawn | light |
|---|---|---|---|---|---|---|---|
| `tilemap_electrical_room` | 2 | — | 1 | 1 | 2 | 2 | 2 |
| `tilemap_electrical_room_2` | 2 | — | 1 | 1 | 1 | 1 | 2 |
| `tilemap_sewer` | 3 | **5** | — | 4 | 2 | 2 | 14 |

> วาล์ว 5 จุดในโถงอุโมงตรงกับ `ObjectiveManager.TargetValves = 5` พอดี

### Tile Layers (ชั้นภาพ)

| ฉาก | ชั้นภาพ |
|---|---|
| `tilemap_electrical_room` | `Background` · `Floor` · `Lockers` · `ElectricalPanel` · `Decoration` · `Decoration2` |
| `tilemap_electrical_room_2` | `Background` · `Floor` · `ElectricalPanel` · `Lockers` · `Decoration` · `Decoration2` |
| `tilemap_sewer` | `ShadowBoundary` · `Background` · `Floor` · `Pipes` · `Valves` · `Lockers` · `Doors` · `Lights` · `Decoration2` · `Decoration3` |

ชั้นที่ระบบ highlight ใช้: `Valves` · `Lockers` · `ElectricalPanel` · `Doors` — `SceneBuilder` จับคู่ entity กับ layer ด้วยชื่อ (ตัด `s` ตัวท้ายออก) ยกเว้น entity `LoadNextScene` ที่ใช้ layer `Doors` ตรง ๆ

### Tilesets (`mapassets/`)

| ไฟล์ | ใช้ใน | grid | ขนาด |
|---|---|---|---|
| `spritesheet_background-texture.png` | ทั้ง 3 ฉาก | 120 | 1080×1080 |
| `asset-1-export.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `asset-2 (1)-export.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `asset-3-Recovered (1)-export.png` | ทั้ง 3 ฉาก | 120 | 1920×1080 |
| `black_opa-export.png` | ทั้ง 3 ฉาก | 120 | 600×120 |
| `Sprite-bgx400%_3.png` | ห้องไฟฟ้า 2 + โถงอุโมง | 120 | 1200×240 |
| `Cha1-export.png` | ห้องไฟฟ้า 2 | 16 | 240×240 |

> ไฟล์ tileset ไม่ต้องลงทะเบียนใน `Content.mgcb` แยก — `LDtkTilemapImporter` โหลดให้จาก path ใน `.ldtk`

## 3. Animation Data

`Content/Character/spritesheet_player.json` — TexturePacker JSON (dataformat `monogame-extended`) คู่กับ `spritesheet_player-texture.png` (960×360, เฟรมละ 120×120, pivot กลาง)

เฟรมที่มีในไฟล์:

| กลุ่มเฟรม | จำนวน | ใช้ทำอะไร |
|---|---|---|
| `sprite_back` | 1 | ท่าหันหลัง (ตอนซ่อม) |
| `sprite_idle_01` … `sprite_idle_12` | 12 | ท่า idle |
| `sprite_walk_01` … `sprite_walk_09` | 9 | ท่าเดิน / ซุ่ม |

`Player.SetUpAnimation()` ประกอบเป็น animation 5 ตัว:

| Animation | เฟรม | Loop | ความยาว/เฟรม |
|---|---|---|---|
| `walk-forward` | `sprite_walk_01..09` | ✅ | 0.15s (เฟรมสุดท้าย 0s) |
| `sneak-forward` | `sprite_walk_01..09` | ✅ | 0.2s |
| `sneak-backward` | `sprite_walk_09..01` (ย้อนกลับ) | ✅ | 0.2s |
| `idle` | `sprite_idle_01..12` | ✅ | 0.4s |
| `back` | `sprite_back` | ❌ | — |
| `check` | `sprite_idle_01` | ❌ | — |

`EnemyManager` ประกอบ animation ของ Autophobia จาก atlas **ตัวเดียวกัน** (`Character/spritesheet_player`) แล้วย้อมสีดำ — `walk-forward` (9 เฟรม) และ `idle`

> ไฟล์ `Character/New_version/` และ `Character/Old_version/` เป็น spritesheet เวอร์ชันเก่า/redesign ที่ยังไม่ถูกโหลดในโค้ด (ไม่ได้ลงทะเบียนใน `Content.mgcb`)

## 4. Tutorial Data

`Content/Datas/tutorial.json` — deserialize ผ่าน `TutorialDataReader : JsonContentTypeReader<TutorialData>` (ลงทะเบียนใน `Content.mgcb` เป็น `ContentType=Imaginophobia.TutorialDataReader, Imaginophobia`)

```json
{
  "Steps": [
    { "Text": "Press A/D to walk around. F to use flashlight", "CompleteOn": "move" },
    { "Text": "Head to the electrical panel to do a task",       "CompleteOn": "repair" }
  ]
}
```

| ฟิลด์ | ชนิด | ความหมาย |
|---|---|---|
| `Steps` | `List<TutorialStep>` | ลำดับข้อความ tutorial |
| `TutorialStep.Text` | `string` | ข้อความที่แสดงบนจอ |
| `TutorialStep.CompleteOn` | `string` | objective id ที่ต้องสำเร็จก่อนจะเลื่อนไป step ถัดไป |

**Objective id ที่ใช้จริง** (เรียกผ่าน `ObjectiveManager.Complete(id)`):

| id | เรียกจาก |
|---|---|
| `move` | `UIManager` (กด A + D + F พร้อมกัน — ชั่วคราว) |
| `repair` | `CollisionManager` (เปิดมินิเกม Connect The Dots) |
| `dots-done` | `ConnectTheDotsScreen` (ซ่อมแผงไฟสำเร็จ) |
| `hide` | `Player.ToggleHide` |
| `door` | `CollisionManager` (ใช้ประตู) |
| `valve-open` | `SpinTheValveScreen` (เปิดมินิเกมวาล์ว) |
| `valve-completed` | `SpinTheValveScreen` (หมุนวาล์วสำเร็จ) |
| `valves-all` | `ObjectiveManager.RepairValve` (ครบ 5 วาล์ว) |
| `entered-sewer` | `GameManager.OnSceneLoaded` |
| `exit-ladder` | `GameManager.OnDoorInteracted` (ประตู `Destination == "end"` และวาล์วครบ) |
| `end` | *(ยังไม่มีที่เรียก — ต้องเพิ่มตอนจบเกม)* |

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
| `SaveScene(sceneName, triggers, lightAreas)` | ก่อนออกจากฉาก (`SceneManager.LoadScene` → `UnloadScene`) |
| `ApplyToScene(sceneName, triggers, lightAreas)` | หลังสร้างฉากใหม่ (`SceneBuilder` สร้างเสร็จ) |
| `TurnAllLightsOn()` | ซ่อมแผงไฟสำเร็จ (`ConnectTheDotsScreen`) |
| `ResetScene(sceneName)` | Scopophobia เกิด (`GameManager.OnEnemySpawned`) |
| `Delete()` | เริ่มเกม / ตาย (`GameManager` constructor + `Restart`) |

> ⚠️ **ยังไม่บันทึก:** ตำแหน่งผู้เล่น · Sanity · Battery · ฉากปัจจุบัน · ข้อความตาย/ชนะที่เก็บได้
> และ `GameManager` เรียก `SaveManager.Delete()` ทุกครั้งที่เริ่มเกม/ตาย → save จึงมีอายุแค่ภายในรอบเล่นเดียว (ใช้จำ progress ของ task ระหว่างสลับฉากเท่านั้น)

## 6. ค่าปรับแต่งเกม (Tuning Values)

> ⚠️ **ช่องว่างที่ใหญ่ที่สุดของโปรเจกต์นี้:** ปัจจุบัน **ยังไม่มีไฟล์ config/data กลาง** — ค่าปรับแต่งทั้งหมด hardcode อยู่ในโค้ด (มีแค่ `Datas/tutorial.json` ที่เป็น data จริง)
> Product backlog ระบุความต้องการไว้ว่า *"enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling"*

| ค่า | ค่าปัจจุบัน | ตำแหน่ง | สถานะ |
|---|---|---|---|
| Player speed | 250 | `Player.MoveSpeed` | ⚠️ hardcode |
| Player sneak | ÷ 1.7 | `Player.Update` | ⚠️ hardcode |
| Player size | 100 × 240 | `Player._size` | ⚠️ hardcode |
| Sanity เริ่มต้น | 100 | `Player.Sanity` | ⚠️ hardcode |
| Battery เริ่มต้น | 100 | `Player.Battery` | ⚠️ hardcode |
| Battery อัตราการลด | 3.5 / วินาที | `Player.Update` | ⚠️ hardcode |
| Sanity ลดในโถงอุโมง | 0.5 / 0.2 / 0 (Normal/Anxious/Insane) | `Player._availableSanityRedectionSpeed` | ⚠️ hardcode |
| Sanity ที่ลดเมื่อชน Autophobia | −10 | `CollisionManager` | ⚠️ hardcode |
| Sanity ที่ลดเมื่อชน Scopophobia | −25 | `CollisionManager` | ⚠️ hardcode |
| Autophobia speed | 400 | `Autophobia.MoveSpeed` | ⚠️ hardcode |
| Autophobia อายุ | 28 วินาที | `Autophobia` constructor | ⚠️ hardcode |
| Scopophobia speed | 2600 | `Scopophobia.MoveSpeed` | ⚠️ hardcode |
| Scopophobia เวลาก่อนพุ่ง | 8 วินาที | `Scopophobia` constructor | ⚠️ hardcode |
| Scopophobia อายุ | 15 วินาที | `Scopophobia` constructor | ⚠️ hardcode |
| Enemy spawn interval | 5 วินาที | `EnemyManager.SpawnInterval` | ⚠️ hardcode |
| Enemy spawn chance | Autophobia 20/35/55 · Scopophobia 5/12/17 (Normal/Anxious/Insane) | `EnemyManager` | ⚠️ hardcode |
| Enemy spawn distance จาก world bound | 100–500 px | `EnemyManager.SpawnEnemy` | ⚠️ hardcode |
| Hide duration | 8 วินาที | `Player.ToggleHide` | ⚠️ hardcode |
| Hide cooldown | 3 วินาที | `Player._hideCoolDown` | ⚠️ hardcode |
| จำนวนวาล์วเป้าหมาย | 5 | `ObjectiveManager.TargetValves` | ⚠️ hardcode |
| ตัวจับเวลาหลัก | 600 วินาที | `GameManager` constructor | ⚠️ hardcode |
| Spin The Valve: progress เป้าหมาย | 200 | `SpinTheValveScreen` | ⚠️ hardcode |
| Spin The Valve: โซนสำเร็จ | 255°–285° (โซนอื่นลด progress) | `SpinTheValveScreen` | ⚠️ hardcode |
| Connect The Dots: ขนาดกระดาน | 6 × 6 | `ConnectTheDotsScreen` | ⚠️ hardcode |
| Connect The Dots: จำนวนรอบซ่อม | 5 (`_maxRepairStage`) | `ConnectTheDotsScreen` | ⚠️ hardcode |
| Connect The Dots: จำนวนสีสูงสุด | 5 | `DotsGenerator._maxDots` | ⚠️ hardcode |
| Connect The Dots: ความยาวเส้นขั้นต่ำ | 6 ช่อง · ระยะปลายทางขั้นต่ำ 4 | `DotsGenerator` | ⚠️ hardcode |
| ระยะ spatial audio | inner 10 / outer 80 (มี default param) | `AudioManager.Play3DSFX` | ⚠️ hardcode |
| LightArea ขนาดพื้นที่ | 600 × 600 (5 tiles) | `LightArea._areaSize` | ⚠️ hardcode |
| LightArea เวลากระพริบ | 1.5 วินาที | `LightArea.StartFlicker` | ⚠️ hardcode |
| กล้อง: FocusLevel ตอนซ่อน/ซ่อม | 1.25 | `Player` | ⚠️ hardcode |
| Penumbra ambient color | `#242323` | `LightManager` | ⚠️ hardcode |

**ข้อเสนอ:** ย้ายค่าทั้งหมดไปไว้ใน `Content/Datas/*.json` แล้วโหลดตอนเริ่มเกม — จะทำให้บาลานซ์เกมได้โดยไม่ต้อง build ใหม่ (ตรงกับความต้องการใน backlog และเป็นรูปแบบเดียวกับ `Datas/tutorial.json` ที่ทำไว้แล้ว)

## Related Documents

- System Design: [System Design](01-system-design.md)
- Class Diagram: [Class Diagram](02-class-diagram.md)
- Asset List: [Asset List](../01_GDD/05-asset-list.md)
- Asset Pipeline: [Asset Pipeline Flow](../01_GDD/06-asset-pipeline-flow.md)
- Mechanics: [Mechanics](../01_GDD/03-mechanics.md)
