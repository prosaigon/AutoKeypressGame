# 🎮 Auto Clicker & Keyboard

Một ứng dụng tự động hóa thao tác bàn phím và chuột hiện đại, được phát triển bằng C# WPF với .NET Framework 4.6.2.

## ✨ Tính năng chính

- **⌨️ Tự động nhấn phím**: Ghi lại và phát lại các thao tác nhấn phím
- **🖱️ Tự động click chuột**: Click tại vị trí chỉ định với các nút trái/phải/giữa
- **📋 Chuỗi hành động tuần tự**: Thực hiện nhiều hành động theo thứ tự
- **🔄 Chế độ lặp**:
  - Không lặp (No Loop)
  - Lặp một lần (Loop Once)
  - Lặp liên tục (Continuous)
  - Lặp tùy chỉnh (Custom)
- **⌨️ Phím tắt**: Kích hoạt/dừng bằng phím tắt (mặc định F6)
- **💾 Lưu cấu hình**: Lưu và tải thiết lập từ file .ini
- **🎨 Giao diện hiện đại**: UI đẹp mắt, dễ sử dụng

## 📋 Yêu cầu hệ thống

- Windows 7/8/10/11
- .NET Framework 4.6.2 trở lên
- Visual Studio 2015 trở lên (để build)

## 🚀 Cài đặt

### Cách 1: Build từ source code

1. Mở solution trong Visual Studio
2. Restore NuGet packages:
   ```
   Install-Package Microsoft.Xaml.Behaviors.Wpf -Version 1.1.39
   ```
3. Build project (Ctrl+Shift+B)
4. Chạy ứng dụng từ thư mục `bin/Debug` hoặc `bin/Release`

### Cách 2: Sử dụng file đã build

Nếu có file exe đã build, chỉ cần:
1. Giải nén thư mục
2. Chạy `AutoClicker.exe`

## 📖 Hướng dẫn sử dụng

### 1. Thêm hành động

#### Thêm phím bấm:
- Nhấn nút **"⌨️ Add Key"** để thêm một hành động nhấn phím
- Mặc định sẽ thêm phím "A", bạn có thể chỉnh sửa trong danh sách

#### Thêm click chuột:
- Di chuyển con trỏ đến vị trí muốn click
- Nhấn nút **"🖱️ Add Click"** để ghi nhận vị trí hiện tại
- Hoặc nhấn **"📍 Get Position"** để xem tọa độ hiện tại

### 2. Cấu hình hành động

Mỗi hành động có thể chỉnh sửa:
- **Loại hành động**: Keyboard hoặc MouseClick
- **Phím/Nút**: Tên phím hoặc loại nút chuột
- **Vị trí**: Tọa độ X, Y (đối với click chuột)
- **Độ trễ**: Thời gian chờ sau khi thực hiện (ms)

### 3. Thiết lập vòng lặp

Chọn chế độ lặp trong phần **Loop Settings**:
- **No Loop**: Chỉ chạy một lần
- **Loop Once**: Chạy 2 lần
- **Continuous**: Chạy liên tục cho đến khi dừng
- **Custom**: Nhập số lần lặp tùy chỉnh

### 4. Thiết lập phím tắt

- Click vào ô **Start/Stop** trong phần Hotkeys
- Nhấn tổ hợp phím muốn sử dụng (ví dụ: F6, CTRL+SHIFT+A, ALT+F1...)
- Phím tắt mặc định là **F6**

### 5. Bắt đầu/Dừng

- Nhấn **"▶️ Start"** để bắt đầu tự động hóa
- Nhấn **"⏹️ Stop"** để dừng
- Hoặc sử dụng phím tắt đã thiết lập

### 6. Lưu/Tải cấu hình

- **💾 Save Config**: Lưu tất cả thiết lập vào file `config.ini`
- **📂 Load Config**: Tải thiết lập từ file `config.ini`

## 📁 Cấu trúc file

```
AutoClicker/
├── AutoClicker.csproj          # File project
├── App.xaml                    # Application definition
├── App.xaml.cs                 # Application code-behind
├── MainWindow.xaml             # Main window UI
├── MainWindow.xaml.cs          # Main window code-behind
├── Models/
│   └── ActionItem.cs           # Model for action items
├── ViewModels/
│   └── MainViewModel.cs        # ViewModel for MVVM pattern
├── Services/
│   ├── IniFileService.cs       # Service for reading/writing INI files
│   └── InputSimulatorService.cs # Service for simulating keyboard/mouse
├── Converters/
│   └── BoolToVisibilityConverter.cs # Value converter for WPF
├── packages.config             # NuGet packages
├── app.config                  # Application configuration
└── config.ini                  # User configuration (generated at runtime)
```

## 🔧 File cấu hình (config.ini)

File `config.ini` được tạo tự động khi lưu cấu hình:

```ini
; Auto Clicker Configuration File
[Settings]
LoopMode=No Loop
LoopIterations=1
DefaultDelay=100
Hotkey=F6
PlaySound=False

[Actions]
Count=2
Action0_Type=Keyboard
Action0_KeyOrButton=A
Action0_Delay=100
Action1_Type=MouseClick
Action1_MouseButton=Left
Action1_X=500
Action1_Y=300
Action1_Delay=100
```

## 🎯 Các phím hỗ trợ

### Phím chữ và số:
- A-Z, 0-9

### Phím đặc biệt:
- ENTER, SPACE, TAB, ESCAPE
- SHIFT, CTRL, ALT
- LEFT, UP, RIGHT, DOWN
- DELETE, INSERT, HOME, END
- PAGEUP, PAGEDOWN
- F1-F12

### Nút chuột:
- Left (Trái)
- Right (Phải)
- Middle (Giữa)

## ⚠️ Lưu ý quan trọng

1. **Quyền admin**: Một số game/application yêu cầu chạy với quyền Administrator
2. **Chống cheat**: Một số game có cơ chế chống auto click, có thể không hoạt động
3. **Sử dụng hợp lý**: Chỉ sử dụng cho mục đích cá nhân, không vi phạm điều khoản dịch vụ
4. **Tạm dừng**: Luôn sẵn sàng dừng bằng phím tắt khi cần thiết

## 🛠️ Build từ command line

```bash
# Sử dụng MSBuild
msbuild AutoClicker.csproj /p:Configuration=Release

# Hoặc sử dụng Visual Studio Developer Command Prompt
devenv AutoClicker.sln /build Release
```

## 📝 License

Dự án mã nguồn mở, tự do sử dụng cho mục đích cá nhân.

## 🤝 Đóng góp

Mọi đóng góp về tính năng hoặc báo lỗi đều được chào đón!

---

**Phát triển với ❤️ bởi C# WPF**
