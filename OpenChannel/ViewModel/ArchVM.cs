using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;

namespace OpenChannel.ViewModel
{
    public class ArchVM : ChannelVM
    {
        private double rbin;
        private double rtin;
        private double rcin;
        private double risein;

        public double RbLeft, RbTop;
        public double RtLeft, RtTop;
        public double RcLeft, RcTop;

        public ArchVM(ArchChannel m) : base()
        {
            model = m;
            rbin = m.rb * 12.0;
            rtin = m.rt * 12.0;
            rcin = m.rc * 12.0;
            risein = m.rise * 12.0;

            slopeChannel = m.S;
            manningsN = m.N;
            depthNormal = m.Dn;
            IsEqualXY = true;

            m.Update();
            discharge = m.Velocity * m.Area;

            RbPath = $"M 0,0 H 100";
            RtPath = $"M 0,100 H 100";
            RcPath = $"M 0,200 H 100";
        }
        public double Rbin
        {
            get => rbin;
            set
            {
                if (Math.Abs(rbin - value) <= 0.001)
                    return;
                rbin = value;

                ((ArchChannel)model).rb = rbin / 12.0;

                model.Update();
                UpdateChart();
                RaisePropertyChanged(nameof(Rbin));
                RaisePropertyChanges();
            }
        }
        public double Rtin
        {
            get => rtin;
            set
            {
                if (Math.Abs(rtin - value) <= 0.001)
                    return;
                rtin = value;

                ((ArchChannel)model).rt = rtin / 12.0;

                model.Update();
                UpdateChart();
                RaisePropertyChanged(nameof(Rtin));
                RaisePropertyChanges();
            }
        }
        public double Rcin
        {
            get => rcin;
            set
            {
                if (Math.Abs(rcin - value) <= 0.001)
                    return;
                rcin = value;

                ((ArchChannel)model).rc = rcin / 12.0;

                model.Update();
                UpdateChart();
                RaisePropertyChanged(nameof(Rcin));
                RaisePropertyChanges();
            }
        }

        public double Risein
        {
            get => risein;
            set
            {
                if (Math.Abs(risein - value) <= 0.001)
                    return;
                risein = value;

                ((ArchChannel)model).rise = risein / 12.0;

                model.Update();
                UpdateChart();
                RaisePropertyChanged(nameof(Risein));
                RaisePropertyChanges();
            }
        }
        public void SetArchParameters(double rb, double rt, double rc, double rise)
        {
            rbin = rb;
            rtin = rt;
            rcin = rc;
            risein = rise;

            ArchChannel m = model as ArchChannel;
            m.rb = rb / 12.0;
            m.rt = rt / 12.0;
            m.rc = rc / 12.0;
            m.rise = rise / 12.0;
            m.Update();
            UpdateChart();
            RaisePropertyChanged(nameof(Rbin));
            RaisePropertyChanged(nameof(Rtin));
            RaisePropertyChanged(nameof(Rcin));
            RaisePropertyChanged(nameof(Risein));
            RaisePropertyChanges();

        }
        public override void RaisePropertyChanges()
        {
            base.RaisePropertyChanged();
            RaisePropertyChanged(nameof(Qmax));
            RaisePropertyChanged(nameof(Ymax));
        }

        private double rb => ((ArchChannel)model).rb;
        private double rt => ((ArchChannel)model).rt;
        private double rc => ((ArchChannel)model).rc;
        private double rise => ((ArchChannel)model).rise;

        private double Theta => Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
        private double Phi => Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
        private double XD => (rb - rc) * Math.Sin(Theta);
        private double YD => rb - (rb - rc) * Math.Cos(Theta);
        private double XE => rb * Math.Sin(Theta);
        private double YE => rb * (1.0 - Math.Cos(Theta));
        private double XG => rt * Math.Sin(Phi);
        private double YG => rise - rt + rt * Math.Cos(Phi);

        public override double Xmin => -XD - rc;
        public override double Xmax => XD + rc;
        public double Qmax => ((ArchChannel)model).Qmax;
        public double YQmax => ((ArchChannel)model).Yqmax;

