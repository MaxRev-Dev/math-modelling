using MathNet.Numerics.LinearAlgebra.Double;
using MM.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Enu = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<double>>;
using F = System.Func<double, double>;

namespace MM.S6
{
    internal class S6M6 : BaseMethod
    {
        #region Props

        public override int Priority => 12;

        public override int Precision => 6;

        public override double ChartStepX => h;

        public override string[] YLegend => new[] { "Orig.", "Approx." };

        [ReflectedUICoefs]
        public static int
            m = 100;

        [ReflectedUICoefs]
        public static double
            a = 0,
            b = 1,
            e = 10;


        private static double
            h = (b - a) / m;

        F y = Math.Sqrt;
        F fi = r => Math.Sqrt(1 + Math.Pow(r * e, 2));

        #endregion

        #region selectors  
        Enu MatR(double[] x)
        {
            for (int i = 0; i < m; i++)
            {
                var localI = i;

                IEnumerable<double> Row()
                {
                    for (int j = 0; j < m; j++)
                    {
                        yield return Math.Sqrt(Math.Pow(x[localI] - x[j], 2));
                    }
                }

                yield return Row();
            }
        }
        Enu MatA(Enu r)
        {
            return r.Select(x => x.Select(f => fi(f)));
        }

        IEnumerable<double> VecF(IEnumerable<double> l, Enu r)
        {
            var lpEnu = l.ToArray();
            var rpEnu = r.Select(x => x.ToArray()).ToArray();
            var range = Enumerable.Range(0, m).ToArray();
            for (int i = 0; i < m; i++)
            {
                yield return range
                    .Select(v => lpEnu[v] * fi(rpEnu[i][v])).Sum();
            }
        }

        #endregion

        [ReflectedTarget]
        public double[][] Solution()
        {
            var xs = RangeOf(m, x => a + x * h).ToArray();
            var matR = MatR(xs).ToArray();
            var ys = RangeOf(m, i => y(xs[i])).ToArray();

            var matA = MatA(matR);
            var denseA = DenseMatrix.Build.DenseOfRows(matA);
            var denseY = DenseVector.Build.DenseOfEnumerable(ys);
            var denseC = denseA.Solve(denseY);

            var vecF = VecF(denseC, matR).ToArray();

            Info.Clear();
            return new[] { ys, vecF };
        }

        private IEnumerable<T> RangeOf<T>(int i, Func<int, T> func)
        {
            return Enumerable.Range(0, i).Select(func);
        }
    }
}