using System;
using System.Linq;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M5_Consolidation : BaseMethod
    {
        public override int Priority => 5;


        [ReflectedUICoefs(P = 6)]
        public static double
            a = 51.2 * Math.Pow(10, -7);
        [ReflectedUICoefs]
        public static double
            N = 5,
            k = 0.001,
            D = 0.02,
            H1 = 59,
            H2 = 26,
            L = 100,
            tau = 30,
            sigma = 0.2,
            h = 10,
            e = 0.6,
            v = 2.8 * Math.Pow(10, -3),
            C1 = 350,
            C2 = 10 + N,
            Cx = 0.15;

        [ReflectedUICoefs(P = 6)]
        public static double
            gm = 2 * Math.Pow(10, -4),
            gmS = 2 * Math.Pow(10, 4);

        [ReflectedUICoefs]
        public static int
            times = 4;

        public override double ChartStepX => h;
        public override double? ChartStepY => tau;
        public override double? MaxX => L;

        [ReflectedTarget]
        public double[][] GetConsolidation()
        {
            var MT = GetMassTransfer();

            Func<double, double> cx0 = x => (H2 - H1) * x / L + H1;
            Func<double, double> c0t = _ => H1;
            Func<double, double> clt = _ => H2;

            var h_2 = Math.Pow(h, 2);

            var a_ = k * (1 + e) / (gmS * a);
            var b_ = v * (1 + e) / (gmS * a);
            var a_const = a_ / h_2;
            var b_const = a_const;
            var c_const = 1f / tau - 2 * b_const; 

            var b = (int)(L / h); // interm points 
            double[]
                alfa = new double[b + 1],
                beta = new double[b + 1];
            var u = new double[times + 1][].Select(x => new double[b + 1]).ToArray();

            u[0][0] = c0t(0);
            u[0][b] = clt(0);
            for (var i = 1; i < b; i++)
            {
                u[0][i] = cx0(i * h);
            }

            for (int tl = 1; tl <= times; tl++) // time layers
            {
                u[tl][0] = c0t(tl);
                u[tl][b] = clt(tl);
                beta[0] = c0t(tl);

                for (var i = 1; i < b; i++)
                {
                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var F1 = b_ * (MT[tl][i - 1] - 2 * MT[tl][i] + MT[tl][i + 1]) / h_2;
                    var f2 = 1f * u[tl - 1][i] / tau + F1;
                    beta[i] = (a_const * beta[i - 1] + f2) /
                              (c_const - alfa[i - 1] * a_const);
                }
                for (int j = b - 1; j > 0; j--)
                {
                    u[tl][j] = alfa[j] * u[tl][j + 1] + beta[j];
                }
            }

            return u.ToArray();
        }

        [ReflectedTarget]
        public double[][] GetMassTransfer()
        {
            var V = (H1 - H2) * k / L; // filtering speed  

            Func<double, double> cx0 = x => C1 * Math.Exp(-x * Math.Log(C1 / C2) / L);
            Func<double, double> c0t = t => C1;
            Func<double, double> clt = i => C2;
             
            var h_2 = Math.Pow(h, 2);
            var r = -V / D;
            var M = 1f / (1 + (h * V) / (2 * D));
            var a_const = M / h_2 - r / h;
            var b_const = M / h_2;
            var c_const = 2 * M / h_2 - r / h + gm / D + sigma / (D * tau);
            var fk = sigma / (D * tau);
            var b = (int)(L / h); // interm points
            var fp = gm * Cx / D;
            double[]
                alfa = new double[b + 1],
                beta = new double[b + 1];
            var u = new double[times + 1][].Select(x => new double[b + 1]).ToArray();

            u[0][0] = c0t(0);
            u[0][b] = clt(0);
            for (var i = 1; i < b; i++)
            {
                u[0][i] = cx0(i*h);
            }

            for (int tl = 1; tl <= times; tl++) // time layers
            {
                u[tl][0] = c0t(tl*h);
                u[tl][b] = clt(tl * h);
                beta[0] = c0t(tl * h);

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