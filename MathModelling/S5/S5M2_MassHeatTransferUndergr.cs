using System;
using System.Linq;
using MM.Abstractions;
using F = System.Func<double, double>;

namespace MM.S5
{
    internal class S5M2_MassHeatTransferUndergr : BaseMethod
    {
        [ReflectedUICoefs]
        public static int
            k = 7,
            times = 4;

        [ReflectedUICoefs]
        public static double
            kappa = 1.5,
            Cp_ = 4.2,
            Cn_ = 3,
            lam = 0.5,
            D = 0.2,
            H1 = 1.5,
            H2 = 0.5,
            l = 100,
            tau = 30,
            sigma = 0.2,
            h = 20,
            T1 = 20 * k,
            T2 = 8 + k;

        [ReflectedUICoefs(P = 3)]
        public double
            Dt = 0.04,
            gamma = 0.065;

        public override int Priority => 2;

        private double GetFilteringSpeed()
        {
            return (H1 - H2) * kappa / l; // filtering speed  
        }

        [ReflectedTarget]
        public double[][] GetHeatTransfer()
        {
            var b = (int) (l / h); // interm points
            double[]
                alfa = new double[b + 1],
                beta = new double[b + 1];
            var u = new double[times + 1][].Select(x => new double[b + 1])
                .ToArray();

            var V = GetFilteringSpeed();
            var Cp = Cp_ * Math.Pow(10, 6);
            var Cn = Cn_ * Math.Pow(10, 6);
            var h_2 = Math.Pow(h, 2);
            var r = -V * Cp / lam;
            var nt = Cn / lam;
            var nt_tau = nt / tau;

            F tx0 = x => (T2 - T1) * x * h / l + T1;
            F t0t = t_ => T1;
            F tlt = t_ => T2;
            u[0][0] = t0t(0);
            u[0][b] = tlt(0);

            for (var i = 1; i < b; i++) u[0][i] = tx0(i);

            var M = 1f / (1 + 0.5 * (h * V * Cp / lam));
            var a_const = M / h_2 - r / h;
            var b_const = M / h_2;
            var c_const = a_const + b_const + nt_tau;

            for (var tl = 1; tl <= times; tl++) // time layers
            {
                u[tl][0] = t0t(tl * tau);
                u[tl][b] = tlt(tl * tau);
                beta[0] = u[tl][0];

                for (var i = 1; i <= b; i++)
                {
                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var f1 = nt_tau * u[tl - 1][i];
                    beta[i] = (a_const * beta[i - 1] + f1) /
                              (c_const - alfa[i - 1] * a_const);
                }

                for (var j = b - 1; j > 0; j--)
                    u[tl][j] = alfa[j] * u[tl][j + 1] + beta[j];
            }

            return u.ToArray();
        }

        [ReflectedTarget]
        public double[][] GetMassTransfer()
        {
            var V = GetFilteringSpeed();

            F cx0 = x => 0;
            F c0t = t_ => Math.Pow(t_, 2) * Math.Exp(-0.1 * t_ * k);
            F clt = t_ => Math.Pow(t_, 2) * Math.Exp(-0.2 * t_ * k);

            var h_2 = Math.Pow(h, 2);
            var r = -V / D;
            var M = 1f / (1 + h * V / (2 * D));

            var a_const = M / h_2 - r / h;
            var b_const = M / h_2;
            var c_const = 2 * M / h_2 - r / h + gamma / D + sigma / (D * tau);

            var Cx = 0.1 * kappa;
            var f2_1 = gamma * Cx / D;
            var fk = sigma / (D * tau);

            var b = (int) (l / h); // interm points
            double[]
                alfa = new double[b + 1],
                beta = new double[b + 1];
            var u = new double[times + 1][].Select(x => new double[b + 1])
                .ToArray();

            for (var i = 1; i <= b; i++) u[0][i] = cx0(i);

            var hC = GetHeatTransfer();

            for (var tl = 1; tl <= times; tl++) // time layers
            {
                u[tl][0] = c0t(tl * tau);
                u[tl][b] = clt(tl * tau);
                beta[0] = c0t(tl * tau);

                for (var i = 1; i < b; i++)
                {
                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var F1 = Dt *
                             (hC[tl][i - 1] - 2 * hC[tl][i] + hC[tl][i + 1]) /
                             (D * h_2);
                    var f2 = f2_1 + F1 + fk * u[tl - 1][i];
                    beta[i] = (a_const * beta[i - 1] + f2) /
                              (c_const - alfa[i - 1] * a_const);
                }

                for (var j = b - 1; j > 0; j--)
                    u[tl][j] = alfa[j] * u[tl][j + 1] + beta[j];
            }

            return u.ToArray();
        }


        #region Helpers

        public override double? ChartStepY => tau;
        public override double ChartStepX => h;
        public override double? MaxX => l;

        #endregion
    }
}