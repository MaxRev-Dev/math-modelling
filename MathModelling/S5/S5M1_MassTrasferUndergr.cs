using System;
using System.Linq;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M1_MassTrasferUndergr : BaseMethod
    {
        public override int Priority => 1;

        [ReflectedUICoefs] public static double
            N = 7,
            k = 1.25,
            D = 0.01 * N,
            H1 = 1.5 + 0.1 * Math.Sin(N),
            H2 = 0.5 + 0.1 * Math.Cos(N),
            l = 50,
            tau = 30,
            sigma = 0.2,
            h = 2.5;

        [ReflectedUICoefs(P = 6)]
        public double
            gamma = 0.00065;
        [ReflectedUICoefs]
        public static int
            times = 4;

        public override double ChartStepX => h;
        public override double? ChartStepY => tau;
        public override double? MaxX => l;

        public override double[][] Calculate()
        {
            var V = (H1 - H2) * k / l; // filtering speed  

            Func<double, double> cx0 = x => (N - 6) + Math.Pow(Math.Sin(N * x * h), 2);
            Func<double, double> c0t = t => 0;
            Func<double, double> clt = i => (N - 6) + Math.Pow(Math.Sin(N * l), 2) + 0.2;

            var Cx = 0.1 * k;
            var h_2 = Math.Pow(h, 2);
            var r = -V / D;
            var M = 1f / (1 + (h * V) / (2 * D));
            var a_const = M / h_2 - r / h;
            var b_const = M / h_2;
            var c_const = 2 * M / h_2 - r / h + gamma / D + sigma / (D * tau);
            var fk = sigma / (D * tau);
            var b = (int)(l / h); // interm points
            var fp = gamma * Cx / D;
            double[]
                alfa = new double[b + 1],
                beta = new double[b + 1];
            var u = new double[times + 1][].Select(x => new double[b + 1]).ToArray();

            for (var i = 1; i < b; i++)
            {
                u[0][i] = cx0(i);
            }

            for (int tl = 1; tl <= times; tl++) // time layers
            {
                u[tl][0] = c0t(tl);
                u[tl][b] = clt(tl);
                beta[0] = c0t(tl);

                for (var i = 1; i <= b; i++)
                {
                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var f = fk + fp;
                    beta[i] = (a_const * beta[i - 1] + f * u[tl - 1][i]) /
                              (c_const - alfa[i - 1] * a_const);
                }
                for (int j = b - 1; j > 0; j--)
                {
                    u[tl][j] = alfa[j] * u[tl][j + 1] + beta[j];
                }
            }

            return u.ToArray();
        }
    }
}