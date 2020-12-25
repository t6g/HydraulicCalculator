using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenChannel.Model;
using Windows.Globalization.NumberFormatting;

namespace OpenChannel.ViewModel
{
    public abstract class ChannelVM : INotifyPropertyChanged
    {
        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public bool bRatingCurve;
        public bool bEnergyCurve;
        public List<ManningsNLine> ManningsNTable;

        // model
        public Channel model;

        protected double manningsN;
        protected double slopeChannel;
        protected double depthNormal;
        protected double discharge;

        public double ManningsN
        {
            get => manningsN;
            set
            {
                if (Math.Abs(manningsN - value) < 0.00001)
                    return;
                manningsN = value;

                model.N = value;
                model.Update();
                UpdateChart();
                RaisePropertyChanged(nameof(ManningsN));
                RaisePropertyChanges();
            }
        }
        public double SlopeChannel
        {
            get => slopeChannel;
            set
            {
                if (Math.Abs(slopeChannel - value) < 0.0001)
                    return;
                slopeChannel = value;

                model.S = value;
                model.Update();
                UpdateChart();

                RaisePropertyChanged(nameof(SlopeChannel));
                RaisePropertyChanges();
            }
        }

        public double DepthNormal
        {
            get => depthNormal;
            set
            {
                if (Math.Abs(depthNormal - value) < 0.001)
                    return;
                depthNormal = value;
                model.Dn = depthNormal;
                model.Update();
                RaisePropertyChanged(nameof(DepthNormal));
                discharge = model.Velocity * model.Area;
                RaisePropertyChanged(nameof(Discharge));
                RaisePropertyChanges();
                UpdateChart();
            }
        }

