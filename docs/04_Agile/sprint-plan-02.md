<!-- Template เต็มไฟล์สำหรับสร้าง docs/agile/sprint-plan-[NN].md ของ Sprint ไหนก็ได้ -->

<!-- ดึง Story ของ Sprint นี้มาจาก docs/agile/02-sprint-backlog.md -->

<!-- Sprint 1: เปลี่ยนชื่อ sprint-01.md จาก Lab 07 เป็น sprint-plan-01.md แล้วแทนที่เนื้อหาด้วย template นี้ -->

<!-- Sprint 2-4 ในแลปถัดไป: คัดลอกไฟล์นี้ทั้งไฟล์ไปสร้าง sprint-plan-02.md, sprint-plan-03.md, sprint-plan-04.md ตามลำดับ -->

# Sprint [2] Plan

**Sprint Goal:** [เกมลเ่นได้โดยมีการเปลี่ยนแปลง/พัฒนามากกว่าเกมใน Jam]
**ระยะเวลา:** [2026-09-13] — [2026-09-26]
**Team:**
เตชินท์ เจริญสิงห์ 682110116 (Techin)
ธีนันทนัช ปานานนท์ 682110120 (Draft)
นาถวัฒน์ เต็มเมือง 682110125 (Mon)
พรภวิษย์ ธนกิจรุ่งโรจน์ 682110132 (Ken)

---

## Sprint Backlog

| # | User Story                                                                                                              | รับผิดชอบ                    | MoSCoW      | Estimate (SP) | Status         |
| - | ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------- | ----------- | ------------- | -------------- |
| 1 | As a player, I want to see my remaining lives                                                                           | เตชินท์                        | Should Have | 2             | 🔄 In Progress |
| 2 | As a Artist, I want to redesign some of the assets to make them clearer and batter guide the player.                    | นาถวัฒน์                      | Should Have | 3             | 🔄 In Progress |
| 3 | As a designer, i want to test out lightings so that the games feel more immersive                                       | ธีนันทนัช                    | Must Have   | 2             | 🔄 In Progress |
| 4 | As a designer, I want fully mapped out levels, so that the players can explores and do objectives                       | ธีนันทนัช + เตชินท์ | Must Have   | 3             | ✅ Done        |
| 5 | As phobias, I want to have many variants, so that I can challenge the player in different ways.                         | ธีนันทนัช                    | Must Have   | 6             | 🔲 Todo        |
| 6 | As a developer, I want to have a menu and settings, so that the players can customize their experience to their liking. | พรภวิษย์ 132                  | Should Have | 3             | 🔄 In Progress |
| 7 | As a player, I want entities to chase me, so that I feel challenged.                                                    | เตชินท์                        | Must Have   | 2             | 🔄 In Progress |
| 8 | As a player, I want to be able to interact with other stuffs, so that I can finish my objectives or hide from monsters. | เตชินท์                        | Must Have   | 2             | ✅ Done        |

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

### Story 2 — [Redesign main character]

- [ ] [Player walk animation]  [owner:: นาถวัฒน์ 125]  [estimate:: 40hrs]  [status::🔄 In Progress]
- [ ] [Player idle animation]  [owner:: นาถวัฒน์ 125]  [estimate:: 20hrs]  [status:: 🔲 Todo]

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

- [X] [Pan effect]  [owner:: เตชินท์ 116]  [estimate:: 3hrs]  [status::✅ Done]
- [X] [Doppler effect]  [owner:: เตชินท์ 116]  [estimate:: 7hrs]  [status::✅ Done]
- [ ] [Test and adjust]  [owner:: เตชินท์ 116]  [estimate:: 6hrs]  [status:: 🔲 Todo]

### Story 9 — []

---

## Weekly Notes


## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/01-product-backlog|Product Backlog]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