        public string RbPath;
        public string RtPath;
        public string RcPath;
        public override void UpdateAxes()
        {
            double xE = ToViewX(XE);
            double xEn = ToViewX(-XE);
            double yE = ToViewY(YE);
            double xG = ToViewX(XG);
            double xGn = ToViewX(-XG);
            double yG = ToViewY(YG);
            double rbx = (rb * ScaleX);
            double rby = (rb * ScaleY);
            double rtx = (rt * ScaleX);
            double rty = (rt * ScaleY);
            double rcx = (rc * ScaleX);
            double rcy = (rc * ScaleY);
            double th = Theta / Math.PI * 180.0;
            double ph = Phi / Math.PI * 180.0;

            //A size rotationAngle isLargeArcFlag sweepDirectionFlag endPoint
            // "M 10,100 A 100,50 45 1 0 200,100" start 10, 100, size 100, 50, angle 45 IsLargeArc true SweepDirection counterclockwise, end 200, 100
            ChCSPath = $"M {xE},{yE} A {rbx}, {rby} {2.0 * th} 0 1 {xEn}, {yE}" +
                       $"A {rcx}, {rcy} {180 - th - ph} 0 1 {xGn}, {yG}" +
                       $"A {rtx}, {rty} {2.0 * ph} 0 1 {xG}, {yG}" +
                       $"A {rcx}, {rcy} {180 - th - ph} 0 1 {xE}, {yE}";

            double x = ArchChannel.Getx(depthNormal, rb, rt, rc, rise);
            double xnl = ToViewX(-x);
            double xnr = ToViewX(x);
            double yn = ToViewY(depthNormal);
            NormPath = $"M {xnl},{yn} H {xnr}";

            x = ArchChannel.Getx(model.Dc, rb, rt, rc, rise);

            double xcl = ToViewX(-x);
            double xcr = ToViewX(x);
            double yc = ToViewY(model.Dc);

            CritPath = $"M {xcl},{yc} H {xcr}";

            double xB = rb * Math.Sin(0.5 * Theta);
            double yB = rb * (1.0 - Math.Cos(0.5 * Theta));
            double xO = (rb - 0.4 * rise) * Math.Sin(0.5 * Theta);
            double yO = rb - (rb - 0.4 * rise) * Math.Cos(0.5 * Theta);

            RbLeft = ToViewX(0.5 * (xB + xO)) + 2;
            RbTop = ToViewY(0.5 * (yB + yO)) - 10;

            RbPath = $"M {ToViewX(xO)},{ToViewY(yO)} L {ToViewX(xB)},{ToViewY(yB)}";

            double xT = rt * Math.Sin(0.5 * Phi);
            double yT = rise - rt + rt * Math.Cos(0.5 * Phi);
            xO = 0.6 * xT;
            yO = rise - rt + 0.6 * rt * Math.Cos(0.5 * Phi);

            RtLeft = ToViewX(0.5 * (xT + xO));
            RtTop = ToViewY(0.5 * (yT + yO));

            RtPath = $"M {ToViewX(xO)},{ToViewY(yO)} L {ToViewX(xT)},{ToViewY(yT)}";

            double xC = XD + rc * Math.Sin(Math.PI / 4.0);
            double yC = YD - rc * Math.Cos(Math.PI / 4.0);
            RcLeft = ToViewX(0.5 * (xC + XD));
            RcTop = ToViewY(0.5 * (yC + YD)) - 15;
            RcPath = $"M {ToViewX(XD)},{ToViewY(YD)} L {ToViewX(xC)},{ToViewY(yC)}";

            RaisePropertyChanged(nameof(RbPath));
            RaisePropertyChanged(nameof(RtPath));
            RaisePropertyChanged(nameof(RcPath));

            RaisePropertyChanged(nameof(RbLeft));
            RaisePropertyChanged(nameof(RtLeft));
            RaisePropertyChanged(nameof(RcLeft));
            RaisePropertyChanged(nameof(RbTop));
            RaisePropertyChanged(nameof(RtTop));
            RaisePropertyChanged(nameof(RcTop));
        }
    }
}
