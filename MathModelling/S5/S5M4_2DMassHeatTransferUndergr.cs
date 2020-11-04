using System;
using System.Collections.Generic;
using System.Linq;
using MM.Abstractions;
using F = System.Func<double, double>;

namespace MM.S5
{
    internal class S5M4_2DMassHeatTransferUndergr : BaseMethod
    {
        [ReflectedUICoefs]
        public static double
            N = 5,
            k = 1.5,
            D = 0.02,
            Dt = 0.04,
            Lx = 100,
            By = 10,
            tau = 30,
            sigma = 0.4,
            hx = 10,
            lam = 0.5,
            hy = 2,
            H1 = 1.5,
            H2 = 0.5,
            C1 = 350,
            C2 = 8 * N,
            Cm = 350,
            Cp_ = 4.2,
            Cn_ = 3,
            T1 = 25 + N,
            T2 = 8 + N,
            T3 = 18;

        [ReflectedUICoefs]
        public static int
            times = 4;

        [ReflectedUICoefs(P = 6)]
        public double
            gamma = 0.065;

        public override int Priority => 4;

        public override double ChartStepX => hx;
        public override double? ChartStepY => hy;
        public override double? StepTime => tau;
        public override double? MaxX => Lx;

        public override bool Is3D => true;

        private double GetFilteringSpeed()
        {
            return (H1 - H2) * k / Lx; // filtering speed  
        }

        public double[][][] CalculateMassTransfer2D()
        {
            var Nx = (int) (Lx / hx); // interm points x 
            var Ny = (int) (By / hy); // interm points y

            var V = GetFilteringSpeed(); // filtering speed  

            var Cx = 0.1 * Cm;
            var h_2 = Math.Pow(hx, 2);
            var r = -V / D;
            var M = 1f / (1 + hx * V / (2 * D));
            var ax_const = M / h_2 - r / hx;
            var bx_const = M / h_2;
            var cx_const = ax_const + bx_const + gamma / (2 * D) +
                           sigma / (D * tau);

            var ay_const = 1f / Math.Pow(hy, 2);
            var by_const = ay_const;
            var cy_const = 2 * ay_const + gamma / (2 * D) + sigma / (D * tau);

            var f2_1 = gamma * Cx / D;
            var fk = sigma / (D * tau);
            var fp = gamma * Cx / (2 * D);

            var layers = new List<double[][]>();

            var hC2d = CalculateHeatTransfer2D();

            void FillOnZero()
            {
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();
                for (var v = 0; v < Ny + 1; v++)
                {
                    layer[v][0] = C1;
                    for (var i = 1; i < Nx; i++)
                        layer[v][i] =
                            v == 0 ? Cm : (C2 - C1) * hx * i / Lx + C1;
                    layer[v][Nx] = C2;
                }

                layers.Add(layer);
            }


            void FillByHalf(int layerTimek)
            {
                // 0.5k -> OX
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();

                var prev = layers[layerTimek - 1];
                var hC = hC2d[layerTimek - 1];
                for (var v = 0; v < Ny + 1; v++)
                    if (v == 0)
                    {
                        layer[v][0] = C1;
                        for (var i = 1; i < Nx; i++) layer[v][i] = Cm;

                        layer[v][Nx] = C2;
                    }
                    else
                    {
                        double[]
                            alfa = new double[Nx],
                            beta = new double[Nx];
                        beta[0] = C1;

                        for (var i = 1; i < Nx; i++)
                        {
                            alfa[i] = bx_const /
                                      (cx_const - alfa[i - 1] * ax_const);

                            var F1 = Dt *
                                     (hC[v][i - 1] - 2 * hC[v][i] +
                                      hC[v][i + 1]) /
                                     (D * h_2);
                            var f2 = f2_1 + F1 + fk * prev[v - 1][i];
                            beta[i] = (ax_const * beta[i - 1] + f2) /
                                      (cx_const - alfa[i - 1] * ax_const);
                        }

                        layer[v][0] = C1;
                        for (var j = Nx - 1; j > 0; j--)
                            layer[v][j] = alfa[j] * layer[v][j + 1] + beta[j];

                        layer[v][Nx] = C2;
                    }

                layers.Add(layer);
            }

            void FillByFull(int layerTimek)
            {
                // 1k -> fill OY
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();

                var prev = layers[layerTimek - 1];
                var hC = hC2d[layerTimek - 1];
                for (var v = 0; v < Nx + 1; v++)
                    if (v == 0)
                    {
                        layer[v][0] = C1;
                        for (var i = 0; i < Nx; i++) layer[v][i] = Cm;

                        layer[v][Nx] = C2;
                    }
                    else
                    {
                        double[]
                            alfa = new double[Ny],
                            beta = new double[Ny];
                        beta[0] = C1;

                        for (var i = 1; i < Ny; i++)
                        {
                            alfa[i] = by_const /
                                      (cy_const - alfa[i - 1] * ay_const);

                            var F1 = Dt *
                                     (hC[i - 1][v] - 2 * hC[i][v] +
                                      hC[i + 1][v]) /
                                     (D * h_2);
                            var f2 = f2_1 + F1 + fk * prev[i][v - 1];

                            beta[i] = (ay_const * beta[i - 1] + f2) /
                                      (cy_const - alfa[i - 1] * ay_const);
                            layer[i][0] = C1;
                        }

                        layer[Ny][0] = C1;
                        layer[Ny][v] = beta[Ny - 1] / (1 - alfa[Ny - 1]);
                        for (var j = Ny - 1; j > 0; j--)
                            layer[j][v] = alfa[j] * layer[j + 1][v] + beta[j];
                    }

                layers.Add(layer);
            }

            for (var layerTimeK = 0;
                layerTimeK < times * 2;
                layerTimeK++) // time layers
                if (layerTimeK == 0)
                {
                    FillOnZero();
                }
                else
                {
                    FillByHalf(layerTimeK);
                    FillByFull(layerTimeK);
                }

            return layers.ToArray();
        }

