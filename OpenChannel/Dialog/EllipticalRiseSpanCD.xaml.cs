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
    public sealed partial class EllipticalRiseSpanCD : ContentDialog
    {
        public int Span { get; set; }
        public int Rise { get; set; }
        public int Index { get; set; }
        public EllipticalRiseSpanCD()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Loading(FrameworkElement sender, object args)
        {
            if (sender == null) return;

            Index = PipeEllipticals.FindIndex(r => r.Span == Span);
        }

        private void SelectAB_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sender == null) return;

            DataGrid dg = sender as DataGrid;
            PipeElliptical pipe = dg.SelectedItem as PipeElliptical;
            if (pipe != null)
            {
                Span = pipe.Span;
                Rise = pipe.Rise;
            }
        }
        internal List<PipeElliptical> PipeEllipticals { get; } = new List<PipeElliptical>
        {
            new PipeElliptical("18\"x12\"",  18, 12),
            new PipeElliptical("23\"x14\"",  23, 14),
            new PipeElliptical("30\"x19\"",  30, 19),
            new PipeElliptical("34\"x22\"",  34, 22),
            new PipeElliptical("38\"x24\"",  38, 24),
            new PipeElliptical("42\"x27\"",  42, 27),
            new PipeElliptical("45\"x29\"",  45, 29),
            new PipeElliptical("49\"x32\"",  49, 32),
            new PipeElliptical("53\"x34\"",  53, 34),
            new PipeElliptical("60\"x38\"",  60, 38),
            new PipeElliptical("68\"x43\"",  68, 43),
            new PipeElliptical("76\"x48\"",  76, 48),
            new PipeElliptical("83\"x53\"",  83, 53),
            new PipeElliptical("91\"x58\"",  91, 58),
            new PipeElliptical("98\"x63\"",  98, 63),
            new PipeElliptical("106\"x68\"", 106, 68),
            new PipeElliptical("113\"x72\"", 113, 72),
            new PipeElliptical("121\"x77\"", 121, 77),
            new PipeElliptical("128\"x82\"", 128, 82),
            new PipeElliptical("136\"x87\"", 136, 87),
            new PipeElliptical("143\"x92\"", 143, 92),
            new PipeElliptical("151\"x97\"", 151, 97),
            new PipeElliptical("161\"x106\"", 161, 106),
            new PipeElliptical("180\"x116\"",  180, 116),
        };
    }
    class PipeElliptical
    {
        public string Name { get; set; }
        public int Span { get; set; }
        public int Rise { get; set; }
        public PipeElliptical(string n, int a, int b)
        {
            Name = n;
            Span = a;
            Rise = b;
        }
    }
}
