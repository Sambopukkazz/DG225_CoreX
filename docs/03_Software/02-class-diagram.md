---
type: software-class-diagram
version: 3.0
date: 2026-10-25
---

# Imaginophobia — Class Diagram

**Version:** 3.0 | **Last Updated:** 2026-10-25

> สร้างจากโค้ดจริงใน `src/Imaginophonia/Imaginophonia/` (ณ commit `cf024ef`, 2026-10-24)
> ไฟล์นี้เดิมอยู่ใน `docs/01_GDD/04-class-diagram.md` — ย้ายมาไว้ในหมวด Software Design เพราะเป็นงานออกแบบซอฟต์แวร์
> `ICollisionActor`, `Screen`, `Tilemap*` มาจากไลบรารี (MonoGame.Extended) — ไม่ได้ประกาศในโปรเจกต์ ส่วน `ISpatialSoundApplicable` ประกาศในโปรเจกต์เองที่ `Scripts/ISpatialSoundApplicable.cs`

## Core Hierarchy

```mermaid
classDiagram
    class GameObject {
        +Transform2 Transform
        +Vector2 Origin
        +GameObject Parent
        +List~GameObject~ Children
        +bool Active
        +bool Visible
        +string Tag
        +string Name
        +Update()
        +Draw()
        +OnDestroyed()
        +AddChild(GameObject)
        +RemoveChild(GameObject)
        +SetActive(bool)
    }

    class Player {
        +CollisionShape2D Shape
        +LineSegment2D EyeSight
        +AudioListener Listener
        +Spotlight FlashLight
        +float MoveSpeed = 250
        +float Sanity = 100
        +float Battery = 100
        +SanityState SanityState
        +CharacterAction CurrentAction
        +BlinkPhase BlinkPhase
        +float FocusLevel
        +Vector2 CameraTarget
        +Vector2 PreviousPos
        +bool CanInteract
        +bool CanRepair
        +bool Blink
        +bool Nyctophobia
        +Timer CooldownTimer
        +event SanityStateChanged
        +Update()
        +Draw()
        +CollideWithWallMove(Vector2)
        +ToggleHide()
        +SetReadyToHide()
        +ToggleRepair()
        +TogglePeek()
        +ToggleFlashLight()
        +CheckSanityStage()
        +LoadScenePosition(Vector2)
        +Reset()
    }

    class AudioSource {
        +SoundEffectInstance AudioPlayer
        +bool IsDisposed
        +UpdateSpatialAudio(Player)
        +PlayLoop(SoundEffect, float)
        +PlayOneShot(SoundEffect, float)
    }

    class Autophobia {
        +float MoveSpeed = 400
        +int Id = 666
        +bool AllowMovement
        +Update()
        +Draw()
    }

    class Scopophobia {
        +float MoveSpeed = 2600
        +int Id = 555
        +int LightBrokeCount
        +Update()
        +Draw()
        +OnDestroyed()
    }

    class Nyctophobia {
        <<marker>>
    }

    class Entamaphobia {
        <<stub>>
        +Update()
    }

    class ISpatialSoundApplicable {
        <<interface>>
        +float MoveSpeed
        +Vector2 Velocity
        +Vector2 Direction
    }
    class ICollisionActor {
        <<interface>>
        +int Id
        +CollisionShape2D Shape
    }

    GameObject <|-- Player
    GameObject <|-- AudioSource
    GameObject <|-- Autophobia
    GameObject <|-- Scopophobia
    GameObject <|-- Nyctophobia
    GameObject <|-- Entamaphobia
    Player ..|> ISpatialSoundApplicable
    Player ..|> ICollisionActor
    Player ..|> IDisposable
    AudioSource ..|> IDisposable
    Autophobia ..|> ISpatialSoundApplicable
    Autophobia ..|> ICollisionActor
    Scopophobia ..|> ISpatialSoundApplicable
    Scopophobia ..|> ICollisionActor
    Scopophobia --> AudioSource : Play3DSFX (child)
```

> `Nyctophobia` เป็น **marker class เปล่า ๆ** — ใช้ให้ `EnemyManager.HasActiveNyctophobia()` ตรวจได้ว่า "ความมืดกำลังเล่นงานอยู่" พฤติกรรมจริงอยู่ใน `LightArea.StartNyctoAlarm()` + `UIManager`
> `Entamaphobia` มี `Update` ครึ่ง ๆ กลาง ๆ (`_stage`, `_value`, `_nextStageValue`, `_speed`) และยังไม่ถูก spawn จาก timer ไหน

## Managers

