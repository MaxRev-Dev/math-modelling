using System;
using System.Collections.Generic;
using System.Linq;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M8_TDS : BaseMethod
    {
        public override int Priority => 8;
        public override int Precision => 6;

        public override double ChartStepX => .1;
         
        public override bool SwapAxis => true;

        public override string[] YLegend
            => new[]
            {
                "Soil",
                "Half-point"
            }; 

        [ReflectedUICoefs(P = 6)]
        public static double
            lam1 = 2.16 * Math.Pow(10, 6),
            lam2 = 2.95 * Math.Pow(10, 6),
            u1 = 9.26 * Math.Pow(10, 5),
            u2 = 1.11 * Math.Pow(10, 6);

        [ReflectedUICoefs]
        public static double
            l1 = 0.5,
            L = 1,
            h = 0.1; 

        private (double[][] tension, double[][] deform) _effector;

        [ReflectedTarget]
        public double[][] GetTransfer()
        {
            var tensions = new List<double[]>();
            var deforms = new List<double[]>();

            var lamn = 17;//lam1 * g;
            var lamw = 10.5;//lam2 - (1 - v1) * pp;

            var den1 = lam1 + 2 * u1;
            var den2 = lam2 + 2 * u2;

            var a1 = lamw / den1;
            var a2 = lamn / den2;
            var l1p2 = Math.Pow(l1, 2);
            var lp2 = Math.Pow(L, 2);

            var c3 = (a2 * lp2 / 2f - (a1 + a2) * l1p2 / 2f
                      + den2 * a2 * l1p2 / den1) /
                     ((1 - den2 / den1) * l1 - L);
            var c1 = den2 * (a2 * l1 + c3) / den1 - a1 * l1;
            var c4 = -c3 * L - a2 * lp2 / 2f;

            double U1(double a, double x) => a * Math.Pow(x, 2) / 2f + c1 * x;
            double U2(double a, double x) => a * Math.Pow(x, 2) / 2f + c3 * x + c4;

            double Deform1(double a, double x) => a * x + c1;
            double Deform2(double a, double x) => a * x + c3;

            double Tension1(double a, double x) => den1 * Deform1(a, x);
            double Tension2(double a, double x) => den2 * Deform2(a, x);

            var b = (int)(L / h);
            double[] soilx = new double[b + 1],
                tension = new double[b + 1],
                deform = new double[b + 1];
            for (int i = 0; i <= b; i++)
            {
                var x = i * h;
                var cx = x < l1;
                var a = cx ? a1 : a2;
                soilx[i] = cx ? U1(a, x) : U2(a, x);
                tension[i] = cx ? Tension1(a, x) : Tension2(a, x);
                deform[i] = cx ? Deform1(a, x) : Deform2(a, x);
            }

            tensions.Add(tension);
            deforms.Add(deform); 

            _effector = (tensions.ToArray(), deforms.ToArray());
            return new[] { soilx }.ToArray();
        }

        [ReflectedTarget]
        public double[][] GetTension()
        {
            GetTransfer();
            return _effector.tension;
        }

        [ReflectedTarget]
        public double[][] GetDeform()
        {
            GetTransfer();
            return _effector.deform;
        }
    }
}