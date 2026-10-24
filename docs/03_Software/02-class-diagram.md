---
type: software-class-diagram
version: 2.0
date: 2026-10-24
---

# Imaginophobia — Class Diagram

**Version:** 2.0 | **Last Updated:** 2026-10-24

> สร้างจากโค้ดจริงใน `src/Imaginophonia/Imaginophonia/`
> ไฟล์นี้เดิมอยู่ใน `docs/01_GDD/04-class-diagram.md` — ย้ายมาไว้ในหมวด Software Design เพราะเป็นงานออกแบบซอฟต์แวร์
> `ICollisionActor` และ `Screen` มาจากไลบรารี MonoGame.Extended ไม่ได้ประกาศในโปรเจกต์

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
        +float MoveSpeed = 250
        +float Sanity = 100
        +float Battery = 100
        +SanityState SanityState
        +CharacterAction CurrentAction
        +bool CanInteract
        +bool CanRepair
        +bool Blink
        +bool Nyctophobia
        +Timer CooldownTimer
        +event SanityStateChanged
        +Update()
        +Draw()
        +ToggleHide()
        +ToggleRepair()
        +ToggleFlashLight()
        +CheckSanityStage()
        +Reset()
    }

    class AudioSource {
        +SoundEffectInstance AudioPlayer
        +UpdateSpatialAudio(Player)
        +PlayLoop(SoundEffect, float)
        +PlayOneShot(SoundEffect, float)
    }

    class Autophobia {
        +float MoveSpeed = 400
        +bool AllowMovement
        +Update()
        +Draw()
    }

    class Scopophobia {
        +float MoveSpeed = 2600
        +Update()
        +Draw()
        +OnDestroyed()
    }

    class IAudioApplicable {
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
    Player ..|> IAudioApplicable
    Player ..|> ICollisionActor
    AudioSource ..|> IDisposable
    Autophobia ..|> IAudioApplicable
    Autophobia ..|> ICollisionActor
    Scopophobia ..|> IAudioApplicable
    Scopophobia ..|> ICollisionActor
```

## Managers

```mermaid
classDiagram
    class GameManager {
        +Update(GameTime)
        +Draw(GameTime)
        -Restart()
        -OnDoorInteracted(string)
        -OnSceneLoaded(Vector2)
        -OnEnemySpawned(ICollisionActor)
        -OnEnemyRemoved(ICollisionActor)
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
    }
    class AudioManager {
        +AudioManager Instance$
        +float AmbVolume
        +float SoundEffectVolume
        +bool IsMuted
        +Update(Player)
        +PlaySFX(Sound, bool, bool)
        +Play3DSFX(Vector2, Sound, float, float) AudioSource
        +PlayStepsSFX(Vector2, float, float)
        +PlayAmbiance(string)
        +StopSFX(Sound)
        +ToggleMute()
    }
    class EnemyManager {
        +List~GameObject~ Enemies
        +Update(Rectangle)
        +Draw()
        +SpawnEnemy()
        +ClearEnemies()
        +HasActiveScopophobia() bool
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
    }
    class TextManager {
        +Update()
        +Draw()
        +ShowMessage(string, float)
        +InputTutorial(string)
    }
    class TutorialManager {
        +GetText() string
    }
    class ObjectiveManager {
        <<static>>
        +int TargetValves = 5$
        +bool AllValvesRepaired$
        +int RepairedValveCount$
        +Complete(string)$
        +IsComplete(string) bool$
        +RepairValve(int)$
        +RepairPanel(int)$
        +ResetValves()$
        +ResetPanels()$
        +event Completed$
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
        +LoadScene(Tilemap)
        +Update()
        +Render(OrthographicCamera)
        +Draw()
    }

    GameManager --> SceneManager
    GameManager --> CollisionManager
    GameManager --> LightManager
    GameManager --> EnemyManager
    GameManager --> UIManager
    GameManager --> Player
    SceneManager --> SceneBuilder
    SceneManager --> TileHighlightRenderer
    SceneBuilder --> CollisionManager
    SceneBuilder --> LightManager
    EnemyManager --> Autophobia
    EnemyManager --> Scopophobia
    AudioManager --> AudioSource
    UIManager --> TextManager
    UIManager --> TutorialManager
    TutorialManager --> ObjectiveManager
    SaveManager --> Trigger
    SaveManager --> LightArea
```

## Screens (Minigames)

```mermaid
classDiagram
    class Screen {
        <<MonoGame.Extended.Screens>>
        +Update(GameTime)
        +Draw(GameTime)
    }
    class GameplayScreen {
        +bool UpdateWhenInactive
        +bool DrawWhenInactive
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
        +bool SkillCheckIsActive
    }

    Screen <|-- GameplayScreen
    Screen <|-- SpinTheValveScreen
    Screen <|-- ConnectTheDotsScreen
    Screen <|-- SkillCheckScreen

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

> `SkillCheckScreen` ไม่มีที่ไหนเรียกใช้แล้ว — เก็บไว้เป็น reference เท่านั้น

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
        +Update()
        +StartFlicker(float)
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
        +List~Timer~ Timers$
        +Update(GameTime)$
        +AddTimer(float) Timer$
        +AddTimer(Action, float, string) Timer$
        +RemoveTimer(Timer)$
    }
    class Timer {
        +string Name
        +float TimeLeft
        +bool Active
        +bool Repeat
        +Update()
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

## Enums

```mermaid
classDiagram
    class SanityState {
        <<enumeration>>
        Normal = 70
        Anxious = 30
        Insane = 0
    }
    class CharacterAction {
        <<enumeration>>
        Repairing
        Checking
        ReadyToHide
        Hiding
        Walking
        Sneaking
        Idle
    }
    class Scene {
        <<enumeration>>
        tilemap_electrical_room
        tilemap_electrical_room_2
        tilemap_sewer
    }
    class Sound {
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
        AutoIndicator
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

- `Screen` มาจากไลบรารี **MonoGame.Extended.Screens** · `ICollisionActor` มาจาก **MonoGame.Extended.Collisions** · `Light` / `PointLight` / `Spotlight` / `PenumbraComponent` มาจาก **Penumbra**
- `SanityState`, `Scene`, `Sound`, `BlinkPhase`, `VignettePhase` อยู่ใน `Scripts/Enums/` แต่ `CharacterAction` อยู่ที่ `Scripts/CharacterAction.cs` โดยตรง
- `BlinkPhase` ประกาศใน namespace `Imaginophobia.Scripts.Enums` — ต่างจาก enum ตัวอื่นที่อยู่ใน `Imaginophobia`
- `Nyctophobia`, `Claustrophobia` และ `Entamaphobia` **ยังไม่มีคลาส** — Nyctophobia/Claustrophobia ทำงานผ่าน `LightArea` + `Player` + `UIManager` ส่วน Entamaphobia ยังเป็นข้อเสนอ (ดู [Mechanics](../01_GDD/03-mechanics.md#entamaphobia--รายละเอียดคอนเซปต์))
- `ObjectiveManager`, `SaveManager`, `InputManager`, `Time` เป็น static class ทั้งหมด — ไม่มี instance และไม่ถูก inject
- `AudioManager` เป็น singleton ที่สร้างใน constructor ของ `GameManager` (ตัวแปร local ไม่ได้เก็บเป็น field)

## Related Documents

- System Design: [System Design](01-system-design.md)
- Data Schema: [Data Schema](03-data-schema.md)
- Mechanics (phobia behavior): [Mechanics](../01_GDD/03-mechanics.md)
