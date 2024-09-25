using GoUtility.Utilities;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsFanFullSpeed = false;
        public TDP.Mode? SmartFanMode;

        public MainWindow()
        {
            InitializeComponent();

            Activated += OnFocus;
            SmartFan.Checked += FanChecked;
            FullFan.Checked += FanChecked;
            TDPQuiet.Checked += TDPChanged;
            TDPBalanced.Checked += TDPChanged;
            TDPPerformance.Checked += TDPChanged;
        }

        private async void OnFocus(object? sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                IsFanFullSpeed = Fan.IsFanAtFullSpeed();
                SmartFanMode = Fan.GetSmartFanMode();
            });

            SmartFan.IsChecked = !IsFanFullSpeed;
            FullFan.IsChecked = IsFanFullSpeed;
            TDPQuiet.IsChecked = SmartFanMode == TDP.Mode.Quiet;
            TDPBalanced.IsChecked = SmartFanMode == TDP.Mode.Balanced;
            TDPPerformance.IsChecked = SmartFanMode == TDP.Mode.Performance;
        }

        private void FanChecked(object sender, RoutedEventArgs e)
        {
            Fan.SetFanFullSpeed(sender == FullFan);
        }

        private void TDPChanged(object sender, RoutedEventArgs e)
        {
            if (sender == TDPQuiet)
            {
                Fan.SetSmartFanMode(TDP.Mode.Quiet);
                TDP.SetPowerLimit(TDP.Mode.Quiet);
            }
            else if (sender == TDPBalanced)
            {
                Fan.SetSmartFanMode(TDP.Mode.Balanced);
                TDP.SetPowerLimit(TDP.Mode.Balanced);
            }
            else if (sender == TDPPerformance)
            {
                Fan.SetSmartFanMode(TDP.Mode.Performance);
                TDP.SetPowerLimit(TDP.Mode.Performance);
            }
        }
    }
}