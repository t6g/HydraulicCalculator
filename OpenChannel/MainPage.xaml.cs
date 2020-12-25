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
using OpenChannel.View;
using OpenChannel.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OpenChannel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Channels MyChannels = null;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Navi_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (sender == null) return;

            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(OptionPage));
            }
            else
            {
                Microsoft.UI.Xaml.Controls.NavigationViewItem selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                if (selectedItem == null) return;
                switch ((string)selectedItem.Tag)
                {
                    case "Triangular":
                        contentFrame.Navigate(typeof(TriangularPage), MyChannels);
                        MyChannels.Current = "Triangular";
                        break;
                    case "Trapezoidal":
                        contentFrame.Navigate(typeof(TrapezoidalPage), MyChannels);
                        MyChannels.Current = "Trapezoidal";
                        break;
                    case "Rectangular":
                        contentFrame.Navigate(typeof(RectangularPage), MyChannels);
                        MyChannels.Current = "Triangular";
                        break;
                    case "Parabolic":
                        contentFrame.Navigate(typeof(ParabolicPage), MyChannels);
                        MyChannels.Current = "Parabolic";
                        break;
                    case "Circular":
                        contentFrame.Navigate(typeof(CircularPage), MyChannels);
                        MyChannels.Current = "Circular";
                        break;
                    case "Elliptical":
                        contentFrame.Navigate(typeof(EllipticalPage), MyChannels);
                        MyChannels.Current = "Elliptical";
                        break;
                    case "Arch":
                        contentFrame.Navigate(typeof(ArchPage), MyChannels);
                        MyChannels.Current = "Arch";
                        break;
                    default:
                        break;
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyChannels = (Channels)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            if (sender == null) return;

            switch (MyChannels.Current)
            {
                case "Triangular":
                    contentFrame.Navigate(typeof(TriangularPage), MyChannels);
                    NaviTria.IsSelected = true;
                    break;
                case "Trapezoidal":
                    contentFrame.Navigate(typeof(TrapezoidalPage), MyChannels);
                    NaviTrap.IsSelected = true;
                    break;
                case "Rectangular":
                    contentFrame.Navigate(typeof(RectangularPage), MyChannels);
                    NaviRect.IsSelected = true;
                    break;
                case "Parabolic":
                    contentFrame.Navigate(typeof(ParabolicPage), MyChannels);
                    NaviPara.IsSelected = true;
                    break;
                case "Circular":
                    contentFrame.Navigate(typeof(CircularPage), MyChannels);
                    NaviCirc.IsSelected = true;
                    break;
                case "Elliptical":
                    contentFrame.Navigate(typeof(EllipticalPage), MyChannels);
                    NaviElli.IsSelected = true;
                    break;
                case "Arch":
                    contentFrame.Navigate(typeof(ArchPage), MyChannels);
                    NaviArch.IsSelected = true;
                    break;
            }

        }
    }
}
