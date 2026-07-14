---
type: gdd-mechanics
version: 0.1
date: 14/7/
---
# Mechanic Design — [Pipe Repair]

## State Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Move : กด Arrow/WASD
    Move --> Idle : ปล่อย Arrow/WASD
    Idle --> Hide : กด Hide
    Hide --> Idle : ออกจากที่ซ่อน
    Idle --> Fix : กด Fix
    Fix --> QTE : แบบ DBD
    QTE --> Idle : ซ่อมเสร็จ
```

## Rules

| State | เข้าเงื่อนไข                        | ออกเงื่อนไข                                              | Note            |
| ----- | ----------------------------------------------- | ------------------------------------------------------------------- | --------------- |
| Idle  | เริ่มเกม / หยุดเคลื่อนที่ | กด input ใดๆ                                                   | Animation loop  |
| Move  | กดปุ่มทิศทาง                        | ปล่อยปุ่ม                                                  | Speed = slow    |
| Hide  | กด Hide                                       | โดนผลักออก / กดออกจากที่ซ่อน               | Have time limit |
| Fix   | กด Fix                                        | ซ่อมไม่สำเร็จหลายครั้ง / ซ่อมสำเร็จ | Have QTE        |
