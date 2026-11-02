---
type: software-system-design
version: 4.0
date: 2026-11-02
---
# Imaginophobia — System Design

**Version:** 4.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

> เอกสารนี้อธิบายสถาปัตยกรรมจริงของโค้ดใน `src/Imaginophobia/Imaginophobia/`
> ตรวจจากโค้ด ณ commit `3a300ec` (2026-11-02)
> อ้างอิง: [Class Diagram](02-class-diagram.md) · [Data Schema](03-data-schema.md)

## 1. ภาพรวม

เกมรันบน **MonoGame (DesktopGL)** ด้วย **.NET 9** โครงสร้างเป็นแบบ **manager-based**: `GameManager` เป็นเจ้าของ manager ของ gameplay ส่วน `MainGame` เป็นเจ้าของระบบระดับเกม (Graphics, Content, Screen, Gum, Audio)

`MainGame` เปิดเกมที่ **`MainMenuScreen`** — ผู้เล่นกดปุ่ม **Shift 1** บนเมนูจึงจะสร้าง `GameplayScreen` (ซึ่งห่อ `GameManager`) ขึ้นมา

```mermaid
flowchart TD
    Program --> MainGame
    MainGame --> AudioManager
    MainGame --> ModifiedScreenManager
    ModifiedScreenManager --> MainMenuScreen
    MainMenuScreen -->|Shift 1| GameplayScreen
    MainMenuScreen --> SettingsScreen
    MainMenuScreen --> CreditsScreen
    GameplayScreen --> GameManager
    GameplayScreen -->|ESC / ปุ่มมุมขวาบน| PauseOverlay[pause overlay ในตัว GameplayScreen]
    GameManager -->|Sanity หมด / หมดเวลา 721 วิ| DeathScreen
    GameManager -->|ประตู end + วาล์วครบ + ไฟ sewer ติดครบ| WinScreen
    GameManager -->|ประตู + Entamaphobia active| JumpScareScreen
    DeathScreen --> MainMenuScreen
    WinScreen --> MainMenuScreen
```

`MainGame` มี static properties กลางให้คลาสอื่นเรียกใช้ได้ เช่น: `Instance`, `Graphics`, `GraphicsDevice`, `SpriteBatch`, `Content`, `ScreenManager`, `GameState`, `Loading`

**หมายเหตุสำคัญ:** `MainGame` บังคับให้มี instance เดียว — สร้างตัวที่สองจะ throw `InvalidOperationException` และ `AudioManager` จะถูก new ขึ้นมาใน `MainGame.Initialize()` ไม่ใช่ใน `GameManager`

หน้าต่างเกมเปิดมาเป็น **fullscreen** (`Graphics.IsFullScreen = true`, 1920×1080 จาก `Layout`)

### GameState (สถานะเกมระดับ static)

`enum GameState` (`Scripts/Enums/GameState.cs`) คือค่าที่ทุกคลาสอ่านเพื่อรู้ว่าตอนนี้เกมอยู่สถานะไหน: `Menu` · `Gameplay` · `Lose` · `Win` · `Pause` · `Jumpscare`

| ค่า | ตั้งโดย | ผล |
| --- | --- | --- |
| `Menu` | `MainMenuScreen.Initialize` / `ReturnToMenu` ของจอจบเกม | — |
| `Gameplay` | `GameplayScreen.Initialize`, `JumpScareScreen` timer, `TogglePause` ตอนปิดเมนู | `GameManager.Update` ทำงานเต็ม |
| `Pause` | `GameplayScreen.TogglePause` | `GameManager.Update` `return` ทันที + `Time.TimeScale = 0` |
| `Lose` | `DeathScreen.Initialize` และ `GameManager.Update` | — |
| `Win` | `WinScreen.Initialize` | — |
| `Jumpscare` | `JumpScareScreen` constructor | `Player.Update` หยุด, `CollisionManager` ไม่รับ interaction |

### Post-process: Saturation (ทั้งเฟรม)

`MainGame.Draw` ไม่ได้วาดลง backbuffer ตรง ๆ แต่ใช้ 2 จังหวะ:

1. วาดทุกอย่าง (`ScreenManager.Draw` + `Gum.Draw`) ลง `_frameRT` (render target ขนาดเท่า viewport — สร้างใหม่/ทิ้งเมื่อ viewport เปลี่ยนขนาด)
2. `DrawGraded()` วาด `_frameRT` ลงจอผ่าน effect `Shaders/Saturation` (เทคนิค `ApplySaturation`) โดยใช้ค่า `ScreenManager.Saturation`
3. `DrawFade()` วาดสี่เหลี่ยมดำทับด้วยค่า alpha จาก `_fadeValue` (ใช้ตอนสลับฉาก)

`Saturation` เป็น **property ของ `ModifiedScreenManager`** — `1.0` = สีปกติ, `0.0` = ขาวดำ `CollisionManager` ตั้งเป็น `0` ทันทีเมื่อผู้เล่นชน Autophobia และ `UIManager` ค่อย ๆ ไล่ค่ากลับขึ้น (+0.3/วินาที)

### Gum (UI toolkit)

เมนู, จอจบเกม, pause overlay และ HUD ฝั่งข้อความสร้างด้วย **Gum** (`Gum.MonoGame` + `KernSmith.MonoGameGum`):

- `GumService.Default` ถูก initialize ใน `MainGame.Initialize()` และอัปเดตทุกเฟรม (`Gum.Update` / `Gum.Draw`)
- ฟอนต์ถูกสร้าง runtime ผ่าน `KernSmithFontCreator` แล้วลงทะเบียนชื่อ **`generationfonting`** จาก `Fonts/TrueTypeFont/GenerationFonting.ttf`
- ปุ่ม/สไลเดอร์/ข้อความทั้งหมดประกอบด้วยโค้ดใน `GumUIHelper` (ตัวช่วยกลาง ไม่ให้แต่ละหน้าจอเขียน style ซ้ำ)
- ทุกหน้าจอ Gum สร้าง `Panel` → `Dock(Dock.Fill)` → `AddToRoot()` และ `RemoveFromRoot()` ตอน `Dispose`
- `CreditsScreen` ถือ `ScrollViewer` และมี **dummy `ScrollViewer` ขนาด 1×1** กำกับไว้ในโค้ด เพราะถ้าหน้าจอที่มี ScrollViewer ตัวเดียวถูก dispose Gum จะวาดหน้าจอถัดไปเป็นสีดำทั้งหมด

