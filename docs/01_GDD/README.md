---
type: doc-index
version: 2.0
date: 2026-11-02
---
# Imaginophobia — GDD Index

**Version:** 2.0 (Release 1.0.0) | **Last Updated:** 2026-11-02

## 📖 เอกสารในโฟลเดอร์นี้

| ไฟล์                                            | เนื้อหา                                                       | สถานะ |
| --------------------------------------------------- | -------------------------------------------------------------------- | ---------- |
| [00-concept.md](00-concept.md)                         | Game concept, pillars, setting, system architecture                  | ✅         |
| [01-core-loop.md](01-core-loop.md)                     | Core loop, nested loop, flowchart, win/lose                          | ✅         |
| [02-scope-features.md](02-scope-features.md)           | USP, in/out of scope, สถานะเทียบผล playtest                         | ✅         |
| [03-mechanics.md](03-mechanics.md)                     | State diagram, rules, phobias, minigames, sanity                     | ✅         |
| [04-asset-list.md](04-asset-list.md)                   | รายการ asset ทั้งหมด + แหล่งที่มา / license | ✅         |
| [05-asset-pipeline-flow.md](05-asset-pipeline-flow.md) | ขั้นตอนการนำ asset เข้าโปรเจกต์              | ✅         |
| [06-narrative.md](06-narrative.md)                     | เรื่องราว, ตัวละคร, โลก, ธีม                   | ✅         |
| [07-art-direction.md](07-art-direction.md)             | โทนภาพ, ตัวละคร, สภาพแวดล้อม, UI             | ✅         |
| [08-audio-direction.md](08-audio-direction.md)         | แนวทางเสียง, ambience, spatial audio                      | ✅         |

> **หมายเหตุ:** ไม่มีเลข `04-` ที่ว่างในโฟลเดอร์นี้แล้ว — class diagram ย้ายไปอยู่ที่ [03_Software/02-class-diagram.md](../03_Software/02-class-diagram.md) และไฟล์ที่เหลือถูกเรียงเลขใหม่เป็น **00–08** ต่อเนื่องกัน
> ลำดับก่อนหน้า (05–09) ถูกเลื่อนขึ้นหนึ่งตำแหน่ง: `05-asset-list` → `04-asset-list` · `06-asset-pipeline-flow` → `05-asset-pipeline-flow` · `07-narrative` → `06-narrative` · `08-art-direction` → `07-art-direction` · `09-audio-direction` → `08-audio-direction`

## 📐 เอกสารที่เกี่ยวข้อง

| หมวด                     | เอกสาร                                                                                                                                         |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Software Design              | [System Design](../03_Software/01-system-design.md) · [Class Diagram](../03_Software/02-class-diagram.md) · [Data Schema](../03_Software/03-data-schema.md) |
| Agile                        | [Product Backlog](../04_Agile/01-product-backlog.md) · [Sprint Backlog](../04_Agile/02-sprint-backlog.md) · [Sprint Plans](../04_Agile/sprint-plans/)       |
| Wiki                         | [Knowledge Hub](../05_Wiki/wiki.md)                                                                                                                     |
| ภาพรวมโปรเจกต์ | [Project Index](../index.md)                                                                                                                            |

## 🏷️ Naming Convention

**Asset:** ดูตารางเต็มใน [Asset List](04-asset-list.md)

| Prefix           | ประเภท                        |
| ---------------- | ----------------------------------- |
| `sprite_`      | Sprite / Texture                    |
| `spritesheet_` | Spritesheet / AnimationSheet + Json |
| `bgm_`         | Background Music                    |
| `amb_`         | Ambient Sound                       |
| `sfx_`         | Sound Effect                        |

## 🧭 Scene Object Naming Convention (LDtk)

การตั้งชื่อ entity ใน LDtk ถูกใช้เป็น **สัญญาณให้โค้ด** โดยตรง — `SceneBuilder` อ่านค่า 2 อย่าง:

- **tag ตัวแรกของ entity** (`__tags[0]`) → `entity.Class` ใช้ตัดสินว่าจะสร้างอะไร
- **ชื่อ entity** (`__identifier`) → `entity.Name` ใช้แยกย่อย (เช่น ชนิดของแสง, ชื่อ tile layer สำหรับ highlight)

| tag            | กลายเป็น                                                               | ชื่อ entity ที่ใช้อยู่                                           |
| -------------- | ------------------------------------------------------------------------------ | ------------------------------------------------------------------------------ |
| `wall`       | `Wall`                                                                       | `InvisWall`                                                                  |
| `skillcheck` | `Trigger` (มินิเกมวาล์ว)                                         | `Valve`                                                                      |
| `dots`       | `Trigger` (มินิเกมเชื่อมจุด)                                 | `ElectricalPanel`                                                            |
| `hideout`    | `Trigger` (ตู้ซ่อนตัว)                                             | `Locker`                                                                     |
| `door`       | `Trigger` + ชื่อ = property `Destination`                              | `LoadNextScene`                                                              |
| `report`     | `Trigger` + ชื่อ = property `Detail`                                   | `Clipboard`                                                                  |
| `spawn`      | `SpawnPoint` + ชื่อ = property `Origin`                                | `PlayerSpawnPoint` (ห้องไฟฟ้า) · `SpawnPoint` (โถงอุโมง) |
| `light`      | `PointLight` / `SpotLight` (+ `LightArea` ถ้าเป็น `PointLight`) | `PointLight` · `DoorLight` · `SpotLight`                               |

**Layer ที่ต้องมี**

| Layer                                                                                        | ชนิด | ใช้ทำอะไร                                                                                                                                                                                                                                        |
| -------------------------------------------------------------------------------------------- | -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Collision`                                                                                | Entities | สร้าง collider / trigger / spawn point                                                                                                                                                                                                               |
| `Lighting`                                                                                 | Entities | สร้างแสง                                                                                                                                                                                                                                          |
| `Valves` · `Lockers` · `ElectricalPanel` · `Doors` · `Clipboard` · `Ladder` | Tiles    | tile ที่ใช้ทำ highlight (ชื่อ layer ต้องตรงกับชื่อ entity โดยตัด `s` ตัวท้ายออก — ยกเว้น `LoadNextScene` ที่ใช้ layer `Doors` และถ้า `Destination == "end"` ใช้ layer `Ladder`) |
