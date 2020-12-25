using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class RectangularChannel : Channel
    {
        public double B;  //bottom width
        public RectangularChannel() : base()
        {
            B = 1.0;
        }
        public RectangularChannel(IModelOptions refOptions) : base(refOptions)
        {
            B = 1.0;
            Update();
        }
        public override void Update()
        {
            area = B * Dn;
            perimeter = B + 2.0 * Dn;

            velocity = Ku / N * Math.Pow(Area / Perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = Math.Pow(velocity * velocity * area * area / g / B / B, 1.0 / 3.0);
            vc = velocity * Area / (B * dc);
            sc = Math.Pow(vc / (Ku / N * Math.Pow((B * dc) / (2.0 * dc + B), 2.0 / 3.0)), 2.0);
            twn = B;
        }

        public static double GetCriticalDepth(double Q, double B, bool bUSCustomary)
        {
            double g = bUSCustomary ? Constants.gUS : Constants.gSI;
            return Math.Pow(Q * Q / g / B / B, 1.0 / 3.0); ;
        }
        public static void CalculateCurves(double B, double S, double N, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            for (int i = 0; i < ys.Length; i++)
            {
                As[i] = B * ys[i];
                Ps[i] = B + 2.0 * ys[i];
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }
    }
}