        [ReflectedTarget]
        public double[][][] GetMassTransfer2D()
        {
            var layers = CalculateMassTransfer2D();
            var ret = new List<double[][]>();
            for (var i = 0; i < layers.Length; i++)
                if (i == 0 || i % 2 == 0)
                    ret.Add(layers[i].Reverse().ToArray());

            return ret.ToArray().Reverse().ToArray();
        }

        [ReflectedTarget]
        public double[][][] GetHeatTransfer2D()
        {
            var layers = CalculateHeatTransfer2D();
            var ret = new List<double[][]>();
            for (var i = 0; i < layers.Length; i++)
                if (i == 0 || i % 2 == 0)
                    ret.Add(layers[i].Reverse().ToArray());

            return ret.ToArray().Reverse().ToArray();
        }

        public double[][][] CalculateHeatTransfer2D()
        {
            var Nx = (int) (Lx / hx); // interm points x 
            var Ny = (int) (By / hy); // interm points y

            var V = GetFilteringSpeed(); // filtering speed  

            var h_2 = Math.Pow(hx, 2);

            F T0 = x => (T2 - T1) * hx * x / Lx + T1;

            var Cp = Cp_ * Math.Pow(10, 6);
            var Cn = Cn_ * Math.Pow(10, 6);
            var r = -V * Cp / lam;
            var nt = Cn / lam;
            var nt_tau = nt / tau;

            var M = 1f / (1 + 0.5 * (hx * V * Cp / lam));
            var bx_const = M / h_2;
            var ax_const = bx_const - r / hx;
            var cx_const = 2 * bx_const - r / hx + nt_tau;

            var ay_const = 1f / Math.Pow(hy, 2);
            var by_const = ay_const;
            var cy_const = 2 * ay_const + nt_tau;


            var layers = new List<double[][]>();

            void FillOnZero()
            {
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();
                for (var v = 0; v < Ny + 1; v++)
                {
                    layer[v][0] = T1;
                    for (var i = 1; i < Nx; i++)
                        layer[v][i] = T0(i);
                    layer[v][Nx] = T2;
                }

                layers.Add(layer);
            }

            void FillByHalf()
            {
                // 0.5k -> OX
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();

                var prev = layers.Last();
                for (var v = 0; v < Ny + 1; v++)
                    if (v == 0)
                    {
                        layer[v][0] = T1;
                        for (var i = 1; i < Nx; i++) layer[v][i] = T3;

                        layer[v][Nx] = T2;
                    }
                    else
                    {
                        double[]
                            alfa = new double[Nx],
                            beta = new double[Nx];
                        beta[0] = T1;

                        for (var i = 1; i < Nx; i++)
                        {
                            alfa[i] = bx_const /
                                      (cx_const - alfa[i - 1] * ax_const);

                            var f1 = nt_tau * prev[v][i];
                            beta[i] = (ax_const * beta[i - 1] + f1) /
                                      (cx_const - alfa[i - 1] * ax_const);
                        }

                        layer[v][0] = T1;
                        for (var j = Nx - 1; j > 0; j--)
                            layer[v][j] = alfa[j] * layer[v][j + 1] + beta[j];

                        layer[v][Nx] = T2;
                    }

                layers.Add(layer);
            }

            void FillByFull()
            {
                // 1k -> fill OY
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();

                var prev = layers.Last();
                for (var v = 0; v < Nx + 1; v++)
                    if (v == 0)
                    {
                        layer[v][0] = T1;
                        for (var i = 1; i < Nx; i++) layer[v][i] = T3;

                        layer[v][Nx] = T2;
                    }
                    else
                    {
                        double[]
                            alfa = new double[Ny],
                            beta = new double[Ny];
                        beta[0] = T1;

                        for (var i = 1; i < Ny; i++)
                        {
                            alfa[i] = by_const /
                                      (cy_const - alfa[i - 1] * ay_const);
                            var f1 = nt_tau * prev[i][v];
                            beta[i] = (ay_const * beta[i - 1] + f1) /
                                      (cy_const - alfa[i - 1] * ay_const);
                            layer[i][0] = T1;
                        }

                        layer[Ny][0] = T1;
                        layer[Ny][v] = beta[Ny - 1] / (1 - alfa[Ny - 1]);
                        for (var j = Ny - 1; j > 0; j--)
                            layer[j][v] = alfa[j] * layer[j + 1][v] + beta[j];
                    }

                layers.Add(layer);
            }

            for (var layerTimeI = 0;
                layerTimeI < times * 2;
                layerTimeI++) // time layers
                if (layerTimeI == 0)
                {
                    FillOnZero();
                }
                else
                {
                    FillByHalf();
                    FillByFull();
                }

            return layers.ToArray();
        }
    }
}