## 2. Manager และความรับผิดชอบ

| Manager                   | หน้าที่                                                                                                                                                                                       | หมายเหตุ                                                                                                                                                          |
| ------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `MainGame`              | ระบบระดับเกม: graphics, content, screen, Gum, saturation pass, ชีวิตของ `AudioManager`, สถานะ `GameState`                                                                                         | เริ่มที่`MainMenuScreen` · fullscreen 1920×1080                                                                                                                                        |
| `ModifiedScreenManager` | สืบทอด `ScreenManager` ของ MonoGame.Extended — เพิ่ม render target กลาง, fade (in/out/pulse), `ShowScreen`/`CloseScreen`/`ClearScreens`/`ReplaceScreen` แบบมี fade, `Saturation`, `MainGame.Loading` | ตัวคุมการเปลี่ยนฉาก/หน้าจอทั้งหมด                                                                                                                                                     |
| `GameManager`           | ศูนย์กลางของ gameplay — ถือ manager ย่อย, คุม state, โหลดฉาก, จัดการเหตุการณ์ (ประตู / enemy เกิด-ตาย)                                      | จุดเริ่มอ่านโค้ด · สร้างใน `GameplayScreen.Initialize`                                                                                         |
| `SceneManager`          | ดูแลฉากที่ active (`SceneBuilder`), สั่ง `Update`/`Draw`/`Render`, สลับฉาก, sync highlight renderer                                                                     | `static GetActiveScene()` · event `SceneLoaded`                                                                                                                      |
| `SceneBuilder`          | สร้างฉากจากไฟล์`.ldtk` — collider + แสง + highlight tile + `PlayerSpawnPoints`                                                                                                | ตัวเชื่อมระหว่าง LDtk กับเกม                                                                                                                        |
| `CollisionManager`      | ลงทะเบียน collider แบบมี layer, ตรวจการชน, จัดการ interaction (ตู้/มินิเกม/คลิปบอร์ด/ประตู/แอบมองประตู)                                                 | `AddCollider(actor, layer)` · event `DoorInteracted` · `FocusedTrigger`                                                                                                                 |
| `LightManager`          | ถือ light ทั้งหมด (Penumbra) และ `LightArea`, อัปเดตตาม transform ของกล้อง                                                                                           | `static Penumbra` · `AllLightsAreOff()` · `ScopoAlarmAll()` · `NyctoAlarmAll()` · `CleanUpScopo()`                                                                                   |
| `AudioManager`          | SFX, ambience, BGM, spatial audio —**singleton** (`AudioManager.Instance`)                                                                                                                  | สร้างใน `MainGame.Initialize()` · สร้างซ้ำ = throw                                                                                                      |
| `EnemyManager`          | สุ่ม spawn และอัปเดต enemy ทุกตัว · เปิด/ปิด spawn timer ตามฉาก · เช็คตัวที่ยัง active                                                                                                     | `SpawnScopophobia` / `SpawnAutophobia` / `SpawnNyctophobia` / `SpawnEntamaphobia` / `ForceSpawnScopophobia` · events `EnemySpawned` / `EnemyRemoved` |
| `UIManager`             | HUD (แถบ Sanity / Battery / ตัวนับวาล์ว / ตัวจับเวลา), vignette 3 ตัว, blink, ข้อความ tutorial + ข้อความชั่วคราว, black screen, saturation recovery | ถือ `TutorialManager` + `TextManager`                                                                                                                              |
| `TextManager`           | วาดข้อความในเกม (tutorial + ข้อความชั่วคราว), ป้ายกำกับ HUD และตัวเลข `MainTimer`                                                                  | อ่าน timer ตัว `MainTimer` ตรงจาก `Time.Timers`                                                                                                                  |
| `ObjectiveManager`      | **static** — ติดตาม objective แบบ id และความคืบหน้าของวาล์ว/แผงไฟ                                                                                         | `TargetValves = 5` · event `Completed` · `RepairValve` / `RepairPanel`                                                                                               |
| `TutorialManager`       | อ่าน `Content/Datas/tutorial.json` แล้วเลื่อน step ตาม objective ที่สำเร็จ                                                                                                            | ผูกกับ `ObjectiveManager.Completed`                                                                                                                               |
| `SaveManager`           | **static** — save/load สถานะ trigger + ไฟ ต่อฉาก ลง `%APPDATA%/Imaginophobia/save.json`                                                                                      | `SaveScene` / `ApplyToScene` / `AllSewerLightsAreOn` / `TurnAllLightsOn` / `ResetScene` / `Delete`                                                                                      |
| `TileHighlightRenderer` | วาด highlight ของวัตถุที่โต้ตอบได้ (วาล์ว / ตู้ / แผงไฟ / ประตู / คลิปบอร์ด / บันได) ด้วย shader 2 ตัว                                 | `Silhouette` + `HighlightComposite` · 6 กลุ่ม                                                                                                                   |
| `InputManager`          | **static** — รวมการอ่านคีย์บอร์ด/เมาส์ไว้ที่เดียว (`A` / `D`) และเรียก `KeyboardExtended.Update()` / `MouseExtended.Update()`                                           | ยังไม่ใช่ที่เดียวที่ผูกปุ่มทั้งหมด (ดู § 14)                                                                                         |
| `GumUIHelper`           | **static** — factory ของ control ทุกตัว (title, text, text button, pause button, slider, value label, dim panel, settings panel) ให้ style ตรงกัน                           | ใช้โดย `MainMenuScreen` / `SettingsScreen` / `CreditsScreen` / `GameplayScreen`                                 |
| `Layout`                | **static** — ค่าคงที่ design-space (1920×1080, ตำแหน่ง panel/title/menu/slider/ปุ่ม)                                                                                      | หน้าจอ Gum อ่านค่าจากที่นี่ ไม่กระจาย literal                                                                                              |

