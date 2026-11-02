---
type: software-class-diagram
version: 4.0
date: 2026-11-02
---

# Imaginophobia — Class Diagram

**Version:** 4.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

> สร้างจากโค้ดจริงใน `src/Imaginophobia/Imaginophobia/` (HEAD `3a300ec`, 2026-11-02) — **71 types · 909 members · 59 ค่า enum · 87 relationships**
> ไฟล์นี้เดิมอยู่ใน `docs/01_GDD/04-class-diagram.md` — ย้ายมาไว้ในหมวด Software Design เพราะเป็นงานออกแบบซอฟต์แวร์
> คลาสที่มาจากไลบรารี (`Game`, `Screen`, `ICollisionActor`, `GameObject` ของ MonoGame.Extended, `Penumbra`) ถูกทำเครื่องหมาย `<<external>>` และ `ISpatialSoundApplicable` ประกาศในโปรเจกต์เองที่ `Scripts/ISpatialSoundApplicable.cs`
> ทุกไดอะแกรมในไฟล์นี้ผ่านการ parse ด้วย Mermaid parser จริง

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
        +Destroy(GameObject)$
        +Destroy(GameObject, float)$
    }

    class Player {
        +CollisionShape2D Shape
        +LineSegment2D EyeSight
        +AudioListener Listener
        +PointLight ScotopicLight
        +Spotlight FlashLight
        +float MoveSpeed = 250
        +float Sanity = 100
        +float Battery = 100
        +float HideTime = 10
        +float HideCooldown = 3
        +float FocusLevel
        +Vector2 CameraTarget
        +Vector2 PreviousPos
        +Trigger CollidingTrigger
        +SanityState SanityState
        +CharacterAction CurrentAction
        +bool Blink
        +bool Nyctophobia
        +event SanityStateChanged
        +Update()
        +Draw()
        +CollideWithWallMove(Vector2)
        +ToggleHide()
        +ToggleFlashLight()
        +ToggleRepairing()
        +TogglePeek()
        +ToggleReading()
        +CheckSanityStage()
        +Dead()
        +LoadScenePosition(Vector2)
        +Reset()
        +Dispose()
    }

    class AudioSource {
        +SoundEffectInstance AudioPlayer
        +bool IsDisposed
        +Update()
        +Draw()
        +PlayLoop(SoundEffect, float)
        +PlayOneShot(SoundEffect, float)
        +UpdateSpatialAudio(Player)
        +FadeVolumnOut(float)
        +Dispose()
    }

    class CameraShaker {
        +Vector2 ShakeOffset
        +Shake(float, float)
        +Update(GameTime)
    }

    class Autophobia {
        +float MoveSpeed = 400
        +int Id = 666
        +Vector2 Direction
        +CollisionShape2D Shape
        +bool AllowMovement
        +Update()
        +Draw()
    }

    class Scopophobia {
        +float MoveSpeed = 5000
        +int Id = 555
        +Vector2 Direction
        +CollisionShape2D Shape
        +int LightPassedCount
        +Update()
        +Draw()
        +OnDestroyed()
    }

    class Nyctophobia {
        <<marker>>
    }

    class Entamaphobia {
        +Update()
        +OnDestroyed()
    }

    class ISpatialSoundApplicable {
        <<interface>>
        +float MoveSpeed
        +Vector2 Velocity
        +Vector2 Direction
    }
    class ICollisionActor {
        <<external>>
        +int Id
        +CollisionShape2D Shape
    }
    class IDisposable {
        <<external>>
        +Dispose()
    }

    GameObject <|-- Player
    GameObject <|-- Autophobia
    GameObject <|-- Scopophobia
    GameObject <|-- Nyctophobia
    GameObject <|-- Entamaphobia
    GameObject <|-- AudioSource
    Player ..|> ISpatialSoundApplicable
    Player ..|> ICollisionActor
    Player ..|> IDisposable
    AudioSource ..|> IDisposable
    Autophobia ..|> ISpatialSoundApplicable
    Autophobia ..|> ICollisionActor
    Scopophobia ..|> ISpatialSoundApplicable
    Scopophobia ..|> ICollisionActor
    Scopophobia --> AudioSource : child
    Entamaphobia --> Player