```mermaid
classDiagram
    class MainGame {
        +MainGame Instance$
        +ScreenManager ScreenManager$
        +float Saturation$
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
        -DrawGraded()
    }
    class GameManager {
        +Update(GameTime)
        +Draw(GameTime)
        -Restart()
        -OnDoorInteracted(string)
        -OnSceneLoaded(Vector2)
        -OnEnemySpawned(GameObject)
        -OnEnemyRemoved(GameObject)
    }
    class SceneManager {
        +GetActiveScene() SceneBuilder$
        +Update(CollisionManager)
        +Draw(OrthographicCamera)
        +Render(OrthographicCamera)
        +LoadScene(string, OrthographicCamera, CollisionManager, LightManager, bool)
        +event SceneLoaded
    }
    class SceneBuilder {
        +string Name
        +Tilemap TileMap
        +List~SpawnPoint~ PlayerSpawnPoints
        +List~Trigger~ Triggers
    }
    class CollisionManager {
        +Trigger FocusedTrigger
        +Update()
        +AddCollider(ICollisionActor)
        +AddCollider(ICollisionActor, string)
        +RemoveCollider(ICollisionActor)
        +ClearColliders()
        +event DoorInteracted
    }
    class LightManager {
        +PenumbraComponent Penumbra$
        +List~LightArea~ LightAreas
        +Update(Matrix)
        +AddLight(Light)
        +AddLight(LightArea)
        +ClearLights()
        +ScopoAlarmAll()
        +NyctoAlarmAll()
        +CleanUpScopo()
    }
    class AudioManager {
        +AudioManager Instance$
        +float BgsVolume
        +float SoundEffectVolume
        +bool IsMuted
        +Update(Player)
        +PlaySFX(SoundEffectName, bool, bool, float)
        +Play3DSFX(Vector2, SoundEffectName, float, float, float) AudioSource
        +PlayStepsSFX(Vector2, float, float)
        +PlayLightFlickerSFX(Vector2, float, float)
        +PlayBackgroundSound(BackgroundSound)
        +StopSFX(SoundEffectName)
        +ClearSFXs()
        +ToggleMute()
    }
    class EnemyManager {
        +List~GameObject~ Enemies
        +Update(Rectangle)
        +Draw()
        +SpawnScopophobia()
        +SpawnAutophobia()
        +SpawnNyctophobia()
        +SpawnEntamaphobia()
        +ClearEnemies()
        +HasActiveScopophobia() bool
        +HasActiveNyctophobia() bool
        +HasActiveEntamaphobia() bool
        +event EnemySpawned
        +event EnemyRemoved
    }
    class UIManager {
        +Update()
        +Draw()
        +ShowMessage(string, float)
        +BlackScreen()
        +NyctoClose()
        +NyctoOpen()
        +BlinkClose()
        +BlinkOpen()
        +ZoomVignette()
        +BackToStageScale()
    }
    class TextManager {
        +Update()
        +Draw()
        +ShowMessage(string, float)
        +InputTutorial(string)
    }
    class TutorialManager {
        +GetText() string
        +OnCompleted(string)
    }
    class ObjectiveManager {
        <<static>>
        +int TargetValves = 5$
        +bool AllValvesRepaired$
        +int RepairedValveCount$
        +Complete(string)$
        +IsComplete(string) bool$
        +IsValveRepaired(int) bool$
        +IsPanelRepaired(int) bool$
        +RepairValve(int)$
        +RepairPanel(int)$
        +ResetValves()$
        +ResetPanels()$
        +Reset()$
        +event Completed
    }
    class SaveManager {
        <<static>>
        +SaveScene(string, List~Trigger~, List~LightArea~)$
        +ApplyToScene(string, List~Trigger~, List~LightArea~)$
        +TurnAllLightsOn()$
        +ResetScene(string)$
        +Delete()$
    }
    class InputManager {
        <<static>>
        +Vector2 Direction$
        +Update()$
    }
    class TileHighlightRenderer {
        +HighlightGroup ValveGroup
        +HighlightGroup LockerGroup
        +HighlightGroup PanelGroup
        +HighlightGroup DoorGroup
        +HighlightGroup ClipboardGroup
        +HighlightGroup LadderGroup
        +LoadScene(Tilemap)
        +Clear()
        +Update()
        +Render(OrthographicCamera)
        +Draw()
    }
    class GumUIHelper {
        <<static>>
        +CreateTitle(string)$
        +CreateText(...)$
        +CreateTextButton(...)$
        +CreatePauseButton(Action)$
        +CreateSlider(int, int)$
        +CreateValueLabel(int, int, float)$
        +CreateDimPanel()$
        +AddSettingsPanel(ContainerRuntime)$
    }
    class Layout {
        <<static>>
        +int ScreenWidth = 1920$
        +int ScreenHeight = 1080$
    }

    GameManager --> SceneManager
    GameManager --> CollisionManager
    GameManager --> LightManager
    GameManager --> EnemyManager
    GameManager --> UIManager
    GameManager --> Player
    MainGame --> AudioManager
    MainGame --> GameManager
    SceneManager --> SceneBuilder
    SceneManager --> TileHighlightRenderer
    SceneBuilder --> CollisionManager
    SceneBuilder --> LightManager
    EnemyManager --> Autophobia
    EnemyManager --> Scopophobia
    EnemyManager --> Nyctophobia
    EnemyManager --> Entamaphobia
    AudioManager --> AudioSource
    UIManager --> TextManager
    UIManager --> TutorialManager
    TutorialManager --> ObjectiveManager
    SaveManager --> Trigger
    SaveManager --> LightArea
```

