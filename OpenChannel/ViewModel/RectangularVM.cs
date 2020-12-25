using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class RectangularVM : ChannelVM
    {
        private double widthBottom;
        public double WidthBottom
        {
            get => widthBottom;
            set
            {
                if (Math.Abs(widthBottom - value) < 0.001)
                    return;
                widthBottom = value;
                ((RectangularChannel)model).B = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(WidthBottom));
                RaisePropertyChanges();
            }
        }
        public RectangularVM(RectangularChannel m) : base()
        {
            model = m;
            widthBottom = m.B;
            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            m.Update();
            discharge = m.Velocity * m.Area;
        }

        public override double Xmin => 0;
        public override double Xmax => 2 + widthBottom;

        public override void UpdateAxes()
        {
            double x0 = ToViewX(0);
            double x1 = ToViewX(1);
            double x2 = ToViewX(1 + widthBottom);
            double x3 = ToViewX(2 + widthBottom);
            double y0 = ToViewY(0);
            double y1 = ToViewY(DepthChannel);
            double yn = ToViewY(model.Dn);
            double yc = ToViewY(model.Dc);

            ChCSPath = $"M {x0},{y1} H {x1} V {y0} H {x2} V {y1} H {x3}";
            NormPath = $"M {x1},{yn} H {x2}";
            CritPath = $"M {x1},{yc} H {x2}";
        }
    }
}
