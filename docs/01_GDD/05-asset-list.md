---
type: asset-list
version: 1.0
date: 2026-10-14
---

# Asset List — Imaginophobia

**Version:** 1.0 | **Last Updated:** 2026-10-14

> ตารางนี้อิงจากไฟล์จริงใน `src/Imaginophonia/Imaginophonia/Content/` และ `Content/Content.mgcb`
> Asset ที่ยังไม่มีให้ใส่ใน `docs/02_Assets/_candidates/` ก่อน แล้วค่อยย้ายเข้า `Content/` ตาม [Asset Pipeline Flow](06-asset-pipeline-flow.md)

## Sprites / Textures

| Asset Name | คำอธิบาย | ขนาด | แหล่งที่มา | License | สถานะ | รับผิดชอบ |
|---|---|---|---|---|---|---|
| `Character/spitesheet_player-texture.png` | Spritesheet ตัวละครหลัก (เวอร์ชันปัจจุบัน) | — | ทีมทำเอง | ทีม | ✅ In repo | นาถวัฒน์ |
| `Character/spitesheet_player.json` | Animation data คู่กับ spritesheet | — | ทีมทำเอง | ทีม | ✅ In repo | นาถวัฒน์ |
| `Character/New_version/` | ตัวละครเวอร์ชัน redesign | — | ทีมทำเอง | ทีม | ✅ In repo | นาถวัฒน์ |
| `Character/Old_version/` | ตัวละครเวอร์ชันก่อน redesign (เก็บไว้เทียบ) | — | ทีมทำเอง | ทีม | ✅ In repo | นาถวัฒน์ |
| `Phobias/Scopophobia-Entity.png` | Entity ของ Scopophobia | — | ทีมทำเอง | ทีม | ✅ In repo | นาถวัฒน์ |
| `Phobias/Autophobia-Entity.png` | Entity ของ Autophobia | — | ทีมทำเอง | ทีม | 🔲 **ยังไม่มีไฟล์** — Autophobia วาดด้วย shape ในโค้ด | เตชินท์ |
| `UI/Needle.png` | เข็มชี้ของมินิเกม Skill Check | — | ทีมทำเอง | ทีม | ✅ In repo | พรภวิษย์ |
| `UI/Radial Circle.png` | วงกลม radial ของ Skill Check | — | ทีมทำเอง | ทีม | ✅ In repo | พรภวิษย์ |
| `UI/ui_spacebar.png` | ปุ่ม Spacebar บน UI | — | ทีมทำเอง | ทีม | ✅ In repo | พรภวิษย์ |
| `UI/sprite_valve.png` | วาล์วสำหรับมินิเกม Spin The Valve | — | ทีมทำเอง | ทีม | ✅ In repo | พรภวิษย์ |
| `Tilemaps/mapassets/spritesheet_background-texture.png` | Tileset พื้นหลังหลัก | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/asset-1-export.png` | Tileset ชุดที่ 1 | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/asset-2 (1)-export.png` | Tileset ชุดที่ 2 | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/asset-3-Recovered (1)-export.png` | Tileset ชุดที่ 3 | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/Cha1-export.png` | Tileset ตัวละคร/ฉาก | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/Sprite-bgx400%_3.png` | พื้นหลังขยาย 400% | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |
| `Tilemaps/mapassets/black_opa-export.png` | พื้นทึบแสงสำหรับ masking | — | ทีมทำเอง | ทีม | ✅ In repo | ธีนันทนัช |

## Audio — SFX

ไฟล์ทั้งหมดอยู่ใน `Content/Audio/` และผูกกับ `enum Sound` ในโค้ด

| Asset Name | คำอธิบาย | แหล่งที่มา | License | สถานะ | รับผิดชอบ |
|---|---|---|---|---|---|
| `sfx_pipe_repairing.mp3` | เสียงระหว่างซ่อมท่อ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_pipe_success.mp3` | ซ่อมท่อสำเร็จ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_pipe_failed.mp3` | ซ่อมท่อล้มเหลว | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_pipe_complete.mp3` | ซ่อมท่อครบทั้งหมด | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_electric_repairing.mp3` | เสียงระหว่างซ่อมไฟ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_electric_success.mp3` | ซ่อมไฟสำเร็จ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_electric_failed.mp3` | ซ่อมไฟล้มเหลว | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_electric_complete.mp3` | ซ่อมไฟครบทั้งหมด | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_door_open.mp3` | เปิดประตู | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_door_close.mp3` | ปิดประตู | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_door_stuck.mp3` | ประตูติด/เปิดไม่ได้ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_locker_open.mp3` | เปิดตู้ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_locker_close.mp3` | ปิดตู้ | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_scopophobia_indicator.mp3` | เสียงเตือนก่อน Scopophobia มา | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_autophobia_indicator.mp3` | เสียงเตือนก่อน Autophobia มา | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `sfx_step_01..08.mp3` | เสียงฝีเท้า 8 แบบ (สุ่ม) | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |

## Audio — Ambience

| Asset Name | คำอธิบาย | แหล่งที่มา | License | สถานะ | รับผิดชอบ |
|---|---|---|---|---|---|
| `amb_sewer.mp3` | บรรยากาศโถงอุโมง (น้ำ dripping, ชื้น) | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| `amb_electricroom.mp3` | บรรยากาศห้องไฟฟ้า | Pixabay | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |

## Audio — Music (BGM)

| Asset Name | คำอธิบาย | แหล่งที่มา | License | สถานะ | รับผิดชอบ |
|---|---|---|---|---|---|
| `dark frequency` | เพลงด่านแรก | https://freetouse.com/music/aylex/dark-frequency | [ต้องยืนยัน] | 🔲 **ยังไม่มีไฟล์ใน `Content/Audio/`** | พรภวิษย์ |

## Fonts

| Asset Name | คำอธิบาย | แหล่งที่มา | License | สถานะ | รับผิดชอบ |
|---|---|---|---|---|---|
| `Font/GenerationFonting.fnt` + `GenerationFonting_0.png` | Bitmap font หลักสำหรับ UI | [ต้องยืนยัน] | [ต้องยืนยัน] | ✅ In repo | พรภวิษย์ |
| ~~`notepen`~~ | ชื่อ font ที่เคยระบุไว้ในเอกสารเดิม — **ไม่พบไฟล์นี้ในโปรเจกต์** | — | — | ❌ ตัดออก | — |

## Shaders

| Asset Name | คำอธิบาย | สถานะ |
|---|---|---|
| `Shaders/Vignette.fx` | เอฟเฟกต์ขอบมืด (ถูกความมืดกลืนกิน) | ✅ In repo |
| `Shaders/BottomFade.fx` | เฟดด้านล่างของจอ | ✅ In repo |

## Data / Config

| Asset Name | คำอธิบาย | สถานะ |
|---|---|---|
| `Datas/tutorial.*` | ข้อมูล tutorial | ⚠️ พบเฉพาะ `.xnb` ที่ build แล้ว ไม่พบไฟล์ต้นทางใน `Content/` — ต้องตรวจสอบ |
| `Character/spitesheet_player.json` | Animation data | ✅ In repo |

## ⚠️ ปัญหาการตั้งชื่อที่ต้องแก้

ตาม [Asset Naming Convention](README.md#asset-naming-convention) ใน README ของ GDD:

| ปัญหา | ตัวอย่าง | ที่ถูกต้อง |
|---|---|---|
| สะกดผิด | `spitesheet_player` | `spritesheet_player` (ขาด `r`) |
| นามสกุลไฟล์ไม่ consistent | `.MP3` ปนกับ `.mp3`, `.PNG` ปนกับ `.png` | เลือกแบบเดียว (แนะนำตัวพิมพ์เล็ก) |
| มีช่องว่างในชื่อไฟล์ | `Radial Circle.png`, `asset-2 (1)-export.png` | ใช้ `_` แทน |
| ชื่อไฟล์มี `(1)` จากการโหลดซ้ำ | `asset-2 (1)-export.png`, `asset-3-Recovered (1)-export.png` | ตั้งชื่อใหม่ให้สื่อความหมาย |
| Prefix ไม่ตรงกับ convention | ไฟล์ภาพไม่มี prefix, เอกสารระบุ `spr_` แต่ README ระบุ `sprite_` | ตกลงให้ตรงกัน |

## Related Documents

- Pipeline: [Asset Pipeline Flow](06-asset-pipeline-flow.md)
- Naming convention: [GDD README](README.md)
- Staging area: [02_Assets](../02_Assets/)
- Art direction: [Art Direction](08-art-direction.md) · Audio: [Audio Direction](09-audio-direction.md)
