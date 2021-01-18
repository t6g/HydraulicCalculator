using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class TriangularChannel : Channel
    {
        public double SL; // left side slope, e.g., 3:1
        public double SR; // right side slope, e.g., 3:1

        public TriangularChannel()
        {
            SL = 3.0;
            SR = 3.0;
        }
        public TriangularChannel(IModelOptions refOptions) : base(refOptions)
        {
            SL = 3.0;
            SR = 3.0;
            Update();
        }

        public override void Update()
        {
            area = 0.5 * (SL + SR) * Dn * Dn;
            perimeter = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR)) * Dn;

            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = Math.Pow(8.0 * velocity * velocity * area * area / g / (SL + SR) / (SL + SR), 1.0 / 5.0);
            double ac = 0.5 * (SL + SR) * dc * dc;
            double pc = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR)) * dc;
            //vc = velocity * area / ac;
            vc = Math.Sqrt(0.5 * g * dc);
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);
            twn = (SL + SR) * Dn;
        }

        public static double GetNormalDepth(double Q, double z1, double z2, double S, double N, bool bUSCustomary)
        {
            //Q     = Ku/n S^1/2 A^5/3 P^-2/3 = Ku/n S^1/2 [0.5 (z1 + z2)]^5/3 (sqrt(1 + zl^2) + sqrt(1 + z2^2))^-2/3 y^8/3  
            //y^8/3 = Qn/Ku S^-1/2 [0.5 (z1 + z2)]^-5/3 (sqrt(1 + zl^2) + sqrt(1 + z2^2))^2/3   normal depth
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double Dn = Math.Pow(Q * N / Ku / Math.Sqrt(S) * Math.Pow(0.5 * (z1 + z2), -5.0 / 3.0) *
                   Math.Pow((Math.Sqrt(1.0 + z1 * z1) + Math.Sqrt(1.0 + z2 * z2)), 2.0 / 3.0), 3.0 / 8.0);
            return Dn;
        }

        public static double GetCriticalDepth(double Q, double z1, double z2, bool bUSCustomary)
        {
            double g = bUSCustomary ? Constants.gUS : Constants.gSI;
            return Math.Pow(8.0 * Q * Q / g / (z1 + z2) / (z1 + z2), 1.0 / 5.0);
        }

        public static void CalculateCurves(double SL, double SR, double S, double N, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;

            double dP = (Math.Sqrt(1.0 + SL * SL) + Math.Sqrt(1.0 + SR * SR));
            for (int i = 0; i < ys.Length; i++)
            {
                As[i] = 0.5 * (SL + SR) * ys[i] * ys[i];
                Ps[i] = dP * ys[i];
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }
    }
}