```

> `Nyctophobia` เป็น **marker class เปล่า ๆ** — ใช้ให้ `EnemyManager.HasActiveNyctophobia()` ตรวจได้ว่า "ความมืดกำลังเล่นงานอยู่" พฤติกรรมจริงอยู่ใน `Player.Nyctophobia` + `LightArea.StartNyctoAlarm()` + `UIManager`
> `Entamaphobia` ผูกกับ `Player` และกับ `Trigger` ของประตู (`_door.Progress = _stage`) — ตัวมันเองไม่ implement `ICollisionActor` เพราะไม่ต้องชนกับผู้เล่น
> `CameraShaker` ถูกประกาศในกลุ่มนี้เพราะเป็นผู้ช่วยของกล้องใน gameplay แต่ **ไม่มีที่ไหนเรียกใช้** — บรรทัดเรียกใน `GameManager` คอมเมนต์ไว้

## Managers

```mermaid
classDiagram
    class MainGame {
        +MainGame Instance$
        +GraphicsDeviceManager Graphics$
        +GraphicsDevice GraphicsDevice$
        +SpriteBatch SpriteBatch$
        +ContentManager Content$
        +ModifiedScreenManager ScreenManager$
        +GameState GameState$
        +bool Loading$
        +Initialize()
        +LoadContent()
        +Update(GameTime)
        +Draw(GameTime)
        +UnloadContent()
        +OnExiting(object, ExitingEventArgs)
    }

    class ModifiedScreenManager {
        +float Saturation
        +Update(GameTime)
        +Draw(GameTime)
        +DrawFade()
        +DrawGraded()
        +FadeIn(float)
        +FadeOut(float)
        +PulseFade(float, float)
        +ShowScreen(Screen, float)
        +ShowScreen(Screen, float, float, float)
        +CloseScreen(float)
        +ClearScreens(Screen, float, float, float)
        +ReplaceScreen(Screen, float, float, float)
    }

    class GameManager {
        +Update(GameTime)
        +Draw(GameTime)
        +Dispose()
    }

    class SceneManager {
        +GetActiveScene()$ SceneBuilder
        +event SceneLoaded
        +LoadScene(string, OrthographicCamera, CollisionManager, LightManager, bool)
        +Update(CollisionManager)
        +Draw(OrthographicCamera)
        +Render(OrthographicCamera)
        +Dispose()
    }

    class SceneBuilder {
        +string Name
        +Tilemap TileMap
        +List~SpawnPoint~ PlayerSpawnPoints
        +List~Trigger~ Triggers
    }

    class CollisionManager {
        +Trigger FocusedTrigger
        +event DoorInteracted
        +Update()
        +AddCollider(ICollisionActor)
        +AddCollider(ICollisionActor, string)
        +RemoveCollider(ICollisionActor)
        +ClearColliders()
        +Dispose()
    }

    class LightManager {
        +PenumbraComponent Penumbra$
        +List~LightArea~ LightAreas
        +AllLightsAreOff()
        +Update(Matrix)
        +AddLight(Light)
        +AddLight(LightArea)
        +ClearLights()
        +ScopoAlarmAll()
        +NyctoAlarmAll()
        +CleanUpScopo()
        +Dispose()
    }

    class AudioManager {
        +AudioManager Instance$
        +bool IsMuted
        +bool IsDisposed
        +float BgsVolume
        +float SoundEffectVolume
        +Update(Player)
        +PlaySFX(SoundEffectName, bool, bool, bool, float)
        +Play3DSFX(Vector2, SoundEffectName, float, float, float) AudioSource
        +PlayBackgroundSound(BackgroundSound)
        +PlayStepsSFX(Vector2, float, float)
        +PlayLightFlickerSFX(Vector2, float, float)
        +StopSFX(SoundEffectName)
        +ClearSFXs()
        +ClearSFXs(string[])
        +ChangeSFXVolume(SoundEffectName, float)
        +ChangeMediaVolume(bool, float)
        +FadeSFXVolumeOut(SoundEffectName, float)
        +PauseAudio()
        +ResumeAudio()
        +MuteAudio()
        +UnmuteAudio()
        +ToggleMute()
        +Dispose()
    }

    class EnemyManager {
        +IReadOnlyList~GameObject~ Enemies
        +event EnemySpawned
        +event EnemyRemoved
        +HasActiveScopophobia()
        +HasActiveNyctophobia()
        +HasActiveEntamaphobia()
        +Update(Rectangle)
        +Draw()
        +ForceSpawnScopophobia()
        +SpawnScopophobia()
        +SpawnAutophobia()
        +SpawnNyctophobia()
        +SpawnEntamaphobia()
        +ClearEnemies()
        +Dispose()
    }

    class UIManager {
        +Update()
        +Draw()
        +BlackScreen()
        +NyctoClose()
        +NyctoOpen()
        +ShowMessage(string, float)
        +BlinkClose()
        +BlinkOpen()
        +ZoomVignette()
        +BackToStageScale()
        +Dispose()
    }

    class TextManager {
        +Update()
        +Draw()
        +InputTutorial(string)
        +ShowMessage(string, float)
    }

    class ObjectiveManager {
        +event Completed$
        +int TargetValves$
        +int RepairedValveCount$
        +bool AllValvesRepaired$
        +IsComplete(string)$
        +Complete(string)$
        +IsValveRepaired(int)$
        +IsPanelRepaired(int)$
        +RepairValve(int)$
        +RepairPanel(int)$
        +ResetValves()$
        +ResetPanels()$
        +Reset()$
    }

    class TutorialManager {
        +OnCompleted(string)
        +GetText()
        +Load()$ TutorialData
        +Dispose()
    }

    class SaveManager {
        +SaveScene(string, List~Trigger~, List~LightArea~)$
        +ApplyToScene(string, List~Trigger~, List~LightArea~)$
        +TurnAllLightsOn()$
        +AllSewerLightsAreOn()$ bool
        +ResetScene(string)$
        +Delete()$
    }

    class InputManager {
        +Vector2 Direction$
        +Update()$
        +KeyUp(Keys)$
        +HoldKey(Keys)$
        +PressedKey(Keys)$
        +ReleasedKey(Keys)$
    }

    MainGame --> ModifiedScreenManager
    MainGame --> AudioManager
    MainGame --> GameState
    ModifiedScreenManager --|> ScreenManagerEx
    GameManager --> SceneManager
    GameManager --> CollisionManager
    GameManager --> LightManager
    GameManager --> EnemyManager
    GameManager --> UIManager
    GameManager --> Player
    SceneManager --> SceneBuilder
    SceneBuilder --> CollisionManager
    SceneBuilder --> LightManager
    EnemyManager --> Autophobia
    EnemyManager --> Scopophobia
    EnemyManager --> Nyctophobia
    EnemyManager --> Entamaphobia
    EnemyManager --> ObjectiveManager
    CollisionManager --> SaveManager
    CollisionManager --> ObjectiveManager
    CollisionManager --> Player
    UIManager --> TextManager
    UIManager --> TutorialManager
    UIManager --> VignetteOverlay
    UIManager --> BottomFadeOverlay
    TutorialManager --> ObjectiveManager
    ObjectiveManager --> SaveManager
    AudioManager --> AudioSource
    AudioManager --> ISpatialSoundApplicable
    class ScreenManagerEx {
        <<external>>
    }
