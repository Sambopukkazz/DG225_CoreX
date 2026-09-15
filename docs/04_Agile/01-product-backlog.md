# Product Backlog

**Version:** 1.0 | **Last Updated:** 2026-09-01

> รวม User Story ทั้งหมดของโปรเจกต์ — ยังไม่ได้แปลว่าต้องทำใน Sprint นี้ทั้งหมด
> โปรเจกต์นี้แบ่งงานตลอดเทอมเป็น **4 Sprint** (Sprint 1-4) — Sprint ไหนหยิบ Story ไปทำ ให้ใส่เลข Sprint นั้น (1-4) ลงคอลัมน์ `Sprint`

## Must Have (MVP)

| # | User Story                                                                                                              | Acceptance Criteria                                                                     | Estimate (SP) | Sprint |
| - | ----------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a player, I want to be able to move, so that I can explore the map and do objectives.                                | เดินซ้ายขวาได้ ไม่เดินทะลุกำแพง                           | 1             | 1      |
| 2 | As a player, I want to be able to interact with other stuffs, so that I can finish my objectives or hide from monsters. | เดินเข้าใกล้ของแล้วมีปุ้มแจ้ง กดแล้วทำ objective   | 2             | —     |
| 3 | As a player, I want entities to chase me, so that I feel challenged.                                                    | หาดโดนแล้วสามารถตายได้                                            | 2             | —     |
| 4 | As a designer, I want fully mapped out levels, so that the players can explores and do objectives                       | สามารถเข้าออกประตุได้ มี objective วางไว้ใน map          | 3             | —     |
| 5 | As phobias, I want to have many variants, so that I can challenge the player in different ways.                         | คิด concept phobia ให้สามารถโจมตี player ได้                        | 6             | —     |
| 6 | As a Artist, I want to redesign the main charecter to make them look more paranoid.                                     | ปรับตัวละครหลักให้ดูมีความหวาดระแวงมากขึ้น    | 5             | 1      |
| 7 | As a Artist, I want to redesign some of the assets to make them clearer and batter guide the player.                   | ปรับ Aessets ให้จัดเจนและนำทางผู้เล่นได้ระดับนึง | 3             | 2      |

## Should Have

| # | User Story                                                                                                                | Acceptance Criteria                                                                                      | Estimate (SP) | Sprint |
| - | ------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a player, I want to see my remaining lives, so that I know how close I am to game over                                 | จำนวนชีวิตแสดงบนจอตลอดเวลา ลดลงทันทีที่โดนโจมตี            | 2             | —     |
| 2 | As a designer, I want stuffs that can interact have highlights, so that the player know what stuff they can interact with | เข้าใกล้สิ่งของแล้วมี highlight ขึ้น เมื่อออกห่างแล้วหายไป | 2             | —     |
| 3 | As a developer, I want to have a menu and settings, so that the players can customize their experience to their liking.   | มี ui settings กดเข้าเกมต่างๆ                                                            | 3             | —     |
| 4 | As a designer, I want the game to have audio and sound effects, so that the game becomes more immersive.                  | ใส่เสียงเดิน เสียงซ่อมเสียง phobia etc                                         | 2             | —     |
| 5 | As a developer,i want the game have skill heal Sanity                                                                    | สกิลที่เพิ่มค่าสติ                                                                     | 2             | ---    |
| 6 | As a Artist, I want to redesign the mini-game UI                                                                         | ปรับ UI มินิเกม ให้ดูดีมากขึ้น                                                 | 3             | 3      |

## Nice to Have

| # | User Story                                                                                                                              | Acceptance Criteria                                                                                                                                           | Estimate (SP) | Sprint |
| - | --------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a designer, I want enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling                         | ปรับค่า spawn rate ในไฟล์ data แล้วรันเกมใหม่ ค่าที่เปลี่ยนมีผลทันทีโดยไม่ต้อง build ใหม่      | 3             | —     |
| 2 | As a designer, I want to make a balance sheet of entities, so that I can understand the balance                                         | สร้าง spreadsheet ที่มีขอมูล entity ต่าง ๆ สามารถดูค่าต่างแล้วไปปรับในโค้ดได้ดดยไม่มีปัญหา | 3             | —     |
| 3 | As a developer, I want to make my game's menu and gui to look nice, so that it looks appealing.                                         | design ui ให้สวยและเช้ากับ theme                                                                                                              | 1             | —     |
| 4 | As a designer, I want to make audio becomes stereo, so that the game gets even more immersive, and can also be a nice gameplay mechanic | ได้ยินเสียงแทรกซ้ายขวา                                                                                                                  | 2             |        |
| 5 | As a designer,iI want to make mini map                                                                                                  | มีmini map อยู่บริเวณมุมบนขวา                                                                                                             |               |        |
| 6 | As a Artist, I want to create a nomal map for an assets to make lighting look more realistic.                                           | ทำ Assets ให้มี nomal map ทำให้แสงตกกระทบดุสมจริงมากขึ้น                                                                 |               |        |

## MoSCoW Legend

- **Must Have** — จำเป็นต่อ core gameplay loop เกมเล่นไม่ได้ถ้าขาด (MVP)
- **Should Have** — เพิ่มคุณภาพเกม แต่เกมเล่นได้โดยไม่มีก็ได้
- **Nice to Have** — ทำถ้ามีเวลาเหลือ

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