## 3. ลำดับการอัปเดต (Update Order)

`MainGame.Update` → `Time.Update(gameTime)` → `ScreenManager.Update` (→ `GameplayScreen.Update` → `GameManager.Update`) → `Gum.Update`

ภายใน `GameManager.Update` เรียงตามนี้:

1. ถ้า `MainGame.GameState == Pause` → `return` ทันที (เกมหยุดทั้งระบบ)
2. `InputManager.Update()` — `KeyboardExtended.Update()` + `MouseExtended.Update()` ก่อนทุกอย่าง
3. `_player.Update()` — ขยับผู้เล่น, ไฟฉาย, สายตา, sanity, animation (ถ้า `GameState == Jumpscare` จะ return ทันที)
4. `_enemyManager.Update(_camera.WorldBounds)` — ขยับศัตรู + เปิด/ปิด spawn timer ตามฉากปัจจุบัน
5. `_camera.LookAt(_player.CameraTarget)` + lerp zoom ตาม `player.FocusLevel` (เข้าเร็ว lerp 0.05, ออกช้า lerp 0.1)
6. `_collisionManager.Update()` — ตรวจการชนทั้งหมดหลัง object ขยับแล้ว
7. `_sceneManager.Update(_collisionManager)` — อัปเดต highlight ตาม `FocusedTrigger`
8. `_lightManager.Update(_camera.GetViewMatrix())` — อัปเดตแสงตามกล้อง
9. `AudioManager.Instance.Update(_player)` — เก็บกวาด `AudioSource` ที่จบแล้ว + อัปเดต spatial audio
10. `_uiManager.Update()` — saturation recovery, vignette, blink, nyctophobia overlay, tutorial text
11. ถ้า `player.Sanity <= 0` และ `GameState == Gameplay` → `AudioManager.ClearSFXs()` + `ScreenManager.ClearScreens(new DeathScreen(), 0, 2, 1)` + `GameState = Lose`
12. `Cheat()` — ปุ่ม debug (`Ctrl + Right Shift + +/−`)

> **เหตุผลของลำดับ:** input → ขยับ → ชน → แสง → เสียง → UI เพราะแต่ละขั้นต้องใช้ผลจากขั้นก่อนหน้า
> **`GameplayScreen` ตั้ง `UpdateWhenInactive` / `DrawWhenInactive = true`** — หน้าจอ gameplay จึงยังอัปเดต/วาดอยู่เบื้องหลังหน้าจอที่ push ทับ (เช่น `DoorPeekingScreen`) โดยดูจาก `IsActive` ว่าต้องแสดง pause button ไหม

## 4. ลำดับการวาด (Draw Order)

`GameManager.Draw` เรียงชั้นการวาดเพื่อให้ highlight อยู่ "หลังผู้เล่น" แต่ยังอยู่ใน lighting capture ของ Penumbra:

```mermaid
sequenceDiagram
    participant MG as MainGame
    participant GM as GameManager
    participant SM as SceneManager
    participant P as Penumbra
    participant SB as SpriteBatch
    participant UI as UIManager

    MG->>MG: SetRenderTarget(_frameRT)
    MG->>GM: ScreenManager.Draw()
    GM->>SM: Render(camera) → วาด highlight ลง render target ของตัวเอง
    GM->>P: BeginDraw()
    GM->>SB: Clear(Color.White)
    GM->>SM: Draw(camera) → tilemap + highlight (อยู่ใน lighting capture)
    GM->>SB: Begin(PointClamp, transformMatrix)
    GM->>SB: player.Draw() · enemies.Draw() · AudioManager.Draw() (debug)
    GM->>SB: End()
    GM->>P: Draw(gameTime) → ปิด lighting capture
    GM->>UI: Draw() → HUD / vignette / ข้อความ (screen space, ไม่ขยับตามกล้อง)
    MG->>MG: Gum.Draw()
    MG->>MG: SetRenderTarget(null) → DrawGraded() (Saturation shader) → จอ
    MG->>MG: DrawFade() → สี่เหลี่ยมดำตามค่า fade
```

## 5. การโหลดฉาก (Scene Loading)

```mermaid
sequenceDiagram
    participant GM as GameManager
    participant SM as SceneManager
    participant SB as SceneBuilder
    participant CM as CollisionManager
    participant LM as LightManager
    participant SV as SaveManager

    GM->>SM: LoadScene(sceneName, camera, collisionManager, lightManager, saveOutgoing)
    SM->>SV: SaveScene(ฉากเดิม, triggers, lightAreas) — ถ้า saveOutgoing
    SM->>SM: UnloadScene() (จำชื่อฉากเดิมไว้ใน _lastSceneName)
    SM->>SB: new SceneBuilder(sceneName, collisionManager, lightManager)
    SB->>SB: อ่าน .ldtk → Tilemap
    SB->>CM: ClearColliders() + สร้าง collider จาก object layer "Collision"
    SB->>LM: ClearLights() + สร้าง light จาก object layer "Lighting"
    SB-->>SM: PlayerSpawnPoints + Triggers
    SM->>SV: ApplyToScene(sceneName, triggers, lightAreas)
    SM->>SM: renderer.LoadTilemap + camera.EnableWorldBounds + IsZoomClampedToWorldBounds
    SM->>SM: PlayBackgroundSound (sewer → AmbSewer, อื่น ๆ → AmbElectric)
    SM->>GM: SceneLoaded(spawnPosition) — เลือก spawn ที่ Origin == ฉากเดิม (หรือ "Start")
    GM->>GM: player.LoadScenePosition + เล่นเสียง DoorClose (+ Complete("entered-sewer") ถ้าเข้าฉาก sewer)
```

ฉากที่มีอยู่ (`enum Scene`): `tilemap_electrical_room`, `tilemap_electrical_room_2`, `tilemap_sewer`

