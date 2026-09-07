<!-- Template เต็มไฟล์สำหรับสร้าง docs/agile/sprint-plan-[NN].md ของ Sprint ไหนก็ได้ -->

<!-- ดึง Story ของ Sprint นี้มาจาก docs/agile/02-sprint-backlog.md -->

<!-- Sprint 1: เปลี่ยนชื่อ sprint-01.md จาก Lab 07 เป็น sprint-plan-01.md แล้วแทนที่เนื้อหาด้วย template นี้ -->

<!-- Sprint 2-4 ในแลปถัดไป: คัดลอกไฟล์นี้ทั้งไฟล์ไปสร้าง sprint-plan-02.md, sprint-plan-03.md, sprint-plan-04.md ตามลำดับ -->

# Sprint [1] Plan

**Sprint Goal:** [เป้าหมายหลักของ Sprint นี้ในหนึ่งประโยค]
**ระยะเวลา:** [2026-08-29] — [2026-10-17]
**Team:**
เตชินท์ เจริญสิงห์ 116
ธีนันทนัช ปานานนท์ 120
นาถวัฒน์ เต็มเมือง 125
พรภวิษย์ ธนกิจรุ่งโรจน์ 132

---

## Sprint Backlog

| # | User Story                                                                                                                              | รับผิดชอบ                           | MoSCoW      | Estimate (SP) | Status         |
| - | --------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- | ----------- | ------------- | -------------- |
| 1 | As a player, I want to be able to move, so that I can explore the map and do objectives.                                                | เตชินท์ 116                           | Must Have   | 1             | ✅ Done        |
| 2 | As a player, I want to be able to interact with other stuffs, so that I can finish my objectives or hide from monsters.                 | เตชินท์ 116                           | Must Have   | 2             | 🔄 In Progress |
| 3 | As a player, I want entities to chase me, so that I feel challenged.                                                                    | เตชินท์ 116                           | Must Have   | 2             | 🔄 In Progress |
| 4 | As a designer, I want fully mapped out levels, so that the players can explores and do objectives                                       | ธีนันทนัช 120 + เตชินท์ 116 | Must Have   | 3             | 🔄 In Progress |
| 5 | As phobias, I want to have many variants, so that I can challenge the player in different ways.                                         | ธีนันทนัช 120                       | Must Have   | 6             | 🔲 Todo        |
| 6 | As a developer, I want to have a menu and settings, so that the players can customize their experience to their liking.                 | พรภวิษย์ 132                         | Should Have | 3             | 🔄 In Progress |
| 7 | As a designer, I want the game to have audio and sound effects, so that the game becomes more immersive.                                | พรภวิษย์ 132 + เตชินท์ 116   | Should Have | 2             | ✅ Done        |
| 8 | As a designer, I want to make audio becomes stereo, so that the game gets even more immersive, and can also be a nice gameplay mechanic | เตชินท์ 116                           | Should Have | 3             | ✅ Done        |
| 9 | As a Artist, I want to redesign the main charecter to make them look more paranoid.                                                     | นาถวัฒน์ 125                        | Must Have   | 5             | 🔄 In Progress |

## Status Legend

- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Tasks

### Story 1 — [Player controls and acitons]

- [X] [Player Class]  [owner:: เตชินท์ 116]  [estimate:: 3hrs]  [status:: ✅ Done]
- [ ] [InputManager Class]  [owner:: เตชินท์ 116]  [estimate:: 2hrs]  [status::✅ Done]

### Story 2 — [Collosion + Interactable]

- [X] [Research and choose suitable collision system]  [owner:: เตชินท์ 116]  [estimate:: 6hrs]  [status:: ✅ Done]
- [ ] [Test basic collision]  [owner:: เตชินท์ 116]  [estimate:: 4hrs]  [status:: 🔄 In Progress]
- [ ] [Flexible collision buildfer for each map/level]  [owner:: เตชินท์ 116]  [estimate:: 8hrs]  [status:: 🔄 In Progress]

### Story 3 — [Enemy behavior]

- [ ] [Enemy base class]  [owner:: เตชินท์ 116]  [estimate:: 3hrs]  [status:: 🔄 In Progress]
- [ ] [Implement predesign enemy]  [owner:: เตชินท์ 116]  [estimate:: 8hrs]  [status:: 🔄 In Progress]

### Story 4 — [Map creation]

- [X] [Build simple test map]  [owner:: ธีนันทนัช 120]  [estimate:: 15hrs]  [status:: ✅ Done]
- [ ] [Mock map layout]  [owner:: ธีนันทนัช 120]  [estimate:: 15hrs]  [status:: 🔄 In Progress]
- [ ] [Check map usability and revise it]  [owner:: เตชินท์ 116]  [estimate:: 14hrs]  [status:: 🔄 In Progress]

### Story 5 — [Transform phobias to enemies mechanics]

- [ ] [Search and interpret phobias]  [owner:: ธีนันทนัช 120]  [estimate:: Nh]  [status:: 🔲 Todo]
- [ ] [Task ย่อย]  [owner:: ชื่อ]  [estimate:: Nh]  [status:: 🔲 Todo]

### Story 6 — [Menu and settings]

- [ ] [Start Menu]  [owner:: พรภวิษย์ 132]  [estimate:: 18hrs]  [status:: 🔄 In Progress]
- [ ] [Setting Menu]  [owner:: พรภวิษย์ 132]  [estimate:: 20hrs]  [status:: 🔲 Todo]
- [ ] [Pause Screen]  [owner:: พรภวิษย์ 132]  [estimate:: 6hrs]  [status:: 🔲 Todo]

### Story 7 — [Sound effect]

- [X] [AudioManager class]  [owner:: เตชินท์ 116]  [estimate:: 7hrs]  [status:: ✅ Done]
- [ ] [Implement sound effect triggers]  [owner:: เตชินท์ 116]  [estimate:: 15h]  [status:: 🔄 In Progress]

### Story 8 — [Spatial Sound effect]

- [x] [Pan effect]  [owner:: เตชินท์ 116]  [estimate:: 3hrs]  [status::✅ Done]
- [x] [Doppler effect]  [owner:: เตชินท์ 116]  [estimate:: 7hrs]  [status::✅ Done]
 
- [ ] [Test and adjust]  [owner:: เตชินท์ 116]  [estimate:: 6hrs]  [status:: 🔲 Todo]

### Story 9 — [Redesign main character]

- [ ] [Player walk animation]  [owner:: นาถวัฒน์ 125]  [estimate:: 40hrs]  [status::🔄 In Progress]
- [ ] [Player idle animation]  [owner:: นาถวัฒน์ 125]  [estimate:: 20hrs]  [status:: 🔲 Todo]

---

## Daily Notes

### [วันที่]

**เมื่อวาน:** ...
**วันนี้:** ...
**Blocked:** ...

---

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/01-product-backlog|Product Backlog]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
