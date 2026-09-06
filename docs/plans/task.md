# Group 1: Task Tracker & Progress Log
> **Group ID**: `progress_tracker`  
> **File Path**: `docs/plans/task.md`  
> **Last Updated**: `2026-09-06 12:05:00`  
> **Current Version**: `v1.12.0`  

## Revision History
| Version | Timestamp | Description |
| :--- | :--- | :--- |
| `v1.0.0` | 2026-09-06 02:00:00 | Khởi tạo bảng checklist tiến độ TASK-1 đến TASK-9 |
| `v1.1.0` | 2026-09-06 03:00:00 | Thêm tính năng Recording TASK-10 đến TASK-14 |
| `v1.2.0` | 2026-09-06 04:00:00 | Thêm phím tắt tùy chỉnh & Audit TASK-15 đến TASK-20 |
| `v1.3.0` | 2026-09-06 04:40:00 | Thêm cải tiến UI & Focus Hotkey TASK-21 đến TASK-25 |
| `v1.4.0` | 2026-09-06 11:45:00 | Thêm hỗ trợ phím Numpad TASK-26 đến TASK-30 |
| `v1.5.0` | 2026-09-06 11:50:00 | Cập nhật quy tắc phân nhóm tài liệu & định dạng timestamp/version |
| `v1.6.0` | 2026-09-06 11:53:00 | Cập nhật Assembly Version 1.5.0 vào `.csproj`, ViewModel & UI Header |
| `v1.7.0` | 2026-09-06 11:55:00 | Tạo `.github/workflows/` (`cli.yml`, `playwright-test.yml`, `deploy.yml`) |
| `v1.8.0` | 2026-09-06 11:57:30 | Gỡ bỏ `playwright-test.yml` không cần thiết đối với ứng dụng Desktop |
| `v1.9.0` | 2026-09-06 11:59:30 | Khởi tạo và checkout nhánh `Dev`, commit toàn bộ mã nguồn & tài liệu dự án |
| `v1.10.0` | 2026-09-06 12:00:30 | Cập nhật `cli.yml` thêm nhánh `Dev`/`dev` vào danh sách push & pull_request triggers |
| `v1.11.0` | 2026-09-06 12:01:30 | Thiết lập quy định Merge PR từ `Dev` sang `main`/`master` (Cần `.github/CODEOWNERS` & Review Approval) |
| `v1.12.0` | 2026-09-06 12:05:00 | Đưa bổ sung quy tắc mới (Push Dev, Master Approve, dùng MCP/Git, GitHub Issues Workflow) vào GEMINI.md & AGENTS.md |

---

## Live Task Tracker