**การเลือกจุดเกิด:** `SceneManager` ไล่ `PlayerSpawnPoints` แล้วเลือกตัวที่ `Name == _lastSceneName` (ชื่อฉากที่เพิ่งออกมา) หรือ `Name == "Start"` — ถ้าไม่เจอเลยจะไม่เรียก `SceneLoaded` (ผู้เล่นค้างอยู่ที่ตำแหน่งเดิม)

**`SceneBuilder.BuildCollision`** อ่าน object layer ชื่อ **`Collision`** แล้วแยกตาม `entity.Class` (= LDtk tag ตัวแรก ไม่ใช่ชื่อ entity):

| `entity.Class`                        | กลายเป็น                                            | ใช้ property |
| --------------------------------------- | ----------------------------------------------------------- | --------------- |
| `wall`                                | `Wall` (layer `walls`)                                  | —              |
| `dots` / `hideout` / `skillcheck` | `Trigger` (layer `triggers`)                            | —              |
| `door`                                | `Trigger` (layer `triggers`) ชื่อ = `Destination` | `Destination` |
| `report`                              | `Trigger` (layer `triggers`) ชื่อ = `Detail`      | `Detail`      |
| `spawn`                               | `SpawnPoint` ชื่อ = `Origin`                        | `Origin`      |

**`SceneBuilder.BuildLight`** อ่าน object layer ชื่อ **`Lighting`** โดยกำหนด `entity.Class == "light"` แล้วแยกต่อด้วย **`entity.Name`**: `PointLight` (ได้ `LightArea` + collider layer `lights`) · `DoorLight` (ได้ `PointLight` เฉย ๆ ไม่มี collider) · `SpotLight`

## 6. Collision Layer

`CollisionManager` ใช้ `CollisionWorld2D` ของ MonoGame.Extended (spatial hash 64×64) และสร้าง layer ใหม่ทุกครั้งที่โหลดฉาก (`ResetLayers()` ถูกเรียกจาก `SceneBuilder.BuildCollision`)

| Layer        | Dynamic | ใส่อะไร                                                          |
| ------------ | ------- | ----------------------------------------------------------------------- |
| `walls`    | ❌      | `Wall` จาก tilemap                                                 |
| `triggers` | ❌      | `Trigger` (ตู้ / มินิเกม / ประตู / คลิปบอร์ด) |
| `enemies`  | ✅      | `Autophobia`, `Scopophobia`                                         |
| `lights`   | ❌      | `LightArea`                                                           |

การตั้งค่าการชนระหว่าง layer:

- ปิดการชนระหว่าง `enemies` ↔ `walls` และ `lights` ↔ `walls`
- เปิดการชนระหว่าง `enemies` ↔ `lights` (เพื่อให้ศัตรูทำให้ไฟกระพริบ/ดับ)

การตรวจการชนที่ทำทุกเฟรม (`CollisionManager.Update`):

| คู่                            | ผล                                                                                                                                                                                                                            |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Player ↔`walls`                | `player.CollideWithWallMove(MTV)`                                                                                                                                                                                             |
| Player ↔`triggers`             | หา `FocusedTrigger` ที่ใกล้ที่สุด (เทียบระยะจาก bounding box · ตัวที่ใกล้กว่าชนะ) · ข้าม trigger ที่ `Interactable == false`, ที่เป็น `dots` ทั้งที่ไฟเปิดครบแล้ว, และตอน `MainGame.Loading` · จัดการ interaction ตาม tag (ดู § 7)                                                       |
| Player ↔`enemies`              | Autophobia →`Sanity −10`, ปิด enemy และตั้ง `Saturation = 0` + เสียง `PhobiaHit` · Scopophobia (ที่ยัง `Visible`) → `Sanity −25` แล้วซ่อน enemy + เสียง `PhobiaHit` |
| Player `EyeSight` ↔ Autophobia | ถ้าตัดกัน →`AllowMovement = false`, ลบออกจาก collision world, `player.Blink = true` (กระพริบตา) แล้ว `SetActive` หลัง 0.15 วินาที                                                |
| Player ↔`lights`               | ตั้ง `player.Nyctophobia = false` ก่อน แล้วถ้า `LightArea.On == false` และไม่กำลัง flicker → เล่น `Darkness` + `player.Nyctophobia = true`                                               |
| `enemies` ↔ `lights`         | Scopophobia (ที่`ScopoHasPassed == false`) → `ScopoHasPassed = true`, `LightPassedCount++` และถ้าไฟยังติด → `TurnOff()` + เสียง `LightBreak` แบบ 3D · Autophobia → `lightArea.StartFlicker()`                                                    |

## 7. Interaction (ปุ่มและผลลัพธ์)

| Trigger tag                     | ปุ่ม  | ผล                                                                                                                                         |
| ------------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| `hideout` (ตู้)            | `Space` | ถ้ายังไม่ซ่อน → เก็บ `PreviousPos`, ย้ายผู้เล่นไปกลาง collider แล้ว `ToggleHide()` + `Complete("interact")`                          |
| `skillcheck` (วาล์ว)     | `Space` | เปิด `SpinTheValveScreen` + `player.ToggleRepairing()`                                                                                  |
| `dots` (แผงไฟ)           | `Space` | เปิด `ConnectTheDotsScreen` + เสียง `ElecRepairing` + `ToggleRepairing()` + `Complete("repair")` + `Complete("interact")`              |
| `report` (คลิปบอร์ด) | `Space` | ถ้ายังไม่ได้เปิดอยู่ → เปิด `ClipboardScreen` + `player.ToggleReading()` + `Complete("interact")`                                |
| `door` (ประตู)           | `Space` | `Complete` ให้ objective ของ tutorial (`move` `interact` `repair` `dots-done` `peek` `door-open`) แล้วยิง event `DoorInteracted` (`GameManager` สลับฉาก)                               |
| `door` (ประตู)           | `E`     | `player.TogglePeek()` + เปิด `DoorPeekingScreen` (fade 0.5 วิ) — **ยกเว้นในฉาก `tilemap_sewer`** |

`GameManager.OnDoorInteracted`:

- `Destination == "end"` → ถ้า `AllValvesRepaired && SaveManager.AllSewerLightsAreOn()` (หรือกด `F10` ค้าง) → `Complete("exit-ladder")` + `SaveManager.Delete()` + `ResetValves()` + `ClearEnemies()` + `ClearSFXs()` + เปิด `WinScreen` (พร้อมเวลาที่เหลือ) · ถ้าไม่ครบ → `UIManager.ShowMessage("I still have work to do.", 2f)`
- ถ้ามี Scopophobia ที่ยัง active → เล่น `DoorStuck` (เปิดประตูไม่ได้)
- ถ้ามี Entamaphobia ที่ยัง active → `ClearEnemies()` + `LoadScene(ปลายทาง)` + เปิด `JumpScareScreen`
- ไม่งั้น → `FadeOut(0.25)` + `ClearSFXs("anxietySound", "Darkness")` + `FadeSFXVolumeOut(Darkness, 1.5)` + `ClearEnemies()` + `LoadScene(ปลายทาง)` + `FadeIn(0.25)`

`GameManager.OnEnemySpawned`:

- **Nyctophobia** → ถ้าไฟยังไม่ดับหมด เล่น `LightFlickerLong` · `SaveManager.ResetScene` ห้องไฟฟ้าทั้งสอง · `ObjectiveManager.ResetPanels()` · `LightManager.NyctoAlarmAll()` แล้ว return (ไม่เพิ่ม collider)
- ตัวอื่น → เพิ่ม collider layer `enemies`
- **Scopophobia** → `ResetScene` ห้องไฟฟ้าทั้งสอง · `LightManager.CleanUpScopo()` (ล้าง `ScopoHasPassed`) · `ResetPanels()` · `UIManager.BlackScreen()` · `LightManager.ScopoAlarmAll()`

`GameManager.OnEnemyRemoved` → ถ้าไม่ใช่ Nyctophobia/Entamaphobia ก็ลบ collider ออก

## 8. Lighting

- ใช้ **MonoGame.Penumbra** ทำแสง 2D — `LightManager.Penumbra` เป็น static `PenumbraComponent` (สร้างครั้งเดียว)
- `Penumbra.AmbientColor = #242323` (แสงแวดล้อมมืด)
- `LightManager.Update(Matrix transformMatrix)` ตั้ง `Penumbra.Transform` ตามกล้อง แล้วอัปเดต `LightArea` ทุกตัว
- `LightArea` ทำหน้าที่เป็นทั้ง collider และแหล่งแสง — มีสถานะ `On` (= `_powered`), `IsFlickering`, `ScopoHasPassed` และเมธอด `StartFlicker(duration = 1.5s)`, `TurnOff()`, `TurnOn()`, `ScopoIncoming()` (แสงเปลี่ยนเป็นสีแดง), `StartNyctoAlarm()` (กระพริบ 3 วินาทีแล้วดับถาวร) · พื้นที่ collider = 600×600
- แสงที่ผูกกับผู้เล่น (สร้างใน `Player` แล้วเพิ่มเข้า `Penumbra.Lights` ตรง ๆ): `PointLight` รอบตัว (`_scotopicLight`, scale 500×500, intensity 0.5, `Color.FromHSV(150, 0.5, 0.5)`) และ `Spotlight` ไฟฉาย (`_flashLight`, scale 600×1250, ชี้ซ้าย/ขวาตามการหันหน้า)
- `DoorPeekingScreen` ปิด light ทุกดวงของฉาก (ยกเว้นไฟฉาย/ไฟรอบตัวผู้เล่น) ตั้ง ambient เป็น `#615D5D` และเพิ่ม `PointLight` ของตัวเองที่เปิดตามสถานะไฟฉาย
- เอฟเฟกต์เสริมใน `Scripts/SpriteEffects/`:
  - `VignetteOverlay` — ขอบจอมืดลง ใช้ 3 ตัวใน `UIManager`: `_vignetteOverlay` (sanity, tween ด้วย `Tweener` ของ MonoGame.Extended), `_blinkOverlay` (กระพริบตา), `_nyctophobiaOverlay` (ถูกความมืดกลืน, `Softness = 15`) · shader `Shaders/Vignette` เทคนิค `EllipseMask`
  - `BottomFadeOverlay` — เฟดด้านล่างของจอ · shader `Shaders/BottomFade`

## 9. Audio

- `AudioManager` เป็น **singleton** (`Instance`) สร้างใน `MainGame.Initialize()` และเป็นเจ้าของ `AudioSource` ที่ active ทั้งหมด (`AudioSource : GameObject, IDisposable`)
- `SfxByName` = `Dictionary<SoundEffectName, SoundEffect>` (**33 ค่า**) โหลดจาก `Content/Audio/` · `_steps` = array 8 เสียงฝีเท้า
- `BgsByName` = `Dictionary<BackgroundSound, Song>` (5 ค่า: `AmbElectric`, `AmbSewer`, `BgmMainMenu`, `BgmDeathScreen`, `BgmWinScreen`) เล่นผ่าน `MediaPlayer` แบบวนซ้ำ · `BgsVolume` default `0.2`
- ตัวที่ implement `ISpatialSoundApplicable` (มี `MoveSpeed` / `Velocity` / `Direction`) จะใช้คำนวณ spatial audio: `Player`, `Autophobia`, `Scopophobia`
- **Spatial audio:** `Play3DSFX(pos, sound, innerRadius = 10, outerRadius = 80, volume = 1)` คืน `AudioSource` (ผู้เรียกเก็บไว้เป็น child ได้ เช่น `Scopophobia`) — `AudioSource.UpdateSpatialAudio(player)` คำนวณ volume จากระยะห่าง, `Pan` จากแกน X และ `Pitch` จาก Doppler effect (`SPEED_OF_SOUND = 343.5`)
- `PlayStepsSFX(pos, inner, outer)` — เสียงฝีเท้า 8 แบบ ไล่ตามลำดับ `_stepOrder`
- `PlayLightFlickerSFX(pos, inner, outer)` — สุ่มจาก `LightFlickerShort1..3` (ใช้ตอนไฟกระพริบ)
- `PlayBackgroundSound(BackgroundSound)` — หยุดเพลงเดิมแล้วเล่นเพลงใหม่แบบวนซ้ำ
- `StopSFX(name)` / `ClearSFXs()` / `ClearSFXs(params string[])` — หยุดเสียงที่ยังเล่นอยู่ (เรียกก่อนเปลี่ยนฉาก)
- `ChangeMediaVolume(swapBack, volumeScale)` — หรี่/คืนเสียง ambience+BGM (ใช้ตอนอ่านคลิปบอร์ดและตอนแอบมองประตู = ×0.5)
- `MuteAudio` / `UnmuteAudio` / `ToggleMute` / `PauseAudio` / `ResumeAudio` พร้อมเก็บ volume เดิมไว้
- ไลบรารี **MonoSound** (`MonoSoundLibrary.Init` / `DeInit` ใน `MainGame`) เปิดทางไว้สำหรับ filter (echo / reverb / biquad) — โค้ดส่วนนั้นยังคอมเมนต์ไว้

