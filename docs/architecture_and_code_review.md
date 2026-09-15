# Group 2: Architecture, Code Evaluation & Compliance Audit
> **Group ID**: `code_architecture`  
> **File Path**: `docs/architecture_and_code_review.md`  
> **Last Updated**: `2026-09-15 22:18:00`  
> **Current Version**: `v2.0.0`  

## Revision History
| Version | Timestamp | Description |
| :--- | :--- | :--- |
| `v1.0.0` | 2026-09-06 02:00:00 | Khởi tạo tài liệu đánh giá mã nguồn `code_evaluation.md` |
| `v1.1.0` | 2026-09-06 04:15:00 | Thực hiện audit tuân thủ quy tắc workspace `rules_compliance_audit.md` |
| `v1.2.0` | 2026-09-06 11:50:00 | Hợp nhất vào tài liệu nhóm duy nhất `architecture_and_code_review.md` theo quy tắc mới |
| `v1.3.0` | 2026-09-06 11:53:30 | Đưa quy tắc cập nhật đồng bộ Assembly Version vào GEMINI.md & AGENTS.md |
| `v1.4.0` | 2026-09-06 12:05:00 | Cập nhật quy tắc Push Dev, PR Approval Master, dùng MCP/Git, và quản lý GitHub Issue |
| `v2.0.0` | 2026-09-15 22:18:00 | Nâng cấp kiến trúc v2.0.0: ProfileService, ScreenCaptureService, PixelWatcherService, 96 tests Passed |

---

## 1. Code Base Architecture & Evaluation

### Overview
Dự án **AutoKeypressGame** (`AutoClicker`) là ứng dụng WPF (.NET 8.0 Windows) tự động hóa bàn phím, chuột và phản xạ theo màn hình (Computer Vision Bot).

### Architecture Components
1. **Core Domain & Models (`AutoClicker.Models`)**:
   - `ActionItem.cs`: Định nghĩa model hành động hỗ trợ Keyboard, MouseClick, WaitForPixelColor, WaitForPixelChange, ConditionalPixelColor.
   - `ProfileModel.cs`: Model cấu hình profile (Tên, Thư mục, Thời gian sửa đổi).
2. **Service Layer (`AutoClicker.Services`)**:
   - `InputSimulatorService.cs`: Giả lập phím/chuột bằng Win32 `SendInput` kernel-level API.
   - `InputRecorderService.cs`: Ghi nhận thao tác bằng Win32 Low-Level Hooks (`WH_KEYBOARD_LL`, `WH_MOUSE_LL`).
   - `IniFileService.cs`: Đọc/ghi file INI an toàn.
   - `ProfileService.cs`: Quản lý danh sách, tạo mới, xóa và nạp cấu hình đa profile.
   - `ScreenCaptureService.cs`: Xử lý thị giác máy tính Win32 GDI+ thuần (`BitBlt`, `GetPixel`, `CaptureRegion`, `CompareImages`, `FindPixelByColor`).
   - `PixelWatcherService.cs`: Engine giám sát pixel chạy ngầm (Background Worker) phục vụ bot phản xạ.
3. **Presentation & ViewModels (`AutoClicker.ViewModels`)**:
   - `MainViewModel.cs`: Quản lý Automation Pipeline, Vision Bot triggers, Macro Editor commands, Profile switching và đồng bộ `AppVersion`.
4. **UI Layer (`AutoClicker.MainWindow`)**:
   - `MainWindow.xaml` & `MainWindow.xaml.cs`: Giao diện WPF hiện đại, thanh chọn Profile, Macro Editor, Panel điều chỉnh thông số thị giác và đăng ký Win32 Hotkeys.

---

## 2. Rules Compliance Audit Report

### Audit Summary
- **Workspace Documentation Rule**: 100% tài liệu được duy trì trong `docs/` phân theo nhóm file duy nhất với timestamp `YYYY-MM-DD HH:mm:ss` và `Version`.
- **Dev Branch & Push Rule**: 100% commit và thay đổi mã nguồn được thực hiện và push trên nhánh `Dev` trước.
- **Master Approval & Merge Rule**: Yêu cầu Pull Request và sự đồng ý phê duyệt (Approve) từ master (maintainer) trước khi merge vào `main`/`master`.
- **MCP & Git Tool Rule**: Ưu tiên sử dụng MCP tools (`github` server) và Git CLI cho thao tác repository.
- **GitHub Issue Workflow Rule**: Tạo GitHub Issue trước cho mọi yêu cầu mới/chỉnh sửa, cập nhật tiến độ và đóng Issue khi hoàn thành.
- **Assembly Version Sync Rule**: Đồng bộ phiên bản Assembly `2.0.0` trong `.csproj`, ViewModel và giao diện ứng dụng.
- **Automated Testing Rule**: 100% thay đổi mã nguồn có unit test tương ứng (**96 / 96 Passed**).
- **Build Quality**: Solution biên dịch thành công **0 Warnings, 0 Errors**.