> `AudioManager` เป็น singleton ที่ถูกสร้างใน **`MainGame.Initialize()`** (ตัวแปร local ไม่ได้เก็บเป็น field) — ไม่ได้สร้างใน `GameManager` อีกแล้ว
> `GameManager` ยังมี `Restart()` แต่ **ไม่มีที่เรียกใช้** (เหลือแค่บรรทัดที่คอมเมนต์ไว้ใน `Update`) — เส้นทางตายคือ `ScreenManager.ReplaceScreen(new DeathScreen())`

## Screens

```mermaid
classDiagram
    class Screen {
        <<MonoGame.Extended.Screens>>
        +bool UpdateWhenInactive
        +bool DrawWhenInactive
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }
    class MainMenuScreen {
        +StartGame()
        +OpenSettings()
        +OpenCredits()
        +ExitGame()
    }
    class SettingsScreen
    class CreditsScreen
    class DeathScreen {
        +RandomizeTip()
        +ReturnToMenu()
    }
    class WinScreen
    class GameplayScreen {
        +bool UpdateWhenInactive = true
        +bool DrawWhenInactive = true
        +TogglePause()
        +ReturnToMenu()
    }
    class DoorPeekingScreen {
        <<draft>>
    }
    class SpinTheValveScreen {
        +bool MinigameActive
        +bool MinigameVisible
    }
    class ConnectTheDotsScreen {
        +bool MinigameActive
        +AddDot(int, int, Color)
        +ScreenToGrid(Vector2) Point
        +TryExtendPath(WirePath, Point)
        +GetCellTopLeft(Point) Vector2
        +GetCellTopCenter(Point) Vector2
    }
    class SkillCheckScreen {
        <<dead code>>
    }
    class PauseScreen {
        <<dead code>>
    }

    Screen <|-- MainMenuScreen
    Screen <|-- SettingsScreen
    Screen <|-- CreditsScreen
    Screen <|-- DeathScreen
    Screen <|-- WinScreen
    Screen <|-- GameplayScreen
    Screen <|-- DoorPeekingScreen
    Screen <|-- SpinTheValveScreen
    Screen <|-- ConnectTheDotsScreen
    Screen <|-- SkillCheckScreen
    Screen <|-- PauseScreen

    GameplayScreen --> GameManager
    MainMenuScreen ..> GameplayScreen
    MainMenuScreen ..> SettingsScreen
    MainMenuScreen ..> CreditsScreen
    DeathScreen ..> MainMenuScreen
    WinScreen ..> MainMenuScreen
    GameplayScreen ..> MainMenuScreen

    class DotsGenerator {
        +PlotMap()
        +TryGenerateDotsMap() GeneratedDotsMap
    }
    class GeneratedDotsMap {
        +int Columns
        +int Rows
        +Dictionary~int,(Point,Point)~ Dots
        +Dictionary~int,int~ PathLengths
    }
    class GridCell {
        +Color Color
        +Point Position
        +bool IsEndpoint
    }
    class WirePath {
        +Color Color
        +List~Point~ Points
        +bool IsComplete
    }

    ConnectTheDotsScreen --> DotsGenerator
    DotsGenerator --> GeneratedDotsMap
    ConnectTheDotsScreen --> GridCell
    ConnectTheDotsScreen --> WirePath
```

