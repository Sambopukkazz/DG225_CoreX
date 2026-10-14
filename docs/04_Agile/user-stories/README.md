---
type: agile-user-stories
version: 1.0
date: 2026-10-14
---

# User Stories — Index

**Version:** 1.0 | **Last Updated:** 2026-10-14

โฟลเดอร์นี้เก็บรายละเอียดของ User Story แต่ละข้อในรูปแบบไฟล์แยก

**สถานะ: ยังว่าง** — ปัจจุบัน User Story ทั้งหมดถูกเก็บเป็นตารางอยู่ใน [Product Backlog](../01-product-backlog.md) โดยตรง

> **TBD:** ยังไม่มี User Story ฉบับเต็มแยกไฟล์ — ถ้าต้องการรายละเอียดต่อ story ให้สร้างไฟล์ตามแม่แบบด้านล่าง แล้วใส่ ID นั้นในตาราง Product Backlog

## แม่แบบ User Story

ตั้งชื่อไฟล์ว่า `US-<Epic>-<Number>.md` เช่น `US-CORE-01.md`

```markdown
# User Story: US-[Epic]-[Number] - [Title]

**Status:** [✅ Done / 🔵 In Progress / 🔲 Todo]
**Epic:** [Epic Title](../01-product-backlog.md)
**Owner:** [Name]

---

## 📖 Description
**ในฐานะ** [Role]
**ฉันต้องการ** [Feature/Action]
**เพื่อให้** [Benefit]

---

## ✅ Acceptance Criteria
1. [ ] [Criteria 1]
2. [ ] [Criteria 2]

---

## 🛠 Technical Tasks (Git Log Updates)
- [ ] [Task 1]
- [ ] [Task 2]

---

## 🔗 Related Files
- Backlog: [Product Backlog](../01-product-backlog.md)
- GDD: [Mechanics](../../01_GDD/03-mechanics.md)
- Sprint Plan: [Sprint 1](../sprint-plans/sprint-plan-01.md)
```

## หมายเหตุ

- ID ของ story ต้องตรงกับแถวใน [Product Backlog](../01-product-backlog.md)
- เมื่อปิด story แล้ว ให้อัปเดตทั้งไฟล์นี้และตารางใน backlog

## Related Documents

- Product Backlog: [Product Backlog](../01-product-backlog.md)
- Sprint Backlog: [Sprint Backlog](../02-sprint-backlog.md)
- Kanban: [Kanban](../kanban.md)