## 10. Highlight System

ระบบ highlight วัตถุที่โต้ตอบได้ ทำงานร่วมกัน 3 คลาส:

| คลาส                  | หน้าที่                                                                                                                                                                           |
| ------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `HighlightSettings`     | พารามิเตอร์ของ effect:`ScanSpeed`, `ScanWidth`, `ScanColor`, `PulseSpeed`, `PulseMin`, `PulseColor`, `OutlineRadius`, `OutlineColor`                       |
| `HighlightGroup`        | กลุ่ม tile ที่จะวาด:`Layer`, `Settings`, `FocusTiles`, `ScanTiles`, `PulseTiles`                                                                                  |
| `TileHighlightRenderer` | ถือ 6 กลุ่ม (`ValveGroup`, `LockerGroup`, `PanelGroup`, `DoorGroup`, `ClipboardGroup`, `LadderGroup`) และวาดผ่าน render target 2 ตัว (mask + composite) |

- `SceneBuilder.BuildHighlightTiles` เอา tile ที่อยู่ในกรอบของ collider (`layer.GetTilesInRegion`) — เลือก layer ด้วย `FindHighlightLayer`: ชื่อ entity ตัด `s`/`S` ตัวท้ายออกแล้วเทียบกับชื่อ tile layer · ยกเว้น `LoadNextScene` ที่ใช้ layer `Doors` และถ้า `Destination == "end"` จะใช้ layer `Ladder`
- `SceneManager.UpdateHighlight` เลือกกลุ่มตามชื่อ tile layer (`Valves` / `Lockers` / `ElectricalPanel` / `Doors` / `Clipboard` / `Ladder`) และข้ามวัตถุที่ `Interactable == false`, แผงไฟที่ไฟเปิดครบแล้ว, วาล์ว/แผงไฟที่กำลังซ่อมอยู่ และตู้ที่กำลังซ่อนอยู่ · ตัวที่อยู่ใต้ `FocusedTrigger` จะเข้า `FocusTiles` ที่เหลือเข้า `ScanTiles` · sync renderer ใหม่ทุกครั้งที่ฉากเปลี่ยน (`ReferenceEquals` กับ `_highlightScene`)
- Settings ต่อกลุ่มถูกตั้งใน `SceneManager` constructor เช่น `_valveSettings` (`ScanSpeed 0.5`, scan `#bfd8ff`, outline 3px `#ffd83f`), `_panelSettings` (`ScanSpeed 0.15`, `ScanWidth 0.04`) และ `_lockerSettings` / `_doorSettings` / `_clipboardSettings` / `_ladderSettings` (scan ปิด, outline `#e0e8e0`)
- shader: `Shaders/Silhouette` (เทคนิค `HighlightMask` — เขียน mask ลงช่อง RGBA ตามชนิด focus/scan/pulse) + `Shaders/HighlightComposite` (เทคนิค `Highlight` — รวม mask เป็น outline/scan/pulse)

## 11. Screens

ทุกหน้าจอสืบทอดจาก `Screen` ของ **MonoGame.Extended.Screens** (`Update(GameTime)` / `Draw(GameTime)`) และถูกเปิดผ่าน `MainGame.ScreenManager.ShowScreen(...)` / `ReplaceScreen(...)` / `CloseScreen()` — ทุกครั้งที่เปิด/ปิดจะผ่าน fade ของ `ModifiedScreenManager`

