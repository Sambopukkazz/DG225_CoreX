# 📝 Documentation Changelog

บันทึกการแก้ไขเอกสารทั้งหมดใน `docs/` — เรียงจากใหม่ไปเก่า

รูปแบบวันที่: `YYYY-MM-DD`

---

## 2026-10-14 — Restructure & Content Fill

**ประเภท:** Restructure + Content

รีสตรัคเจอร์เอกสารทั้งชุดให้เป็นหมวดหมู่ที่ชัดเจน และเติมเนื้อหาที่ขาดจากสไลด์นำเสนอของทีม

### เพิ่มโฟลเดอร์ใหม่

| โฟลเดอร์ | เนื้อหา |
|---|---|
| `02_Assets/` | พื้นที่พัก asset ก่อนเข้าโปรเจกต์ (เดิมถูกอ้างถึงแต่ไม่มีอยู่จริง) |
| `03_Software/` | รวมงานออกแบบซอฟต์แวร์ไว้ที่เดียว |
| `05_Wiki/` | Knowledge hub, guidelines, และประวัติ Game Jam |
| `04_Agile/standups/` | แยกบันทึก standup ออกจาก meeting-notes |
| `04_Agile/retrospectives/` | แยก retrospective ออกจาก meeting-notes |
| `04_Agile/sprint-plans/` | รวมแผนราย Sprint ไว้ที่เดียว |
| `04_Agile/reports/` | รายงานการทดสอบ |
| `04_Agile/user-stories/` | รายละเอียด User Story รายข้อ |

### ย้ายไฟล์

| จาก | ไป | เหตุผล |
|---|---|---|
| `03_GameJam/*` | `05_Wiki/history/*` | เป็นเอกสารชุดเหตุการณ์ 48 ชม. เก็บเป็นประวัติ |
| `03_GameJam/README-.md` | `05_Wiki/history/README.md` | แก้ชื่อไฟล์ที่มีขีดเกิน |
| `04_Agile/meeting-notes/standup-week-*.md` | `04_Agile/standups/` | แยกประเภทเอกสาร |
| `04_Agile/meeting-notes/retrospective-sprint-*.md` | `04_Agile/retrospectives/` | แยกประเภทเอกสาร |
| `04_Agile/sprint-plan-0*.md` | `04_Agile/sprint-plans/` | จัดกลุ่ม |
| `01_GDD/04-class-diagram.md` | `03_Software/02-class-diagram.md` | เป็นงานออกแบบซอฟต์แวร์ |
| `01_GDD/05-asset-list-pipeline-flow.md` | `01_GDD/06-asset-pipeline-flow.md` | แก้เลขซ้ำกับ asset list |

### เติมเนื้อหา (จากสไลด์นำเสนอของทีม)

| ไฟล์ | สิ่งที่เติม |
|---|---|
| `01_GDD/00-concept.md` | ทั้งฉบับ — เดิมเป็นแม่แบบว่าง · **แก้ Engine จาก Unity เป็น MonoGame** |
| `01_GDD/01-core-loop.md` | Core loop, nested loop (3 ระดับ), flowchart, win/lose |
| `01_GDD/02-scope-features.md` | USP, in/out of scope, risks จากผล playtest |
| `01_GDD/03-mechanics.md` | ตาราง phobia 5 ตัว, มินิเกม 3 แบบ, ระบบ Sanity |
| `01_GDD/05-asset-list.md` | รายการ asset จริงจาก `Content/` + ปัญหาการตั้งชื่อ |
| `01_GDD/06-asset-pipeline-flow.md` | ขั้นตอนครบ + โครงสร้าง `Content/` |
| `01_GDD/07-narrative.md` | **ใหม่** — story, ธีม, twist, ตัวละคร |
| `01_GDD/08-art-direction.md` | **ใหม่** — จานสีตัวละคร, สภาพแวดล้อม, enemy design, UI |
| `01_GDD/09-audio-direction.md` | **ใหม่** — แนวทางเสียง, spatial audio, ขั้นตอนหาเสียง |
| `03_Software/01-system-design.md` | **ใหม่** — สถาปัตยกรรมจริงจากโค้ด |
| `03_Software/02-class-diagram.md` | เขียนใหม่จากคลาสจริงใน `Scripts/` |
| `03_Software/03-data-schema.md` | **ใหม่** — enum, level data, ค่าปรับแต่ง, save game |
| `04_Agile/03-meeting-backlogs.md` | **ใหม่** — hub |
| `04_Agile/04-retrospectives-backlog.md` | **ใหม่** — hub |
| `04_Agile/05-report-backlog.md` | **ใหม่** — hub |
| `04_Agile/kanban.md` | **ใหม่** — กระดาน Kanban |
| `04_Agile/reports/playtest-2026-09-25.md` | **ใหม่** — รายงาน playtest ฉบับเต็ม |
| `04_Agile/user-stories/README.md` | **ใหม่** — index + แม่แบบ |
| `05_Wiki/wiki.md` | **ใหม่** — Knowledge Hub |
| `05_Wiki/guidelines/README.md` | **ใหม่** — index ของ guideline |
| `02_Assets/README.md` | **ใหม่** |
| `index.md` | **ใหม่** — Project Index ระดับโปรเจกต์ |
| `changelog.md` | **ใหม่** — ไฟล์นี้ |

### แก้ไขลิงก์

- แก้ลิงก์เสีย 3 จุดใน `01_GDD/README.md` (ชี้ไปชื่อไฟล์ที่ไม่มีอยู่)
- แปลง wikilink `[[...]]` ทั้งหมด (29 จุด) เป็น relative markdown link — ของเดิมชี้ไป path ที่ไม่มีอยู่ (`docs/gdd/`, `docs/agile/`) และ `[[...]]` ไม่แสดงผลบน GitHub
- ลบลิงก์ที่ชี้ไป `../lab_discussion_05_game_jam_prep.md` ซึ่งไม่มีอยู่ใน repo
- อัปเดตลิงก์ทั้งหมดที่เสียจากการย้ายไฟล์

### แก้ไข metadata

- แก้ typo `agtype:` → `type:` ใน class diagram
- ปรับรูปแบบวันที่ใน frontmatter เป็น `YYYY-MM-DD` ทั้งหมด (เดิมปนกันหลายแบบ)

---

## ก่อนหน้า

> **TBD:** ยังไม่มีบันทึกการแก้ไขเอกสารก่อนวันที่นี้ — เริ่มบันทึกจากวันนี้เป็นต้นไป