```

> `MainGame` เป็นเจ้าของทุกอย่างในระดับเกม · `GameManager` เป็นเจ้าของทุกอย่างในระดับ gameplay (สร้างใน `GameplayScreen.Initialize`)
> `AudioManager` เป็น singleton ที่ `MainGame.Initialize()` สร้าง — `SfxByName` (33 ค่า) และ `BgsByName` (5 ค่า) อยู่ในตัวมัน
> `ObjectiveManager` / `SaveManager` / `InputManager` เป็น **static ทั้งคู่** — ไม่มี instance ให้ถือ
> `SaveManager` เป็น static ที่ `CollisionManager` และ `GameManager` เรียกใช้ตรง ๆ (เช็ค `AllSewerLightsAreOn` / `ResetScene` / `SaveScene`)

## Screens

```mermaid
classDiagram
    class Screen {
        <<external>>
        +Update(GameTime)
        +Draw(GameTime)
        +Initialize()
        +Dispose()
    }

    class MainMenuScreen {
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }

    class SettingsScreen {
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }

    class CreditsScreen {
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }

    class GameplayScreen {
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +UnloadContent()
        +Dispose()
    }

    class DeathScreen {
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }

    class WinScreen {
        +WinScreen(float)
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
        +OnActivated()
        +OnDeactivated()
        +Dispose()
    }

    class JumpScareScreen {
        +Update(GameTime)
        +Draw(GameTime)
    }

    class SpinTheValveScreen {
        +bool MinigameActive
        +bool MinigameVisible
        +Update(GameTime)
        +Draw(GameTime)
    }

    class ConnectTheDotsScreen {
        +bool MinigameActive
        +Update(GameTime)
        +Draw(GameTime)
        +CreateGrid()
        +AddDot(int, int, Color)
        +ScreenToGrid(Vector2) Point
        +TryExtendPath(WirePath, Point)
        +GetCellTopCenter(Point)
        +GetCellTopLeft(Point)
        +CleanUp()
    }

    class DoorPeekingScreen {
        +Initialize()
        +Update(GameTime)
        +Draw(GameTime)
    }

    class ClipboardScreen {
        +Vector2 ClipboardPos
        +int CurrentPage
        +int MaxPage
        +Update(GameTime)
        +Draw(GameTime)
        +Dispose()
    }

    Screen <|-- MainMenuScreen
    Screen <|-- SettingsScreen
    Screen <|-- CreditsScreen
    Screen <|-- GameplayScreen
    Screen <|-- DeathScreen
    Screen <|-- WinScreen
    Screen <|-- JumpScareScreen
    Screen <|-- SpinTheValveScreen
    Screen <|-- ConnectTheDotsScreen
    Screen <|-- DoorPeekingScreen
    Screen <|-- ClipboardScreen
    MainMenuScreen --> EyeField
    GameplayScreen --> GameManager
    GameplayScreen --> GumUIHelper
    DeathScreen --> GumUIHelper
    WinScreen --> GumUIHelper
    MainMenuScreen --> GumUIHelper
    CreditsScreen --> GumUIHelper
    SettingsScreen --> GumUIHelper
    SpinTheValveScreen --> Trigger
    ConnectTheDotsScreen --> DotsGenerator
    ConnectTheDotsScreen --> WirePath
    DoorPeekingScreen --> Trigger
    ClipboardScreen --> Trigger
