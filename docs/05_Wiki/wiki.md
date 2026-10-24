---
type: wiki-hub
version: 1.0
date: 2026-10-14
---

# 🌐 Imaginophobia — Knowledge Wiki

**Version:** 1.1 | **Last Updated:** 2026-10-24 | **Maintained by:** ทีม CoreX

---

## 🎯 Quick Access

- **[Game Concept]**: [00-concept.md](../01_GDD/00-concept.md)
- **[Core Loop]**: [01-core-loop.md](../01_GDD/01-core-loop.md)
- **[Core Mechanics]**: [03-mechanics.md](../01_GDD/03-mechanics.md)
- **[System Design]**: [01-system-design.md](../03_Software/01-system-design.md)
- **[Current Sprint]**: [Sprint Backlog](../04_Agile/02-sprint-backlog.md)
- **[Project Index]**: [index.md](../index.md)

---

## 🧠 Knowledge Base

*ความรู้ งานวิจัย และคู่มือเฉพาะทางที่ไม่ได้เป็นส่วนของ GDD*

### 📊 Reports & Research

- [System Test Reports](../04_Agile/05-report-backlog.md)
- [Playtest 25/09/2569](../04_Agile/reports/playtest-2026-09-25.md)
- [Sprint Retrospectives](../04_Agile/04-retrospectives-backlog.md)

### 📖 Guides & Process

- [Guidelines](guidelines/) — กฎและกระบวนการที่ทีมตกลงร่วมกัน
- [Game Jam History](history/README.md) — เอกสารชุด Game Jam 48 ชั่วโมง (กรกฎาคม 2026)

### 📚 เอกสารอ้างอิงภายนอก

| แหล่ง | ใช้ทำอะไร |
|---|---|
| [MonoGame docs](https://docs.monogame.net/) | API หลักของ engine |
| [MonoGame.Extended](https://www.monogameextended.net/) | Sprites, Screens, Camera |
| [Gum](https://gum.monoGame.com/) | เครื่องมือทำ UI |
| [LDtk](https://ldtk.io/) | Level editor |
| [Penumbra](https://github.com/discosultan/penumbra) | แสง 2D |

---

## 📁 โครงสร้าง Wiki

```
05_Wiki/
├── wiki.md          ← หน้าหลัก (ไฟล์นี้)
├── guidelines/      ← กฎและกระบวนการของทีม
└── history/         ← เอกสาร Game Jam (เก็บเป็นประวัติ)
    ├── README.md
    ├── 00-timeline.md
    ├── 01-pipeline-checklist.md
    ├── 02-roles.md
    ├── 03-workflow-rules.md
    ├── 04-feedback.md
    └── image/
```

---

## 📝 วิธีเพิ่มความรู้ใหม่

1. ตัดสินใจว่าองค์ความรู้นั้นเป็น **กฎ/กระบวนการ** (→ `guidelines/`) หรือ **บันทึก/งานวิจัย** (→ โฟลเดอร์ใหม่ตามหัวข้อ)
2. สร้างไฟล์ `.md` ใส่ frontmatter `type`, `version`, `date`
3. เพิ่มลิงก์ในส่วน Knowledge Base ของไฟล์นี้
4. อัปเดต `**Last Updated:**` ของไฟล์ที่แก้ และของ [Project Index](../index.md)

---

*Imaginophobia — CoreX · Digital Games, Chiang Mai University*
