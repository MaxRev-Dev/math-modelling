using System;
using System.Collections.Generic;
using MaxRev.Extensions.Matrix;

namespace Belman
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Belman.CreateContext().Solve();
        }
    }

    internal class Belman
    {
        private static readonly double M = 1000;

        private double[,] Minimization(double[,] C)
        {
            var n = C.GetLength(0);
            var Cf = new double[n, n];
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                if (i != j)
                {
                    var mins = M;
                    for (var k = 0; k < n; k++)
                    {
                        var mm = C[i, k] + C[k, j];
                        if (Math.Abs(mm - 1) < 0.001) break;

                        if (mm < mins) mins = mm;
                        Cf[i, j] = mins;
                    }
                }
                else
                {
                    Cf[i, j] = 0;
                }

            return Cf;
        }

        public void Solve()
        {
            var m = new[,]
            {
                {0, M, 2, 1, M, M},
                {M, 0, M, 7, 3, M},
                {2, M, 0, 3, 4, 1},
                {1, 7, 3, 0, 2, M},
                {M, 3, 4, 2, 0, M},
                {M, M, 1, M, 4, 0}
            };
            m.Print();
            var shortestPath = FindShortestPath(m, 0, 1);
            Console.WriteLine(
                $@"Shortest path: {string.Join(",", shortestPath)}");
        }

        private int[] FindShortestPath(double[,] matrix, int from, int to)
        {
            var current = matrix;
            while (true)
            {
                var m1 = Minimization(current);
                var m2 = Minimization(m1);
                if (MatrixEquals(m1, m2))
                {
                    current = m1;
                    break;
                }

                current = m2;
            }

            return Path(matrix, current, from, to);
        }

        private bool MatrixEquals(double[,] m1, double[,] m2)
        {
            for (var i = 0; i < m1.GetLength(0); i++)
            for (var j = 0; j < m1.GetLength(1); j++)
                if (Math.Abs(m1[i, j] - m2[i, j]) > 0.001)
                    return false;

            return true;
        }

        private int[] Path(double[,] matrix, double[,] c, int from, int to)
        {
            var num = new List<int>();
            var result = new List<int>();
            for (var i = 0; i < c.GetLength(0); i++) num.Add(i);

            num.Remove(from);
            result.Add(from + 1);
            var k = 1;
            for (var i = 0; i < c.GetLength(0); i++)
            {
                if (from == to) continue;

                var mins = M;
                foreach (var j in num)
                {
                    var mm = matrix[from, j] + c[j, to];
                    if (Math.Abs(mm - 1) < 0.0001) // mm==1
                    {
                        k = j;
                        break;
                    }

                    if (!(mm < mins))
                        continue;
                    k = j;
                    mins = mm;
                }

                result.Add(k + 1);
                num.Remove(k);
                from = k;
            }

            return result.ToArray();
        }

        public static Belman CreateContext()
        {
            return new Belman();
        }
    }
}