```

> หน้าจอทั้งหมดสืบทอด `Screen` ของ MonoGame.Extended และถูกเปิดผ่าน `ModifiedScreenManager` (มี fade ทุกครั้ง)
> `GameplayScreen` ตั้ง `UpdateWhenInactive` / `DrawWhenInactive = true` — gameplay จึงยังเดินอยู่เบื้องหลังหน้าจอที่ push ทับ (เช่น `DoorPeekingScreen`, `ClipboardScreen`, `JumpScareScreen`)
> `SpinTheValveScreen` / `ConnectTheDotsScreen` / `DoorPeekingScreen` / `ClipboardScreen` รับ `Player` และ/หรือ `Trigger` เข้ามาในคอนสตรัคเตอร์และเขียน progress กลับลง trigger ตัวนั้น
> ไม่มี `SkillCheckScreen` หรือ `PauseScreen` ในโปรเจกต์แล้ว (โฟลเดอร์ `Scraped/` ถูกนำออกทั้งโฟลเดอร์) — pause กลายเป็น overlay ที่อยู่ใน `GameplayScreen`

## Colliders, Highlight & Support

```mermaid
classDiagram
    class Trigger {
        +int Id
        +CollisionShape2D Shape
        +string Name
        +string Tag
        +bool Interactable
        +float Progress
        +TilemapTileLayer TileLayer
        +List~TilemapTileEntry~ Tiles
        +CompareTag(string)
    }

    class Wall {
        +int Id
        +CollisionShape2D Shape
    }

    class LightArea {
        +CollisionShape2D Shape
        +int Id
        +string Tag
        +Light Light
        +bool IsFlickering
        +bool On
        +bool ScopoHasPassed
        +Update()
        +ScopoIncoming()
        +StartNyctoAlarm()
        +StartFlicker(float)
        +TurnOff()
        +TurnOn()
    }

    class HighlightGroup {
        +TilemapTileLayer Layer
        +HighlightSettings Settings
        +List~TilemapTileEntry~ FocusTiles
        +List~TilemapTileEntry~ ScanTiles
        +List~TilemapTileEntry~ PulseTiles
    }

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
        +Dispose()
    }

    class SpawnPoint {
        +string Name
        +Vector2 Position
    }

    class Layout {
        +int ScreenWidth$
        +int ScreenHeight$
        +int PanelX$
        +float TitleFontScale$
        +float MenuFontScale$
        +int MenuButtonX$
        +float BackButtonFontScale$
        +int SliderX$
        +int SliderWidth$
        +Rectangle FullScreen$
        +Rectangle Panel$
    }

    class GumUIHelper {
        +CreateTitle(string, bool)$
        +CreateText(string, float, int, int, int, int, bool, int)$
        +CreateTextButton(string, float, int, int, int, int, Action, bool)$
        +CreatePauseButton(Action)$
        +CreateSlider(int, int)$
        +CreateValueLabel(int, int, float)$
        +CreateDimPanel()$
        +AddSettingsPanel(ContainerRuntime, bool)$
    }

    class Time {
        +float DeltaTime$
        +float UnscaledDeltaTime$
        +float TimeScale$
        +List~Timer~ Timers$
        +Update(GameTime)$
        +AddTimer(float, string)$
        +AddTimer(Action, float, string)$
        +AddTimer(Action, int, int, string)$
        +RemoveTimer(Timer)$
        +RemoveTimer(string)$
        +TimerExist(string)$
        +GetTimer(string)$
        +Purge()$
    }

    class Timer {
        +string Name
        +float TimeLeft
        +bool Active
        +bool Repeat
        +bool Random
        +Update(float)
        +ToggleActive()
        +SetInActive()
        +Reset()
        +Pause()
        +Resume()
    }

    class VignetteOverlay {
        +Vector2 Center
        +Vector2 Scale
        +float Softness
        +Color Color
        +string Shape
        +float CameraZoom
        +Render()
        +Dispose()
    }

    class BottomFadeOverlay {
        +Render()
        +Dispose()
    }

    Trigger ..|> ICollisionActor
    Wall ..|> ICollisionActor
    LightArea ..|> ICollisionActor
    TileHighlightRenderer --> HighlightGroup
    HighlightGroup --> HighlightSettings
    Time --> Timer
    Trigger --> TilemapTileLayer
    LightArea --> Light
