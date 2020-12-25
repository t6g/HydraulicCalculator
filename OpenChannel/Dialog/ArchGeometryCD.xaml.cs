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
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenChannel.Dialog
{
    public sealed partial class ArchGeometryCD : ContentDialog
    {
        public double Rbin;
        public double Rtin;
        public double Rcin;
        public double Risein;
        public int Index;
        public ArchGeometryCD()
        {
            this.InitializeComponent();
        }

        private void SelectArch_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            ArchParameter pipe = dg.SelectedItem as ArchParameter;
            if (pipe != null)
            {
                Rbin = pipe.Rb;
                Rtin = pipe.Rt;
                Rcin = pipe.Rc;
                Risein = pipe.Rise;
            }
        }

        private void ContentDialog_Loading(FrameworkElement sender, object args)
        {
            if (sender == null) return;

            Index = ArchParameters.FindIndex(r => r.Rise == Risein);
        }
        internal List<ArchParameter> ArchParameters { get; } = new List<ArchParameter>
        {
            new ArchParameter("13x22", 27.5, 13.75, 5.25, 13.5),
            new ArchParameter("18x28", 40.7, 14.75, 4.03, 18.0),
            new ArchParameter("22x36", 51.0, 18.75, 5.13, 22.5),
            new ArchParameter("26x44", 62.0, 22.50, 8.50, 26.5),
            new ArchParameter("31x51", 72.0, 26.25, 7.75, 31.3),
            new ArchParameter("38x55", 84.0, 30.00, 8.85, 38.0),
            new ArchParameter("44x65", 92.5, 33.38, 10.0, 40.0),
            new ArchParameter("45x73", 105.0, 37.5, 11.0, 45.0),
            new ArchParameter("54x88", 126.0, 45.0, 13.31, 54.0),
            new ArchParameter("62x102", 162.5, 52, 14.5, 62),
            new ArchParameter("72x115", 183.0, 59.0, 15.0, 72.0),
            new ArchParameter("77x122", 218.0, 62.0, 20.0, 77.5),
            new ArchParameter("87x138", 259.0, 70.0, 22.0, 87.1),
            new ArchParameter("97x154", 301.4, 78.0, 24.0, 98.9),
            new ArchParameter("106x160",329.0, 85.63, 26.6, 106.5)
        };
    }
    class ArchParameter
    {
        public string Name { get; set; }
        public double Rb { get; set; }
        public double Rt { get; set; }
        public double Rc { get; set; }
        public double Rise { get; set; }
        public ArchParameter(string n, double b, double t, double c, double r)
        {
            Name = n;
            Rb = b;
            Rt = t;
            Rc = c;
            Rise = r;
        }
    }
}
