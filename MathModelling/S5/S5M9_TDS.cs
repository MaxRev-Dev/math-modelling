using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M9_TDS : BaseMethod
    {
        private (double[][] task1, double[][] task2) _effector;
        private readonly Random _rand = new Random();

        public override int Priority => 9;
        public override int Precision => 6;

        public override double ChartStepX => h;

        public override bool SwapAxis => false; 

        [ReflectedUICoefs]
        public static double
            L = 10,
            Q = 1,
            D = 1,
            gm = 1,
            x0 = 2.82,
            xi = 2.8,
            h = 1;

        public void CalculateCore()
        {
            var w = Math.Sqrt(gm / D);
            double A0 = Q / (w * D * Math.Sinh(L));
            double A1(double x) => Math.Sinh(w * (L - x));
            double A2(double x) => Math.Sinh(w * x);

            var b = (int)(L / h);
            double Cx1(double xc, double x) => A0 * A1(xc) * A2(x);
            double Cx2(double xc, double x) => A0 * A2(xc) * A1(x);

            double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));
            double[] Concentration(double r)
            {
                var ret = new List<double>();
                for (int i = 0; i <= b; i++)
                {
                    var x = i * h;
                    var cx = x < r;
                    ret.Add(cx ? Cx1(r, x) : Cx2(r, x));
                }
                return ret.ToArray();
            }

            double GetValue(double cil)
            {
                double x;
                if (xi < L / 2f)
                {
                    x = L - 1 * (Asinh(cil / (A0 * Math.Sinh(w * xi)))) / w;
                }
                else
                {
                    x = 1 * (Asinh(cil / (A0 * Math.Sinh(w * (L - xi))))) / w;
                }

                return x;
            }
            var task1 = Concentration(x0);

            var rand = Enumerable.Range(0, b)
                .Select(_ => _rand.NextDouble() * 10)
                .Concat(new[] { L / 2f }).ToArray();
             
            var ci1 = 0.416;
            var ci2 = 0.39; 
            Info.Clear();
            Info.AppendLine($"Exact: { GetValue(ci1)}");
            Info.AppendLine($"Approx: { GetValue(ci2)}");
            _effector = (new[] { task1.ToArray() },
                rand.Select(Concentration).ToArray()
                );
        }

        [ReflectedTarget]
        public double[][] GetTask1()
        {
            CalculateCore();
            return _effector.task1;
        }

        [ReflectedTarget]
        public double[][] GetTask2()
        {
            CalculateCore();
            return _effector.task2;
        }
    }
}