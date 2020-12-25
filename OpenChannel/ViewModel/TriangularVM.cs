using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class TriangularVM : ChannelVM
    {
        private double slopeLeft;
        private double slopeRight;

        public TriangularVM(TriangularChannel m) : base()
        {
            model = m;
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
                ((TriangularChannel)model).SR = value;
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
                ((TriangularChannel)model).SL = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(SlopeRight));
                RaisePropertyChanges();
            }
        }
        public override double Xmin => 0;
        public override double Xmax => 2 + (slopeLeft + slopeRight) * DepthChannel;

        public override void UpdateAxes()
        {
            double x0 = ToViewX(0);
            double x1 = ToViewX(1);
            double x2 = ToViewX(1 + slopeLeft * DepthChannel);
            double x3 = ToViewX(1 + (slopeLeft + slopeRight) * DepthChannel);
            double x4 = ToViewX(2 + (slopeLeft + slopeRight) * DepthChannel);
            double y0 = ToViewY(0);
            double y1 = ToViewY(DepthChannel);
            double xln = ToViewX(1 + slopeLeft * (DepthChannel - model.Dn));
            double xrn = ToViewX(1 + slopeLeft * DepthChannel + slopeRight * model.Dn);
            double yn = ToViewY(model.Dn);
            double xlc = ToViewX(1 + slopeLeft * (DepthChannel - model.Dc));
            double xrc = ToViewX(1 + slopeLeft * DepthChannel + slopeRight * model.Dc);
            double yc = ToViewY(model.Dc);

            ChCSPath = $"M {x0},{y1} H {x1} L {x2},{y0} L {x3},{y1} H {x4}";
            NormPath = $"M {xln},{yn} H {xrn}";
            CritPath = $"M {xlc},{yc} H {xrc}";
        }
    }
}
