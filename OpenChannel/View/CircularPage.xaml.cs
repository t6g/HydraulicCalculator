using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OpenChannel.Model;
using OpenChannel.ViewModel;
using OpenChannel.Dialog;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenChannel.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CircularPage : Page
    {
        public CircularVM ViewModel;
        public CircularPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null) return;

            Channels chans = e.Parameter as Channels;
            ViewModel = new CircularVM(chans.Circ);
            ViewModel.ManningsNTable = chans.ManningsNTable;
            ViewModel.bEnergyCurve = chans.IsRatingCurve;
            ViewModel.bRatingCurve = chans.IsRatingCurve;
            base.OnNavigatedTo(e);
        }

        private void ChartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender == null) return;
            double dh = Math.Abs(e.NewSize.Height - e.PreviousSize.Height) / e.NewSize.Height;
            double dw = Math.Abs(e.NewSize.Width - e.PreviousSize.Width) / e.NewSize.Width;
            if (dh < 0.01 && dw < 0.01) return;

            ViewModel.UpdateViewSizeChange(ChartGrid.ActualWidth, ChartGrid.ActualHeight);
        }
        private async void SelectN_Click(object sender, RoutedEventArgs e)
        {
            ManningsNCD dlg = new ManningsNCD
            {
                ManningsNTable = ViewModel.ManningsNTable
            };
            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.ManningsN = dlg.NSel;
            }
        }

        private async void SelectDiameter_Click(object sender, RoutedEventArgs e)
        {
            CircularDiameterCD dlg = new CircularDiameterCD();
            dlg.Diameter = (int)ViewModel.Diameter;

            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Diameter = dlg.Diameter;
            }
        }
        internal List<CircularPipeDiameter> CircularPipeDiameters { get; } = new List<CircularPipeDiameter>
        {
            new CircularPipeDiameter("8\"",  8),
            new CircularPipeDiameter("10\"", 10),
            new CircularPipeDiameter("12\"", 12),
            new CircularPipeDiameter("15\"", 15),
            new CircularPipeDiameter("18\"", 18),
            new CircularPipeDiameter("21\"", 21),
            new CircularPipeDiameter("24\"", 24),
            new CircularPipeDiameter("30\"", 30),
            new CircularPipeDiameter("33\"", 33),
            new CircularPipeDiameter("36\"", 36),
            new CircularPipeDiameter("42\"", 42),
            new CircularPipeDiameter("48\"", 48),
            new CircularPipeDiameter("54\"", 54),
            new CircularPipeDiameter("60\"", 60),
            new CircularPipeDiameter("66\"", 66),
            new CircularPipeDiameter("72\"", 72),
            new CircularPipeDiameter("78\"", 78),
            new CircularPipeDiameter("84\"", 84),
            new CircularPipeDiameter("90\"", 90),
            new CircularPipeDiameter("96\"", 96),
            new CircularPipeDiameter("102\"", 102),
            new CircularPipeDiameter("108\"", 108),
            new CircularPipeDiameter("114\"", 114),
            new CircularPipeDiameter("120\"", 120),
            new CircularPipeDiameter("126\"", 126),
            new CircularPipeDiameter("132\"", 132),
            new CircularPipeDiameter("138\"", 138),
            new CircularPipeDiameter("144\"", 144),
        };

        private void Page_Loading(FrameworkElement sender, object args)
        {
            if (!(ViewModel.model.options.bUSCustomary))
            {
                UnitDI.Text = "mm";
                UnitSC.Text = "m/m";
                UnitQn.Text = "m^3/s";
                UnitDn.Text = "m";

                UnitFA.Text = "m^2";
                UnitWP.Text = "m";
                UnitHR.Text = "m";
                UnitVE.Text = "m/s";
                UnitTW.Text = "m";
                UnitDC.Text = "m";
                UnitVC.Text = "m/s";
                UnitCS.Text = "m/m";

                UnitQM.Text = "m^3/s";
                UnitYM.Text = "m";
            }
            if (!(ViewModel.bEnergyCurve))
            {
                PathEnergyCurve.Visibility = Visibility.Collapsed;
            }
            if (!(ViewModel.bRatingCurve))
            {
                PathRatingCurve.Visibility = Visibility.Collapsed;
            }
        }
    }
    class CircularPipeDiameter
    {
        public string Name { get; set; }
        public int Diameter { get; set; }
        public CircularPipeDiameter(string n, int d)
        {
            Name = n;
            Diameter = d;
        }
    }
}
