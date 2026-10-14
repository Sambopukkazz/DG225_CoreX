---
type: software-class-diagram
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Class Diagram

**Version:** 1.0 | **Last Updated:** 2026-10-14

> สร้างจากโค้ดจริงใน `src/Imaginophonia/Imaginophonia/Scripts/`
> ไฟล์นี้เดิมอยู่ใน `docs/01_GDD/04-class-diagram.md` — ย้ายมาไว้ในหมวด Software Design เพราะเป็นงานออกแบบซอฟต์แวร์

## Core Hierarchy

```mermaid
classDiagram
    class GameObject {
        +string Name
        +string Tag
        +List~GameObject~ Children
        +Update()
        +Draw()
        +AddChild(GameObject)
        +RemoveChild(GameObject)
    }

    class Player {
        -Vector2 _position
        -float _speed
        +bool IsAlive
        +Update()
        +Draw()
        +Reset()
    }

    class AudioSource {
        +PlayLoop(SoundEffect, float)
        +PlayOneShot(SoundEffect, float)
        +UpdateSpatialAudio(Player)
    }

    class Autophobia {
        +Update()
        +Draw()
        -UpdateShape()
    }

    class Scopophobia {
        +Update()
        +Draw()
        -UpdateShape()
    }

    class IAudioApplicable {
        <<interface>>
    }
    class ICollisionActor {
        <<interface>>
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
    }
    class SceneManager {
        +GetActiveScene() SceneBuilder
        +Update(GameTime)
        +Draw(OrthographicCamera)
    }
    class SceneBuilder {
        +TileMap
        +List~SpawnPoint~ PlayerSpawnPoints
        +string Name
    }
    class CollisionManager {
        +Update()
        +AddCollider(ICollisionActor)
        +AddCollider(ICollisionActor, string)
        +RemoveCollider(ICollisionActor)
    }
    class LightManager {
        +Update(Matrix)
        +AddLight(Light)
        +AddLight(LightArea)
    }
    class AudioManager {
        +Update(Player)
        +Draw()
        +PlaySFX(Sound, bool, bool)
        +Play3DSFX(Vector2, Sound, float, float) AudioSource
        +PlayStepsSFX(Vector2, float, float)
        +PlayAmbiance(string)
    }
    class EnemyManager {
        +Update(Rectangle)
        +Draw()
        +SpawnEnemy()
    }
    class UIManager {
        +Update()
        +Draw()
    }
    class ObjectiveManager
    class DialogueManager {
        +Update()
    }
    class InputManager {
        <<static>>
        +Update()$
    }

    GameManager --> SceneManager
    GameManager --> CollisionManager
    GameManager --> LightManager
    GameManager --> AudioManager
    GameManager --> EnemyManager
    GameManager --> UIManager
    GameManager --> ObjectiveManager
    GameManager --> DialogueManager
    SceneManager --> SceneBuilder
    SceneBuilder --> CollisionManager
    SceneBuilder --> LightManager
    EnemyManager --> Autophobia
    EnemyManager --> Scopophobia
    AudioManager --> AudioSource
```

## Screens (Minigames)

```mermaid
classDiagram
    class Screen {
        <<MonoGame.Extended.Screens>>
        +Update(GameTime)
        +Draw(GameTime)
    }
    class GameplayScreen
    class SkillCheckScreen
    class ConnectTheDotsScreen {
        +AddDot(int, int, Color)
        +DrawLineSegment(Vector2, Vector2, Color, int)
        +GetCellCenter(Point) Vector2
    }
    class SpinTheValveScreen

    Screen <|-- GameplayScreen
    Screen <|-- SkillCheckScreen
    Screen <|-- ConnectTheDotsScreen
    Screen <|-- SpinTheValveScreen

    class DotsGenerator
    class GeneratedDotsMap
    class GridCell
    class WirePath

    ConnectTheDotsScreen --> DotsGenerator
    DotsGenerator --> GeneratedDotsMap
    GeneratedDotsMap --> GridCell
    ConnectTheDotsScreen --> WirePath
```

## Colliders & Support

```mermaid
classDiagram
    class ICollisionActor {
        <<interface>>
    }
    class Wall
    class Trigger
    class Collectible
    class LightArea {
        +Update()
    }
    Wall ..|> ICollisionActor
    Trigger ..|> ICollisionActor
    Collectible ..|> ICollisionActor
    LightArea ..|> ICollisionActor

    class Time {
        <<static>>
        +AddTimer(float) Timer$
        +AddTimer(Action, float) Timer$
        +RemoveTimer(Timer)$
    }
    class Timer {
        +Update()
        +Reset()
    }
    class CameraShaker {
        +Update(GameTime)
    }
    class SpawnPoint {
        +string Name
        +Vector2 Position
    }
    class VignetteOverlay
    class BottomFadeOverlay

    Time --> Timer
```

## Enums

```mermaid
classDiagram
    class MentalState {
        <<enumeration>>
        Normal = 75
        Anxious = 50
        Insane = 25
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
```

## หมายเหตุ

- `Screen` มาจากไลบรารี **MonoGame.Extended.Screens** ไม่ได้ประกาศในโปรเจกต์
- `ICollisionActor` ประกาศอยู่ในไฟล์ `Scripts/Managers/CollisionManager.cs`
- `Nyctophobia` และ `Claustrophobia` **ยังไม่มีคลาส** — อยู่ในแผนพัฒนา
- `ObjectiveManager`, `DotsGenerator`, `GeneratedDotsMap`, `GridCell`, `WirePath`, `SceneBuilder`, `VignetteOverlay`, `BottomFadeOverlay` มีรายละเอียดสมาชิกไม่ครบในไดอะแกรมนี้ — ดูโค้ดจริงเมื่อแก้ไข

## Related Documents

- System Design: [System Design](01-system-design.md)
- Data Schema: [Data Schema](03-data-schema.md)
- Mechanics (phobia behavior): [Mechanics](../01_GDD/03-mechanics.md)
