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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OpenChannel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
                        contentFrame.Navigate(typeof(TriangularPage));
                        break;
                    case "Trapezoidal":
                        contentFrame.Navigate(typeof(TrapezoidalPage));
                        break;
                    case "Rectangular":
                        contentFrame.Navigate(typeof(RectangularPage));
                        break;
                    case "Parabolic":
                        contentFrame.Navigate(typeof(ParabolicPage));
                        break;
                    case "Circulcar":
                        contentFrame.Navigate(typeof(CircularPage));
                        break;
                    case "Elliptical":
                        contentFrame.Navigate(typeof(EllipticalPage));
                        break;
                    case "Arch":
                        contentFrame.Navigate(typeof(ArchPage));
                        break;
                }
            }
        }
    }
}
