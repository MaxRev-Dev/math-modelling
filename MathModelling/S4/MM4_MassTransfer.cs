using System;
using System.Linq;
using MM.Abstractions;

namespace MM.S4
{
    internal class MM4_MassTransfer : BaseMethod
    {
        [ReflectedUICoefs]
        public double
            k_ = 1.65,
            D = 0.08,
            Cm_ = 10;

        [ReflectedUICoefs]
        public int
            t_ = 5,
            n_ = 10,
            l_ = 50;

        public override double[][] Calculate()
        {
            return MassTransfer(13.0 / 23, D,
                FilteringSpeed(k_, l_, n_, t_),
                19.5 * Math.Pow(10, -5), Cm_, l_, n_, t_);
        }

        private double[][] MassTransfer(double NN, double D, double[][] U,
            double y,
            double Cm, double l, int n, int t)
        {
            var h = l * 1.0 / n;
            var T = 1;
            var inline = new double[n + 1];
            var tIrs = new double[t + 1][];

            inline[0] = n;
            inline[n] = Cm;
            for (var k = n - 1; k > 0; k--)
                inline[k] = Cm * Math.Exp(-5 * n * h);
            tIrs[0] = inline;

            for (var i = 1; i < t + 1; i++)
            {
                double[]
                    a = new double[n + 1],
                    b = new double[n + 1],
                    c = new double[n + 1],
                    f = new double[n + 1];
                inline = new double[n + 1];

                for (var j = 0; j < n + 1; j++)
                {
                    var N = D / (1 + h * Math.Abs(U[i][j]) / (2 * D));
                    var R = (-U[i][j] + Math.Abs(U[i][j])) / 2;
                    var r = (-U[i][j] - Math.Abs(U[i][j])) / 2;
                    a[j] = T * (N / Math.Pow(h, 2) - r / h) / NN;
                    b[j] = T * (N / Math.Pow(h, 2) - R / h) / NN;
                    c[j] = 1.0 + T * (a[j] + b[j] + y) / NN;
                    f[j] = T * y * Cm / NN;
                }

                var L = new double[n];
                var B = new double[n];
                B[0] = Cm;

                for (var m = 1; m < n; m++)
                {
                    L[m] = b[m] / c[m] - a[m] * L[m - 1];
                    B[m] = (a[m] * B[m - 1] + tIrs[i - 1][m] + f[m]) /
                           (c[m] - a[m] * L[m - 1]);
                }

                inline[n] = (-U[0][n] * 10 * h / D + B[n - 1]) /
                            (1 - U[0][n] * h / D - L[n - 1]);
                for (var k = n - 1; k > 0; k--)
                    inline[k] = L[k] * inline[k + 1] + B[k];

                inline[0] = Cm;

                tIrs[i] = inline;
            }

            return tIrs.ToArray();
        }

        private double[][] FilteringSpeed(double k, int l, int n, int t)
        {
            var Q = new double[t + 1].Select(x => new double[l + 1]).ToArray();
            var h = l / n;
            for (var i = 0; i < t + 1; i++)
            for (var j = 0; j < l + 1; j++)
                Q[i][j] = k * (l - 2 * j * h);

            return Q;
        }
    }
}