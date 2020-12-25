using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public static class Constants
    {
        public const double X = 2.0 / 3.0;
        public const double Y = 1.0 / 2.0;
        public const double KuUS = 1.49;
        public const double KuSI = 1.0;
        public const double KuCulvertUS = 1.0;
        public const double KuCulvertSI = 1.811;
        public const double KuHUS = 29.0;
        public const double KuHSI = 19.63;
        public const double gUS = 32.2;
        public const double gSI = 9.81;
        public const double TolD = 0.0001;
        public const double TolQ = 0.0001;
        public const double TolAngle = 0.000001;
        public const double ThetaMaxCircle = 5.27810713; //theta to reach maximum discharge using Manning's equation Q = f(A^5/3 P^-2/3) 
        public const int MaxCount = 100;
        public const double XExtension = 0.1;  //draw channel profile
        public const double CWeir = 0.61; //weir coefficient
    }
}
