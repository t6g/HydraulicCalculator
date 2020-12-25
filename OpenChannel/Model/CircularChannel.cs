using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class CircularChannel : Channel
    {
        public double r;
        public double Qmax => qmax;
        public double Yqmax => yqmax;
        private double qmax;
        private double yqmax;
        public CircularChannel() : base()
        {
            r = 1.0;
        }
        public CircularChannel(IModelOptions refOptions) : base(refOptions)
        {
            r = 1.0;
            Update();
        }

        public override void Update()
        {
            double thetan = 2.0 * Math.Acos(1.0 - Dn / r);

            // A = (theta - sin(theta))r^2 / 2
            area = (thetan - Math.Sin(thetan)) * r * r / 2.0;
            // P = theta r
            perimeter = thetan * r;

            velocity = Ku / N * Math.Pow(area / perimeter, 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            dc = GetCriticalDepth(area * velocity, r, options.bUSCustomary);
            double thetac = 2.0 * Math.Acos(1.0 - dc / r);
            double ac = (thetac - Math.Sin(thetac)) * r * r / 2.0;
            double pc = thetac * r;
            vc = velocity * area / ac;
            sc = Math.Pow(vc / (Ku / N * Math.Pow(ac / pc, 2.0 / 3.0)), 2.0);

            twn = 2.0 * r * Math.Sin(thetan / 2.0);

            yqmax = r * 1.87636243;
            qmax = GetMaxDischarge(r, S, N, options.bUSCustomary);
        }
        public static void CalculateCurves(double r, double S, double N, bool bUSCustomary,
                                           double[] ys, double[] As, double[] Ps, double[] vs)
        {
            if (ys == null || As == null || Ps == null || vs == null) return;
            if (!(ys.Length == As.Length && ys.Length == Ps.Length && ys.Length == vs.Length)) return;

            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double theta;

            for (int i = 0; i < ys.Length; i++)
            {
                theta = 2.0 * Math.Acos(1.0 - ys[i] / r);
                As[i] = (theta - Math.Sin(theta)) * r * r / 2.0;
                Ps[i] = theta * r;
                vs[i] = Ku / N * Math.Pow(As[i] / Ps[i], 2.0 / 3.0) * Math.Pow(S, 1.0 / 2.0);
            }
        }

        public static double GetMaxDischarge(double r, double S, double N, bool bUSCustomary)
        {
            double theta = Constants.ThetaMaxCircle;
            // A = (theta - sin(theta))r^2 / 2
            double A = (theta - Math.Sin(theta)) * r * r / 2.0;
            // P = theta r
            double P = theta * r;
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double v = Ku / N * Math.Pow(A / P, Constants.X) * Math.Pow(S, Constants.Y);
            return v * A;
        }


        public static double GetNormalDepth(double Q, double r, double S, double N, bool bUSCustomary)
        {
            double Qmax = GetMaxDischarge(r, S, N, bUSCustomary);
            if (Q >= Qmax)
                return 2.0 * r; //r * (1.0 - Math.Cos(Constants.ThetaMaxCircle / 2.0));
            // y = r (1 - cos(theta/2))

            // Q     = Ku/n S^1/2 A^5/3 P^-2/3 
            // f     = Ku/n S^1/2 A^5/3 P^-2/3 - Q
            // df    = Ku/n S^1/2 (5/3 A^2/3 P^-2/3 dA/dtheta - 2/3 A^5/3 P^-5/3 dP/dtheta)
            double Ku = bUSCustomary ? Constants.KuUS : Constants.KuSI;
            double A, P, dAdtheta, dPdtheta = r, f = 10.0, df;
            double deltatheta = 10.0;
            double thetai = Math.PI;
            int count = 0;

            while (Math.Abs(deltatheta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                // A = (theta - sin(theta))r^2 / 2
                A = (thetai - Math.Sin(thetai)) * r * r / 2.0;
                // A' = (1 - cos(theta))r^2 / 2
                dAdtheta = (1.0 - Math.Cos(thetai)) * r * r / 2.0; ;
                // P = theta r
                P = thetai * r;
                f = Ku / N * Math.Sqrt(S) * Math.Pow(A, 5.0 / 3.0) * Math.Pow(P, -2.0 / 3.0) - Q;
                df = Ku / N * Math.Sqrt(S) * (5.0 / 3.0 * Math.Pow(A / P, 2.0 / 3.0) * dAdtheta - 2.0 / 3.0 * Math.Pow(A / P, 5.0 / 3.0) * dPdtheta);
                deltatheta = f / df;
                thetai -= deltatheta;
                count++;
                if (count > Constants.MaxCount) break;
            }
            // y = r (1 - cos(theta/2))
            double y = r * (1.0 - Math.Cos(thetai / 2.0));
            return r * (1.0 - Math.Cos(thetai / 2.0));
        }
        public static double GetCriticalDepth(double Q, double r, bool bUSCustomary)
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

            double g = bUSCustomary ? Constants.gUS : Constants.gSI;

            double A, dAdy, dAdtheta, ddAdydtheta, f = 10.0, df;
            double deltatheta = 10.0;
            int count = 0;
            //use French 1985 Table 2.1 Equation to estimate yc
            double yi = Math.Min(1.01 * Math.Pow(2.0 * r, -0.26) * Math.Pow(Q * Q / g, 0.25), 0.85 * 2 * r);

            double thetai = 2.0 * Math.Acos(1.0 - yi / r);

            while (Math.Abs(deltatheta) > Constants.TolAngle && Math.Abs(f) > Constants.TolD)
            {
                A = (thetai - Math.Sin(thetai)) * r * r / 2.0;
                dAdy = 2.0 * r * Math.Sin(thetai / 2.0);
                f = g * A * A * A - Q * Q * dAdy;
                dAdtheta = 0.5 * (1.0 - Math.Cos(thetai)) * r * r;
                ddAdydtheta = r * Math.Cos(thetai / 2.0);
                df = 3.0 * g * A * A * dAdtheta - Q * Q * ddAdydtheta;
                deltatheta = f / df;
                thetai -= deltatheta;
                count++;
                if (count > Constants.MaxCount) break;
            }

            return r * (1.0 - Math.Cos(thetai / 2.0));
        }
    }
}
