using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class ParabolicVM : ChannelVM
    {
        private double widthTop;
        private double depth;

        public ParabolicVM(ParabolicChannel m) : base()
        {
            model = m;
            widthTop = m.T;
            depth = m.d;
            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            m.Update();
            discharge = m.Velocity * m.Area;
        }

        public double WidthTop
        {
            get => widthTop;
            set
            {
                if (Math.Abs(widthTop - value) < 0.001)
                    return;
                widthTop = value;
                ((ParabolicChannel)model).T = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(WidthTop));
                RaisePropertyChanges();
            }
        }

        public double Depth
        {
            get => depth;
            set
            {
                if (Math.Abs(depth - value) < 0.001)
                    return;
                depth = value;
                ((ParabolicChannel)model).d = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(Depth));
                RaisePropertyChanges();
            }
        }
        public override double Xmin => Math.Min(-0.5 * widthTop * Math.Sqrt(DepthChannel / depth), -0.5 * widthTop);
        public override double Xmax => Math.Max(0.5 * widthTop * Math.Sqrt(DepthChannel / depth), 0.5 * widthTop);

        public override void UpdateAxes()
        {
            double T = ((ParabolicChannel)model).T;
            double d = ((ParabolicChannel)model).d;
            double x0 = ToViewX(-0.5 * T);
            double y0 = ToViewY(d);
            double x1 = ToViewX(0);
            double y1 = ToViewY(-d);
            double x2 = ToViewX(0.5 * T);

            //"M 10,10 Q50,70 100,10"
            ChCSPath = $"M {x0},{y0} Q {x1}, {y1} {x2}, {y0}";

            double x = 0.5 * T * Math.Sqrt(model.Dn / d);
            double xnl = ToViewX(-x);
            double xnr = ToViewX(x);
            double yn = ToViewY(model.Dn);
            NormPath = $"M {xnl},{yn} H {xnr}";

            x = 0.5 * T * Math.Sqrt(model.Dc / d);

            double xcl = ToViewX(-x);
            double xcr = ToViewX(x);
            double yc = ToViewY(model.Dc);

            CritPath = $"M {xcl},{yc} H {xcr}";
        }
    }
}