        public double Discharge
        {
            get => model.Velocity * model.Area;
            set
            {
                if (Math.Abs(Discharge - value) < 0.01)
                    return;

                discharge = value;

                switch (model)
                {
                    case TriangularChannel m:
                        depthNormal = TriangularChannel.GetNormalDepth(discharge, m.SL, m.SR, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case RectangularChannel m:
                        depthNormal = TrapezoidalChannel.GetNormalDepth(discharge, m.B, 0.0, 0.0, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case TrapezoidalChannel m:
                        depthNormal = TrapezoidalChannel.GetNormalDepth(discharge, m.B, m.SL, m.SR, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case ParabolicChannel m:
                        depthNormal = ParabolicChannel.GetNormalDepth(discharge, m.T, m.d, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case CircularChannel m:
                        depthNormal = CircularChannel.GetNormalDepth(discharge, m.r, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case EllipticalChannel m:
                        depthNormal = EllipticalChannel.GetNormalDepth(discharge, m.a, m.b, slopeChannel, manningsN, m.options.bUSCustomary);
                        break;
                    case ArchChannel m:
                        double Qmax = m.Qmax;
                        if (discharge >= Qmax)
                        {
                            discharge = Qmax;
                            depthNormal = m.Yqmax;
                        }
                        else
                        {
                            depthNormal = ArchChannel.GetNormalDepth(discharge, m.rb, m.rt, m.rc, m.rise, slopeChannel, manningsN, m.options.bUSCustomary);
                        }
                        break;
                    default: throw new ArgumentException("Model variable is not a recognized channel model!");
                }

                model.Dn = depthNormal;
                model.Update();

                RaisePropertyChanged(nameof(DepthNormal));
                RaisePropertyChanges();
                UpdateChart();
            }
        }
        public virtual void RaisePropertyChanges()
        {
            RaisePropertyChanged(nameof(Discharge));
            RaisePropertyChanged(nameof(AreaNormal));
            RaisePropertyChanged(nameof(PerimeterNormal));
            RaisePropertyChanged(nameof(HydraulicRadiusNormal));
            RaisePropertyChanged(nameof(TopWidthNormal));
            RaisePropertyChanged(nameof(VelocityNormal));
            RaisePropertyChanged(nameof(DepthCritical));
            RaisePropertyChanged(nameof(VelocityCritical));
            RaisePropertyChanged(nameof(SlopeCritical));
            RaisePropertyChanged(nameof(FroudeNumber));
        }

        public double AreaNormal => model.Area;

        public double PerimeterNormal => model.Perimeter;

        public double HydraulicRadiusNormal => model.Area / model.Perimeter;

        public double TopWidthNormal => model.Twn;
        public double VelocityNormal => model.Velocity;
        public double DepthCritical => model.Dc;

        public double VelocityCritical => model.Vc;

        public double SlopeCritical => model.Sc;

        public double FroudeNumber => model.Velocity / model.Vc;

        //view
        public int OffsetLeft;
        public int OffsetTop;
        public double ViewWidth;
        public double ViewHeight;

        public bool IsEqualXY;
        public abstract double Xmin { get; }
        public abstract double Xmax { get; }
        public double Ymin => 0;
        public double Ymax => DepthChannel;
        public double DepthChannel
        {
            get
            {
                switch (model)
                {
                    //close conduits
                    case CircularChannel mc: return 2.0 * mc.r;
                    case EllipticalChannel me: return 2.0 * me.b;
                    case ArchChannel ma: return ma.rise;
                    //open channel
                    default:
                        double d = Math.Max(1.0, Math.Max(Math.Ceiling(model.Dn), Math.Ceiling(model.Dc)));
                        if (model is ParabolicChannel mp) d = Math.Max(d, mp.d);
                        return d;
                }
            }
        }
        private double sX => ViewWidth / (Xmax - Xmin);
        private double sY => ViewHeight / (Ymax - Ymin);

        public double ScaleX => (IsEqualXY ? Math.Min(sX, sY) : sX);
        public double ScaleY => (IsEqualXY ? Math.Min(sX, sY) : sY);
        protected double ToViewX(double x)
        {

            if (Xmin >= 0)  //for triangular, trapezoidal, rectangular, irregular, 0 starts at offsetleft or left of chartarea
                return (OffsetLeft + (x - Xmin) * ScaleX);
            else           //for circular, parabolic, 0 starts in them middle of the chart area
                return (OffsetLeft + ViewWidth / 2.0 + x * ScaleX);
        }
        protected double ToViewY(double y)
        {
            return (OffsetTop + ViewHeight - (y - Ymin) * ScaleY);
        }

        protected double ToViewX1(double x)  // x normalized
        {
            return (OffsetLeft + x * ViewWidth / 1.3);
        }
        protected double ToViewX2(double x)  // x normalized
        {
            return (OffsetLeft + x * ViewWidth / 2.0);
        }
        protected double ToViewY1(double y)  // y normalized with maximum = 1.0
        {
            return (OffsetTop + ViewHeight - y * ViewHeight);
        }

        public ObservableCollection<TickLabelINPC> TickLabels;

        public double XIncrement
        {
            get
            {
                int maxticks = 8;
                double nicerange = NiceScaleStatic.niceNum(Xmax - Xmin, false);
                double tickSpacing = NiceScaleStatic.niceNum(nicerange / (maxticks - 1), true);
                return tickSpacing;
            }
        }
        public double YIncrement
        {
            get
            {
                int maxticks = 5;
                double nicerange = NiceScaleStatic.niceNum(Ymax - Ymin, false);
                double tickSpacing = NiceScaleStatic.niceNum(nicerange / (maxticks - 1), true);
                return tickSpacing;
            }
        }

        protected int nInc;
        private double[] ys, As, Ps, vs;

        public string ChCSPath;  //channel cross section
        public string NormPath;
        public string CritPath;
        public string GridPath;
        public string AreaPath;
        public string PerimeterPath;
        public string HydraulicRadiusPath;
        public string EnergyPath;
        public string VelocityPath;
        public string DischargePath;

        public ChannelVM()
        {
            manningsN = 0.05;
            slopeChannel = 0.01;
            depthNormal = 0.5;

            TickLabels = new ObservableCollection<TickLabelINPC>();

            OffsetLeft = 30;
            OffsetTop = 20;
            ViewWidth = 300;
            ViewHeight = 200;

            IsEqualXY = false;

            nInc = 50;

            ChCSPath = $"M 0,0 H 100";
            NormPath = $"M 0,0 H 100";
            CritPath = $"M 0,0 H 100";
            GridPath = $"M 0,0 H 100";
            AreaPath = $"M 0,0 H 100";
            PerimeterPath = $"M 0,0 H 100";
            HydraulicRadiusPath = $"M 0,0 H 100";
            EnergyPath = $"M 0,0 H 100";
            VelocityPath = $"M 0,0 H 100";
            DischargePath = $"M 0,0 H 100";

            ys = new double[nInc];
            As = new double[nInc];
            Ps = new double[nInc];
            vs = new double[nInc];
        }

        public DecimalFormatter DF2
        {
            get
            {
                IncrementNumberRounder rounder2 = new IncrementNumberRounder
                {
                    Increment = 0.01
                };

                return new DecimalFormatter
                {
                    FractionDigits = 2,
                    NumberRounder = rounder2
                };
            }
        }

        public DecimalFormatter DF4
        {
            get
            {
                IncrementNumberRounder rounder4 = new IncrementNumberRounder
                {
                    Increment = 0.0001
                };

                return new DecimalFormatter
                {
                    FractionDigits = 4,
                    NumberRounder = rounder4
                };
            }
        }
        public void UpdateViewSizeChange(double width, double height)
        {
            ViewWidth = width - 2.0 * OffsetLeft;
            ViewHeight = height - 2.0 * OffsetTop;

            RaisePropertyChanged(nameof(ViewWidth));
            RaisePropertyChanged(nameof(ViewHeight));

            UpdateChart();
        }

        public abstract void UpdateAxes();
        public void UpdateChart()
        {
            UpdateAxes();

            // grid lines and labels
            string sPath = "";

            if (TickLabels.Count > 0) TickLabels.Clear();

            double xinc = XIncrement * ScaleX;

            double xx;
            if (Xmin >= 0)
            {
                xx = Xmin;

                TickLabels.Add(new TickLabelINPC(OffsetLeft - 0.25 * OffsetTop, ViewHeight + 1.25 * OffsetTop, xx.ToString()));

                // vertical grid lines
                for (double x = OffsetLeft + xinc; x < ViewWidth; x += xinc)
                {
                    sPath += $"M {(int)x}, {OffsetTop} V {ViewHeight + OffsetTop} ";
                    // x tick labels
                    xx += XIncrement;
                    TickLabels.Add(new TickLabelINPC(x - 0.25 * OffsetTop, ViewHeight + 1.25 * OffsetTop, xx.ToString()));
                }
            }
            else
            {
                xx = 0;
                for (double x = OffsetLeft + ViewWidth / 2; x < ViewWidth; x += xinc)
                {
                    sPath += $"M {(int)x}, {OffsetTop} V {ViewHeight + OffsetTop} ";
                    // x tick labels
                    TickLabels.Add(new TickLabelINPC(x - 0.25 * OffsetTop, ViewHeight + 1.25 * OffsetTop, xx.ToString()));
                    xx += XIncrement;
                }

                xx = 0;
                for (double x = OffsetLeft + ViewWidth / 2 - xinc; x > OffsetLeft; x -= xinc)
                {
                    sPath += $"M {(int)x}, {OffsetTop} V {ViewHeight + OffsetTop} ";
                    // x tick labels
                    xx -= XIncrement;
                    TickLabels.Add(new TickLabelINPC(x - 0.25 * OffsetTop, ViewHeight + 1.25 * OffsetTop, xx.ToString()));
                }
            }

            double yinc = YIncrement * ScaleY;

            double yy = Ymin;

            // horizontal grid lines
            for (double y = ViewHeight + OffsetTop - yinc; y >= OffsetTop; y -= yinc)
            {
                yy += YIncrement;

                sPath += $"M {OffsetLeft}, {(int)y} H {ViewWidth + OffsetLeft} ";

                // ytick labels
                TickLabels.Add(new TickLabelINPC(0.25 * OffsetLeft, y - 0.25 * OffsetLeft, yy.ToString()));
            }

            GridPath = sPath;

            //Update curves e.g., rating curves, energy curves
            //double[] ys = new double[nInc];
            //double[] As = new double[nInc];
            //double[] Ps = new double[nInc];
            //double[] vs = new double[nInc];

            double delta = DepthChannel / nInc;

            for (int i = 0; i < nInc; i++)
            {
                ys[i] = (i + 1) * delta;
            }

            switch (model)
            {
                case RectangularChannel m:
                    RectangularChannel.CalculateCurves(m.B, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case TriangularChannel m:
                    TriangularChannel.CalculateCurves(m.SL, m.SR, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case TrapezoidalChannel m:
                    TrapezoidalChannel.CalculateCurves(m.B, m.SL, m.SR, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case ParabolicChannel m:
                    ParabolicChannel.CalculateCurves(m.T, m.d, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case CircularChannel m:
                    CircularChannel.CalculateCurves(m.r, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case EllipticalChannel m:
                    EllipticalChannel.CalculateCurves(m.a, m.b, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                case ArchChannel m:
                    ArchChannel.CalculateCurves(m.rb, m.rt, m.rc, m.rise, slopeChannel, manningsN, m.options.bUSCustomary, ys, As, Ps, vs);
                    break;
                default: throw new ArgumentException("Model variable is not a recognized channel model!");
            }

            int iy;
            double ey;

            string Ap = "", Pp = "", Rp = "", Vp = "", Qp = "", Ep = "";

            double Rf = As[nInc - 1] / Ps[nInc - 1];
            double Qf = vs[nInc - 1] * As[nInc - 1];
            for (int i = 0; i < nInc; i++)
            {
                iy = (int)ToViewY(ys[i]);
                ey = Discharge * Discharge / As[i] / As[i] / 2.0 / 32.2 + ys[i];

                if (i == 0)
                {
                    Ap = $"M {(int)ToViewX1(As[i] / As[nInc - 1])} {iy} ";
                    Pp = $"M {(int)ToViewX1(Ps[i] / Ps[nInc - 1])} {iy} ";
                    Rp = $"M {(int)ToViewX1(As[i] / Ps[i] / Rf)} {iy} ";
                    Vp = $"M {(int)ToViewX1(vs[i] / vs[nInc - 1])} {iy} ";
                    Qp = $"M {(int)ToViewX1(vs[i] * As[i] / Qf)} {iy} ";
                    if (ey <= 2.0 * DepthChannel)
                        Ep = $"M {(int)ToViewX2(ey / DepthChannel)} {iy} ";
                }
                else
                {
                    Ap += $"L {(int)ToViewX1(As[i] / As[nInc - 1])} {iy} ";
                    Pp += $"L {(int)ToViewX1(Ps[i] / Ps[nInc - 1])} {iy} ";
                    Rp += $"L {(int)ToViewX1(As[i] / Ps[i] / Rf)} {iy} ";
                    Vp += $"L {(int)ToViewX1(vs[i] / vs[nInc - 1])} {iy} ";
                    Qp += $"L {(int)ToViewX1(vs[i] * As[i] / Qf)} {iy} ";

                    if (ey <= 2.0 * DepthChannel)
                    {
                        if (Ep.Length == 0)
                            Ep = $"M {(int)ToViewX2(ey / DepthChannel)} {iy} ";
                        else
                            Ep += $"L {(int)ToViewX2(ey / DepthChannel)} {iy} ";
                    }
                }
            }

            AreaPath = Ap;
            PerimeterPath = Pp;
            HydraulicRadiusPath = Rp;
            VelocityPath = Vp;
            DischargePath = Qp;

            RaisePropertyChanged(nameof(ChCSPath));
            RaisePropertyChanged(nameof(NormPath));
            RaisePropertyChanged(nameof(CritPath));
            RaisePropertyChanged(nameof(GridPath));

            if (Ep == "")
            {

            }
            else
            {
                EnergyPath = Ep;
                RaisePropertyChanged(nameof(EnergyPath));
            }

            RaisePropertyChanged(nameof(DischargePath));
        }
    }
}
