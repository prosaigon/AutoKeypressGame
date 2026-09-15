# 🎮 Auto Clicker & Keyboard Automation Tool (v2.0.0)

> **Hệ Thống Tự Động Hóa Bàn Phím, Chuột & Bot Phản Xạ Màn Hình (Computer Vision) Cho Windows (.NET 8.0 WPF)**

![.NET 8.0](https://img.shields.io/badge/.NET-8.0--windows-512BD4?logo=dotnet)
![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen?logo=github-actions)
![Tests](https://img.shields.io/badge/Unit%20Tests-96%2F96%20Passed-success)
![Version](https://img.shields.io/badge/Version-2.0.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?logo=windows)

---

## 🌟 Giới Thiệu Sản Phẩm (Overview)

**AutoKeypressGame (`AutoClicker`)** là giải pháp phần mềm tự động hóa thao tác người dùng (Keyboard & Mouse Automation) kết hợp **Thị giác máy tính (Computer Vision Bot Engine)** thế hệ mới trên hệ điều hành Windows. Được xây dựng chuẩn hóa trên nền tảng **.NET 8.0 WPF (Windows Presentation Foundation)** và **Win32 Native Interop API**, ứng dụng giải quyết triệt để vấn đề giật lag, trễ thao tác và lãng phí tài nguyên CPU của các công cụ tự động truyền thống.

Ứng dụng đáp ứng hoàn hảo cho cả nhu cầu **Automation Testing**, **Tự động hóa công việc lặp đi lặp lại (RPA)**, **Tự động phản xạ theo màn hình game (Screen-Reactive Gaming Bot)** và **Quản lý đa kịch bản (Multi-Profile Presets)**.

---

## 🚀 Các Điểm Mạnh Khoa Học & Kỹ Thuật Nổi Bật (Core Strengths)

### 1. 👁️ Động Cơ Thị Giác Máy Tính Tốc Độ Cao (Screen-Reactive Vision Bot)
- Tích hợp `ScreenCaptureService` và `PixelWatcherService` sử dụng Win32 GDI+ thuần (`BitBlt`, `GetPixel`), không phụ thuộc thư viện bên ngoài cồng kềnh.
- Hỗ trợ các hành động thị giác nâng cao:
  - **`🎯 Pixel Wait`**: Tự động dừng luồng cho tới khi pixel tại tọa độ đạt đúng màu sắc mục tiêu (`#RRGGBB`) với mức dung sai (Tolerance 0-255).
  - **`👁️ Vision Watch`**: Bot phản xạ thông minh — phát hiện màu sắc trên màn hình và tự động kích hoạt tổ hợp phím/chuột tương ứng.
  - **`🎨 Pick Color`**: Trích xuất mã màu và tọa độ tức thời tại vị trí con trỏ chuột.

### 2. 📁 Hệ Thống Đa Cấu Hình & Preset Độc Lập (Multi-Profile System)
- Quản lý linh hoạt danh sách cấu hình (`ProfileService`), lưu trữ độc lập tại `profiles/{ProfileName}/config.ini`.
- Chuyển đổi nhanh chóng giữa các kịch bản chơi game / công việc khác nhau ngay trên thanh Header bar mà không cần khởi động lại ứng dụng.

### 3. 📋 Trình Chỉnh Sửa Kịch Bản Trực Quan (Macro Editor & Inspector)
- Hỗ trợ sắp xếp lại thứ tự bước chạy bằng nút `↑ Move Up` / `↓ Move Down`.
- Nhân bản nhanh bước thao tác bằng nút `📋 Duplicate`.
- Bảng **`🎯 Action Details & Vision`** bên phải cho phép điều chỉnh trực tiếp phím bấm, thời gian delay, màu sắc và timeout của từng hành động.

### 4. ⚙️ Động Cơ Giả Lập Win32 Native (`SendInput` Engine)
- Sử dụng trực tiếp Win32 API `SendInput` để bơm sự kiện bàn phím/chuột ở cấp độ nhân (Kernel-level simulation).
- Chuẩn hóa cấu trúc bộ nhớ `INPUT` struct với `uint` alignment chuẩn xác cho kiến trúc x64, ngăn ngừa lỗi sai lệch bộ nhớ (memory misalignment) và xung đột tiến trình.
- Giữ ứng dụng chạy nhẹ nhàng với **< 0.5% CPU Usage**.

### 5. 🔴 Bộ Ghi Thao Tác Thời Gian Thực (Low-Level Hook Input Recorder)
- Tích hợp service `InputRecorderService` sử dụng Win32 Hooks (`WH_KEYBOARD_LL` và `WH_MOUSE_LL`) để bắt trọn từng thao tác phím bấm, nhấp chuột và tọa độ màn hình `(X, Y)` với khoảng trễ trôi qua (delay) thực tế tính bằng miligiây.

### 6. ⌨️ Hỗ Trợ Toàn Diện Bàn Phím Số Sub-Keypad (Full Numpad Support)
- Ánh xạ chuẩn xác 100% tất cả các phím số phụ **Numpad 0 đến Numpad 9** (`VK_NUMPAD0` - `VK_NUMPAD9`), các phím phép tính toán tử (`+`, `-`, `*`, `/`, `.`, `,`) và trạng thái phím **NumLock**.

### 7. ⚡ Hệ Thống Phím Tắt Toàn Cục An Toàn (Win32 Global Hotkeys)
- Lắng nghe 4 phím tắt nhanh (`F6`: Start/Stop, `F7`: Record, `F8`: Pause/Resume, `F9`: Clear Actions) trên toàn hệ điều hành thông qua Win32 API `RegisterHotKey` gắn với WPF `HwndSource` Hook.

### 8. 🧪 Kiểm Thử Tự Động 100% & CI/CD Pipeline (xUnit & GitHub Actions)
- Bộ kiểm thử tự động với **96 / 96 Test Cases PASS (100%)** phủ rộng toàn bộ các modules.
- Tự động hóa quy trình tích hợp và đóng gói liên tục qua GitHub Actions (`.github/workflows/cli.yml` và `deploy.yml`).

---

## 🏗️ Kiến Trúc Hệ Thống (System Architecture)

```mermaid
flowchart TD
    UI[🖥️ WPF User Interface / MainWindow] -->|Data Binding / Commands| VM[🧠 MainViewModel]
    VM -->|Multi-Profile System| PS[📁 ProfileService - profiles/]
    PS -->|Config Persistence| INI[📄 IniFileService]
    VM -->|Automation Exec| SIM[⚡ InputSimulatorService]
    VM -->|Record Input| REC[🔴 InputRecorderService]
    VM -->|Computer Vision Bot| SCS[👁️ ScreenCaptureService]
    VM -->|Background Monitor| PWS[📡 PixelWatcherService]
    
    SIM -->|Win32 SendInput| WIN32[🪟 Windows OS Kernel]
    REC -->|Win32 Hooks WH_KEYBOARD_LL / WH_MOUSE_LL| WIN32
    SCS -->|Win32 GetDC / BitBlt / GDI+| WIN32
    UI -->|HwndSource Hook / RegisterHotKey| WIN32
```

---

## 📊 Phân Nhóm Tài Liệu Dự Án (Documentation Structure)

Tài liệu kỹ thuật của dự án được lưu trữ trong thư mục `docs/` được phân chia thành **4 nhóm duy nhất**:

| Nhóm Tài Liệu | Thư Mục / Đường Dẫn | Nội Dung Quản Lý |
| :--- | :--- | :--- |
| **Group 1** | [`docs/plans/task.md`](file:///d:/project/gemini-learning/AutoKeypressGame/docs/plans/task.md) | **Task Tracker & Progress Log**: Nhật ký tiến độ công việc (TASK-1 đến TASK-37) |
| **Group 2** | [`docs/architecture_and_code_review.md`](file:///d:/project/gemini-learning/AutoKeypressGame/docs/architecture_and_code_review.md) | **Architecture & Code Review**: Đánh giá kiến trúc mã nguồn & Audit tuân thủ quy tắc |
| **Group 3** | [`docs/system_features_design.md`](file:///d:/project/gemini-learning/AutoKeypressGame/docs/system_features_design.md) | **System Features Design**: Thiết kế kỹ thuật chi tiết của tất cả các tính năng |
| **Group 4** | [`docs/unit_testing_strategy.md`](file:///d:/project/gemini-learning/AutoKeypressGame/docs/unit_testing_strategy.md) | **Automated Testing Strategy**: Chiến lược & báo cáo kiểm thử unit test tự động |

---

## 🛠️ Hướng Dẫn Biên Dịch & Chạy Dự Án (Build & Run)

### Yêu Cầu Môi Trường
- **OS**: Windows 10 / Windows 11 (64-bit)
- **SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) hoặc mới hơn
- **IDE Khuyên Dùng**: Visual Studio 2022 / VS Code / JetBrains Rider

### Lệnh Biên Dịch & Kiểm Thử
```bash
# 1. Clone repository
git clone https://github.com/prosaigon/AutoKeypressGame.git
cd AutoKeypressGame

# 2. Restore dependencies
dotnet restore AutoKeypressGame.sln

# 3. Biên dịch dự án (Build)
dotnet build AutoKeypressGame.sln --configuration Release

# 4. Chạy bộ tự động kiểm thử (Unit Tests)
dotnet test AutoKeypressGame.sln

# 5. Khởi chạy ứng dụng
dotnet run --project AutoClicker/AutoClicker.csproj
```

---

## 🛡️ Quy Trình Đóng Góp & Phát Triển (Contribution & Dev Workflow)

1. Mọi commit và nâng cấp mã nguồn bắt buộc thực hiện trên nhánh **`Dev`**.
2. Khi hoàn thành, tạo **Pull Request (PR)** từ `Dev` vào `main`.
3. Yêu cầu **Pass 100% CI Status Checks** (`cli.yml`) và nhận được **Approve** từ Maintainer (theo `.github/CODEOWNERS`) mới được phép Merge.

---

## 📄 Bản Quyền (License)
Dự án được phát hành dưới bản quyền [MIT License](LICENSE).