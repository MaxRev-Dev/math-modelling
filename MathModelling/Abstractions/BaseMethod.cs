using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace MM.Abstractions
{
    internal abstract class BaseMethod
    {
        private readonly CultureInfo ci;

        protected BaseMethod()
        {
            ci = CultureInfo.GetCultureInfo("en-US");
        }

        public virtual int Priority { get; } = 0;
        public virtual int Precision { get; } = 4;
        public virtual string[] YLegend { get; }
        public virtual double ChartStepX { get; } = 1;
        public virtual double? ChartStepY { get; } = 1;
        public virtual double? StepTime { get; } = 1;
        public virtual double? MaxX { get; } = default;
        public virtual string SwitchItem { get; set; }
        public virtual string[] SwitchData { get; set; }
        public virtual bool Is3D => false;

        public virtual SeriesChartType? SeriesType { get; } = default;

        public virtual double[][] Calculate()
        {
            return new double[0][];
        }

        public virtual double[][][] Calculate3D()
        {
            return new double[0][][];
        }

        public string AsString(double[][] result)
        {
            var s = new StringBuilder();
            foreach (var t in result.Reverse())
            {
                foreach (var v in t) s.Append(v.ToString($"f{Precision}", ci).PadLeft(11));

                s.AppendLine();
            }

            return s.ToString();
        }
    }
}