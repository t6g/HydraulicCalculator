using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class EllipticalVM : ChannelVM
    {
        private double span;  //inch
        private double rise;  // inch
        public EllipticalVM(EllipticalChannel m) : base()
        {
            model = m;
            span = m.a * 12.0 * 2.0;
            rise = m.b * 12.0 * 2.0;
            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            IsEqualXY = true;
            m.Update();
            discharge = m.Velocity * m.Area;
        }

        public double Span
        {
            get => span;
            set
            {
                if (Math.Abs(span - value) < 0.001)
                    return;
                span = value;
                ((EllipticalChannel)model).a = value / 12.0 / 2.0;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(Span));
                RaisePropertyChanges();
            }
        }
        public double Rise
        {
            get => rise;
            set
            {
                if (Math.Abs(rise - value) < 0.001)
                    return;
                rise = value;
                ((EllipticalChannel)model).b = value / 12.0 / 2.0;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(Rise));
                RaisePropertyChanges();
            }
        }

        private double a => ((EllipticalChannel)model).a;
        private double b => ((EllipticalChannel)model).b;
        public override double Xmin => -a;
        public override double Xmax => a;
        public double Qmax => ((EllipticalChannel)model).Qmax;
        public double YQmax => ((EllipticalChannel)model).Yqmax;
        public override void RaisePropertyChanges()
        {
            base.RaisePropertyChanged();
            RaisePropertyChanged(nameof(Qmax));
            RaisePropertyChanged(nameof(Ymax));
        }

        public override void UpdateAxes()
        {
            double xc = ToViewX(0);
            double yc = ToViewY(b);
            double rx = a * ScaleX;
            double ry = b * ScaleY;

            //"M (CX - R), CY
            // a R, R 0 1, 0 (2*R), 0
            // a R, R 0 1, 0 -(2*R), 0"
            ChCSPath = $"M {xc - rx},{yc} a {rx}, {ry} 0 1, 0 {2 * rx}, 0 a  {rx}, {ry} 0 1, 0 {-2 * rx}, 0";

            double alpha = Math.Acos(1.0 - depthNormal / b);
            double x = a * Math.Sin(alpha);
            double xnl = ToViewX(-x);
            double xnr = ToViewX(x);
            double yn = ToViewY(depthNormal);
            NormPath = $"M {xnl},{yn} H {xnr}";

            alpha = Math.Acos(1.0 - model.Dc / b); ;
            x = a * Math.Sin(alpha);

            double xcl = ToViewX(-x);
            double xcr = ToViewX(x);
            yc = ToViewY(model.Dc);

            CritPath = $"M {xcl},{yc} H {xcr}";
        }
    }
}
