using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class EllipticalChannel : Channel
    {
        public double a;  //span
        public double b;  //rise

        private double qmax;
        private double yqmax;
        public double Qmax => qmax;
        public double Yqmax => yqmax;
        public EllipticalChannel() : base()
        {
            a = 1.583334;
            b = 1.0;
        }
        public EllipticalChannel(IModelOptions refOptions) : base(refOptions)
        {
            a = 1.583334;
            b = 1.0;
            Update();
        }
        public override void Update()
        {
            double alpha = Math.Acos(1.0 - Dn / b);

            area = GetArea(alpha, a, b);
            perimeter = GetPerimeter(alpha, area, b);

            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = GetCriticalDepth(area * velocity, a, b, options.bUSCustomary);
            alpha = Math.Acos(1.0 - dc / b); ;
            double ac = GetArea(alpha, a, b);
            double pc = GetPerimeter(alpha, a, b);
            vc = velocity * area / ac;
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);
            twn = 2 * a * Math.Sin(Math.Acos(1.0 - Dn / b));

            qmax = GetMaxDischarge(a, b, S, N, options.bUSCustomary);
            yqmax = GetyQmax(a, b);
        }
        public static void CalculateCurves(double a, double b, double S, double N, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double alpha;

            for (int i = 0; i < ys.Length; i++)
            {
                alpha = Math.Acos(1.0 - Math.Min(ys[i] / b, 2.0));
                As[i] = GetArea(alpha, a, b);
                Ps[i] = GetPerimeter(alpha, a, b);
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }

        public static double GetPerimeter(double alpha, double a, double b)
        {
            double sinal = Math.Sin(alpha);
            double cosal = Math.Cos(alpha);
            double sinco = sinal * cosal;

            double intsc = a <= b ? 0.5 * sinal * cosal + 0.5 * alpha    // \int_0^alpha cos^2 t dt 
                                  : -0.5 * sinal * cosal + 0.5 * alpha;  // \int_0^alpha sin^2 t dt

            double k2 = a >= b ? 1.0 - b * b / a / a : 1.0 - a * a / b / b;

            double prefix = 0.5 * k2;
            double p = alpha - prefix * intsc;

            int n = 2;
            double delta;
            do
            {
                if (a < b)
                {
                    sinco *= cosal * cosal;
                    intsc = sinco / 2.0 / (double)n + (2.0 * (double)n - 1.0) / 2.0 / (double)n * intsc;
                }
                else
                {
                    sinco *= sinal * sinal;
                    intsc = -sinco / 2.0 / (double)n + (2.0 * (double)n - 1.0) / 2.0 / (double)n * intsc;
                }

                prefix *= k2 / 2.0 / (double)n;
                delta = prefix * intsc;
                p -= delta;
                n++;
            } while (delta > Constants.TolAngle);

            return 2.0 * p * Math.Max(a, b);
        }

        public static double GetArea(double alpha, double a, double b)
        {
            return a * b * (alpha - Math.Sin(alpha) * Math.Cos(alpha));
        }
        public static double GetAlphaMaxDischarge(double a, double b)
        {
            double alpha = 7.0 / 8.0 * Math.PI;
            double delta;
            double A, dA, ddA, P, dP, ddP, ds, f, df;

            do
            {
                A = GetArea(alpha, a, b);
                dA = a * b * (1.0 - Math.Cos(2.0 * alpha));
                ddA = 2.0 * a * b * Math.Sin(2.0 * alpha);
                P = GetPerimeter(alpha, a, b);
                ds = Math.Sqrt(a * a * Math.Cos(alpha) * Math.Cos(alpha) +
                               b * b * Math.Sin(alpha) * Math.Sin(alpha));
                dP = 2.0 * ds;
                ddP = -(a * a - b * b) * Math.Sin(2.0 * alpha) / ds;
                f = 5.0 * P * dA - 2 * A * dP;

                if (Math.Abs(f) < 1.0e-6) break;
                df = 3.0 * dP * dA + 5.0 * P * ddA - 2.0 * A * ddP;
                delta = f / df;

                alpha -= delta;

            } while (Math.Abs(delta) > 0.000001);

            return alpha;
        }

        public static double GetMaxDischarge(double a, double b, double S, double N, bool bUSCustomary)
        {
            double alpha = GetAlphaMaxDischarge(a, b);
            double A = GetArea(alpha, a, b);
            double P = GetPerimeter(alpha, a, b);
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double v = Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y);
            return v * A;
        }

        public static double GetyQmax(double a, double b)
        {
            double alpha = GetAlphaMaxDischarge(a, b);
            return b * (1.0 - Math.Cos(alpha));
        }
        public static double GetNormalDepth(double Q, double a, double b, double S, double N, bool bUSCustomary)
        {
            double Qmax = GetMaxDischarge(a, b, S, N, bUSCustomary);
            if (Q >= Qmax) return 2.0 * b;

            // Q     = Ku/n S^1/2 A^5/3 P^-2/3 
            // f     = Ku/n S^1/2 A^5/3 P^-2/3 - Q
            // df    = Ku/n S^1/2 (5/3 A^2/3 P^-2/3 dA/dtheta - 2/3 A^5/3 P^-5/3 dP/dtheta)
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double A, P, dA, dP, f = 10.0, df;
            double delta = 10.0;
            double alphai = 0.5 * Math.PI;
            int count = 0;

            while (Math.Abs(delta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                A = GetArea(alphai, a, b);
                dA = a * b * (1.0 - Math.Cos(2.0 * alphai));
                P = GetPerimeter(alphai, a, b);
                dP = 2 * Math.Sqrt(a * a * Math.Sin(alphai) * Math.Sin(alphai) +
                                   b * b * Math.Cos(alphai) * Math.Cos(alphai));

                f = Ku / N * Math.Sqrt(S) * Math.Pow(A, 5.0 / 3.0) * Math.Pow(P, -2.0 / 3.0) - Q;
                df = Ku / N * Math.Sqrt(S) * (5.0 / 3.0 * Math.Pow(A / P, 2.0 / 3.0) * dA - 2.0 / 3.0 * Math.Pow(A / P, 5.0 / 3.0) * dP);
                delta = f / df;
                alphai -= delta;
                count++;
                if (count > Constants.MaxCount) break;
            }

            return b * (1.0 - Math.Cos(alphai));
        }
        public static double GetCriticalDepth(double Q, double a, double b, bool bUSCustomary)
        {
            // E               = v^2/2g + y = Q^2/2gA^2 + y
            // dE/y            = -Q^2/gA^3 dA/y + 1 = 0
            // f(alpha)        = g A^3 - Q^2 dA/dy = 0
            // dA/dy           = dA/dtheta / dy/dtheta = 2 a sin(alpha)
            // dfdalpha        = 3 gA^2 dA/dalpha - Q^2 d(dA/dy)/dalpha
            // d(dA/dy)/dalpha = 2 a cos(alpha)

            double g = bUSCustomary ? Constants.gUS : Constants.gSI;

            double A, dAdy, dAdalpha, ddAdydalpha, f = 10.0, df;
            double deltaalpha = 10.0;
            double amin = 0.0, amax = Math.PI;
            int count = 0;
            //use French 1985 Table 2.1 Equation to estimate yc
            double yi = b;

            double alphai = Math.PI;

            while (Math.Abs(deltaalpha) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                A = GetArea(alphai, a, b);
                dAdy = 2.0 * a * Math.Sin(alphai);
                f = g * A * A * A - Q * Q * dAdy;
                if (f > 0)
                    amax = alphai;
                else
                    amin = alphai;

                dAdalpha = a * b * (1.0 - Math.Cos(2.0 * alphai));
                ddAdydalpha = 2.0 * a * Math.Cos(alphai);
                df = 3.0 * g * A * A * dAdalpha - Q * Q * ddAdydalpha;
                deltaalpha = f / df;

                if (alphai - deltaalpha < amin)
                {
                    alphai = 0.5 * (alphai + amin);
                    continue;
                }

                if (alphai - deltaalpha > amax)
                {
                    alphai = 0.5 * (alphai + amax);
                    continue;
                }

                alphai -= deltaalpha;
                count++;
                if (count > Constants.MaxCount) break;
            }

            return b * (1.0 - Math.Cos(alphai));
        }
    }
}