> `SkillCheckScreen` และ `PauseScreen` อยู่ในโฟลเดอร์ `Scraped/` — ไม่มีที่ไหนเรียกใช้แล้ว (ถูกแทนที่ด้วย `SpinTheValveScreen` และ pause overlay ใน `GameplayScreen`)
> ทุกหน้าจอ UI สร้างด้วย **Gum** (`Panel` → `Dock(Dock.Fill)` → `AddToRoot()`) และประกอบคอนโทรลผ่าน `GumUIHelper` โดยวางตำแหน่งด้วยค่าคงที่ใน `Layout`

## Colliders, Highlight & Support

```mermaid
classDiagram
    class ICollisionActor {
        <<interface>>
        +int Id
        +CollisionShape2D Shape
    }
    class Wall
    class Trigger {
        +int Id
        +string Name
        +string Tag
        +bool Interactable
        +float Progress
        +TilemapTileLayer TileLayer
        +List~TilemapTileEntry~ Tiles
        +CompareTag(string) bool
    }
    class Collectible {
        +string Tag
    }
    class LightArea {
        +Light Light
        +bool On
        +bool IsFlickering
        +bool ScopoHasPassed
        +Update()
        +StartFlicker(float)
        +StartNyctoAlarm()
        +TurnOff()
        +TurnOn()
        +ScopoIncoming()
    }
    Wall ..|> ICollisionActor
    Trigger ..|> ICollisionActor
    Collectible ..|> ICollisionActor
    LightArea ..|> ICollisionActor

    class HighlightSettings {
        +float ScanSpeed
        +float ScanWidth
        +Color ScanColor
        +float PulseSpeed
        +float PulseMin
        +Color PulseColor
        +float OutlineRadius
        +Color OutlineColor
    }
    class HighlightGroup {
        +TilemapTileLayer Layer
        +HighlightSettings Settings
        +List~TilemapTileEntry~ FocusTiles
        +List~TilemapTileEntry~ ScanTiles
        +List~TilemapTileEntry~ PulseTiles
    }
    TileHighlightRenderer --> HighlightGroup
    HighlightGroup --> HighlightSettings

    class Time {
        <<static>>
        +float DeltaTime$
        +TimeSpan ElapsedTime$
        +float TimeScale$
        +float TimeDilation$
        +List~Timer~ Timers$
        +Update(GameTime)$
        +AddTimer(float, string) Timer$
        +AddTimer(Action, float, string) Timer$
        +AddTimer(Action, int, int, string) Timer$
        +GetTimer(string) Timer$
        +TimerExist(string) bool$
        +RemoveTimer(Timer)$
        +RemoveTimer(string)$
        +Purge()$
    }
    class Timer {
        +string Name
        +float TimeLeft
        +bool Active
        +bool Repeat
        +bool Random
        +(int,int) MinMax
        +Update()
        +Pause()
        +Resume()
        +ToggleActive()
        +Reset()
        +event OnTimerCompleted
    }
    class CameraShaker {
        +Vector2 ShakeOffset
        +Shake(float, float)
        +Update(GameTime)
    }
    class SpawnPoint {
        +string Name
        +Vector2 Position
    }
    class VignetteOverlay {
        +Vector2 Center
        +Vector2 Scale
        +float Softness
        +Color Color
        +float CameraZoom
        +Render()
    }
    class BottomFadeOverlay {
        +Render()
    }

    Time --> Timer
    UIManager --> VignetteOverlay
    UIManager --> BottomFadeOverlay
    GameManager --> CameraShaker
    SceneBuilder --> SpawnPoint
```

## Data & Persistence

```mermaid
classDiagram
    class GameData {
        +Dictionary~string,SceneData~ Scenes
    }
    class SceneData {
        +List~TriggerData~ Triggers
        +List~LightData~ Lights
    }
    class TriggerData {
        +int Id
        +float Progress
        +bool Interactable
    }
    class LightData {
        +int Id
        +bool On
    }
    class TutorialData {
        +List~TutorialStep~ Steps
    }
    class TutorialStep {
        +string Text
        +string CompleteOn
    }
    class TutorialDataReader {
        <<JsonContentTypeReader~TutorialData~>>
    }

    GameData --> SceneData
    SceneData --> TriggerData
    SceneData --> LightData
    TutorialData --> TutorialStep
    TutorialDataReader --> TutorialData
    SaveManager --> GameData
    TutorialManager --> TutorialData
```

> `SaveManager.SaveScene` ข้าม trigger ที่ `Tag == "door"` (ประตูไม่ถูกบันทึกสถานะ) และจัดเก็บเป็น JSON ที่ `%APPDATA%\Imaginophobia\save.json`

## Enums