```

> `Trigger` / `Wall` / `LightArea` ทั้งสามเป็น `ICollisionActor` ที่ถูกใส่ลง `CollisionManager` คนละ layer (`triggers` / `walls` / `lights`)
> `TileHighlightRenderer` ถือ 6 กลุ่ม (`ValveGroup`, `LockerGroup`, `PanelGroup`, `DoorGroup`, `ClipboardGroup`, `LadderGroup`) และวาดผ่าน render target 2 ตัว (mask + composite)
> `Layout` และ `GumUIHelper` เป็น static helper ที่หน้าจอ Gum ใช้ร่วมกัน — `Time` / `Timer` เป็นระบบเวลากลางที่ทุก subsystem เรียกใช้

## Menu Eyes & Puzzle Data

```mermaid
classDiagram
    class EyeField {
        +int EyeCount
        +float MinEyeWidth
        +float MaxEyeWidth
        +float RotationRange
        +float EyeSpacing
        +float TwitchMagnitude
        +float GazeFollowSpeed
        +bool FollowMouse
        +bool LookAtClick
        +bool RandomTwitch
        +bool TwitchOnClick
        +bool SynchronizedTwitch
        +Rectangle DarkArea
        +Update()
        +Draw()
        +PlaceEyes()
        +TwitchAll()
    }

    class EyeInstance {
        +EyeSpriteSet SpriteSet
        +Vector2 Center
        +float Scale
        +float Rotation
        +float FootprintRadius
        +float PupilRadius
        +float NextTwitch
        +EyeShape Shape
        +EyeMotion Motion
    }

    class EyeSpriteSet {
        +Texture2D Sclera
        +Texture2D Pupil
        +Texture2D Eyelid
        +Vector2 ScleraPivot
        +Vector2 PupilPivot
        +Rectangle ScleraBounds
        +Rectangle PupilBounds
        +Rectangle EyelidBounds
        +ScleraField Field
    }

    class EyeShape {
        +Vector2 Center
        +float HalfWidth
        +float HalfHeight
        +float Rotation
        +ClampPupil(Vector2, float)
        +ClearanceAt(Vector2)
        +PupilFits(Vector2, float)
        +MaxTravel(Vector2, Vector2, float)
    }

    class EyeMotion {
        +Vector2 PupilPosition
        +bool IsTwitching
        +SetGazeOffset(Vector2, float)
        +Twitch(Vector2, float, float)
        +CancelTwitch()
        +Update()
    }

    class ScleraField {
        +Vector2 Pivot
        +Vector2 DeepPoint
        +Sample(Vector2)
    }

    class DotsGenerator {
        +PlotMap()
        +TryGenerateDotsMap()
    }

    class GeneratedDotsMap {
        +int Columns
        +int Rows
        +Dictionary~int, DotsPair~ Dots
        +Dictionary~int, int~ PathLengths
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

    EyeField --> EyeInstance
    EyeInstance --> EyeSpriteSet
    EyeInstance --> EyeShape
    EyeInstance --> EyeMotion
    EyeSpriteSet --> ScleraField
    EyeShape --> ScleraField
    EyeMotion --> EyeShape
    DotsGenerator --> GeneratedDotsMap
    DotsGenerator ..> GridCell
    ConnectTheDotsScreen --> GridCell
```

> `EyeField` ใช้กับ `MainMenuScreen` เท่านั้น — วางดวงตาลงบนบริเวณมืดของ `UI/background_main.png` (หาโซนมืดด้วย integral image), ให้รูม่านตาจ้องเมาส์ และกระพริบเป็นจังหวะ
> `EyeShape` ใช้ signed-distance field (`ScleraField`) กันไม่ให้รูม่านตาหลุดออกนอกตาขาว — `ClampPupil` / `PupilFits` / `MaxTravel` เป็นตัวคุมการเคลื่อนไหว
> `ConnectTheDotsScreen` เป็นเจ้าของ `GridCell` 6×6, `WirePath` (เส้นที่ผู้เล่นลาก) และเรียก `DotsGenerator` สุ่มแผนใหม่ทุกรอบซ่อม

## Data & Persistence

```mermaid
classDiagram
    class GameData {
        +Dictionary~string, SceneData~ Scenes
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

    GameData --> SceneData
    SceneData --> TriggerData
    SceneData --> LightData
    TutorialData --> TutorialStep
    SaveManager ..> GameData
    TutorialManager ..> TutorialData
```

> `GameData` / `SceneData` / `TriggerData` / `LightData` คือโครงของ `%APPDATA%\Imaginophobia\save.json` (serialize ด้วย `System.Text.Json`)
> `TutorialData` / `TutorialStep` อ่านจาก `Content/Datas/tutorial.json` — ไม่ได้ผ่าน MGCB (copy ไป output ด้วย csproj แล้วอ่านด้วย `File.ReadAllText`)
> ทั้งสองชุดเป็น POCO ล้วน ไม่มี logic

## Enums

```mermaid
classDiagram
    class SanityState {
        <<enumeration>>
        Normal = 80
        Anxious = 40
        Insane = 0
    }

    class CharacterAction {
        <<enumeration>>
        Repairing
        Checking
        Reading
        Peeking
        Hiding
        Walking
        Idle
    }

    class GameState {
        <<enumeration>>
        Menu
        Gameplay
        Lose
        Win
        Pause
        Jumpscare
    }

    class Scene {
        <<enumeration>>
        tilemap_electrical_room
        tilemap_electrical_room_2
        tilemap_sewer
    }

    class BackgroundSound {
        <<enumeration>>
        AmbElectric
        AmbSewer
        BgmMainMenu
        BgmDeathScreen
        BgmWinScreen
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
        EntamaAdvance
        EntamaDecrease
        EntamaPresence
        EntamaJumpscare
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
        ClipboardPageAdvance
        ClipboardPageReverse
        ClipboardCheck
    }

    class VignettePhase {
        <<enumeration>>
        StopNyctophobia
        StartNyctophobia
    }

    Player --> SanityState
    Player --> CharacterAction
    MainGame --> GameState
    SceneManager --> Scene
    AudioManager --> SoundEffectName
    AudioManager --> BackgroundSound
    UIManager --> VignettePhase
    EnemyManager --> SanityState
    CollisionManager --> CharacterAction
```

> `SanityState` ใช้ค่า enum เป็น threshold จริง (80 / 40 / 0) · `SoundEffectName` มี **33 ค่า** (ไม่นับเสียงฝีเท้า 8 ไฟล์ที่โหลดเป็น array)
> `GameState` ถูกใช้เป็น static `MainGame.GameState` และมีผลกับ `Player.Update`, `GameManager.Update`, `CollisionManager` และ `ConnectTheDotsScreen`
> enum `BlinkPhase` ถูกลบออกจากโปรเจกต์แล้ว

## หมายเหตุ

- จำนวน type ทั้งหมด **71** (นับจาก `extract_csharp_types.py` บน `src/Imaginophobia/Imaginophobia/`) — รวมคลาสใน `Scripts/Eyes/` ที่ไม่ได้อยู่ในไดอะแกรมกลุ่มอื่น
- `PenumbraComponent` / `Light` / `Spotlight` / `PointLight` / `AudioListener` / `Screen` / `ICollisionActor` / `Game` / `Tilemap*` มาจากไลบรารี (MonoGame.Extended, MonoGame.Framework, Penumbra) — ไม่ได้ประกาศในโปรเจกต์
- static members ถูกทำเครื่องหมายด้วย `$` ต่อท้ายชื่อ (ตาม syntax ของ Mermaid)

## Related Documents

- System Design: [System Design](01-system-design.md)
- Data Schema (รายละเอียด enum / level data / save / tuning): [Data Schema](03-data-schema.md)
- Concept (ภาพรวมสถาปัตยกรรม): [Game Concept](../01_GDD/00-concept.md#7-system-architecture)
