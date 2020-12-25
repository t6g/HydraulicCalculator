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
    public sealed partial class ArchPage : Page
    {
        public ArchVM ViewModel;
        public ArchPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null) return;

            Channels chans = e.Parameter as Channels;
            ViewModel = new ArchVM(chans.Arch);
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

        private async void SelectArch_Click(object sender, RoutedEventArgs e)
        {
            ArchGeometryCD dlg = new ArchGeometryCD
            {
                Rbin = ViewModel.Rbin,
                Rtin = ViewModel.Rtin,
                Rcin = ViewModel.Rcin,
                Risein = ViewModel.Risein,
            };

            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.SetArchParameters(dlg.Rbin, dlg.Rtin, dlg.Rcin, dlg.Risein);
            }
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            if (!(ViewModel.model.options.bUSCustomary))
            {
                UnitRT.Text = "mm";
                UnitRB.Text = "mm";
                UnitRC.Text = "mm";
                UnitRI.Text = "mm";

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
