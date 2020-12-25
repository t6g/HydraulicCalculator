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
    public sealed partial class EllipticalPage : Page
    {
        public EllipticalVM ViewModel;
        public EllipticalPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null) return;

            Channels chans = e.Parameter as Channels;
            ViewModel = new EllipticalVM(chans.Elli);
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

        private void SwitchAB_Click(object sender, RoutedEventArgs e)
        {
            double tmp = ViewModel.Span;
            ViewModel.Span = ViewModel.Rise;
            ViewModel.Rise = tmp;
        }

        private async void SelectAB_Click(object sender, RoutedEventArgs e)
        {
            EllipticalRiseSpanCD dlg = new EllipticalRiseSpanCD()
            {
                Span = (int)ViewModel.Span,
                Rise = (int)ViewModel.Rise
            };
            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Span = dlg.Span;
                ViewModel.Rise = dlg.Rise;
            }
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            if (!(ViewModel.model.options.bUSCustomary))
            {
                UnitAA.Text = "mm";
                UnitBB.Text = "mm";
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
}
