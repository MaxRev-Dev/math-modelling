using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MM.Abstractions;
using F = System.Func<double, double>;

namespace MM.S6
{
    internal class S6M2 : BaseMethod
    {
        #region Props

        public override int Priority => 11;

        public override int Precision => 6;

        public override double ChartStepX => h;
        public override string[] YLegend => new[] { "Orig.", "Approx." };

        [ReflectedUICoefs]
        public static int
            n = 10;

        [ReflectedUICoefs]
        public static double
            a = 0,
            b = 1;

        private static int
            nn = n * n;

        private static double
            h = (b - a) / nn;

        private enum T
        {
            FI,
            FIDX,
            F
        }

        F p = x => 1;
        F g = x => 1;
        F f = x => -x;

        #endregion

        #region selectors
        private static double Fi(double i, double x) => x * Math.Exp(i * x);

        private static double Fidx(double i, double x) => Math.Exp(i * x) + i * x * Math.Exp(i * x);

        private static double Psi(double i, double x) => x * Math.Exp(Math.Pow(-1, i) * i * x);

        private static double Psidx(double i, double x)
        {
            var mI = Math.Pow(-1, i);
            var vI = mI * i * x;
            return Math.Exp(vI) + vI * x * Math.Exp(vI);
        }

        private F SelectorFunc(double i, double j, T type)
        {
            return type switch
            {
                T.FIDX => x => p(x) * Fidx(j, x) * Psidx(i, x),
                T.FI => x => g(x) * Fi(j, x) * Psi(i, x),
                T.F => x => f(x) * Psi(i, x),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        IEnumerable<IEnumerable<double>> MatA(Func<double, double, T, F> wrapSelector)
        {
            for (int i = 0; i < n; i++)
            {
                var localI = i;

                IEnumerable<double> Row()
                {
                    for (int j = 0; j < n; j++)
                    {
                        yield return Tpz(wrapSelector(localI + 1, j + 1, T.FIDX)) + Tpz(wrapSelector(localI + 1, j + 1, T.FI));
                    }
                }

                yield return Row();
            }
        }

        IEnumerable<double> VecF(Func<double, double, T, F> wrapSelector)
        {
            for (int i = 0; i < n; i++)
            {
                yield return Tpz(wrapSelector(i + 1, 0, T.F));
            }
        }

        private static double Tpz(F func)
        {
            return h * (0.5 * (func(a) + func(b)) +
                        Enumerable.Range(1, nn).Select(x => func(a + x * h)).Sum());
        }

        private static IEnumerable<double> CalculatedU(IEnumerable<int> xi, IEnumerable<double> c)
        {
            return xi.Select(i => a + i * h)
                .Select(x => c.Select((v, i) => v * Fi(i + 1, x)).Sum());
        }

        private static IEnumerable<double> OriginalU(IEnumerable<int> xi)
        {
            return xi.Select(i => a + i * h).Select(x =>
                Math.Exp(1) * (Math.Exp(x) - Math.Exp(-x)) / (Math.Pow(Math.Exp(1), 2) + 1) - x);
        }

        #endregion

        [ReflectedTarget]
        public double[][] Solution()
        {
            var matA = MatA(SelectorFunc);
            var vecF = VecF(SelectorFunc);
            var denseA = DenseMatrix.Build.DenseOfRows(matA);
            var denseF = DenseVector.Build.DenseOfEnumerable(vecF);
            var denseC = denseA.Solve(denseF);

            Info.Clear();

            var xi = Enumerable.Range(0, nn).ToArray();
            return new[] { OriginalU(xi).ToArray(), CalculatedU(xi, denseC).ToArray() };
        }
    }
}