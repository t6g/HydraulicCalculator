using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class TrapezoidalChannel : Channel
    {
        public double SL; // left side slope, e.g., 3:1
        public double SR; // right side slope, e.g., 3:1
        public double B;  // bottom width

        public TrapezoidalChannel() : base()
        {
            B = 1.0;
            SL = 3.0;
            SR = 3.0;
        }
        public TrapezoidalChannel(IModelOptions refOptions) : base(refOptions)
        {
            B = 1.0;
            SL = 3.0;
            SR = 3.0;
            Update();
        }
        public override void Update()
        {
            area = 0.5 * (SL + SR) * Dn * Dn + B * Dn;
            perimeter = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR)) * Dn + B;

            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = GetCriticalDepth(area * velocity, B, SL, SR, options.bUSCustomary);
            double ac = 0.5 * (SL + SR) * dc * dc + B * dc;
            double pc = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR)) * dc + B;
            vc = velocity * area / ac;
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);
            twn = (SL + SR) * Dn + B;
        }

        public static double GetNormalDepth(double Q, double b, double z1, double z2, double S, double N, bool bUSCustomary)
        {
            // v     = Ku/n A^2/3 P^-2/3 S^1/2
            // Q     = Ku/n S^1/2 A^5/3 P^-2/3 = Ku/n S^1/2 [by + 0.5 (z1 + z2)y^2]^5/3 (b + (sqrt(1 + zl^2) + sqrt(1 + z2^2))y)^-2/3  
            // f     = Ku/n S^1/2 A^5/3 P^-2/3 - Q
            // df    = Ku/n S^1/2 (5/3 A^2/3 P^-2/3 dA/dy - 2/3 A^5/3 P^-5/3 dP/dy)
            if (Q <= 0) return 0;

            //double Ku = 1.487;
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double A, P, dAdy, dPdy, f = 10.0, df;
            double deltay = 10.0;
            double yi = 0.5;
            dPdy = Math.Sqrt(1.0 + z1 * z1) + Math.Sqrt(1.0 + z2 * z2);
            int count = 0;

            while (Math.Abs(deltay) > Constants.TolD && Math.Abs(f) > Constants.TolD)
            {
                A = b * yi + 1.0 / 2.0 * yi * yi * (z1 + z2);
                dAdy = b + (z1 + z2) * yi;
                P = b + (Math.Sqrt(1.0 + z1 * z1) + Math.Sqrt(1.0 + z2 * z2)) * yi;
                f = Ku / N * Math.Sqrt(S) * Math.Pow(A, 5.0 / 3.0) * Math.Pow(P, -2.0 / 3.0) - Q;
                df = Ku / N * Math.Sqrt(S) * (5.0 / 3.0 * Math.Pow(A / P, 2.0 / 3.0) * dAdy - 2.0 / 3.0 * Math.Pow(A / P, 5.0 / 3.0) * dPdy);
                deltay = f / df;
                yi -= deltay;
                count++;
                if (count > Constants.MaxCount) break;
            }

            if (double.IsNaN(yi))
            {
                //MessageBox.Show("Normal depth is NaN!");
            }
            return yi;
        }
        public static double GetCriticalDepth(double Q, double b, double z1, double z2, bool bUSCustomary)
        {
            // E         = v^2/2g + y = Q^2/2gA^2 + y
            // dE/dy     = -Q^2/gA^3 dA/dy + 1 = 0
            // f(y)      = gA^3 - Q^2 dA/dy 
            // df        = 3gA^2 dA/dy - Q^2 d^2A/dy^2
            // A         = by + 0.5(z1 + z2)y^2
            // dA/dy     = b + (z1 + z2)y
            // d^2A/dy^2 = z1 + z2
            // y         = y - f/f'
            double g = bUSCustomary ? Constants.gUS : Constants.gSI;

            double A, dAdy, d2Ady2, f = 10.0, df;
            double deltay = 10.0;
            int count = 0;
            d2Ady2 = (z1 + z2);

            double yi;
            //use Equations in Table 2.1 French 1985 to estimate yi
            if (Q / Math.Pow(b, 2.5) < 0.1)
                yi = Math.Pow(Q * Q / g / b / b, 1.0 / 3.0);
            else
                yi = 0.81 * Math.Pow((Q * Q / g * Math.Pow(0.5 * (z1 + z2), -0.75) * Math.Pow(b, -1.25)), 0.27) - b / 15.0 / (z1 + z2);

            while (Math.Abs(deltay) > Constants.TolD && Math.Abs(f) > Constants.TolD)
            {
                A = b * yi + 1.0 / 2.0 * yi * yi * (z1 + z2);
                dAdy = b + (z1 + z2) * yi;
                f = g * A * A * A - Q * Q * dAdy;
                df = 3.0 * g * A * A * dAdy - Q * Q * d2Ady2;
                deltay = f / df;
                yi -= deltay;
                count++;
                if (count > Constants.MaxCount) break;
            }
            return yi;
        }

        public static void CalculateCurves(double B, double SL, double SR, double N, double S, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;

            double dP = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR));
            for (int i = 0; i < ys.Length; i++)
            {
                As[i] = 0.5 * (SL + SR) * ys[i] * ys[i] + B * ys[i];
                Ps[i] = dP * ys[i] + B;
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }
    }
}
