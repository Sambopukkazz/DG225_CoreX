# 02_Assets — Staging Area

**Last Updated:** 2026-10-14

โฟลเดอร์นี้เป็น **พื้นที่พัก asset** ก่อนคัดเลือกเข้าโปรเจกต์จริง — ยังไม่ถูก build และไม่ถูกใช้โดยเกม

## โครงสร้าง

| โฟลเดอร์ | เก็บอะไร | ผู้รับผิดชอบ |
|---|---|---|
| [`_candidates/sprites/`](_candidates/sprites/) | Sprite / Texture / Spritesheet | คนที่ 1 |
| [`_candidates/sfx/`](_candidates/sfx/) | Sound Effects | คนที่ 2 |
| [`_candidates/music/`](_candidates/music/) | Music / BGM | คนที่ 3 |
| [`_candidates/fonts/`](_candidates/fonts/) | Fonts + Data | คนที่ 4 |

## ขั้นตอน

1. โหลด asset ที่ต้องการลงใน `_candidates/<ประเภท>/`
2. **บันทึกที่มา + license ลงใน [Asset List](../01_GDD/05-asset-list.md) ทันที** — ห้ามปล่อยว่าง
3. คัดเลือกตัวที่ใช้ แล้วตั้งชื่อตาม [Naming Convention](../01_GDD/README.md#asset-naming-convention)
4. ย้ายไฟล์ที่เลือกไปที่ `src/Imaginophonia/Imaginophonia/Content/<ประเภท>/`
5. ลงทะเบียนใน `Content/Content.mgcb`

รายละเอียดเต็ม: [Asset Pipeline Flow](../01_GDD/06-asset-pipeline-flow.md)

> ⚠️ ห้ามวาง asset ที่มีลิขสิทธิ์ไม่ชัดเจนไว้ในโฟลเดอร์นี้ — repo เป็น public