| Task ID | Task Description | Status | Target File(s) | Verification |
| :--- | :--- | :--- | :--- | :--- |
| TASK-1 | Tạo tài liệu đánh giá mã nguồn `docs/code_evaluation.md` & `docs/plans/task.md` | Completed | `docs/code_evaluation.md`, `docs/plans/task.md` | Kiểm tra sự tồn tại của các file tài liệu |
| TASK-2 | Refactor Win32 Interop `InputSimulatorService.cs` (x64 Alignment, `SendInput` cho Mouse, bổ sung Virtual Keys) | Completed | `AutoClicker/Services/InputSimulatorService.cs` | Đã chuẩn hóa struct INPUT uint, SendInput & key parsing |
| TASK-3 | Refactor Hotkey Listener sang Win32 `RegisterHotKey` / `UnregisterHotKey` qua `HwndSource` | Completed | `AutoClicker/MainWindow.xaml.cs`, `AutoClicker/ViewModels/MainViewModel.cs` | Đã chuyển sang RegisterHotKey qua HwndSource Hook |
| TASK-4 | Sửa lỗi `Enum.TryParse`, An toàn Cancellation & UI Dispatcher trong `MainViewModel.cs` | Completed | `AutoClicker/ViewModels/MainViewModel.cs` | Đã dùng Enum.TryParse và kiểm tra null Dispatcher |
| TASK-5 | Biên dịch hệ thống & Kiểm tra tổng thể | Completed | `AutoClicker/AutoClicker.csproj` | Lệnh `dotnet build` thành công: 0 Warning, 0 Error |
| TASK-6 | Tạo dự án Unit Test `AutoClicker.Tests.csproj` (xUnit) & Thư mục `docs/unit_testing_strategy.md` | Completed | `AutoClicker.Tests/AutoClicker.Tests.csproj`, `docs/unit_testing_strategy.md` | `dotnet build AutoClicker.Tests.csproj` thành công |
| TASK-7 | Viết Unit Test cho `InputSimulatorServiceTests` & `ActionItemTests` | Completed | `AutoClicker.Tests/Services/InputSimulatorServiceTests.cs`, `AutoClicker.Tests/Models/ActionItemTests.cs` | Test case cho phím đơn, phím combo, phím dấu & model |
| TASK-8 | Viết Unit Test cho `IniFileServiceTests` & `MainViewModelTests` | Completed | `AutoClicker.Tests/Services/IniFileServiceTests.cs`, `AutoClicker.Tests/ViewModels/MainViewModelTests.cs` | Test case cho ghi/đọc INI & nạp config an toàn |
| TASK-9 | Kiểm thử tự động (Auto Test) & Xác minh toàn bộ Test Suite PASS 100% | Completed | `AutoClicker.Tests/` | `dotnet test` PASS 35/35 (100% Passed) |
| TASK-10 | Tạo tài liệu thiết kế tính năng Record & Playback `docs/recording_feature_design.md` | Completed | `docs/recording_feature_design.md` | Đã tạo file thiết kế ghi thao tác |
| TASK-11 | Xây dựng Service Ghi Thao Tác `InputRecorderService.cs` dùng Win32 Hooks | Completed | `AutoClicker/Services/InputRecorderService.cs` | Đã tạo service InputRecorderService |
| TASK-12 | Cập nhật ViewModel & UI với Nút Record & Hotkey `F7` | Completed | `AutoClicker/ViewModels/MainViewModel.cs`, `AutoClicker/MainWindow.xaml` | Giao diện hiển thị nút Record & phản hồi trạng thái |
| TASK-13 | Bổ sung Unit Test cho `InputRecorderServiceTests.cs` | Completed | `AutoClicker.Tests/Services/InputRecorderServiceTests.cs` | Chạy `dotnet test` |
| TASK-14 | Biên dịch solution & Chạy lại toàn bộ bộ Auto Test | Completed | `AutoKeypressGame.sln` | `dotnet test` PASS 47/47 (100% Passed) |
| TASK-15 | Audit tuân thủ quy tắc `docs/rules_compliance_audit.md` | Completed | `docs/rules_compliance_audit.md` | Đã kiểm tra 100% tuân thủ Documentation & Testing Rules |
| TASK-16 | Tạo tài liệu thiết kế hệ thống Phím Tắt Tùy Chỉnh `docs/hotkey_system_design.md` | Completed | `docs/hotkey_system_design.md` | Đã tạo file thiết kế hệ thống phím tắt tùy chỉnh |
| TASK-17 | Cập nhật `MainViewModel.cs` nạp/lưu 4 Phím Tắt (F6, F7, F8, F9) & Xử lý Pause/Resume | Completed | `AutoClicker/ViewModels/MainViewModel.cs` | Đã cập nhật Save/LoadConfig & Pause/Resume state |
| TASK-18 | Cập nhật `MainWindow.xaml.cs` & `MainWindow.xaml` cho 4 Phím Tắt tùy chỉnh | Completed | `AutoClicker/MainWindow.xaml.cs`, `AutoClicker/MainWindow.xaml` | Đã đăng ký 4 Win32 Hotkeys & UI Settings |
| TASK-19 | Bổ sung Unit Tests cho tính năng Lưu Phím Tắt & Pause/Clear Actions | Completed | `AutoClicker.Tests/` | `dotnet test` |
| TASK-20 | Biên dịch Solution & Xác minh 100% Auto Test PASS | Completed | `AutoKeypressGame.sln` | `dotnet test` PASS 48/48 (100% Passed) |
| TASK-21 | Tạo tài liệu thiết kế cải tiến Giao diện & Tương tác Phím Tắt `docs/ui_and_hotkey_redesign.md` | Completed | `docs/ui_and_hotkey_redesign.md` | Đã tạo file thiết kế cải tiến UI & Hotkeys |
| TASK-22 | Cập nhật `MainViewModel.cs` bổ sung Reset Hotkeys & Pause Button state | Completed | `AutoClicker/ViewModels/MainViewModel.cs` | Đã thêm Reset Commands & PauseButtonText property |
| TASK-23 | Đồng nhất Labels, Icons và thêm nút `⏸️ Pause`, `🗑️ Clear` trong `MainWindow.xaml` | Completed | `AutoClicker/MainWindow.xaml` | Đã đồng nhất labels/icons, thêm nút Pause và Clear All |
| TASK-24 | Nâng cấp cơ chế tương tác bắt phím trực quan (Focus Prompt) trong `MainWindow.xaml.cs` | Completed | `AutoClicker/MainWindow.xaml.cs` | Bắt phím trực quan khi focus và tự động bỏ focus |
| TASK-25 | Bổ sung Unit Tests mới & Xác minh toàn bộ Solution PASS 100% | Completed | `AutoClicker.Tests/` | Lệnh `dotnet test` PASS 51/51 (100% Passed) |
| TASK-26 | Tạo tài liệu thiết kế hỗ trợ Numpad `docs/numpad_support_design.md` | Completed | `docs/numpad_support_design.md` | Đã tạo file thiết kế hỗ trợ phím Numpad |
| TASK-27 | Cập nhật `InputRecorderService.cs` ánh xạ VK Codes Numpad (0x60-0x6F, 0x90) | Completed | `AutoClicker/Services/InputRecorderService.cs` | Đã hỗ trợ ghi nhận tên phím Numpad (NUMPAD0..9, *, +, -, ., /, NUMLOCK) |
| TASK-28 | Cập nhật `InputSimulatorService.cs` ánh xạ chuỗi Numpad sang Virtual Keys | Completed | `AutoClicker/Services/InputSimulatorService.cs` | Đã ánh xạ tên phím Numpad sang VK_NUMPAD0..9 & VK_NUMLOCK |
| TASK-29 | Cập nhật `MainWindow.xaml.cs` nhận diện phím Numpad khi cài đặt Hotkey | Completed | `AutoClicker/MainWindow.xaml.cs` | Đã hỗ trợ gán phím Numpad trong ô cài đặt Hotkey |
| TASK-30 | Bổ sung Unit Test cho Numpad & Xác minh toàn bộ Test Suite PASS 100% | Completed | `AutoClicker.Tests/` | `dotnet test` PASS 66/66 (100% Passed) |
| TASK-31 | Cập nhật quy tắc lưu tài liệu theo Nhóm (1 File / Nhóm) & Timestamp / Version Tracking | Completed | `GEMINI.md`, `.agent/AGENTS.md`, `docs/` | Đã phân nhóm tài liệu docs/ & cập nhật quy tắc workspace |
| TASK-32 | Đồng bộ Assembly Version `1.5.0` vào `.csproj`, ViewModel & WPF UI Header | Completed | `AutoClicker.csproj`, `MainViewModel.cs`, `MainWindow.xaml`, `AutoClicker.Tests/` | Assembly Version & UI hiển thị `v1.5.0`, `dotnet test` PASS 67/67 |
| TASK-33 | Tạo hệ thống CI/CD Workflows (`cli.yml`, `deploy.yml`) cho WPF Desktop | Completed | `.github/workflows/` | Kiểm tra sự tồn tại của `cli.yml` & `deploy.yml` |
| TASK-34 | Khởi tạo nhánh `Dev` & commit toàn bộ thay đổi dự án | Completed | Git Branch `Dev` | Nhánh `Dev` đã được khởi tạo và commit 100% mã nguồn & tài liệu |
| TASK-35 | Cập nhật `cli.yml` hỗ trợ kích hoạt CI/CD khi push/PR lên nhánh `Dev` | Completed | `.github/workflows/cli.yml` | `cli.yml` đã được cập nhật trigger nhánh `Dev` & `dev` |
| TASK-36 | Tạo `.github/CODEOWNERS` & Quy định Phê duyệt Review khi Merge PR từ `Dev` sang `main`/`master` | Completed | `.github/CODEOWNERS`, `docs/system_features_design.md` | Đã thiết lập file CODEOWNERS và quy định phê duyệt PR Merge |
| TASK-37 | Bổ sung 4 quy tắc workspace mới (Push Dev, Master Approve Merge, MCP/Git, GitHub Issues) | Completed | `GEMINI.md`, `.agent/AGENTS.md`, `docs/` | Đã cập nhật quy tắc trong GEMINI.md, AGENTS.md và docs |