| Screen                   | ใช้กับ                                                     | หมายเหตุ                                                                                                                                                                                                                            |
| ------------------------ | ---------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `MainMenuScreen`       | หน้าจอแรกของเกม                                   | พื้นหลัง `UI/background_main.png` + `EyeField` · ปุ่ม: Shift 1 (เริ่มเกม), Index (ปิดใช้งาน), Settings, Credits, Exit · ลูกศร `<` `>` ปิดใช้งาน · เล่น `BgmMainMenu` · ตั้ง `GameState = Menu` และ `Time.TimeScale = 1`                                                                          |
| `SettingsScreen`       | ตั้งค่าเสียง                                         | `GumUIHelper.AddSettingsPanel` (สไลเดอร์ Ambience / SFX volume) + ปุ่ม Back · `ESC` = กลับ                                                                                                                             |
| `CreditsScreen`        | เครดิตทีม                                               | `ScrollViewer` เลื่อนรายชื่อได้ (รางเป็นภาพ `slider_placeholder`, หัวจับหมุนตามการเลื่อน) + dummy ScrollViewer 1×1                                                                                               |
| `GameplayScreen`       | หน้าหลักของการเล่น                             | ห่อ `GameManager` (สร้างใน `Initialize` พร้อม `BoxingViewportAdapter`) · `UpdateWhenInactive` / `DrawWhenInactive = true` · **pause overlay** ในตัว (dim panel + settings + ปุ่ม Return to Menu) เปิด/ปิดด้วย `ESC` หรือปุ่มมุมขวาบน (ตั้ง `TimeScale = 0` + `PauseAudio`) |
| `DeathScreen`          | Sanity ถึง 0                                                  | พื้นหลัง `UI/background_death.png` + ข้อความ tip สุ่ม 1 จาก 9 · ปุ่ม "< Return to Menu" · `ESC` = กลับเมนู · เล่น `BgmDeathScreen` · ตั้ง `GameState = Lose`                                                           |
| `WinScreen`            | วาล์วครบ + ไฟ sewer ติดครบ แล้วออกประตู`end` | พื้นหลัง `UI/background_win.png` · "You survived." · "Time left: mm:ss" (รับ `timeLeft` มาจาก `MainTimer`) · "Thanks for playing." · ปุ่มกลับเมนู · เล่น `BgmWinScreen`                                                              |
| `JumpScareScreen`      | เปิดประตูขณะ Entamaphobia ยัง active                  | วาด `entama_jumpscare` เต็มจอ + เสียง `EntamaJumpscare` (volume 0.2) · ตั้ง `GameState = Jumpscare` · ครบ 1.6 วินาทีแล้วปิดตัวเองและคืน `GameState = Gameplay`                             |
| `SpinTheValveScreen`   | ซ่อมวาล์ว (trigger tag `skillcheck`)                  | ใช้`UI/sprite_valve.png` · หมุนด้วยเมาส์ค้างแล้วลากเป็นวงกลม · `MinigameVisible` ถูกปิดชั่วคราวตอน Scopophobia มา (black screen) · `Q` = ออก (บันทึก progress ลง trigger)                             |
| `ConnectTheDotsScreen` | ซ่อมไฟ (trigger tag `dots`)                              | ใช้`UI/spritesheet_minigame_dot` · กระดาน 6×6 · helper `AddDot`, `ScreenToGrid`, `TryExtendPath`, `GetCellTopLeft`, `GetCellTopCenter` · คลิกขวา = ล้างเส้น · `Q` = ออก (บันทึก progress) |
| `DoorPeekingScreen`    | แอบมองใต้ช่องประตู (ปุ่ม `E`)            | พื้นหลัง `background_peek_er1/er2` + `foreground_peek_door` + ภาพ `entama_stage_{Trigger.Progress}` · ไฟฉาย (`F`) เป็นตัวส่องให้เห็น (ถ้าไม่เปิดจอจะมืด) · `Q` = ออก (`Complete("peek")`) · ใช้ได้เฉพาะนอกฉาก sewer |
| `ClipboardScreen`      | อ่านคลิปบอร์ด (trigger tag `report`)               | 4 หน้าต่อห้อง + หน้าลับ (โอกาส 3%) · แว่นขยายตามเมาส์ (scroll = ซูม 1.5×–8×) · pause `MainTimer` + `EntamaSpawnTimer` ระหว่างอ่าน · `Q` = ออก |

## 12. Utility

