---
type: gdd-core-loop
version: 1.0
date: 2026-10-14
---

# Imaginophobia — Core Loop & Gameplay

**Version:** 1.0 | **Last Updated:** 2026-10-14

## Core Loop

เจอ Monster → ซ่อน / ต่อสู้ → เอาตัวรอด → ซ่อมท่อ → (ซ่อมครบตามที่กำหนด) → หลบหนีได้สำเร็จ

```mermaid
flowchart LR
    A[เจอ Monster] --> B[ซ่อน / ต่อสู้]
    B --> C[เอาตัวรอด]
    C --> A
    C --> D[ซ่อมท่อ]
    D --> C
    D -->|ซ่อมท่อครบตามที่กำหนด| E[หลบหนีได้สำเร็จ]
```

## Nested Loop (3 ระดับ)

เกมมี loop ซ้อนกันสามชั้น — ชั้นนอกคือการเอาตัวรอดจาก phobia, ชั้นกลางคือการรักษาสภาพตัวเอง, ชั้นในคือการแก้ puzzle

```mermaid
flowchart TD
    P[เอาตัวรอดจาก Phobia]
    P --> S[Scopophobia<br/>Fear of Being Watched]
    P --> N[Nyctophobia<br/>Fear of Darkness]
    P --> A[Autophobia<br/>Fear of Being Alone]
    S --> S1[หาที่ซ่อน]
    S1 --> S2[หาที่ที่มีไฟ / ห้ามอยู่ในที่มืด]
    S2 --> S3[ส่องไฟฉายใส่]
    A --> S3

    subgraph Cyan[Survival loop]
      E1[สำรวจแมพ] --> E2[รักษาค่า Sanity ให้มากกว่า 0]
      E2 --> E3[รักษาแบต ไฟฉาย]
      E3 --> E1
    end
    P --> E1
    E3 --> P

    subgraph Green[Puzzle loop]
      Q[แก้ puzzle] --> Q1[ช่องท่อ]
      Q1 --> Q2[หมุนท่อให้หลอดเต็ม]
      Q2 --> Q
      Q --> Q3[ช่องไฟ]
      Q3 --> Q4[เล่น Connect The Dots]
      Q4 --> Q
    end
    E1 --> Q
    Q --> T[เวลาหมดให้สำเร็จ / กับเวลา]
```

## Flowchart รวม (เงื่อนไขการดำเนินเกม)

```mermaid
flowchart TD
    Start([เริ่ม]) --> Fix[ช่องท่อ / ช่องไฟ]
    Fix --> Scopo{เจอ Scopophobia?}
    Scopo -->|ใช่| ScopoY[หาคู่]
    Scopo -->|ไม่ใช่| Auto{เจอ Autophobia?}
    Auto -->|ใช่| AutoY[ส่องไฟฉายใส่ / หาคู่]
    Auto -->|ไม่ใช่| Nycto{เจอ Nyctophobia?}
    Nycto -->|ใช่| NyctoY[กลับไปอยู่ในที่มืดแสงสว่าง]
    Nycto -->|ไม่ใช่| AllFixed{ช่องทุกจุดแล้ว?}
    ScopoY --> SanityDrop[Sanity ลด]
    AutoY --> SanityDrop
    NyctoY --> SanityDrop
    SanityDrop --> Thresh{Sanity ถึง Threshold?}
    Thresh -->|ใช่| Zero{Sanity == 0?}
    Zero -->|ใช่| End([จบ])
    Zero -->|ไม่ใช่| Stage[เปลี่ยน Stage]
    Stage --> Indicator[มี Visual Indicator ที่สื่อถึง sanity ที่ลดไป]
    Indicator --> AllFixed
    Thresh -->|ไม่ใช่| AllFixed
    AllFixed -->|ใช่| End
    AllFixed -->|ไม่ใช่| Fix
```

## Player Actions

| Action | Input | Result | Notes |
|---|---|---|---|
| เดิน | Arrow / WASD | ตัวละครเคลื่อนที่ | `CharacterAction.Walking` |
| ซ่อน | กด Hide (ใกล้ตู้) | เข้าสู่ตู้ซ่อนตัว | `CharacterAction.ReadyToHide` → `Hiding` |
| ซ่อม | กด Fix / Space | เข้ามินิเกมซ่อม | `CharacterAction.Repairing` |
| ส่องไฟฉาย | — | ทำให้ Autophobia หายไป / ใช้ในความมืด | `CharacterAction.Checking` |

> **TBD:** ตารางปุ่มจริง (key binding) ยังไม่ยืนยัน — `InputManager` เป็นที่เดียวที่ผูกปุ่มทั้งหมด ให้ตรวจสอบจากโค้ดก่อนแก้ตารางนี้

## Win / Lose Conditions

- **ชนะเมื่อ:** ซ่อมท่อ/ช่องไฟครบทุกจุดตามที่กำหนด แล้วหลบหนีออกไปได้สำเร็จ (`หลบหนีได้สำเร็จ`)
- **แพ้เมื่อ:**
  - ถูก Autophobia / Scopophobia จับได้
  - ค่า Sanity ลดถึง 0 (`MentalState` มี 3 ระดับ: Normal 75 / Anxious 50 / Insane 25)
  - Nyctophobia — อยู่ในความมืดนานเกินไปหรือหมดเวลา แล้วถูกความมืดกลืนกิน

> **TBD:** ค่า threshold ของ Sanity ที่ทำให้แพ้ และอัตราการลด Sanity ต่อวินาที ต้องยืนยันจากโค้ด/การทดสอบ — ดู [Data Schema](../03_Software/03-data-schema.md)

## Related Documents

- Mechanics: [Mechanics](03-mechanics.md)
- Concept: [Game Concept](00-concept.md)
- System Design: [System Design](../03_Software/01-system-design.md)
