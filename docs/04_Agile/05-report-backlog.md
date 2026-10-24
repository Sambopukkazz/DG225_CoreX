---
type: agile-hub
version: 1.0
date: 2026-10-14
---

# Report Backlog — Hub

**Version:** 1.1 | **Last Updated:** 2026-10-24

ศูนย์รวมรายงานผลการทดสอบและผลการทดลอง (playtest / QA) ของเกม

## รายงานที่มี

| วันที่ | ประเภท | ไฟล์ | สรุปผล |
|---|---|---|---|
| 25/09/2569 | Playtest (User Study) | [playtest-2026-09-25.md](reports/playtest-2026-09-25.md) | ความน่ากลัว 2.5/5 · ต้อง redesign monster และเพิ่ม visual indicator |

## โครงสร้างโฟลเดอร์

```
reports/
└── playtest-YYYY-MM-DD.md   ← รายงาน playtest / user study
```

> รายงานทุกประเภท (playtest, system test, balance, บั๊ก) เก็บเป็นไฟล์ `.md` ในโฟลเดอร์นี้โดยตรง — ไม่แยกโฟลเดอร์ย่อย

## สรุปประเด็นสำคัญจาก Playtest ล่าสุด

| ประเด็น | จำนวนคนเห็นด้วย |
|---|---|
| มอนต้องน่ากลัวขึ้น | 3/6 |
| Minigame ยากเกินไป / ไม่มี guide | 2/6 |
| Objective ต้องชัดเจนขึ้น | 2/6 |
| ประตู/ทางเดินต้องแยกจากพื้นหลัง | 2/6 |
| Sanity/Battery UI ต้องชัด + ต้องส่งผลจริง | 2/6 |
| Visual indicator ตอนมอนเกิด | 1/6 |

งานที่ต้องทำต่อจากผลนี้ ถูกแปลงเป็นความเสี่ยงและแผนรับมือใน [Scope: Risks](../01_GDD/02-scope-features.md#risks--assumptions)

## Related Documents

- Playtest report: [playtest-2026-09-25.md](reports/playtest-2026-09-25.md)
- Scope & Risks: [Scope & Feature List](../01_GDD/02-scope-features.md)
- Product Backlog: [Product Backlog](01-product-backlog.md)
