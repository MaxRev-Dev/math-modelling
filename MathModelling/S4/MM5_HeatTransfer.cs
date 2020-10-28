using System;
using System.Linq;
using MM.Abstractions;

namespace MM.S4
{
    internal class MM5_HeatTransfer : BaseMethod
    {
        [ReflectedUICoefs]
        public double
            k_ = 1.65,
            lam_ = 93,
            Ct_ = 720,
            Cp_ = 440,
            Tz_ = 40,
            alfa_ = 0.1;

        [ReflectedUICoefs]
        public int
            t_ = 5,
            l_ = 2,
            n_ = 10;

        public override double[][] Calculate()
        {
            return HeatTransfer(Ct_, lam_,
                FilteringSpeed(k_, l_, n_, t_),
                Cp_, Tz_, alfa_, l_, n_, t_);
        }

        private double[][] HeatTransfer(double Ct, double lam, double[][] U,
            double Cp,
            double Tz, double alfa, int l, int n, int t)
        {
            var h = l * 1.0 / n;
            var c0 = 5;
            var T = 1;
            var inline = new double[n + 1];
            var tIrs = new double[t + 1][];

            inline[0] = c0;
            inline[n] = 2;
            for (var k = n - 1; k > 0; k--)
                inline[k] = 5 * Math.Exp(l - k * h);
            tIrs[0] = inline;

            for (var i = 1; i < t + 1; i++)
            {
                double[]
                    a = new double[n + 1],
                    b = new double[n + 1],
                    c = new double[n + 1];
                inline = new double[n + 1];

                for (var j = 0; j < n + 1; j++)
                {
                    var N = lam /
                            (1 + h * Math.Abs(1003 * Cp * U[i][j]) / (2 * lam));
                    var R = (-U[i][j] + Math.Abs(U[i][j])) / 2;
                    var r = (-U[i][j] - Math.Abs(U[i][j])) / 2;
                    a[j] = T * (N / Math.Pow(h, 2) - r / h) / Ct;
                    b[j] = T * (N / Math.Pow(h, 2) - R / h) / Ct;
                    c[j] = 1.0 + T * (a[j] + b[j]) / Ct;
                }

                var L = new double[n];
                var B = new double[n];
                L[0] = lam / (h * alfa + lam);
                B[0] = alfa * h * Tz / (h * alfa + lam);

                for (var m = 1; m < n; m++)
                {
                    L[m] = b[m] / c[m] - a[m] * L[m - 1];
                    B[m] = (a[m] * B[m - 1] + tIrs[i - 1][m]) /
                           (c[m] - a[m] * L[m - 1]);
                }

                inline[n] = (-alfa * h * L[1] * Tz + lam + B[1]) /
                            (-(lam + alfa * h) * L[1] + lam);
                for (var k = n - 1; k > 0; k--)
                    inline[k] = L[k] * inline[k + 1] + B[k];

                inline[0] = c0;

                tIrs[i] = inline;
            }

            return tIrs.ToArray();
        }

        private double[][] FilteringSpeed(double k, int l, int n, int t)
        {
            var Q = new double[t + 1][];
            var h = l / n;
            var len = (l + 1) * (t + 1);
            var q = new double[len];
            for (var i = 0; i < t + 1; i++)
            {
                for (var j = 0; j < len; j++) q[j] = -k * (5 * l - 10 * j * h);

                Q[i] = q.ToArray();
            }

            return Q;
        }
    }
}