```mermaid
classDiagram
    class SanityState {
        <<enumeration>>
        Normal = 85
        Anxious = 40
        Insane = 0
    }
    class CharacterAction {
        <<enumeration>>
        Repairing
        Checking
        Peeking
        ReadyToHide
        Hiding
        Walking
        Idle
    }
    class Scene {
        <<enumeration>>
        tilemap_electrical_room
        tilemap_electrical_room_2
        tilemap_sewer
    }
    class SoundEffectName {
        <<enumeration>>
        DoorClose
        DoorOpen
        DoorStuck
        LockerClose
        LockerOpen
        PipeComplete
        PipeFail
        PipeRepairing
        PipeSuccess
        ElecComplete
        ElecFail
        ElecRepairing
        ElecSuccess
        ScopoIndicator
        PhobiaHit
        HeartBeat
        Painting
        LightBreak
        Darkness
        ScreenOff
        ScreenOn
        FlashlightToggle
        LightFlickerLong
        LightFlickerShort1
        LightFlickerShort2
        LightFlickerShort3
    }
    class BackgroundSound {
        <<enumeration>>
        AmbElectric
        AmbSewer
        BgmMainMenu
        BgmDeathScreen
        BgmWinScreen
    }
    class GameState {
        <<enumeration>>
        Menu
        Gameplay
        Lose
        Win
        Pause
    }
    class BlinkPhase {
        <<enumeration>>
        Open
        Closing
        Closed
        Opening
    }
    class VignettePhase {
        <<enumeration>>
        StopNyctophobia
        StartNyctophobia
    }
```

## หมายเหตุ

- `Screen` มาจากไลบรารี **MonoGame.Extended.Screens** · `ICollisionActor` มาจาก **MonoGame.Extended.Collisions** · `Light` / `PointLight` / `Spotlight` / `PenumbraComponent` มาจาก **Penumbra** · `Keyframe`/`SpriteSheet` ของ animation มาจาก **MonoGame.Extended.Graphics**
- คลาสที่ประกาศในโปรเจกต์ทั้งหมด: `GameObject`, `Player`, `AudioSource`, `Autophobia`, `Scopophobia`, `Nyctophobia`, `Entamaphobia`, `ISpatialSoundApplicable`, manager ทั้งหมด, `Screen` ทุกตัวในรายการ, `Trigger`/`Wall`/`Collectible`/`LightArea`, `HighlightGroup`/`HighlightSettings`, `Time`/`Timer`/`CameraShaker`/`Layout`/`GumUIHelper`, `SpawnPoint`, `VignetteOverlay`/`BottomFadeOverlay`, `DotsGenerator`/`GeneratedDotsMap`/`GridCell`/`WirePath`, data class ทั้งหมดใน `Scripts/Datas/`
- enum `SanityState`, `Scene`, `SoundEffectName`, `BackgroundSound`, `GameState`, `VignettePhase` อยู่ใน `Scripts/Enums/` · `CharacterAction` อยู่ที่ `Scripts/CharacterAction.cs` โดยตรง · `BlinkPhase` ประกาศใน namespace `Imaginophobia.Scripts.Enums` — ต่างจาก enum ตัวอื่นที่อยู่ใน `Imaginophobia`
- `GameState` (Menu / Gameplay / Lose / Win / Pause) **ยังไม่มีที่ใช้** — สถานะเกมตอนนี้สื่อผ่าน "หน้าจอที่ active" ของ `ScreenManager` แทน
- `SoundEffectName` มี 26 ค่า แต่ยังมี 2 ค่าที่ "โหลดไว้แต่ไม่เคยเล่น": `DoorOpen` และ `PipeSuccess` (นับจากการเรียก `PlaySFX` ในโค้ด) — `Painting` ผูกกับไฟล์ `sfx_panting.mp3` (ชื่อ enum ไม่ตรงกับไฟล์ ซึ่งชื่อไฟล์ก็พิมพ์ผิดอยู่แล้ว)
- `ObjectiveManager`, `SaveManager`, `InputManager`, `Time`, `Layout`, `GumUIHelper` เป็น static class ทั้งหมด — ไม่มี instance และไม่ถูก inject
- `Nyctophobia` / `Entamaphobia` ยังไม่มี asset และยังไม่ถูกผูกเข้า spawn timer (ดู [System Design § 14](01-system-design.md#14-known-issues--technical-debt))

## Related Documents

- System Design: [System Design](01-system-design.md)
- Data Schema: [Data Schema](03-data-schema.md)
- Mechanics (phobia behavior): [Mechanics](../01_GDD/03-mechanics.md)
