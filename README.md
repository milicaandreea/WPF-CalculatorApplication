#  CalculatorApplicationWPF

A feature-rich calculator application built with WPF and C#, designed to replicate the functionality of the Windows Calculator. The project is structured using the MVVM architectural pattern, ensuring a clear separation between UI and logic. It supports two operational modes (Standard and Programmer), persistent user settings, full keyboard support, digit grouping, memory operations, and copy-paste functionalities.

## ⚙️ Features

###  Modes & Input
- **Standard Mode**:
  - Arithmetic: `+`, `-`, `×`, `÷`
  - Unary operations: `%`, `√`, `x²`, `1/x`, `+/-`
- **Programmer Mode**:
  - Numerical base selection: `BIN`, `OCT`, `DEC`, `HEX`
  - Input limited to valid characters for the selected base
  - Real-time base conversion

###  Memory System
- Stack-based memory operations:
  - `MS`: Save current value
  - `MR`: Recall last saved value
  - `MC`: Clear memory
  - `M+` / `M-`: Modify top value in memory
  - `M>`: Show entire memory stack

###  Display & Formatting
- Read-only `TextBlock` display
- Locale-aware digit grouping (e.g., `1.000` vs. `1,000`)
- Optional digit grouping saved as user setting

###  Input & Interaction
- Fully supports both **mouse** and **keyboard**
  - `Enter` = Evaluate
  - `ESC` = Clear
  - `Backspace` = Delete last character
- Clipboard support via custom logic: `Cut`, `Copy`, `Paste`

###  Persistence
User preferences are stored using `.NET Properties.Settings`, including:
- Last calculator mode (Standard / Programmer)
- Last selected base (BIN / OCT / DEC / HEX)
- Digit grouping state

###  Architecture & Technologies
- **MVVM pattern** with separate View, ViewModel, and Model layers
- `RelayCommand` for command binding
- `INotifyPropertyChanged` for UI updates
- `IValueConverter` for visibility control
- `CultureInfo` for formatting
- Clipboard access through `System.Windows.Clipboard`

## 📌 Requirements

- Visual Studio 2022 or newer
- .NET Framework (WPF project template)

