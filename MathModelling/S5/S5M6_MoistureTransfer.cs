using System;
using System.Collections.Generic;
using System.Linq;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M6_MoistureTransfer : BaseMethod
    {
        [ReflectedUICoefs(P = 6)]
        public static double
            a = 51.2 * Math.Pow(10, -7);

        [ReflectedUICoefs]
        public static double
            N = 5,
            k = 0.001,
            D = 0.02,
            H1 = 7,
            H2 = 20,
            L = 15,
            tau = 360,
            sigma = 0.5,
            h = 1.5,
            g = 9.8,
            e = 0.6,
            v = 2.8 * Math.Pow(10, -5),
            C0 = 0,
            C1 = 0,
            C2 = 10,
            Cx = 350,
            p = 1000,
            lam = 0.1;

        [ReflectedUICoefs(P = 6)]
        public static double
            gm = 0.0065,
            Dm = Math.Pow(10, -6),
            gmS = 2 * Math.Pow(10, 4);

        [ReflectedUICoefs]
        public static int
            times = 4;

        private (double[][] filt, double[][] mass, double[][] moist)? _result;

        public override int Priority => 6;

        public override double ChartStepX => h;
        public override double? ChartStepY => tau;
        public override double? MaxX => L;

        private void CalculateCore()
        {

            var b = (int)(L / h); // interm points 
            var h_2 = Math.Pow(h, 2);

            double[] massTransfer(int tl, double[] massPrev, double[] filtK)
            {
                Func<double, double> cx0 = x => C0;
                Func<double, double> c0t = t => C1;
                Func<double, double> clt = i => C2;

                var mass = new double[b + 1];
                if (tl == 0)
                {
                    mass[0] = c0t(0);
                    mass[b] = clt(0);
                    for (var i = 1; i < b; i++) mass[i] = cx0(i * h);
                    return mass;
                }

                double[]
                    alfa = new double[b + 1],
                    beta = new double[b + 1];
                mass[0] = c0t(tl * h);
                mass[b] = clt(tl * h);
                beta[0] = c0t(tl * h);

                var fp = Cx * gm;
                var fk = sigma / tau;

                for (var i = 1; i < b; i++)
                {
                    var rp = (-filtK[i] + Math.Abs(filtK[i])) / 2;
                    var rm = (-filtK[i] - Math.Abs(filtK[i])) / 2;
                    var r = rp + rm;
                    var n = 1f / (1 + h * Math.Abs(r) / 2);
                     
                    var a_const = n / h_2 - rm / h;
                    var b_const = n / h_2 - rp / h;
                    var c_const = a_const + b_const + gm + sigma / tau;
                     
                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var f = fp + fk;
                    beta[i] = (a_const * beta[i - 1] + f * massPrev[i]) /
                              (c_const - alfa[i - 1] * a_const);
                }

                for (var j = b - 1; j > 0; j--)
                    mass[j] = alfa[j] * mass[j + 1] + beta[j];
                return mass;
            }

            double[] moistureTransfer(int tl, double[] moistK, double[] massK)
            {
                Func<double, double> cx0 = x => (H2 - H1) * x / L + H1;
                Func<double, double> c0t = _ => H1;
                Func<double, double> clt = _ => H2;
                var moist = new double[b + 1];
                if (tl == 0)
                {
                    moist[0] = c0t(0);
                    moist[b] = clt(0);
                    for (var i = 1; i < b; i++) moist[i] = cx0(i * h);
                    return moist;
                }
                double[]
                    alfa = new double[b + 1],
                    beta = new double[b + 1];
                moist[0] = c0t(tl * h);
                moist[b] = clt(tl * h);
                beta[0] = c0t(tl * h);
                for (var i = 1; i < b; i++)
                {
                    var hk = moistK;
                    var hp = hk[i - 1];
                    var hn = hk[i + 1];
                    var M = a * p * gm * (1 - 2 * h / (hn - hp));
                    var a_const = k / h_2;
                    var b_const = a_const;
                    var c_const = M / tau + 2 * k / h_2;

                    alfa[i] = b_const / (c_const - alfa[i - 1] * a_const);

                    var f = M / tau * hk[i] - v * (massK[i - 1] - 2 * massK[i] + massK[i + 1]) / h_2;
                    beta[i] = (a_const * beta[i - 1] + f * moistK[i]) /
                              (c_const - alfa[i - 1] * a_const);
                }

                for (var j = b - 1; j > 0; j--)
                    moist[j] = alfa[j] * moist[j + 1] + beta[j];
                return moist;
            }

            double[] filtration(double[] moist, double[] mass)
            {
                var filt = new double[b + 1];

                for (var i = 1; i < b; i++)
                {
                    filt[i] = -k * (moist[i + 1] - moist[i - 1]) / 2 * h +
                              v * (mass[i + 1] - 2 * mass[i] + mass[i - 1]) / h_2;
                }

                return filt;
            }

            var massLocal = massTransfer(0, default, default);
            var moisLocal = moistureTransfer(0, default, default);
            var f1list = new List<double[]>();
            var m1list = new List<double[]>();
            var m2list = new List<double[]>();
            m1list.Add(massLocal);
            m2list.Add(moisLocal);
            for (var tl = 1; tl <= times; tl++) // time layers
            {
                var filtK = filtration(moisLocal, massLocal);
                f1list.Add(filtK);
                m1list.Add(massLocal = massTransfer(tl, massLocal, filtK));
                m2list.Add(moisLocal = moistureTransfer(tl, moisLocal, filtK));
            }

            _result = (f1list.ToArray(), m1list.ToArray(), m2list.ToArray());
        }

        [ReflectedTarget]
        public double[][] GetMoistureTransfer()
        {
            if (!_result.HasValue)
            {
                CalculateCore();
            }

            return _result!.Value.moist;
        }

        [ReflectedTarget]
        public double[][] GetMassTransfer()
        {
            if (!_result.HasValue)
            {
                CalculateCore();
            }

            return _result!.Value.mass;
        }
        [ReflectedTarget]
        public double[][] GetFiltration()
        {
            if (!_result.HasValue)
            {
                CalculateCore();
            }

            return _result!.Value.filt;
        }
    }
}