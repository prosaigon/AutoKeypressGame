# Group 3: System Features & Technical Design
> **Group ID**: `feature_design`  
> **File Path**: `docs/system_features_design.md`  
> **Last Updated**: `2026-09-06 12:01:30`  
> **Current Version**: `v1.8.0`  

## Revision History
| Version | Timestamp | Description |
| :--- | :--- | :--- |
| `v1.0.0` | 2026-09-06 03:00:00 | Khởi tạo thiết kế tính năng Ghi thao tác `recording_feature_design.md` |
| `v1.1.0` | 2026-09-06 04:00:00 | Khởi tạo thiết kế hệ thống Phím tắt tùy chỉnh `hotkey_system_design.md` |
| `v1.2.0` | 2026-09-06 04:40:00 | Khởi tạo thiết kế cải tiến UI & Focus Hotkey `ui_and_hotkey_redesign.md` |
| `v1.3.0` | 2026-09-06 11:44:00 | Khởi tạo thiết kế hỗ trợ Numpad `numpad_support_design.md` |
| `v1.4.0` | 2026-09-06 11:50:00 | Hợp nhất tất cả thiết kế tính năng vào tài liệu nhóm duy nhất `system_features_design.md` |
| `v1.5.0` | 2026-09-06 11:55:00 | Thêm cấu hình GitHub Actions Workflows |
| `v1.6.0` | 2026-09-06 11:57:30 | Gỡ bỏ `playwright-test.yml` (dự án WPF Desktop), giữ lại `cli.yml` và `deploy.yml` |
| `v1.7.0` | 2026-09-06 12:00:30 | Cập nhật `cli.yml` kích hoạt CI build/test tự động cho nhánh `Dev` |
| `v1.8.0` | 2026-09-06 12:01:30 | Thiết lập quy trình Merge PR từ `Dev` sang `main`/`master` (Cần Review Approval & CI Status Check) |

---

## 1. Feature 1: Input Recording & Playback System

### Description
Cho phép ứng dụng ghi nhận chính xác tất cả thao tác bàn phím và nhấp chuột của người dùng theo thời gian thực (bao gồm tọa độ X, Y và khoảng thời gian trễ Delay giữa các hành động) thông qua Win32 Low-Level Hooks (`WH_KEYBOARD_LL` và `WH_MOUSE_LL`).

---

## 2. Feature 2: Quick-Control Hotkey System

### Description
Cung cấp 4 phím tắt hệ thống (Quick-Control Hotkeys) hoạt động toàn cục:
1. `StartStopHotkey` (Mặc định: `F6`): Bắt đầu / Dừng chạy kịch bản tự động.
2. `RecordHotkey` (Mặc định: `F7`): Bật / Tắt ghi thao tác người dùng.
3. `PauseResumeHotkey` (Mặc định: `F8`): Tạm dừng / Tiếp tục chạy kịch bản.
4. `ClearActionsHotkey` (Mặc định: `F9`): Xóa toàn bộ kịch bản hiện tại.

Tất cả 4 phím tắt được lưu/nạp tự động qua file `config.ini` và đăng ký toàn cục qua Win32 API `RegisterHotKey` / `UnregisterHotKey`.

---

## 3. Feature 3: Interactive UI & Hotkey Focus Mechanism

### Description
- **Focus Prompt**: Khi người dùng click vào ô gán phím tắt, giao diện lập tức hiển thị `"Press key..."`. Khi bấm tổ hợp phím bất kỳ, phím được gán ngay vào ViewModel và tự động bỏ focus (`Keyboard.ClearFocus()`).
- **Label Unification**: Đồng nhất icon và nhãn miêu tả giữa bảng cài đặt phím tắt và các nút chức năng bên dưới (`▶️ Start / ⏹️ Stop`, `🔴 Record / ⏹️ Stop Rec`, `⏸️ Pause / ▶️ Resume`, `🗑️ Clear Actions`).
- **Reset Controls**: Thêm nút `↺` bên cạnh từng phím tắt và nút `🔄 Reset All Hotkeys` để khôi phục nhanh về cấu hình mặc định.

---

## 4. Feature 4: Full Numpad Support (0-9, Operators, NumLock)

### Description
- **Recording**: Ánh xạ mã Win32 Virtual Key `0x60`-`0x6F` và `0x90` thành chuỗi `NUMPAD0`..`NUMPAD9`, `*`, `+`, `-`, `.`, `/`, `NUMLOCK`.
- **Playback**: Giải mã tên phím `NUMPAD0`..`NUMPAD9` về `VK_NUMPAD0` (0x60) .. `VK_NUMPAD9` (0x69) để Win32 `SendInput` giả lập chính xác phím số bàn phím phụ.
- **Hotkey Binding**: Hỗ trợ WPF `Key.NumPad0`..`Key.NumPad9` trong ô thiết lập phím tắt.

---

## 5. Feature 5: Desktop CI/CD GitHub Actions Workflows

### Description
Hệ thống tự động hóa CI/CD tối ưu cho ứng dụng Windows Desktop (.NET 8 WPF) trong thư mục `.github/workflows/`:
1. **`.github/workflows/cli.yml`**: Tự động restore, build Solution `AutoKeypressGame.sln` và chạy toàn bộ unit/auto test suite trên môi trường `windows-latest` mỗi khi push/PR tới các nhánh `main`, `master`, `Dev`, `dev`.
2. **`.github/workflows/deploy.yml`**: Tự động đóng gói bản build Win-x64 SingleFile executable và phát hành GitHub Release khi đẩy Git Tag dạng `v*` hoặc thực thi thủ công.

---

## 6. Feature 6: Pull Request & Branch Protection Merge Policy

### Description
Quy định chặt chẽ luồng tích hợp mã nguồn khi nhánh `Dev` đã hoàn thiện và chuẩn bị merge vào `main` / `master`:
1. **Bắt buộc tạo Pull Request (PR)**: Không cho phép push trực tiếp vào `main`/`master`. Mọi thay đổi phải tạo PR từ `Dev` sang `main`/`master`.
2. **Bắt buộc Pass CI Checks**: Luồng `.github/workflows/cli.yml` phải chạy thành công 100% (PASS 67 unit tests & Build 0 errors).
3. **Bắt buộc được Admin / Maintainer phê duyệt (Review Approval)**: Cần ít nhất 1 xác nhận đồng ý (Approve) từ Quản trị viên/Chủ dự án (theo cấu hình `.github/CODEOWNERS`) mới cho phép ấn nút **Merge Pull Request**.
