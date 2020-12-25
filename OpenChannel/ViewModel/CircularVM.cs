using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class CircularVM : ChannelVM
    {
        private double diameter; //inch
        public CircularVM(CircularChannel m) : base()
        {
            model = m;
            diameter = m.r * 2.0 * 12.0;
            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            IsEqualXY = true;
            m.Update();
            discharge = m.Velocity * m.Area;
        }

        public double Diameter
        {
            get => diameter;
            set
            {
                if (Math.Abs(diameter - value) < 0.001)
                    return;
                diameter = value;
                ((CircularChannel)model).r = value / 12.0 / 2.0;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(Diameter));
                RaisePropertyChanges();
            }
        }
        public override void RaisePropertyChanges()
        {
            base.RaisePropertyChanges();
            RaisePropertyChanged(nameof(Qmax));
            RaisePropertyChanged(nameof(Ymax));
        }
        public override double Xmin => -diameter / 12.0 / 2.0;
        public override double Xmax => diameter / 12.0 / 2.0;
        public double Qmax => ((CircularChannel)model).Qmax;
        public double YQmax => ((CircularChannel)model).r * 1.87636243;
        public override void UpdateAxes()
        {
            double r = ((CircularChannel)model).r;
            double xc = ToViewX(0);
            double yc = ToViewY(r);
            double rx = r * ScaleX;
            double ry = r * ScaleY;

            //"M (CX - R), CY
            // a R, R 0 1, 0 (2*R), 0
            // a R, R 0 1, 0 -(2*R), 0"
            ChCSPath = $"M {xc - rx},{yc} a {rx}, {ry} 0 1, 0 {2 * rx}, 0 a  {rx}, {ry} 0 1, 0 {-2 * rx}, 0";

            double theta = 2.0 * Math.Acos(1.0 - depthNormal / r);
            double x = r * Math.Sin(theta / 2.0);
            double xnl = ToViewX(-x);
            double xnr = ToViewX(x);
            double yn = ToViewY(depthNormal);
            NormPath = $"M {xnl},{yn} H {xnr}";

            theta = 2.0 * Math.Acos(1.0 - model.Dc / r);
            x = r * Math.Sin(theta / 2.0);

            double xcl = ToViewX(-x);
            double xcr = ToViewX(x);
            yc = ToViewY(model.Dc);

            CritPath = $"M {xcl},{yc} H {xcr}";
        }
    }
}
