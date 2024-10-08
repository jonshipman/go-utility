﻿using GoUtility.Utilities;
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
        public bool IsChargeLimited = false;

        public MainWindow()
        {
            InitializeComponent();

            Activated += OnFocus;
            SmartFan.Checked += FanChecked;
            FullFan.Checked += FanChecked;
            TDPQuiet.Checked += TDPChanged;
            TDPBalanced.Checked += TDPChanged;
            TDPPerformance.Checked += TDPChanged;
            TDPCustom.Checked += TDPChanged;
            BatteryLimit.Checked += BatteryChargeChanged;
            BatteryFull.Checked += BatteryChargeChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private async void OnFocus(object? sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                IsFanFullSpeed = Fan.IsFanAtFullSpeed();
                SmartFanMode = Fan.GetSmartFanMode();
                IsChargeLimited = TDP.GetBatteryChargeLimitStatus();
            });

            SmartFan.IsChecked = !IsFanFullSpeed;
            FullFan.IsChecked = IsFanFullSpeed;
            TDPQuiet.IsChecked = SmartFanMode == TDP.Mode.Quiet;
            TDPBalanced.IsChecked = SmartFanMode == TDP.Mode.Balanced;
            TDPPerformance.IsChecked = SmartFanMode == TDP.Mode.Performance;
            TDPCustom.IsChecked = SmartFanMode == TDP.Mode.Custom;
            BatteryLimit.IsChecked = IsChargeLimited;
            BatteryFull.IsChecked = !IsChargeLimited;
        }

        private void FanChecked(object sender, RoutedEventArgs e)
        {
            Fan.SetFanFullSpeed(sender == FullFan);
        }

        private void BatteryChargeChanged(object sender, RoutedEventArgs e)
        {
            TDP.SetBatteryChargeLimit(sender == BatteryLimit);
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
            else if (sender == TDPCustom)
            {
                Fan.SetFanTableForCustomMode();
                Fan.SetSmartFanMode(TDP.Mode.Custom);
                TDP.SetPowerLimit(TDP.Mode.Custom);
            }
        }
    }
}