| Class                    | หน้าที่                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Time` (static)        | ตัวจับเวลากลาง —`DeltaTime`, `UnscaledDeltaTime`, `ElapsedTime`, `TimeScale`, `AddTimer(seconds, name)`, `AddTimer(Action, seconds, name)`, `AddTimer(Action, min, max, name)` (สุ่มช่วง + repeat), `RemoveTimer(Timer)`, `RemoveTimer(name)`, `GetTimer(name)`, `TimerExist(name)`, `Purge()` · `MainGame.Update` เรียก `Time.Update` ให้ทุกเฟรม (timer ที่ `Active == false` จะถูกตัดออกอัตโนมัติ) · timer ที่ชื่อ `"ScreenTimer"` ใช้ `UnscaledDeltaTime` จึงเดินต่อแม้ `TimeScale = 0` |
| `Timer`                | ตัวจับเวลาเดี่ยว —`TimeLeft`, `Active`, `Repeat`, `Random`, `MinMax`, `Pause()`, `Resume()`, `ToggleActive()`, `SetInActive()`, `Reset()`, event `OnTimerCompleted`                                                                                                                                                                                                                                                                                                     |
| `CameraShaker`         | สั่นกล้อง —`Shake(duration, magnitude)` แล้ว `Update(gameTime)` ให้ `ShakeOffset` · **ไม่ถูกเรียกใช้จากที่ไหน** (โค้ดใน `GameManager` คอมเมนต์ไว้)                                                                                                                                                                                                                                                                                                                                                 |
| `Layout` (static)      | ค่าคงที่ layout ของทุกหน้าจอ (1920×1080 + ตำแหน่ง panel/title/menu/slider)                                                                                                                                                                                                                                                                                                                                                                                      |
| `GumUIHelper` (static) | factory ของ Gum control ทุกตัวที่หน้าจอใช้                                                                                                                                                                                                                                                                                                                                                                                                                             |
| `SpawnPoint`           | จุดเกิด (`Name` + `Position`)                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| `GameObject`           | base class ของทุกอย่างในฉาก —`Transform2`, `Parent`/`Children`, `Active`/`Visible`, `Update()`/`Draw()` เป็น virtual, `Destroy(...)`, `OnDestroyed()`                                                                                                                                                                                                                                                                                                                  |
| `EyeField` + `EyeInstance` · `EyeSpriteSet` · `EyeShape` · `EyeMotion` · `ScleraField` | ระบบดวงตาบนเมนูหลัก — สุ่มวางดวงตาลงบนบริเวณมืดของภาพพื้นหลัง, มองตามเมาส์, กระพริบ, และตอบสนองการคลิก (`TwitchAll`) · `EyeShape` ใช้ signed-distance field (`ScleraField`) กันรูม่านตาไม่ให้หลุดออกนอกตาขาว |

## 13. จุดที่ต้องแก้เมื่อเพิ่มของใหม่ (Extension Points)

| อยากเพิ่ม                                                      | ต้องแตะ                                                                                                                                                                                                                                                                                                   |
| ----------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Phobia ตัวใหม่ | สร้างคลาสใน `Scripts/Enemies/` (สืบทอด `GameObject`, implement `ISpatialSoundApplicable` + `ICollisionActor` ตามต้องการ), เพิ่ม `Time.AddTimer(...)` ใน `EnemyManager` constructor, เช็ค `HasActive*` ในเมธอด spawn ตัวอื่น, ผูก logic ใน `GameManager.OnEnemySpawned`/`OnEnemyRemoved`, เพิ่ม asset ใน `Content/Phobias/` + `Content.mgcb` |
| Minigame ใหม่                                                       | สร้างคลาสใน `Scripts/Screen/` สืบทอด `Screen` แล้วเปิดจาก `CollisionManager` ตาม tag ของ `Trigger`                                                                                                                                                                     |
| ฉากใหม่                                                          | เพิ่ม `.ldtk` ใน `Content/Tilemaps/` + ลงทะเบียนใน `Content.mgcb` + เพิ่มค่าใน `enum Scene` (ชื่อไฟล์ต้องตรงกับค่า enum)                                                                                                                                |
| เสียงใหม่                                                      | เพิ่มไฟล์ใน `Content/Audio/` + ลงทะเบียนใน `Content.mgcb` + เพิ่มค่าใน `enum SoundEffectName` (หรือ `BackgroundSound`) + เพิ่ม entry ใน `SfxByName` / `BgsByName`                                                                                         |
| หน้าจอเมนู/UI ใหม่                                        | สร้าง `Screen` ใหม่ที่ประกอบจาก `GumUIHelper` + วางตำแหน่งด้วย `Layout` แล้วเปิดผ่าน `ScreenManager.ShowScreen`                                                                                                                                           |
| ข้อความ tutorial ใหม่                                        | เพิ่ม step ใน `Content/Datas/tutorial.json` (`Text` + `CompleteOn` = objective id) แล้วเรียก `ObjectiveManager.Complete(id)` ที่จุดที่ต้องการ (ต้องมี id จริง ไม่งั้น step นั้นจะค้าง)                                                        |
| ค่าปรับแต่ง (speed, sanity rate)                             | **ยังไม่มีศูนย์กลาง** — กระจายอยู่ในโค้ด (ดู [Data Schema](03-data-schema.md#6-ค่าปรับแต่งเกม-tuning-values))                                                                                                                                               |

## 14. Technical Notes (ข้อเท็จจริงเชิงเทคนิค)

| เรื่อง                                                                | รายละเอียด                                                                                                                                                                                                                                                                                                                                                                           |
| ------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ค่าปรับแต่งกระจายอยู่ทั่วโค้ด                | ไม่มี config/data file กลาง — มีแค่`Datas/tutorial.json` ที่เป็น data จริง นอกนั้น hardcode (ดู [Data Schema § 6](03-data-schema.md#6-ค่าปรับแต่งเกม-tuning-values))                                                                                                                                                                                             |
| `InputManager` ไม่ได้เป็นที่เดียวที่ผูกปุ่ม | `A`/`D` อยู่ที่ `InputManager` แต่ `Left Ctrl`, `F`, `Space`, `Q`, `E` อ่านตรงใน `Player.cs`, `CollisionManager.cs` และหน้าจอ task · `ESC` อ่านผ่าน `GumService.Default.Keyboard` ในแต่ละหน้าจอ · cheat keys อยู่ใน `GameManager.Cheat()` และ `OnDoorInteracted` |
| `CameraShaker` ไม่ถูกใช้งาน                    | คลาสยังอยู่และสมบูรณ์ แต่ `GameManager` มีบรรทัดเรียก `Shake`/`Update` คอมเมนต์ไว้                                    |
| Save ยังไม่ครอบคลุมทุกอย่าง                         | `SaveManager` บันทึกแค่ trigger (progress / interactable) และสถานะไฟ — **ไม่บันทึก** ตำแหน่งผู้เล่น, Sanity, Battery, ฉากปัจจุบัน, objective · `GameManager` เรียก `SaveManager.Delete()` ทุกครั้งที่สร้าง `GameManager` ใหม่ |
| ตัวจับเวลาหลักไปจบที่`DeathScreen`                   | `Time.AddTimer(_player.Dead, 721, "MainTimer")` — หมดเวลา 721 วินาทีแล้ว Sanity ถูกตั้งเป็น 0 → `DeathScreen` (เวลาที่ตั้งไว้จึงทำหน้าที่เป็นเวลาสูงสุดของรอบเล่น) |
| `GameManager.Restart()` ไม่มีที่เรียก | เมธอดยังอยู่ในโค้ดแต่ไม่มีจุดไหนเรียกใช้ — ตายแล้วผู้เล่นกลับเมนูแล้วเริ่มรอบใหม่จาก `MainMenuScreen` แทน |
| `Time.Purge()` | ถูกเรียกจาก `GameManager.Dispose()` — ตั้ง timer ทุกตัวให้ inactive ยกเว้น `"ScreenTimer"` |
| ชื่อไฟล์/เมธอดสะกดไม่ตรงกัน               | `Scripts/Datas/TurotialData.cs` (ไฟล์สะกดผิด) · `Painting` map กับไฟล์ `sfx_panting.mp3` · `AudioSource.FadeVolumnOut` · `Player._sewerSanityRedectionSpeed` |
| ไฟล์ใน `Content/bin/` ล้าสมัย                              | `bin/` มี 117 ชื่อไฟล์ เทียบกับ 99 ไฟล์ใน source — มีของที่ไม่มีต้นฉบับแล้ว 30 รายการ เช่น `sfx_autophobia_indicator`, `sfx_light_flickering_long`, `sfx_light_flickering_short_01 ` (มีช่องว่างท้ายชื่อ), `spitesheet_player`, `imaginophobia_bg_placerholder`, `sprite_spacebar`, `spritesheet_extra_decoration_0` (เป็น build artifact)                                                                              |

## Related Documents

- Class Diagram: [Class Diagram](02-class-diagram.md)
- Data Schema: [Data Schema](03-data-schema.md)
- Concept (architecture summary): [Game Concept](../01_GDD/00-concept.md#7-system-architecture)
- Mechanics: [Mechanics](../01_GDD/03-mechanics.md)
