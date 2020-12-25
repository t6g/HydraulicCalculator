using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class ParabolicChannel : Channel
    {
        public double d;
        public double T;
        public ParabolicChannel() : base()
        {
            d = 2.0;
            T = 10.0;
        }
        public ParabolicChannel(IModelOptions refOptions) : base(refOptions)
        {
            d = 2.0;
            T = 10.0;
            Update();
        }

        public override void Update()
        {
            double pt2;
            double pt1 = T * T / 16.0 / d;
            area = 2.0 * T / 3.0 / Math.Sqrt(d) * Dn * Math.Sqrt(Dn);
            pt2 = Math.Sqrt(Dn * Dn + pt1 * Dn);
            perimeter = pt1 * Math.Log((pt2 + Dn) / (pt2 - Dn)) + 2.0 * pt2;

            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = GetCriticalDepth(area * velocity, T, d, options.bUSCustomary);
            double ac = 2.0 * T / 3.0 / Math.Sqrt(d) * Dc * Math.Sqrt(Dc); ;
            pt2 = Math.Sqrt(Dc * Dc + pt1 * Dc);
            double pc = pt1 * Math.Log((pt2 + Dc) / (pt2 - Dc)) + 2.0 * pt2;
            vc = velocity * area / ac;
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);
            twn = T * Math.Sqrt(Dn / d);
        }
        public static void CalculateCurves(double T, double D, double S, double N, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double pt2;
            double pt1 = T * T / 16.0 / D;

            for (int i = 0; i < ys.Length; i++)
            {
                As[i] = 2.0 * T / 3.0 / Math.Sqrt(D) * ys[i] * Math.Sqrt(ys[i]);
                pt2 = Math.Sqrt(ys[i] * ys[i] + pt1 * ys[i]);
                Ps[i] = pt1 * Math.Log((pt2 + ys[i]) / (pt2 - ys[i])) + 2.0 * pt2;
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }

        public static double GetNormalDepth(double Q, double T, double D, double S, double N, bool bUSCustomary)
        {
            double y = D;
            double A = 2.0 * T / 3.0 / Math.Sqrt(D) * y * Math.Sqrt(y);
            double a = T * T / 16.0 / D;
            double b = Math.Sqrt(y * y + a * y);
            double P = a * Math.Log((b + y) / (b - y)) + 2.0 * b;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;

            double v = Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y);
            double Qmax = v * A;

            if (Q >= Qmax)
                return D;

            double dAdy, dPdy, f = 10.0, df;
            double deltay = 10.0;
            int count = 0;

            y = 0.5 * D;

            while (Math.Abs(deltay) > Constants.TolD && Math.Abs(f) > Constants.TolD)
            {
                A = 2.0 * T / 3.0 / Math.Sqrt(D) * y * Math.Sqrt(y);
                dAdy = T * Math.Sqrt(y / D);
                b = Math.Sqrt(y * y + a * y);
                P = a * Math.Log((b + y) / (b - y)) + 2.0 * b;
                dPdy = a * ((2.0 * y + a) / b + 1) / (b + y) - a * ((2.0 * y + a) / b - 1) / (b - y) + (2.0 * y + a) / b;

                f = Ku / N * Math.Sqrt(S) * Math.Pow(A, 5.0 / 3.0) * Math.Pow(P, -2.0 / 3.0) - Q;
                df = Ku / N * Math.Sqrt(S) * (5.0 / 3.0 * Math.Pow(A / P, 2.0 / 3.0) * dAdy - 2.0 / 3.0 * Math.Pow(A / P, 5.0 / 3.0) * dPdy);
                deltay = f / df;

                while (deltay >= y)
                    deltay /= 2.0;

                while (deltay <= y - D)
                    deltay /= 2.0;

                y -= deltay;
                count++;
                if (count > Constants.MaxCount) break;
            }
            return y;
        }
        public static double GetCriticalDepth(double Q, double T, double D, bool bUSCustomary)
        {
            // E      = v^2/2g + y = Q^2/2gA^2 + y
            // dE/y   = -Q^2/gA^3 dA/y + 1 = 0
            // f(y)   = g A^3 - Q^2 dA/dy = 0
            // f(y)'  = 3g A^2 dA/dy - Q^2 d^2A/dy^2

            double g = bUSCustomary ? Constants.gUS : Constants.gSI;

            double A, dAdy, ddAdyy, f = 10.0, df;
            double deltay = 10.0;
            int count = 0;

            double y = 0.5 * D;

            while (Math.Abs(deltay) > Constants.TolD && Math.Abs(f) > Constants.TolD)
            {
                A = 2.0 * T / 3.0 / Math.Sqrt(D) * y * Math.Sqrt(y);
                dAdy = T * Math.Sqrt(y / D);
                ddAdyy = 0.5 * T / Math.Sqrt(D * y);
                f = g * A * A * A - Q * Q * dAdy;
                df = 3.0 * g * A * A * dAdy - Q * Q * ddAdyy;
                deltay = f / df;
                while (deltay >= y)
                    deltay /= 2.0;

                while (deltay <= y - D)
                    deltay /= 2.0;

                y -= deltay;
                count++;
                if (count > Constants.MaxCount) break;
            }

            return y;
        }
    }
}
