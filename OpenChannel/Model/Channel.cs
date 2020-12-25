using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenChannel.Model
{
    public abstract class Channel
    {
        public double S;    // Slope
        public double N;    // Manning's N
        public double Dn;   // normal depth

        protected double area;
        protected double perimeter;
        protected double velocity;
        protected double dc;
        protected double vc;
        protected double sc;
        protected double twn; //top width normal

        public double Area => area;
        public double Perimeter => perimeter;
        public double Velocity => velocity;
        public double Dc => dc;
        public double Vc => vc;
        public double Sc => sc;
        public double Twn => twn;

        protected double Ku => options.bUSCustomary ? Constants.KuUS : Constants.KuSI;
        protected double g => options.bUSCustomary ? Constants.gUS : Constants.gSI;

        [XmlIgnore]
        public IModelOptions options;

        public Channel()
        {
            N = 0.05;
            S = 0.01;
            Dn = 0.5;
        }
        public Channel(IModelOptions refOptions)
        {
            options = refOptions;
            N = 0.05;
            S = 0.01;
            Dn = 0.5;
        }
        //Update calculations
        public abstract void Update();
    }
}
