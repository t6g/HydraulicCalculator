using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class TrapezoidalVM : ChannelVM
    {
        private double slopeLeft;
        private double slopeRight;
        private double widthBottom;

        public TrapezoidalVM(TrapezoidalChannel m) : base()
        {
            model = m;
            widthBottom = m.B;
            slopeLeft = m.SL;
            slopeRight = m.SR;
            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            m.Update();
            discharge = m.Velocity * m.Area;
        }
        public double SlopeLeft
        {
            get => slopeLeft;
            set
            {
                if (Math.Abs(slopeLeft - value) < 0.001)
                    return;
                slopeLeft = value;
                ((TrapezoidalChannel)model).SR = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(SlopeLeft));
                RaisePropertyChanges();
            }
        }

        public double SlopeRight
        {
            get => slopeRight;
            set
            {
                if (Math.Abs(slopeRight - value) < 0.001)
                    return;
                slopeRight = value;
                ((TrapezoidalChannel)model).SL = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(SlopeRight));
                RaisePropertyChanges();
            }
        }

        public double WidthBottom
        {
            get => widthBottom;
            set
            {
                if (Math.Abs(widthBottom - value) < 0.001)
                    return;
                widthBottom = value;
                ((TrapezoidalChannel)model).B = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(WidthBottom));
                RaisePropertyChanges();
            }
        }
        public override double Xmin => 0;

        public override double Xmax => 2 + (slopeLeft + slopeRight) * DepthChannel + widthBottom;

        public override void UpdateAxes()
        {
            double x = 0.0;
            double x0 = ToViewX(x);
            x += 1.0;
            double x1 = ToViewX(x);
            x += slopeLeft * DepthChannel;
            double x2 = ToViewX(x);
            x += widthBottom;
            double x3 = ToViewX(x);
            x += slopeRight * DepthChannel;
            double x4 = ToViewX(x);
            x += 1.0;
            double x5  = ToViewX(x);
            double y0  = ToViewY(0);
            double y1  = ToViewY(DepthChannel);
            double xln = ToViewX(1 + slopeLeft * (DepthChannel - model.Dn));
            double xrn = ToViewX(1 + slopeLeft * DepthChannel + widthBottom + slopeRight * model.Dn);
            double yn  = ToViewY(model.Dn);
            double xlc = ToViewX(1 + slopeLeft * (DepthChannel - model.Dc));
            double xrc = ToViewX(1 + slopeLeft * DepthChannel + widthBottom + slopeRight * model.Dc);
            double yc  = ToViewY(model.Dc);

            ChCSPath = $"M {x0},{y1} H {x1} L {x2},{y0} H {x3} L {x4},{y1} H {x5}";
            NormPath = $"M {xln},{yn} H {xrn}";
            CritPath = $"M {xlc},{yc} H {xrc}";
        }
    }
}
