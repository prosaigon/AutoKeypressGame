# Group 2: Architecture, Code Evaluation & Compliance Audit
> **Group ID**: `code_architecture`  
> **File Path**: `docs/architecture_and_code_review.md`  
> **Last Updated**: `2026-09-06 12:05:00`  
> **Current Version**: `v1.4.0`  

## Revision History
| Version | Timestamp | Description |
| :--- | :--- | :--- |
| `v1.0.0` | 2026-09-06 02:00:00 | Khởi tạo tài liệu đánh giá mã nguồn `code_evaluation.md` |
| `v1.1.0` | 2026-09-06 04:15:00 | Thực hiện audit tuân thủ quy tắc workspace `rules_compliance_audit.md` |
| `v1.2.0` | 2026-09-06 11:50:00 | Hợp nhất vào tài liệu nhóm duy nhất `architecture_and_code_review.md` theo quy tắc mới |
| `v1.3.0` | 2026-09-06 11:53:30 | Đưa quy tắc cập nhật đồng bộ Assembly Version vào GEMINI.md & AGENTS.md |
| `v1.4.0` | 2026-09-06 12:05:00 | Cập nhật quy tắc Push Dev, PR Approval Master, dùng MCP/Git, và quản lý GitHub Issue |

---

## 1. Code Base Architecture & Evaluation

### Overview
Dự án **AutoKeypressGame** (`AutoClicker`) là ứng dụng WPF (.NET 8.0 Windows) tự động hóa bàn phím và chuột cho trải nghiệm chơi game và công việc lặp đi lặp lại.

### Architecture Components
1. **Core Domain & Models (`AutoClicker.Models`)**:
   - `ActionItem.cs`: Định nghĩa model hành động (Keyboard press, Mouse click) chứa Delay, Coordinate, Key/Button binding.
2. **Win32 Interop Services (`AutoClicker.Services`)**:
   - `InputSimulatorService.cs`: Thực thi giả lập bàn phím và chuột bằng Win32 `SendInput` API với struct alignment uint x64 chuẩn xác.
   - `InputRecorderService.cs`: Thu thập sự kiện chuột & bàn phím toàn hệ thống qua low-level Win32 Hooks (`WH_KEYBOARD_LL`, `WH_MOUSE_LL`).
   - `IniFileService.cs`: Quản lý lưu trữ/nạp cấu hình ứng dụng (`config.ini`) an toàn.
3. **Presentation & ViewModels (`AutoClicker.ViewModels`)**:
   - `MainViewModel.cs`: Quản lý trạng thái automation loop, recording state, hotkey properties, `AppVersion`, và các RelayCommand.
4. **UI Layer (`AutoClicker.MainWindow`)**:
   - `MainWindow.xaml` & `MainWindow.xaml.cs`: Đăng ký Win32 Hotkeys qua `HwndSource` Hook, xử lý tương tác bắt phím `GotFocus`/`PreviewKeyDown` trực quan và hiển thị `AppVersion` trên header.

---

## 2. Rules Compliance Audit Report

### Audit Summary
- **Workspace Documentation Rule**: 100% tài liệu được duy trì trong `docs/` phân theo nhóm file duy nhất với timestamp `YYYY-MM-DD HH:mm:ss` và `Version`.
- **Dev Branch & Push Rule**: 100% commit và thay đổi mã nguồn được thực hiện và push trên nhánh `Dev` trước.
- **Master Approval & Merge Rule**: Yêu cầu Pull Request và sự đồng ý phê duyệt (Approve) từ master (maintainer) trước khi merge vào `main`/`master`.
- **MCP & Git Tool Rule**: Ưu tiên sử dụng MCP tools (`github` server) và Git CLI cho thao tác repository.
- **GitHub Issue Workflow Rule**: Tạo GitHub Issue trước cho mọi yêu cầu mới/chỉnh sửa, cập nhật tiến độ và đóng Issue khi hoàn thành.
- **Assembly Version Sync Rule**: Đồng bộ phiên bản Assembly trong `.csproj` và giao diện ứng dụng khi đổi version.
- **Automated Testing Rule**: 100% thay đổi mã nguồn có unit test tương ứng (`67/67 Passed`).
- **Build Quality**: Solution biên dịch thành công **0 Warnings, 0 Errors**.
