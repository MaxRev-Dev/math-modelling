using MM.Abstractions;
using System;
using System.Collections.Generic;

namespace MM.S5
{
    internal class S5M7_TDS : BaseMethod
    {
        public override int Priority => 7;

        public override double ChartStepX => 1;

        public override string[] YLegend
            => new[]
            {
                "Dry",
                "Dewy",
                "Filt"
            };
        public override double? MaxX => L;

        [ReflectedUICoefs(P = 6)]
        public static double
            E = 2.5 * Math.Pow(10, 6);

        [ReflectedUICoefs]
        public static int
            L = 10;

        [ReflectedUICoefs]
        public static double
            H1 = 1,
            H2 = 5,
            g = 9.8,
            v = 0.35,
            ps = 2200,
            pp = 1000;

        private (double[][] tension, double[][] deform) _effector;

        [ReflectedTarget]
        public double[][] GetTDS()
        {
            var lam = (E * v) / ((1 + v) * (1 - 2 * v));
            var u = E / (2 * (1 + v));
            var den = lam + 2 * u;

            double U(double a, double x) => a * x * (x - 1) / 2;
            double Up(double a, double x) => a * x - a / 2;
            double Sp(double a, double x) => (lam + 2 * u) * Up(a, x);

            var a1 = (ps * g) / den;
            var a2 = (-pp + ps) * g / den;
            var a3 = g * (ps + ((H2 - H1) / L - 1) * pp) / den;

            var b = L;
            double[] dry = new double[b + 1],
                dewy = new double[b + 1],
                filt = new double[b + 1];
            var tensions = new List<double[]>();
            var deforms = new List<double[]>();

            foreach (var a in new[] { a1, a2, a3 })
            {
                double[] tension = new double[b + 1],
                    deform = new double[b + 1];
                for (int i = 0; i <= b; i++)
                {
                    var x = i * .1;
                    tension[i] = Up(a, x);
                    deform[i] = Sp(a, x);
                }
                tensions.Add(tension);
                deforms.Add(deform);
            }
            for (int i = 1; i < b; i++)
            {
                var x = i * .1;
                dry[i] = U(a1, x);
                dewy[i] = U(a2, x);
                filt[i] = U(a3, x);
            }

            _effector = (tensions.ToArray(), deforms.ToArray());
            return new List<double[]>(new[] { dry, dewy, filt }).ToArray();
        }

         
        [ReflectedTarget]
        public double[][] GetTension()
        {
            GetTDS();
            return _effector.tension;
        }

        [ReflectedTarget]
        public double[][] GetDeform()
        {
            GetTDS();
            return _effector.deform;
        }
    }
}