---
type: doc-index
version: 1.0
date: 2026-10-14
---

# Imaginophobia — GDD Index

**Version:** 1.2 | **Last Updated:** 2026-10-25

## 📖 เอกสารในโฟลเดอร์นี้

| ไฟล์ | เนื้อหา | สถานะ |
|---|---|---|
| [00-concept.md](00-concept.md) | Game concept, pillars, setting, system architecture | ✅ |
| [01-core-loop.md](01-core-loop.md) | Core loop, nested loop, flowchart, win/lose | ✅ |
| [02-scope-features.md](02-scope-features.md) | USP, in/out of scope, risks | ✅ |
| [03-mechanics.md](03-mechanics.md) | State diagram, rules, phobias, minigames, sanity | ✅ |
| [05-asset-list.md](05-asset-list.md) | รายการ asset ทั้งหมด + ปัญหาการตั้งชื่อ | ✅ |
| [06-asset-pipeline-flow.md](06-asset-pipeline-flow.md) | ขั้นตอนการนำ asset เข้าโปรเจกต์ | ✅ |
| [07-narrative.md](07-narrative.md) | เรื่องราว, ตัวละคร, โลก, ธีม | ✅ |
| [08-art-direction.md](08-art-direction.md) | โทนภาพ, ตัวละคร, สภาพแวดล้อม, UI | ✅ |
| [09-audio-direction.md](09-audio-direction.md) | แนวทางเสียง, ambience, spatial audio | ✅ |

> **หมายเหตุ:** ไม่มีเลข `04-` ในโฟลเดอร์นี้ — class diagram ย้ายไปอยู่ที่ [03_Software/02-class-diagram.md](../03_Software/02-class-diagram.md) แล้ว เพื่อให้งานออกแบบซอฟต์แวร์อยู่รวมกันเป็นหมวดเดียว
> เลข `05-` เดิมถูกใช้ซ้ำสองไฟล์ (asset list + pipeline flow) — แก้เป็น `05-` และ `06-` แล้ว

## 📐 เอกสารที่เกี่ยวข้อง

| หมวด | เอกสาร |
|---|---|
| Software Design | [System Design](../03_Software/01-system-design.md) · [Class Diagram](../03_Software/02-class-diagram.md) · [Data Schema](../03_Software/03-data-schema.md) |
| Agile | [Product Backlog](../04_Agile/01-product-backlog.md) · [Sprint Backlog](../04_Agile/02-sprint-backlog.md) · [Sprint Plans](../04_Agile/sprint-plans/) |
| Wiki | [Knowledge Hub](../05_Wiki/wiki.md) |
| ภาพรวมโปรเจกต์ | [Project Index](../index.md) |

## 🏷️ Naming Convention

**Asset:** ดูตารางเต็มใน [Asset List](05-asset-list.md)

| Prefix | ประเภท |
|---|---|
| `sprite_` | Sprite / Texture |
| `spritesheet_` | Spritesheet / AnimationSheet + Json |
| `bgm_` | Background Music |
| `font_` | Font |
| `data_` | Data / Config |
| `sfx_` | Sound Effect |
| `amb_` | Ambient Sound |

> ⚠️ ไฟล์ในโปรเจกต์ปัจจุบันยังไม่ตรงกับ convention นี้ทั้งหมด — ดูรายการที่ต้องแก้ใน [Asset List](05-asset-list.md#-ปัญหาการตั้งชื่อที่ต้องแก้)

**เอกสาร:** ไฟล์ใน `docs/01_GDD/` เรียงลำดับด้วย prefix ตัวเลข 2 หลัก (`00-`, `01-`, ...) ตามลำดับที่สร้างขึ้นในแต่ละ Lab — ห้ามสลับเลขไฟล์ที่มีอยู่แล้ว เพิ่มไฟล์ใหม่ให้ต่อเลขถัดไป

## 📁 ใครดูแลส่วนไหน

| คนที่ | รับผิดชอบ | โฟลเดอร์ staging |
|---|---|---|
| 1 | Sprites / Textures | `docs/02_Assets/_candidates/sprites/` |
| 2 | Sound Effects (SFX) | `docs/02_Assets/_candidates/sfx/` |
| 3 | Music / BGM | `docs/02_Assets/_candidates/music/` |
| 4 | Fonts + Data | `docs/02_Assets/_candidates/fonts/` |

## 🧭 Scene Object Naming Convention (LDtk)

การตั้งชื่อ entity ใน LDtk ถูกใช้เป็น **สัญญาณให้โค้ด** โดยตรง — `SceneBuilder` อ่านค่า 2 อย่าง:

- **tag ตัวแรกของ entity** (`__tags[0]`) → `entity.Class` ใช้ตัดสินว่าจะสร้างอะไร
- **ชื่อ entity** (`__identifier`) → `entity.Name` ใช้แยกย่อย (เช่น ชนิดของแสง)

| tag | กลายเป็น | ชื่อ entity ที่ใช้อยู่ |
|---|---|---|
| `wall` | `Wall` | `InvisWall` |
| `skillcheck` | `Trigger` (มินิเกมวาล์ว) | `Valve` |
| `dots` | `Trigger` (มินิเกมเชื่อมจุด) | `ElectricalPanel` |
| `hideout` | `Trigger` (ตู้ซ่อนตัว) | `Locker` |
| `door` | `Trigger` + ชื่อ = property `Destination` | `LoadNextScene` |
| `report` | `Trigger` + ชื่อ = property `Detail` | `Clipboard` |
| `spawn` | `SpawnPoint` + ชื่อ = property `Origin` | `PlayerSpawnPoint` (ห้องไฟฟ้า) · `SpawnPoint` (โถงอุโมง) |
| `light` | `PointLight` / `SpotLight` (+ `LightArea` ถ้าเป็น `PointLight`) | `PointLight` · `DoorLight` · `SpotLight` |

**Layer ที่ต้องมี**

| Layer | ชนิด | ใช้ทำอะไร |
|---|---|---|
| `Collision` | Entities | สร้าง collider / trigger / spawn point |
| `Lighting` | Entities | สร้างแสง |
| `Valves` · `Lockers` · `ElectricalPanel` · `Doors` · `Clipboard` · `Ladder` | Tiles | tile ที่ใช้ทำ highlight (ชื่อ layer ต้องตรงกับชื่อ entity โดยตัด `s` ตัวท้ายออก — ยกเว้น `LoadNextScene` ที่ใช้ layer `Doors` และถ้า `Destination == "end"` ใช้ layer `Ladder`) |

> **TBD:** ชื่อ entity บางตัวยังไม่ consistent — `PlayerSpawnPoint` กับ `SpawnPoint` ทำหน้าที่เดียวกัน และ `InvisWall` เป็นชื่อที่บอกว่า "มองไม่เห็น" ไม่ได้บอกว่าเป็นกำแพงชน ควรตกลงชื่อใหม่ทั้งชุด
> รายละเอียด entity / property / จำนวนจริงในแต่ละฉาก: [Data Schema](../03_Software/03-data-schema.md#2-ข้อมูลระดับ-level-data)
