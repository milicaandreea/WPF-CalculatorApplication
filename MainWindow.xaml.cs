using Calculator.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Calculator.Views
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PreviewKeyDown += MainWindow_PreviewKeyDown;

            var vm = new MainViewModel();
            vm.LoadSettings();  
            DataContext = vm;
        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as Calculator.ViewModels.MainViewModel;
            if (vm == null) return;

            string keyText = e.Key.ToString();

            if (e.Key == Key.Enter)
            {
                vm.EqualCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                vm.ClearCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Back)
            {
                vm.BackspaceCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = (e.Key - Key.D0).ToString();
                vm.DigitCommand.Execute(digit);
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = (e.Key - Key.NumPad0).ToString();
                vm.DigitCommand.Execute(digit);
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.A && e.Key <= Key.F)
            {
                string letter = e.Key.ToString();
                vm.DigitCommand.Execute(letter);
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Add:
                case Key.OemPlus:
                    vm.OperatorCommand.Execute("+");
                    e.Handled = true;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    vm.OperatorCommand.Execute("-");
                    e.Handled = true;
                    break;
                case Key.Multiply:
                    vm.OperatorCommand.Execute("*");
                    e.Handled = true;
                    break;
                case Key.Divide:
                case Key.Oem2:
                    vm.OperatorCommand.Execute("/");
                    e.Handled = true;
                    break;
                case Key.OemComma:
                case Key.Decimal:
                case Key.OemPeriod:
                    vm.DigitCommand.Execute(".");
                    e.Handled = true;
                    break;
            }
        }
        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aplicație realizată de: Milica Andreea\nGrupa: 10LF233", "Despre aplicație",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.SaveSettings();

            base.OnClosing(e);
        }

    }
}