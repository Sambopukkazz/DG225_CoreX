---
type: jam-pipeline
version: 0.1
date: 21/7/26
team: CoreX
---
# Pipeline Function Checklist — [Imaginophobia]

> เกมทุกแนวต้องมี pipeline ขั้นต่ำนี้ก่อนถึงจะเรียกว่า "เล่นได้" — เติมชื่อผู้รับผิดชอบและติ๊กสถานะระหว่างลงมือทำจริง เพิ่มแถวได้ถ้าเกมของทีมต้องการ module อื่น

## Must-Have (ต้องมีก่อน Hour 24 — Playable Build)

| Module                                         | หน้าที่                                                                                         | สถานะ     | ผู้รับผิดชอบ |
| ---------------------------------------------- | ------------------------------------------------------------------------------------------------------ | -------------- | ------------------------ |
| `GameStateManager`                           | สลับ state (Menu / Playing / Pause / GameOver)                                                     | 🔲 Not Started | [เตชินท์ 116]     |
| `InputManager`                               | รับ input keyboard/gamepad แบบรวมศูนย์                                                   | 🔲 Not Started | [เตชินท์ 116]     |
| Core `Update()` logic ของกลไกหลัก | logic ของ core mechanic 1 อย่างที่เป็นหัวใจเกม                                  | 🔲 Not Started | [เตชินท์ 116]     |
| Collision / interaction พื้นฐาน         | ตรวจชน/ตรวจ trigger ระหว่าง entity                                                    | 🔲 Not Started | [เตชินท์ 116]     |
| Lighting / Shadow                              | จัดแสงในฉาก ใส่เงาให้วัตถุต่างๆ                                          | 🔲 Not Started | [พรภวิษย์ 132]   |
| Objective Sequence                             | ลำดับเหตุการณ์ที่ผู้เล่นต้องเจอ                                         | 🔲 Not Started | [ธีนันทนัช 120] |
| Paint Map                                      | ใช้ Tile Palette paint TIle Map ตาม Level Design                                                 | 🔲 Not Started | [พรภวิษย์ 132]   |
| Dialogue ระหว่างเล่น/ตาย         | มีข้อความแสดงสิ่งที่ต้องทำหรือข้อผิดพลาดที่ไม่ควรทำ | 🔲 Not Started | [ธีนันทนัช 120] |
| Asset Creation                                 | วาด Asset ประกอบในฉาก                                                                    | 🟡 Ongoing    | [นาถวัฒน์ 125]   |

## Nice-to-Have (ทำถ้าเหลือเวลา — Hour 24–34)

| Module                                | หน้าที่                           | สถานะ     | ผู้รับผิดชอบ |
| ------------------------------------- | ---------------------------------------- | -------------- | ------------------------ |
| `AudioManager` (SFX พื้นฐาน) | เสียงตอบสนอง action หลัก | 🔲 Not Started | [พรภวิษย์ 132]   |
| Setting System                        | ปรับลดเสียงแยกประเภท | 🔲 Not Started | [เตชินท์ 116]     |
| Music / เพลงประกอบ          | เพิ่มบรรยากาศ               | 🔲 Not Started | [พรภวิษย์ 132]   |
| Start Menu                            | หน้าก่อนเข้าเกม           | 🔲 Not Started | [เตชินท์ 116]     |

## Cut-List (ตัดทิ้งก่อนถ้าเวลาไม่พอ — ห้ามเริ่มก่อน Must-Have เสร็จ)

- ❌ Save/Load
- ❌ Settings menu
- ❌ [feature อื่นที่ทีมตกลงตัดก่อน]

> เมื่อถึง **Feature Freeze (Hour 34)** ทุก module ในตารางนี้ต้องมีสถานะ ✅ Done หรือถูกย้ายไป Cut-List อย่างชัดเจน — ห้ามค้างเป็น 🔲/🟡
