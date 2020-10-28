using System;
using System.Linq;
using MM.Abstractions;

namespace MM.S4
{
    internal class MM3_Diff : BaseMethod
    {
        [ReflectedUICoefs]
        public static int
            b = 5,
            a = 3,
            m = a,
            i;

        [ReflectedUICoefs]
        public double
            n = 0.68,
            teta0 = 0.02,
            l = 0.5,
            h = 0.1,
            sigma = 1.0 / 6;

        private double DiffusionCoef(int _m, double teta)
        {
            return Math.Pow(10, -6) * (1 - Math.Exp(-_m * teta));
        }

        public override double[][] Calculate()
        {
            var diff = DiffusionCoef(6, teta0);

            var apw = diff; //lamda * 1.0 / (c * ro);
            var tau = Math.Pow(h, 2) * sigma / apw;
            var u = new double[a + 1]
                .Select(x => new double[b + 1].ToArray()).ToArray();

            u[0][0] = u[0][b] = n;

            for (i = 1; i < (int) (l / h); i++)
                u[0][i] = teta0; //(txb - tx0) * gx(h * i) / gx(l) + tx0;

            double[]
                alfa = new double[b],
                beta = new double[b],
                _a = new double[b],
                _b = new double[b],
                _c = new double[b];

            for (var j = 0; j < b; j++)
            {
                _a[j] = _b[j] = apw * tau * 1.0 / Math.Pow(h, 2);
                _c[j] = 1 + sigma * 2;
            }

            for (var it = 1; it <= m; it++)
            {
                u[it][0] = n;
                u[it][b] = n;
                beta[0] = n;

                for (i = 1; i <= b - 1; i++)
                {
                    alfa[i] = _b[i] * 1.0 / (_c[i] - alfa[i - 1] * _a[i]);
                    beta[i] = (_a[i - 1] * beta[i - 1] + u[it - 1][i]) * 1.0 /
                              (_c[i] - alfa[i - 1] * _a[i]);
                }

                for (var j = b - 1; j > 0; j--)
                    u[it][j] = alfa[j] * u[it][j + 1] + beta[j];
            }

            return u.Reverse().ToArray();
        }
    }
}