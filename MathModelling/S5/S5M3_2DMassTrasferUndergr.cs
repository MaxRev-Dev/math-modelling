using System;
using System.Collections.Generic;
using System.Linq;
using MM.Abstractions;

namespace MM.S5
{
    internal class S5M3_2DMassTrasferUndergr : BaseMethod
    {
        [ReflectedUICoefs]
        public static double
            N = 5,
            k = 1.5,
            D = 0.02,
            Lx = 100,
            By = 10,
            tau = 30,
            sigma = 0.4,
            hx = 10,
            hy = 2,
            H1 = 1.5,
            H2 = 0.5,
            C1 = 350,
            C2 = 8 * N,
            Cm = 350;

        [ReflectedUICoefs]
        public static int
            times = 4;

        [ReflectedUICoefs(P = 6)]
        public double
            gamma = 0.0065;

        public override int Priority => 3;

        public override double ChartStepX => hx;
        public override double? ChartStepY => hy;
        public override double? StepTime => tau;
        public override double? MaxX => Lx;

        public override bool Is3D => true;

        private double GetFilteringSpeed()
        {
            return (H1 - H2) * k / Lx; // filtering speed  
        }

        public override double[][][] Calculate3D()
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

            var fk = sigma / (D * tau);
            var fp = gamma * Cx / (2 * D);

            var layers = new List<double[][]>();

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

            void FillByHalf()
            {
                // 0.5k -> OX
                var layer = new double[Ny + 1].Select(x => new double[Nx + 1])
                    .ToArray();

                var prev = layers.Last();
                for (var v = 0; v < Ny + 1; v++)
                    if (v == 0)
                    {
                        layer[v][0] = C1;
                        for (var i = 0; i < Nx; i++) layer[v][i] = Cm;

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
                            beta[i] =
                                (ax_const * beta[i - 1] + fk * prev[v][i] +
                                 fp) /
                                (cx_const - alfa[i - 1] * ax_const);
                        }

                        layer[v][0] = C1;
                        for (var j = Nx - 1; j > 0; j--)
                            layer[v][j] = alfa[j] * layer[v][j + 1] + beta[j];

                        layer[v][Nx] = C2;
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
                            beta[i] =
                                (ay_const * beta[i - 1] + fk * prev[i][v] +
                                 fp) /
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

            var ret = new List<double[][]>();
            for (var i = 0; i < layers.Count; i++)
                if (i % 2 == 1)
                    ret.Add(layers[i].Reverse().ToArray());

            return ret.ToArray();
        }
    }
}