# Group 4: Automated Testing Strategy & Test Reports
> **Group ID**: `testing_strategy`  
> **File Path**: `docs/unit_testing_strategy.md`  
> **Last Updated**: `2026-09-15 22:18:00`  
> **Current Version**: `v2.0.0`  

## Revision History
| Version | Timestamp | Description |
| :--- | :--- | :--- |
| `v1.0.0` | 2026-09-06 02:30:00 | Khởi tạo tài liệu chiến lược kiểm thử unit test `unit_testing_strategy.md` (35 test cases) |
| `v1.1.0` | 2026-09-06 03:30:00 | Bổ sung test case cho service ghi thao tác `InputRecorderServiceTests` (47 test cases) |
| `v1.2.0` | 2026-09-06 04:40:00 | Bổ sung test case cho Reset Hotkeys & Pause Button (51 test cases) |
| `v1.3.0` | 2026-09-06 11:50:00 | Bổ sung test case cho phím Numpad (66 test cases) & Cập nhật chuẩn định dạng tài liệu nhóm |
| `v2.0.0` | 2026-09-15 22:18:00 | Bổ sung test cases cho ProfileService, ScreenCaptureService, PixelWatcherService & Macro Editor (96 test cases) |

---

## 1. Goal & Testing Framework
- **Framework**: xUnit (.NET 8.0 Windows), `Microsoft.NET.Test.Sdk`
- **Current Test Coverage**: **96 / 96 Test Cases Passed (100%)**

---

## 2. Test Suite Breakdown

### A. Win32 Virtual Key & Numpad Parsing (`InputSimulatorServiceTests`)
- Conversion of single keys (A-Z, 0-9, F1-F12, Space, Enter, Tab).
- Combo key parsing (`CTRL+F6` ➔ `VK_F6`).
- Symbol key parsing (`,`, `.`, `-`, `=`, `/`, `;`, `[`, `]`).
- **Numpad key parsing**: `NUMPAD0`..`NUMPAD9` ➔ `VK_NUMPAD0` (0x60) .. `VK_NUMPAD9` (0x69) and `NUMLOCK` (0x90).

### B. Input Recording Conversion (`InputRecorderServiceTests`)
- Low-level hook VK code to key name conversion.
- Numpad VK codes `0x60`-`0x6F` and `0x90` ➔ `NUMPAD0`..`NUMPAD9`, `*`, `+`, `-`, `.`, `/`, `NUMLOCK`.
- Recording state toggle start/stop safety.

### C. INI Configuration Persistence (`IniFileServiceTests`)
- Reading/writing String, Int, and Bool values.
- Safe default fallback handling when keys/sections are missing.

### D. Profile Management (`ProfileServiceTests`) - NEW
- Constructor default profile initialization.
- Creating profiles, preventing duplicate/empty profile names.
- Deleting custom profiles and preventing deletion of `Default` profile.
- Renaming profiles safely.
- Profile config path generation & active profile switching.

### E. Computer Vision & Pixel Detection (`ScreenCaptureServiceTests`) - NEW
- Exact color matching & tolerance color matching.
- Color hex conversion (`ColorToHex` / `HexToColor`) roundtrip & fallback.
- Pixel-by-pixel image comparison (`CompareImages`).
- Region pixel search by color (`FindPixelByColor`).

### F. Screen-Reactive Bot Engine (`PixelWatcherServiceTests`) - NEW
- Adding, removing, clearing watch conditions.
- Asynchronous background watcher event triggering on target color appearance (`WaitForColor`).
- Asynchronous background watcher event triggering on pixel color change (`WaitForChange`).

### G. ViewModel Logic, Macro Editor & UI Commands (`MainViewModelTests`)
- Adding/deleting actions (`AddKeyboardAction`, `AddClickAction`, `DeleteAction`).
- Loop mode selection & `IsCustomLoopVisible` toggle.
- Hotkey persistent config save/load & reset commands.
- Macro Editor: Move action up/down reordering (`MoveActionUpAndDown`).
- Macro Editor: Duplicating selected action (`DuplicateAction`).
- Profile dropdown initialization (`Profiles_PopulatedWithDefaultProfile`).
- Assembly version format validation (`AppVersion_ReturnsFormattedAssemblyVersion`).
