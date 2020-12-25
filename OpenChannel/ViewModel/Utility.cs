using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenChannel.ViewModel
{
    public static class NiceScaleStatic
    {

        public static void Calculate(double min, double max, int maxTicks, out double range, out double tickSpacing, out double niceMin, out double niceMax)
        {
            range = niceNum(max - min, false);
            tickSpacing = niceNum(range / (maxTicks - 1), true);
            niceMin = Math.Floor(min / tickSpacing) * tickSpacing;
            niceMax = Math.Ceiling(max / tickSpacing) * tickSpacing;
        }

        public static double niceNum(double range, bool round)
        {
            double pow = Math.Pow(10, Math.Floor(Math.Log10(range)));
            double fraction = range / pow;

            double niceFraction;
            if (round)
            {
                if (fraction < 1.5)
                {
                    niceFraction = 1;
                }
                else if (fraction < 3)
                {
                    niceFraction = 2;
                }
                else if (fraction < 7)
                {
                    niceFraction = 5;
                }
                else
                {
                    niceFraction = 10;
                }
            }
            else
            {
                if (fraction <= 1)
                {
                    niceFraction = 1;
                }
                else if (fraction <= 2)
                {
                    niceFraction = 2;
                }
                else if (fraction <= 5)
                {
                    niceFraction = 5;
                }
                else
                {
                    niceFraction = 10;
                }
            }

            return niceFraction * pow;
        }
    }
    public class NiceScale
    {

        public double NiceMin { get; set; }
        public double NiceMax { get; set; }
        public double TickSpacing { get; private set; }

        private double _minPoint;
        private double _maxPoint;
        private double _maxTicks = 5;
        private double _range;

        /**
            * Instantiates a new instance of the NiceScale class.
            *
            * @param min the minimum data point on the axis
            * @param max the maximum data point on the axis
            */
        public NiceScale(double min, double max)
        {
            _minPoint = min;
            _maxPoint = max;
            Calculate();
        }

        /**
            * Calculate and update values for tick spacing and nice
            * minimum and maximum data points on the axis.
            */
        private void Calculate()
        {
            _range = NiceNum(_maxPoint - _minPoint, false);
            TickSpacing = NiceNum(_range / (_maxTicks - 1), true);
            NiceMin = Math.Floor(_minPoint / TickSpacing) * TickSpacing;
            NiceMax = Math.Ceiling(_maxPoint / TickSpacing) * TickSpacing;
        }

        /**
            * Returns a "nice" number approximately equal to range Rounds
            * the number if round = true Takes the ceiling if round = false.
            *
            * @param range the data range
            * @param round whether to round the result
            * @return a "nice" number to be used for the data range
            */
        private double NiceNum(double range, bool round)
        {
            double exponent; /** exponent of range */
            double fraction; /** fractional part of range */
            double niceFraction; /** nice, rounded fraction */

            exponent = Math.Floor(Math.Log10(range));
            fraction = range / Math.Pow(10, exponent);

            if (round)
            {
                if (fraction < 1.5)
                    niceFraction = 1;
                else if (fraction < 3)
                    niceFraction = 2;
                else if (fraction < 7)
                    niceFraction = 5;
                else
                    niceFraction = 10;
            }
            else
            {
                if (fraction <= 1)
                    niceFraction = 1;
                else if (fraction <= 2)
                    niceFraction = 2;
                else if (fraction <= 5)
                    niceFraction = 5;
                else
                    niceFraction = 10;
            }

            return niceFraction * Math.Pow(10, exponent);
        }

        /**
            * Sets the minimum and maximum data points for the axis.
            *
            * @param minPoint the minimum data point on the axis
            * @param maxPoint the maximum data point on the axis
            */
        public void SetMinMaxPoints(double minPoint, double maxPoint)
        {
            _minPoint = minPoint;
            _maxPoint = maxPoint;
            Calculate();
        }

        /**
            * Sets maximum number of tick marks we're comfortable with
            *
            * @param maxTicks the maximum number of tick marks for the axis
            */
        public void SetMaxTicks(double maxTicks)
        {
            _maxTicks = maxTicks;
            Calculate();
        }
    }

    public class StringPathINPC : INotifyPropertyChanged
    {
        private string path;
        public StringPathINPC(string p)
        {
            path = p;
        }
        public string Path
        {
            get => path;
            set
            {
                if (path == value) return;
                path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
    public class XYINPC : INotifyPropertyChanged
    {
        private double x;
        private double y;
        public double X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChanged(nameof(X));
            }
        }
        public double Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChanged(nameof(Y));
            }
        }
        public XYINPC(double xa, double ya)
        {
            x = xa; y = ya;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }

    public class XYZINPC : XYINPC
    {
        private double z;
        public double Z
        {
            get => z;
            set
            {
                if (Math.Abs(z - value) < 1e-5)
                    z = value;
                RaisePropertyChanged(nameof(Z));
            }
        }
        public XYZINPC(double x, double y, double z) : base(x, y)
        {
            this.z = z;
        }

    }
    public class EllipseINPC : INotifyPropertyChanged
    {
        private double width;
        private double height;
        private double left;
        private double top;

        public EllipseINPC(double w, double h, double l, double t)
        {
            width = w;
            height = h;
            left = l;
            top = t;
        }

        public double Width
        {
            get => width;
            set
            {
                width = value;
                RaisePropertyChanged(nameof(Width));
            }
        }

        public double Height
        {
            get => height;
            set
            {
                height = value;
                RaisePropertyChanged(nameof(Height));
            }
        }

        public double Left
        {
            get => left;
            set
            {
                left = value;
                RaisePropertyChanged(nameof(Left));
            }
        }

        public double Top
        {
            get => top;
            set
            {
                top = value;
                RaisePropertyChanged(nameof(Top));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
    public class TickLabel
    {
        public double Left;
        public double Top;
        public string Text;

        public TickLabel(double aleft, double atop, string atext)
        {
            Left = aleft;
            Top = atop;
            Text = atext;
        }
    }

    public class TickLabelINPC : INotifyPropertyChanged
    {
        private double left;
        private double top;
        private string text;

        public TickLabelINPC(double aleft, double atop, string atext)
        {
            left = aleft;
            top = atop;
            text = atext;
        }

        public double Left
        {
            get => left;
            set
            {
                if (Math.Abs(left - value) < 1e-3) return;
                left = value;
                RaisePropertyChanged(nameof(Left));
            }
        }
        public double Top
        {
            get => top;
            set
            {
                if (Math.Abs(top - value) < 1e-3) return;
                top = value;
                RaisePropertyChanged(nameof(Top));
            }
        }
        public string Text
        {
            get => text;
            set
            {
                if (text != value) return;
                text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }

    public class LineData
    {
        public double X1, X2, Y1, Y2;

        public LineData(double ax1, double ay1, double ax2, double ay2)
        {
            X1 = ax1; Y1 = ay1; X2 = ax2; Y2 = ay2;
        }
    }


    public class LineINPC : INotifyPropertyChanged
    {
        private readonly double tol = 1.0e-6;
        private double x1, x2, y1, y2;

        public LineINPC(double ax1, double ay1, double ax2, double ay2)
        {
            x1 = ax1; y1 = ay1; x2 = ax2; y2 = ay2;
        }
        public double X1
        {
            get => x1;
            set
            {
                if (Math.Abs(x1 - value) < tol) return;
                x1 = value;
                RaisePropertyChanged(nameof(X1));
            }
        }

        public double X2
        {
            get => x2;
            set
            {
                if (Math.Abs(x2 - value) < tol) return;
                x2 = value;
                RaisePropertyChanged(nameof(X2));
            }
        }

        public double Y1
        {
            get => y1;
            set
            {
                if (Math.Abs(y1 - value) < tol) return;
                y1 = value;
                RaisePropertyChanged(nameof(Y1));
            }
        }

        public double Y2
        {
            get => y2;
            set
            {
                if (Math.Abs(y2 - value) < tol) return;
                y2 = value;
                RaisePropertyChanged(nameof(Y2));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] String PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
    public class DoubleConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                return string.Format(formatString, value);
            }
            // If the format string is null or empty, simply call ToString()
            // on the value.
            return value.ToString();
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            string strValue = value as string;
            if (double.TryParse(strValue, out double resultDouble))
            {
                return resultDouble;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
