using CalculatorWPF.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Globalization;

namespace Calculator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string display = "0";
        public string Display
        {
            get => display;
            set
            {
                display = value;
                OnPropertyChanged();
            }
        }

        public ICommand DigitCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand UnaryOperatorCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemorySaveCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryShowStackCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand ToggleDigitGroupingCommand { get; }

        public ICommand ToggleProgrammerModeCommand { get; }
        public enum CalculatorMode { Standard, Programmer }

        private double? operand = null;
        private string? currentOperator = null;
        private bool justEvaluated = false;
        private MemoryModel memory = new MemoryModel();
        private bool isDigitGroupingEnabled = false;
        private double currentValue = 0;
        private string rawInput = "";

        public bool IsDigitGroupingEnabled
        {
            get => isDigitGroupingEnabled;
            set
            {
                isDigitGroupingEnabled = value;
                FormatDisplay();
                OnPropertyChanged();
            }
        }
        private CalculatorMode mode = CalculatorMode.Standard;
        public CalculatorMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsProgrammerMode));

                rawInput = SelectedBase switch
                {
                    NumberBase.BIN => Convert.ToString((long)currentValue, 2),
                    NumberBase.OCT => Convert.ToString((long)currentValue, 8),
                    NumberBase.DEC => currentValue.ToString(),
                    NumberBase.HEX => Convert.ToString((long)currentValue, 16).ToUpper(),
                    _ => currentValue.ToString()
                };
                FormatDisplay();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsProgrammerMode));

                FormatDisplay();
            }
        }

        public bool IsProgrammerMode => Mode == CalculatorMode.Programmer;
        public enum NumberBase { BIN = 2, OCT = 8, DEC = 10, HEX = 16 }

        private NumberBase selectedBase = NumberBase.DEC;
        public NumberBase SelectedBase
        {
            get => selectedBase;
            set
            {
                selectedBase = value;
                OnPropertyChanged();
                FormatDisplay();
            }
        }

        public ICommand SetBaseCommand { get; }

        public MainViewModel()
        {
            Display = "0";

            DigitCommand = new RelayCommand(param => AppendDigit(param?.ToString() ?? ""));
            OperatorCommand = new RelayCommand(param => SetOperator(param?.ToString() ?? ""));
            EqualCommand = new RelayCommand(_ => Evaluate());
            ClearCommand = new RelayCommand(_ => ClearAll());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            BackspaceCommand = new RelayCommand(_ => Backspace());
            UnaryOperatorCommand = new RelayCommand(param => ApplyUnaryOperator(param?.ToString() ?? ""));
            MemoryClearCommand = new RelayCommand(_ => memory.Clear());
            MemoryRecallCommand = new RelayCommand(_ =>
            {
                var value = memory.Recall();
                if (value != null)
                {
                    currentValue = value.Value;
                    rawInput = currentValue.ToString(CultureInfo.CurrentCulture);
                    justEvaluated = true;
                    FormatDisplay();
                }
            });
            MemorySaveCommand = new RelayCommand(_ =>
            {
                var val = ParseInput();
                if (val != null)
                    memory.Save(val.Value);
            });

            MemoryAddCommand = new RelayCommand(_ =>
            {
                var val = ParseInput();
                if (val != null)
                    memory.Add(val.Value);
            });

            MemorySubtractCommand = new RelayCommand(_ =>
            {
                var val = ParseInput();
                if (val != null)
                    memory.Subtract(val.Value);
            });

            MemoryShowStackCommand = new RelayCommand(_ =>
            {
                var values = memory.GetAllValues();
                if (values.Count > 0)
                {
                    Display = string.Join(", ", values.Select(v =>
                        v % 1 == 0
                            ? ((long)v).ToString()
                            : v.ToString("0.################", CultureInfo.CurrentCulture)));
                }
            });
            CopyCommand = new RelayCommand(_ => Copy());
            PasteCommand = new RelayCommand(_ => Paste());
            CutCommand = new RelayCommand(_ => Cut());
            ToggleDigitGroupingCommand = new RelayCommand(_ =>
            {
                IsDigitGroupingEnabled = !IsDigitGroupingEnabled;
            });

            ToggleProgrammerModeCommand = new RelayCommand(_ =>
            {
                Mode = Mode == CalculatorMode.Standard ? CalculatorMode.Programmer : CalculatorMode.Standard;
            });
            SetBaseCommand = new RelayCommand(param =>
            {
                if (Enum.TryParse(param.ToString(), out NumberBase nb))
                    SelectedBase = nb;
            });
        }

        private void AppendDigit(string digit)
        {
            var nfi = CultureInfo.CurrentCulture.NumberFormat;
            string separator = nfi.NumberDecimalSeparator;

            if (IsProgrammerMode)
            {
                digit = digit.ToUpper();

                string allowed = SelectedBase switch
                {
                    NumberBase.BIN => "01",
                    NumberBase.OCT => "01234567",
                    NumberBase.DEC => "0123456789",
                    NumberBase.HEX => "0123456789ABCDEF",
                    _ => "0123456789"
                };

                if (!allowed.Contains(digit))
                {
                    if (string.IsNullOrEmpty(rawInput))
                    {
                        rawInput = "0";
                        Display = "0";
                    }
                    return;
                }

                if (justEvaluated)
                {
                    if (rawInput != "0")
                        rawInput = "";
                    justEvaluated = false;
                }

                if (rawInput == "0")
                    rawInput = "";

                rawInput += digit;

                try
                {
                    currentValue = Convert.ToInt64(rawInput, (int)SelectedBase);
                    Display = rawInput;
                }
                catch
                {
                    currentValue = 0;
                    Display = "Error";
                }

                return;
            }

            if (digit == "." || digit == ",")
                digit = separator;

            if (!char.IsDigit(digit[0]) && digit != separator)
                return;
            if (justEvaluated)
            {
                rawInput = "";
                justEvaluated = false;
            }

            if (rawInput == "0" && digit != separator)
                rawInput = "";

            if (digit == separator)
            {
                if (rawInput.Contains(separator))
                    return;

                if (string.IsNullOrEmpty(rawInput))
                    rawInput = "0";

                rawInput += separator;
            }
            else
            {
                rawInput += digit;
            }
            if (rawInput.EndsWith(separator))
            {
                if (IsDigitGroupingEnabled)
                {
                    if (double.TryParse(rawInput + "0", NumberStyles.Any, CultureInfo.CurrentCulture, out double temp))
                    {
                        Display = temp.ToString("#,0", CultureInfo.CurrentCulture) + separator;
                    }
                    else
                    {
                        Display = rawInput;
                    }
                }
                else
                {
                    Display = rawInput;
                }
                return;
            }

            if (double.TryParse(rawInput, NumberStyles.Any, CultureInfo.CurrentCulture, out double parsed))
            {
                currentValue = parsed;

                if (IsDigitGroupingEnabled)
                {
                    if (rawInput.Contains(separator))
                    {
                        if (rawInput.EndsWith("0"))
                            Display = rawInput;
                        else
                            Display = parsed.ToString("#,0.################", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        Display = ((long)parsed).ToString("#,0", CultureInfo.CurrentCulture);
                    }
                }

                else
                {
                    Display = rawInput;
                }
            }
            else
            {
                Display = rawInput;
            }
        }
        private void SetOperator(string op)
        {
            var value = ParseInput();
            if (value == null) return;

            if (operand != null && currentOperator != null)
            {
                operand = Calculate(operand.Value, value.Value, currentOperator);
                currentValue = operand.Value;
                if (IsProgrammerMode)
                {
                    rawInput = SelectedBase switch
                    {
                        NumberBase.BIN => Convert.ToString((long)currentValue, 2),
                        NumberBase.OCT => Convert.ToString((long)currentValue, 8),
                        NumberBase.DEC => currentValue.ToString(),
                        NumberBase.HEX => Convert.ToString((long)currentValue, 16).ToUpper(),
                        _ => currentValue.ToString()
                    };
                }
                else
                {
                    rawInput = currentValue.ToString(CultureInfo.CurrentCulture);
                }


            }
            else
            {
                operand = value.Value;
            }

            currentOperator = op;
            justEvaluated = true;

            if (IsProgrammerMode)
            {
                Display = rawInput;
            }
            else if (IsDigitGroupingEnabled)
            {
                if (operand.Value % 1 == 0)
                    Display = ((long)operand.Value).ToString("#,0", CultureInfo.CurrentCulture);
                else
                    Display = operand.Value.ToString("#,0.################", CultureInfo.CurrentCulture);
            }
            else
            {
                if (operand.Value % 1 == 0)
                    Display = ((long)operand.Value).ToString();
                else
                    Display = operand.Value.ToString("0.################", CultureInfo.CurrentCulture);
            }
        }

        private void Evaluate()
        {
            var value = ParseInput();
            if (operand != null && currentOperator != null && value != null)
            {
                operand = Calculate(operand.Value, value.Value, currentOperator);
                currentValue = operand.Value;
                if (IsProgrammerMode)
                {
                    rawInput = SelectedBase switch
                    {
                        NumberBase.BIN => Convert.ToString((long)currentValue, 2),
                        NumberBase.OCT => Convert.ToString((long)currentValue, 8),
                        NumberBase.DEC => currentValue.ToString(),
                        NumberBase.HEX => Convert.ToString((long)currentValue, 16).ToUpper(),
                        _ => currentValue.ToString()
                    };
                }
                else
                {
                    rawInput = currentValue.ToString(CultureInfo.CurrentCulture);
                }


                currentOperator = null;
                justEvaluated = true;

                if (IsProgrammerMode)
                {
                    Display = rawInput;
                }
                else if (IsDigitGroupingEnabled)
                {
                    if (operand.Value % 1 == 0)
                        Display = ((long)operand.Value).ToString("#,0", CultureInfo.CurrentCulture);
                    else
                        Display = operand.Value.ToString("#,0.################", CultureInfo.CurrentCulture);
                }
                else
                {
                    if (operand.Value % 1 == 0)
                        Display = ((long)operand.Value).ToString();
                    else
                        Display = operand.Value.ToString("0.################", CultureInfo.CurrentCulture);
                }


            }
        }
        private double Calculate(double left, double right, string op)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right != 0 ? left / right : double.NaN,
                _ => right
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void ClearAll()
        {
            Display = "0";
            operand = null;
            currentOperator = null;
            justEvaluated = false;
            currentValue = 0;
            rawInput = "";
        }

        private void ClearEntry()
        {
            Display = "0";
            currentValue = 0;
            rawInput = "";
        }
        private void Backspace()
        {
            if (IsProgrammerMode)
            {
                if (Display.Length > 1)
                {
                    Display = Display.Substring(0, Display.Length - 1);
                    rawInput = Display;
                }
                else
                {
                    Display = "0";
                    rawInput = "0";
                }
                try
                {
                    currentValue = Convert.ToInt64(Display, (int)SelectedBase);
                }
                catch
                {
                    currentValue = 0;
                    Display = "Error";
                    rawInput = "0";
                }
                return;
            }

            if (!justEvaluated && rawInput.Length > 0)
            {
                rawInput = rawInput.Substring(0, rawInput.Length - 1);

                if (string.IsNullOrEmpty(rawInput))
                    rawInput = "0";
                var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (rawInput.EndsWith(separator))
                {
                    if (IsDigitGroupingEnabled)
                    {
                        if (double.TryParse(rawInput + "0", NumberStyles.Any, CultureInfo.CurrentCulture, out double temp))
                        {
                            Display = temp.ToString("#,0", CultureInfo.CurrentCulture) + separator;
                        }
                        else
                        {
                            Display = rawInput;
                        }
                    }
                    else
                    {
                        Display = rawInput;
                    }
                    return;
                }
                if (double.TryParse(rawInput, NumberStyles.Any, CultureInfo.CurrentCulture, out double parsed))
                {
                    currentValue = parsed;

                    if (IsDigitGroupingEnabled)
                    {
                        if (rawInput.Contains(separator))
                        {
                            if (rawInput.EndsWith("0"))
                                Display = rawInput;
                            else
                                Display = parsed.ToString("#,0.################", CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            Display = ((long)parsed).ToString("#,0", CultureInfo.CurrentCulture);
                        }
                    }

                    else
                    {
                        Display = rawInput;
                    }
                }
                else
                {
                    Display = "Error";
                }
            }
        }
        private void ApplyUnaryOperator(string op)
        {
            if (IsProgrammerMode)
            {
                Display = "Error";
                return;
            }

            var value = ParseInput();
            if (value == null)
            {
                Display = "Error";
                return;
            }

            double result = value.Value;

            switch (op)
            {
                case "√":
                    result = result >= 0 ? Math.Sqrt(result) : double.NaN;
                    break;
                case "x²":
                    result = result * result;
                    break;
                case "1/x":
                    result = result != 0 ? 1.0 / result : double.NaN;
                    break;
                case "+/-":
                    result = -result;
                    break;
                case "%":
                    if (operand != null && currentOperator != null)
                        result = operand.Value * result / 100.0;
                    else
                        result = 0;
                    break;
            }

            currentValue = result;

            rawInput = result.ToString(CultureInfo.CurrentCulture);
            justEvaluated = true;

            if (IsDigitGroupingEnabled)
            {
                if (result % 1 == 0)
                    Display = ((long)result).ToString("#,0", CultureInfo.CurrentCulture);
                else
                    Display = result.ToString("#,0.################", CultureInfo.CurrentCulture);
            }
            else
            {
                if (result % 1 == 0)
                    Display = ((long)result).ToString();
                else
                    Display = result.ToString("0.################", CultureInfo.CurrentCulture);
            }
        }

        private void Copy()
        {
            Clipboard.SetText(currentValue.ToString(CultureInfo.CurrentCulture));
        }


        private void Paste()
        {
            var text = Clipboard.GetText();
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out double val))
            {
                currentValue = val;
                rawInput = text; 
                justEvaluated = true;
                FormatDisplay();
            }
            else
            {
                Display = "Error";
            }
        }



        private void Cut()
        {
            Clipboard.SetText(currentValue.ToString(CultureInfo.CurrentCulture));
            Display = "0";
            currentValue = 0;
            rawInput = "";
            justEvaluated = true;
            FormatDisplay();
        }
        private void FormatDisplay()
        {
            double value = currentValue;

            if (IsProgrammerMode)
            {
                long intValue = (long)value;  
                Display = SelectedBase switch
                {
                    NumberBase.BIN => Convert.ToString(intValue, 2),
                    NumberBase.OCT => Convert.ToString(intValue, 8),
                    NumberBase.DEC => intValue.ToString(),
                    NumberBase.HEX => Convert.ToString(intValue, 16).ToUpper(),
                    _ => intValue.ToString()
                };
            }
            else
            {
                if (IsDigitGroupingEnabled)
                {
                    if (value % 1 == 0)
                        Display = ((long)value).ToString("#,0", CultureInfo.CurrentCulture);
                    else
                        Display = value.ToString("#,0.################", CultureInfo.CurrentCulture);
                }
                else
                {
                    if (value % 1 == 0)
                        Display = ((long)value).ToString();
                    else
                        Display = value.ToString("0.################", CultureInfo.CurrentCulture); 
                }
            }
        }

        private double? ParseInput()
        {
            if (IsProgrammerMode)
            {
                try
                {
                    long val = Convert.ToInt64(Display, (int)SelectedBase);
                    return val;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                if (double.TryParse(rawInput, NumberStyles.Any, CultureInfo.CurrentCulture, out double val))
                    return val;
                return null;
            }
        }


        public void LoadSettings()
        {
            IsDigitGroupingEnabled = Properties.Settings.Default.DigitGroupingEnabled;

            if (Enum.TryParse(Properties.Settings.Default.LastMode, out CalculatorMode mode))
                Mode = mode;

            if (Enum.TryParse(Properties.Settings.Default.LastBase, out NumberBase nb))
                SelectedBase = nb;
        }
        public void SaveSettings()
        {
            Properties.Settings.Default.DigitGroupingEnabled = IsDigitGroupingEnabled;
            Properties.Settings.Default.LastMode = Mode.ToString();
            Properties.Settings.Default.LastBase = SelectedBase.ToString();
            Properties.Settings.Default.Save();
        }

    }
}
