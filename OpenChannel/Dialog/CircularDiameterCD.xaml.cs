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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenChannel.Dialog
{
    public sealed partial class CircularDiameterCD : ContentDialog
    {
        public int Diameter;
        public CircularDiameterCD()
        {
            this.InitializeComponent();
        }

        private void DiameterSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CircularPipeDiameter pipe = DiameterSelect.SelectedItem as CircularPipeDiameter;
            Diameter = pipe.Diameter;
        }

        private void DiameterSelect_Loading(FrameworkElement sender, object args)
        {
            if (sender == null) return;

            int index = CircularPipeDiameters.FindIndex(r => r.Diameter == Diameter);
            DiameterSelect.SelectedIndex = index;
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
