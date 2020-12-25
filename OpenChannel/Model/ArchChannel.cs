using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class ArchChannel : Channel
    {
        public double rb;
        public double rt;
        public double rc;
        public double rise;
        public double Qmax => qmax;
        public double Yqmax => yqmax;
        private double qmax;
        private double yqmax;
        public ArchChannel() : base()
        {
            rb = 15.25;
            rt = 4.916667;
            rc = 1.25;
            rise = 6.0;
            N = 0.013;
        }
        public ArchChannel(IModelOptions refOptions) : base(refOptions)
        {
            rb = 15.25;
            rt = 4.916667;
            rc = 1.25;
            rise = 6.0;
            N = 0.013;
            Update();
        }

        public override void Update()
        {
            if (Dn < 0.000001) return;
            if (Dn > rise) return;

            GetAP(Dn, rb, rt, rc, rise, ref area, ref perimeter);
            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);

            double Q = velocity * area;
            dc = GetCriticalDepth(Q, rb, rt, rc, rise, options.bUSCustomary);

            double ac = 0, pc = 0;
            GetAP(dc, rb, rt, rc, rise, ref ac, ref pc);
            vc = velocity * area / ac;
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);

            twn = 2.0 * Getx(Dn, rb, rt, rc, rise);

            yqmax = GetyQmax(rb, rt, rc, rise);
            qmax = GetMaxDischarge(rb, rt, rc, rise, S, N, options.bUSCustomary);

            return;
        }

        public static void CalculateCurves(double rb, double rt, double rc, double rise, double S, double N, bool bUSCustomary,
                                   double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double areai = 0, perimeteri = 0;

            for (int i = 0; i < ys.Length; i++)
            {
                //GetAreaPerimeter(ys[i], rb, rt, rc, rise, ref areai, ref perimeteri);
                GetAP(ys[i], rb, rt, rc, rise, ref areai, ref perimeteri);
                As[i] = areai;
                Ps[i] = perimeteri;
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }

        //private double Theta => Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
        //private double Phi => Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
        //private double XD => (rb - rc) * Math.Sin(Theta);
        //private double YD => rb - (rb - rc) * Math.Cos(Theta);
        //private double XE => rb * Math.Sin(Theta);
        //private double YE => rb * (1.0 - Math.Cos(Theta));
        //private double XG => rt * Math.Sin(Phi);
        //private double YG => rise - rt + rt * Math.Cos(Phi);
        //private double AE => rb * rb * (Theta - Math.Sin(Theta) * Math.Cos(Theta));
        //private double AF => AE + rc * rc * (Math.PI / 2.0 - Theta) + (XD + XE) * (YD - YE);
        //private double AG => AF + rc * rc * (Math.PI / 2.0 - Phi) + (XD + XG) * (YG - YD);
        //private double ATotal => AG + rt * rt * (Phi - Math.Sin(Phi) * Math.Cos(Phi));
        //private double PTotal => 2.0 * rb * Theta + 2.0 * rc * (Math.PI - Theta - Phi) + 2.0 * rt * Phi;

        public static void GetAP(double y, double rb, double rt, double rc, double rise, ref double Area, ref double Perimeter)
        {
            if (y <= 0) return;

            double Theta = Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
            double YE = rb * (1.0 - Math.Cos(Theta));
            if (y <= YE)
            {
                double t = Math.Acos(1.0 - y / rb);
                Area = rb * rb * (t - Math.Sin(t) * Math.Cos(t));
                Perimeter = 2.0 * rb * t;
                return;
            }

            double XD = (rb - rc) * Math.Sin(Theta);
            double YD = rb - (rb - rc) * Math.Cos(Theta);
            double XE = rb * Math.Sin(Theta);
            double AE = rb * rb * (Theta - Math.Sin(Theta) * Math.Cos(Theta));
            if (y <= YD)  //YD = YF
            {
                double omega = Math.Acos((YD - y) / rc) - Theta;
                double xl = XD + rc * Math.Cos(Theta + omega) * Math.Tan(Theta);
                Area = AE + rc * rc * omega - rc * rc * (Math.Sin(omega + Theta) - Math.Cos(omega + Theta) * Math.Tan(Theta)) * Math.Cos(omega + Theta) + (XE + xl) * (y - YE);
                Perimeter = 2.0 * rb * Theta + 2.0 * rc * omega; ;
                return;
            }

            double Phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double YG = rise - rt + rt * Math.Cos(Phi);
            double AF = AE + rc * rc * (Math.PI / 2.0 - Theta) + (XD + XE) * (YD - YE);
            if (y <= YG)
            {
                double eta = Math.Asin((y - YD) / rc);
                Area = AF + rc * rc * eta + (2.0 * XD + rc * Math.Cos(eta)) * rc * Math.Sin(eta);
                Perimeter = 2.0 * rb * Theta + 2.0 * rc * (Math.PI / 2.0 - Theta + eta);
                return;
            }

            double XG = rt * Math.Sin(Phi);
            double AG = AF + rc * rc * (Math.PI / 2.0 - Phi) + (XD + XG) * (YG - YD);
            if (y >= rise)
            {
                Area = AG + rt * rt * (Phi - Math.Sin(Phi) * Math.Cos(Phi));
                Perimeter = 2.0 * rb * Theta + 2.0 * rc * (Math.PI - Theta - Phi) + 2.0 * rt * Phi;
                return;
            }

            double yc = rise - rt;
            double tt = Math.Acos((y - yc) / rt);
            Area = AG + rt * rt * (Phi - Math.Sin(Phi) * Math.Cos(Phi)) - rt * rt * (tt - Math.Sin(tt) * Math.Cos(tt));
            Perimeter = 2.0 * rb * Theta + 2.0 * rc * (Math.PI - Theta - Phi) + 2.0 * rt * (Phi - tt);
            return;
        }
        public static double Getx(double y, double rb, double rt, double rc, double rise)
        {
            if (y <= 0.0) return 0.0;
            if (y >= rise) return 0.0;

            double theta = Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
            double ye = rb * (1.0 - Math.Cos(theta));

            if (y <= ye)
            {
                double t = Math.Acos(1.0 - y / rb);
                return rb * Math.Sin(t);
            }

            double xd = (rb - rc) * Math.Sin(theta);
            double yd = rb - (rb - rc) * Math.Cos(theta);
            if (y <= yd)
            {
                double omegaPtheta = Math.Acos((yd - y) / rc);
                return xd + rc * Math.Sin(omegaPtheta);
            }

            double yc = rise - rt;
            double phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double yg = yc + rt * Math.Cos(phi);
            if (y <= yg)
            {
                double eta = Math.Asin((y - yd) / rc);
                return xd + rc * Math.Cos(eta);
            }

            double zeta = Math.Acos((y - yc) / rt);
            return rt * Math.Sin(zeta);
        }

        public static double GetTopWidth(double y, double rb, double rt, double rc, double rise)
        {
            if (y <= 0) return 0.0;

            double theta = Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
            double ye = rb * (1.0 - Math.Cos(theta));

            if (y <= ye)
            {
                double t = Math.Acos(1.0 - y / rb);
                return 2.0 * rb * Math.Sin(t);
            }

            double xd = (rb - rc) * Math.Sin(theta);
            double yd = rb - (rb - rc) * Math.Cos(theta);

            if (y <= yd)
            {
                double omegaPtheta = Math.Acos((yd - y) / rc);
                return 2.0 * xd + 2.0 * rc * Math.Sin(omegaPtheta);
            }

            double yc = rise - rt;
            double phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double yg = yc + rt * Math.Cos(phi);

            if (y <= yg)
            {
                double eta = Math.Asin((y - yd) / rc);
                return 2.0 * xd + 2.0 * rc * Math.Cos(eta);
            }

            if (y >= rise)
            {
                return 0.0;
            }

            double tt = Math.Acos((y - yc) / rt);

            return 2.0 * rt * Math.Sin(tt);
        }
        public static double GetZetaMaxDischarge(double rb, double rt, double rc, double rise)
        {
            // maximum discharge occurs close to the top of the arch
            // f(yita)  = 5A'P - 2AP'
            // f'(yita) = 5A''P + 3A'P' - 2AP"
            // P" = 0
            double A, P, dA, ddA, dP = -2.0 * rt, ddP = 0.0, f = 100.0, df;
            double dzeta = 10.0;
            double phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double zetai = 0.5 * phi, zetamin = 0.0, zetamax = phi;
            double Atotal = 0; // GetAreaTotal(rb, rt, rc, rise);
            double Ptotal = 0; // GetPerimeterTotal(rb, rt, rc, rise);
            int count = 0;
            GetAP(rise, rb, rt, rc, rise, ref Atotal, ref Ptotal);

            while (Math.Abs(dzeta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                A = Atotal - rt * rt * (zetai - Math.Sin(zetai) * Math.Cos(zetai));
                dA = -1.0 * rt * rt * (1.0 - Math.Cos(2.0 * zetai));
                ddA = -2.0 * rt * rt * Math.Sin(2.0 * zetai);
                P = Ptotal - 2.0 * rt * zetai;
                f = 5.0 * dA * P - 2.0 * A * dP;

                if (Math.Abs(f) < Constants.TolD) break;

                df = 3.0 * dA * dP + 5.0 * ddA * P;
                try
                {
                    dzeta = f / df;

                    if (zetai - dzeta <= zetamin)
                    {
                        zetai = 0.5 * (zetai + zetamin);
                        continue;
                    }

                    if (zetai - dzeta >= zetamax)
                    {
                        zetai = 0.5 * (zetai + zetamax);
                    }

                    if (f <= 0)
                        zetamax = zetai;
                    else
                        zetamin = zetai;

                    zetai -= dzeta;
                }
                catch (Exception ex)
                {
                    if (f <= 0)
                        zetamax = zetai;
                    else
                        zetamin = zetai;

                    zetai = 0.5 * (zetamin + zetamax);
                }

                count++;
                if (count > Constants.MaxCount) break;
            }

            return zetai;
        }
        public static double GetyQmax(double rb, double rt, double rc, double rise)
        {
            double zeta = GetZetaMaxDischarge(rb, rt, rc, rise);
            return rise - rt + rt * Math.Cos(zeta);
        }
        public static double GetMaxDischarge(double rb, double rt, double rc, double rise, double S, double N, bool bUSCustomary)
        {
            // maximum discharge occurs close to the top of the arch
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double zeta = GetZetaMaxDischarge(rb, rt, rc, rise);
            //double A = GetAreaTotal(rb, rt, rc, rise) - rt * rt * (zeta - Math.Sin(zeta) * Math.Cos(zeta));
            //double P = GetPerimeterTotal(rb, rt, rc, rise) - 2.0 * rt * zeta;

            double AT = 0, PT = 0;
            GetAP(rise, rb, rt, rc, rise, ref AT, ref PT);
            double Am = AT - rt * rt * (zeta - Math.Sin(zeta) * Math.Cos(zeta));
            double Pm = PT - 2.0 * rt * zeta;
            double v = Ku / N * Math.Pow(Am / Pm, Constants.X) * Math.Pow(S, Constants.Y);
            return v * Am;
        }

        public static double GetNormalDepth(double Q, double rb, double rt, double rc, double rise, double S, double N, bool bUSCustomary)
        {
            if (Q <= 0.0) return 0.0;

            double Qmax = GetMaxDischarge(rb, rt, rc, rise, S, N, bUSCustomary);
            double zetamax = GetZetaMaxDischarge(rb, rt, rc, rise);
            if (Q >= Qmax)
            {
                double ymax = rise - rt + rt * Math.Cos(zetamax);
                if (Q > Qmax)
                    return rise;
                else
                    return ymax;
            }

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double A, P, dA, dP, f = 100.0, df;
            double dd = 10.0, xmin, xmax;
            int count = 0;

            double theta = Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
            double ye = rb * (1.0 - Math.Cos(theta));
            double Ae = rb * rb * (theta - Math.Sin(theta) * Math.Cos(theta));
            double Pe = 2.0 * rb * theta;
            double Qe = Ae * Ku / N * Math.Pow(Ae / Pe, Constants.X) * Math.Pow(S, Constants.Y);

            if (Math.Abs(Q - Qe) < Constants.TolD) return ye;

            if (Q <= Qe)
            {
                xmin = 0.0;
                xmax = theta;
                double xtheta = 0.5 * (xmin + xmax);
                dP = 2.0 * rb;

                while (Math.Abs(dd) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
                {
                    A = rb * rb * (xtheta - Math.Sin(xtheta) * Math.Cos(xtheta));
                    dA = rb * rb * (1.0 - Math.Cos(2.0 * xtheta));
                    P = 2.0 * rb * xtheta;
                    f = A * Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y) - Q;

                    if (f <= 0)
                        xmin = xtheta;
                    else
                        xmax = xtheta;

                    if (Math.Abs(f) < Constants.TolD) break;

                    df = Ku / N / 3.0 * Math.Pow(A / P, Constants.X) * (5.0 * dA - 2.0 * A / P * dP) * Math.Pow(S, Constants.Y);
                    try
                    {
                        dd = f / df;

                        if (xtheta - dd <= xmin || xtheta - dd >= xmax)
                        {
                            if (f > 0)
                            {
                                xtheta = 0.5 * (xtheta + xmin);
                            }
                            else
                            {
                                xtheta = 0.5 * (xtheta + xmax);
                            }
                            continue;
                        }

                        xtheta -= dd;
                    }
                    catch (Exception ex)
                    {
                        xtheta = 0.5 * (xmin + xmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return rb * (1.0 - Math.Cos(xtheta));
            }

            double x, xl, y;
            double yd = rb - (rb - rc) * Math.Cos(theta);
            double xe = rb * Math.Sin(theta);
            double xd = (rb - rc) * Math.Sin(theta);
            double Af = Ae + rc * rc * (Math.PI / 2.0 - theta) + (xd + xe) * (yd - ye);
            double Pf = Pe + 2.0 * rc * (Math.PI / 2.0 - theta);
            double Qf = Af * Ku / N * Math.Pow(Af / Pf, Constants.X) * Math.Pow(S, Constants.Y);

            if (Math.Abs(Q - Qf) < Constants.TolD) return yd;

            if (Q <= Qf)
            {
                f = 100.0;
                xmin = 0.0;
                xmax = Math.PI / 2.0 - theta;
                double omega = 0.5 * (xmin + xmax);
                dP = 2.0 * rc;

                while (Math.Abs(dd) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
                {

                    //x = xd + rc * Math.Sin(theta + omega);
                    xl = xd + rc * Math.Cos(theta + omega) * Math.Tan(theta);
                    y = yd - rc * Math.Cos(theta + omega);
                    //A = Ae + rc * rc * (omega - Math.Sin(omega + theta) + Math.Cos(omega + theta) * Math.Tan(theta)) * Math.Cos(omega + theta) + (xe + x) * (y - ye);
                    A = Ae + rc * rc * omega - rc * rc * (Math.Sin(omega + theta) - Math.Cos(omega + theta) * Math.Tan(theta)) * Math.Cos(omega + theta) + (xe + xl) * (y - ye);
                    P = Pe + 2.0 * rc * omega;
                    dA = rc * rc * (1.0 - Math.Cos(2.0 * (omega + theta)) - Math.Cos(2.0 * (omega + theta)) * Math.Tan(theta))
                       + rc * (xe + xl) * Math.Sin(omega + theta) + rc * (y - ye) * Math.Cos(omega + theta);
                    f = A * Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y) - Q;

                    if (f <= 0)
                        xmin = omega;
                    else
                        xmax = omega;

                    if (Math.Abs(f) < Constants.TolD) break;

                    df = Ku / N / 3.0 * Math.Pow(A / P, Constants.X) * (5.0 * dA - 2.0 * A / P * dP) * Math.Pow(S, Constants.Y);
                    try
                    {
                        dd = f / df;

                        if (omega - dd <= xmin || omega - dd >= xmax)
                        {
                            if (f > 0)
                            {
                                omega = 0.5 * (omega + xmin);
                            }
                            else
                            {
                                omega = 0.5 * (omega + xmax);
                            }
                            //dd = 0.5 * (omega - xmin);
                            continue;
                        }

                        omega -= dd;
                    }
                    catch (Exception ex)
                    {
                        omega = 0.5 * (xmin + xmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return yd - rc * Math.Cos(omega + theta);
            }


            double yc = rise - rt;
            double phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double XG = rt * Math.Sin(phi);
            double yg = yc + rt * Math.Cos(phi);
            double Ag = Af + rc * rc * (Math.PI / 2.0 - phi) + (xd + XG) * (yg - yd);
            double Pg = Pf + 2.0 * rc * (Math.PI / 2.0 - phi);
            double Qg = Ag * Ku / N * Math.Pow(Ag / Pg, Constants.X) * Math.Pow(S, Constants.Y);

            if (Math.Abs(Q - Qg) < Constants.TolD) return yg;
            double eta;

            if (Q <= Qg)
            {
                dd = 100.0;
                f = 100.0;
                xmin = 0.0;
                xmax = Math.PI / 2.0 - phi;
                eta = 0.5 * (xmin + xmax);
                dP = 2.0 * rc;

                while (Math.Abs(dd) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
                {

                    x = xd + rc * Math.Cos(eta);
                    y = yd - rc * Math.Sin(eta);
                    A = Af + rc * rc * eta + (2.0 * xd + rc * Math.Cos(eta)) * rc * Math.Sin(eta);
                    P = Pf + 2.0 * rc * eta;
                    dA = rc * rc + 2.0 * xd * rc * Math.Cos(eta) + rc * rc * Math.Cos(2.0 * eta);
                    f = A * Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y) - Q;

                    if (f <= 0)
                        xmin = eta;
                    else
                        xmax = eta;

                    if (Math.Abs(f) < Constants.TolD) break;

                    df = Ku / N / 3.0 * Math.Pow(A / P, Constants.X) * (5.0 * dA - 2.0 * A / P * dP) * Math.Pow(S, Constants.Y);
                    try
                    {
                        dd = f / df;

                        if (eta - dd <= xmin || eta - dd >= xmax)
                        {
                            //eta = 0.5 * (eta - xmin);
                            if (f > 0)
                            {
                                eta = 0.5 * (eta + xmin);
                            }
                            else
                            {
                                eta = 0.5 * (eta + xmax);
                            }

                            continue;
                        }

                        eta -= dd;
                    }
                    catch (Exception ex)
                    {
                        eta = 0.5 * (xmin + xmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return yd + rc * Math.Sin(eta);
            }

            double At = Ag + rt * rt * (phi - Math.Sin(phi) * Math.Cos(phi));
            double Pt = Pg + 2.0 * rt * phi;
            double delta = 10.0;
            xmin = zetamax;
            xmax = phi;
            eta = 0.5 * (xmin + xmax);
            dP = -2.0 * rt;

            while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                A = At - rt * rt * (eta - Math.Sin(eta) * Math.Cos(eta));
                P = Pt - 2.0 * rt * eta;
                dA = -rt * rt * (1.0 - Math.Cos(2.0 * eta));
                f = A * Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y) - Q;

                if (f >= 0)
                    xmin = eta;
                else
                    xmax = eta;

                if (Math.Abs(f) < Constants.TolD) break;

                df = Ku / N / 3.0 * Math.Pow(A / P, Constants.X) * (5.0 * dA - 2.0 * A / P * dP) * Math.Pow(S, Constants.Y);
                try
                {
                    delta = f / df;

                    if (eta - delta <= xmin)
                    {
                        delta = 0.5 * (eta - xmin);
                        continue;
                    }

                    if (eta - delta >= xmax)
                    {
                        delta = 0.5 * (eta - xmax);
                    }

                    eta -= delta;
                }
                catch (Exception ex)
                {
                    eta = 0.5 * (xmin + xmax);
                }

                count++;
                if (count > Constants.MaxCount) break;
            }

            return yc + rt * Math.Cos(eta);
        }
        public static double GetCriticalDepth(double Q, double rb, double rt, double rc, double rise, bool bUSCustomary)
        {
            // E               = v^2/2g + y = Q^2/2gA^2 + y
            // dE/y            = -Q^2/gA^3 dA/y + 1 = 0
            // f(theta)        = g A^3 - Q^2 dA/dy = 0
            // dA/dy           = dA/dtheta / dy/dtheta = 2 r sin(theta/2)
            // dfdtheta        = 3 gA^2 dA/dtheta - Q^2 d(dA/dy)/dtheta
            // d(dA/dy)/dtheta = r cos(theta/2)
            // A               = (theta - sin(theta))r^2 / 2
            // dA/dtheta       = (1 - cos(theta)) r^2 / 2
            // y               = r (1 - cos(theta/2))
            // dy/dtheta       = -r sin(theta/2)/2

            if (Q <= 0.0) return 0.0;

            double g = bUSCustomary ? Constants.gUS : Constants.gSI;

            double Theta = Math.Acos(((rb - rc) * (rb - rc) + (rb + rt - rise) * (rb + rt - rise) - (rt - rc) * (rt - rc)) / 2.0 / (rb - rc) / (rb + rt - rise));
            double XE = rb * Math.Sin(Theta);
            double YE = rb * (1.0 - Math.Cos(Theta));
            double AE = rb * rb * (Theta - Math.Sin(Theta) * Math.Cos(Theta));
            double EE = YE + Q * Q / AE / AE / 2.0 / g;
            double dAdy = 2.0 * rb * Math.Sin(Theta);
            double QcE = Math.Sqrt(g * AE * AE * AE / dAdy);
            double ti, tmin, tmax, delta = 10.0;
            double A, ddAdydt, dAdt, dydt, f = 100.0, df;
            int count = 0;

            if (Q <= QcE)
            {
                tmin = 0.0;
                tmax = Theta;
                ti = 0.5 * (tmin + tmax);

                while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
                {
                    A = rb * rb * (ti - Math.Sin(ti) * Math.Cos(ti));
                    dAdt = rb * rb * (1.0 - Math.Cos(2.0 * ti));
                    dAdy = 2.0 * rb * Math.Sin(ti);
                    ddAdydt = 2.0 * rb * Math.Cos(ti);
                    f = g * A * A * A - Q * Q * dAdy;

                    if (Math.Abs(f) < Constants.TolD) break;

                    if (f > 0)
                        tmax = Math.Min(ti, tmax);
                    else
                        tmin = Math.Max(ti, tmin);

                    df = 3.0 * g * A * A * dAdt - Q * Q * ddAdydt;
                    try
                    {
                        delta = f / df;

                        if (ti - delta <= tmin || ti - delta >= tmax)
                        {
                            if (f > 0)
                                ti = 0.5 * (ti + tmin);
                            else
                                ti = 0.5 * (ti + tmax);
                            continue;
                        }

                        ti -= delta;
                    }
                    catch (Exception ex)
                    {
                        ti = 0.5 * (tmin + tmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return rb * (1.0 - Math.Cos(ti));
            }

            double Phi = Math.Acos(((rt - rc) * (rt - rc) + (rb + rt - rise) * (rb + rt - rise) - (rb - rc) * (rb - rc)) / 2.0 / (rt - rc) / (rb + rt - rise));
            double XD = (rb - rc) * Math.Sin(Theta);
            double YF = rb - (rb - rc) * Math.Cos(Theta);
            double AF = AE + rc * rc * (Math.PI / 2.0 - Theta) + (XD + XE) * (YF - YE);
            double EF = YF + Q * Q / AF / AF / 2.0 / g;
            // at F, theta + omega = PI/2

            dydt = rc;
            double XL, dXLdt = -rc * Math.Tan(Theta);
            dAdt = rc * rc * 2.0 + 2.0 * XD * dydt + dXLdt * (YF - YE);
            dAdy = dAdt / dydt;
            double QcF = Math.Sqrt(g * AF * AF * AF / dAdy);
            double y;

            if (Q <= QcF)
            {
                tmin = 0.0;
                tmax = Math.PI / 2.0 - Theta;
                ti = 0.5 * (tmin + tmax);
                double ddXLdt, ddAdt, ddydt;


                while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
                {
                    XL = XD + rc * Math.Cos(Theta + ti) * Math.Tan(Theta);
                    y = YF - rc * Math.Cos(Theta + ti);
                    A = AE + rc * rc * ti - rc * rc * (Math.Sin(ti + Theta) - Math.Cos(ti + Theta) * Math.Tan(Theta)) * Math.Cos(ti + Theta) + (XE + XL) * (y - YE);
                    dydt = rc * Math.Sin(ti + Theta);
                    ddydt = rc * Math.Cos(ti + Theta);
                    dXLdt = -rc * Math.Sin(ti + Theta) * Math.Tan(Theta);
                    ddXLdt = -rc * Math.Cos(ti + Theta) * Math.Tan(Theta);
                    dAdt = rc * rc * (1.0 - Math.Cos(2.0 * (ti + Theta)) - Math.Sin(2.0 * (ti + Theta)) * Math.Tan(Theta)) + (XE + XL) * dydt + dXLdt * (y - YE);
                    dAdy = dAdt / dydt;
                    ddAdt = 2.0 * rc * rc * (Math.Sin(2.0 * (ti + Theta)) - Math.Cos(2.0 * (ti + Theta)) * Math.Tan(Theta)) + (XE + XL) * ddydt + dXLdt * dydt + ddXLdt * (y - YE) + dXLdt * ddydt;
                    ddAdydt = (ddAdt * dydt - dAdt * ddydt) / dydt / dydt;
                    f = g * A * A * A - Q * Q * dAdy;

                    if (Math.Abs(f) < Constants.TolD) break;

                    if (f > 0)
                        tmax = Math.Min(ti, tmax);
                    else
                        tmin = Math.Max(ti, tmin);

                    df = 3.0 * g * A * A * dAdt - Q * Q * ddAdydt;
                    try
                    {
                        delta = f / df;

                        if (ti - delta <= tmin || ti - delta >= tmax)
                        {
                            if (f > 0)
                                ti = 0.5 * (ti + tmin);
                            else
                                ti = 0.5 * (ti + tmax);
                            continue;
                        }

                        ti -= delta;
                    }
                    catch (Exception ex)
                    {
                        ti = 0.5 * (tmin + tmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return YF - rc * Math.Cos(ti + Theta);
            }


            double XG = rt * Math.Sin(Phi);
            double YG = rise - rt + rt * Math.Cos(Phi);
            double AG = AF + rc * rc * (Math.PI / 2.0 - Phi) + (XD + XG) * (YG - YF);
            double EG = YG + Q * Q / AG / AG / 2.0 / g;
            double eta = Math.PI / 2.0 - Phi;
            dydt = rc * Math.Cos(eta);
            dAdt = rc * rc + 2.0 * XD * rc * Math.Cos(eta) + rc * rc * Math.Cos(2.0 * eta);
            dAdy = dAdt / dydt;
            double QcG = Math.Sqrt(g * AG * AG * AG / dAdy);

            if (Q <= QcG)
            {
                tmin = 0.0;
                tmax = Math.PI / 2.0 - Phi;
                ti = 0.5 * (tmin + tmax);
                double ddAdt, ddydt;


                while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD && Math.Abs(tmax - tmin) > Constants.TolAngle)
                {
                    y = YF + rc * Math.Sin(ti);
                    A = AF + rc * rc * ti + (2.0 * XD + rc * Math.Cos(ti)) * rc * Math.Sin(ti);
                    dydt = rc * Math.Cos(ti);                                                          //y'
                    ddydt = -rc * Math.Sin(ti);                                                        //y''
                    dAdt = rc * rc + 2.0 * XD * rc * Math.Cos(ti) + rc * rc * Math.Cos(2.0 * ti);      //A'
                    ddAdt = -2.0 * XD * rc * Math.Sin(ti) - 2.0 * rc * rc * Math.Sin(2.0 * ti);         //A'' 
                    dAdy = dAdt / dydt;
                    ddAdydt = (ddAdt * dydt - dAdt * ddydt) / dydt / dydt;
                    f = g * A * A * A - Q * Q * dAdy;

                    if (Math.Abs(f) < Constants.TolD) break;

                    if (f > 0)
                        tmax = Math.Min(ti, tmax);
                    else
                        tmin = Math.Max(ti, tmin);

                    df = 3.0 * g * A * A * dAdt - Q * Q * ddAdydt;
                    try
                    {
                        delta = f / df;

                        if (ti - delta <= tmin || ti - delta >= tmax)
                        {
                            if (f > 0)
                                ti = 0.5 * (ti + tmin);
                            else
                                ti = 0.5 * (ti + tmax);
                            continue;
                        }

                        ti -= delta;
                    }
                    catch (Exception ex)
                    {
                        ti = 0.5 * (tmin + tmax);
                    }

                    count++;
                    if (count > Constants.MaxCount) break;
                }

                return YF + rc * Math.Sin(ti);
            }


            double AT = AG + rt * rt * (Phi - Math.Sin(Phi) * Math.Cos(Phi));
            double ET = rise + Q * Q / AT / AT / 2.0 / g;

            tmin = 0.0;
            tmax = Phi;
            ti = 0.5 * (tmin + tmax);

            while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD && Math.Abs(tmax - tmin) > Constants.TolAngle)
            {
                A = AT - rt * rt * (ti - Math.Sin(ti) * Math.Cos(ti));
                dAdt = -rt * rt * (1 - Math.Cos(2.0 * ti));
                dAdy = 2.0 * rt * Math.Sin(ti);
                ddAdydt = 2.0 * rt * Math.Cos(ti); ;
                f = g * A * A * A - Q * Q * dAdy;

                if (Math.Abs(f) < Constants.TolD) break;

                if (f < 0)
                    tmax = Math.Min(ti, tmax);
                else
                    tmin = Math.Max(ti, tmin);

                df = 3.0 * g * A * A * dAdt - Q * Q * ddAdydt;
                try
                {
                    delta = f / df;

                    if (ti - delta <= tmin || ti - delta >= tmax)
                    {
                        if (f < 0)
                            ti = 0.5 * (ti + tmin);
                        else
                            ti = 0.5 * (ti + tmax);
                        continue;
                    }

                    ti -= delta;
                }
                catch (Exception ex)
                {
                    ti = 0.5 * (tmin + tmax);
                }

                count++;
                if (count > Constants.MaxCount) break;
            }

            return rise - rt + rt * Math.Cos(ti);
        }
    }
}
