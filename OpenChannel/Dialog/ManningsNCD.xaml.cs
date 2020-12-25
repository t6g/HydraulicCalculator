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
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenChannel.Dialog
{
    public sealed partial class ManningsNCD : ContentDialog
    {
        public List<ManningsNLine> ManningsNTable { set; get; }
        public double NSel { set; get; }
        public ManningsNCD()
        {
            this.InitializeComponent();
            IsPrimaryButtonEnabled = false;
        }

        private void SelectN_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            string str = ((TextBlock)e.OriginalSource).Text;
            if (double.TryParse(str, out double val))
            {
                NSel = val;
                IsPrimaryButtonEnabled = true;
            }
            else
                IsPrimaryButtonEnabled = false;
        }
    }
}
