using MM.Abstractions;

namespace MM.S4
{
    internal class MM6_ArmsRace : BaseMethod
    {
        [ReflectedUICoefs]
        public double
            L1 = 2.2,
            L2 = 0.8,
            B1 = 0.35,
            B2 = 0.8,
            T = 0.5;

        [ReflectedUICoefs]
        public int
            Y1 = 1,
            Y2 = 2,
            M10 = 160,
            M20 = 172,
            N = 5;

        public override double[][] Calculate()
        {
            return ArmsRace(L1, L2, B1, B2, Y1, Y2, M10, M20, T, N);
        }

        private double[][] ArmsRace(
            double l1, double l2,
            double b1, double b2,
            int y1, int y2,
            int m10, int m20,
            double t, int n)
        {
            var k = (int) (n / t);
            double[] m1 = new double[k],
                m2 = new double[k];
            m1[0] = m10;
            m2[0] = m20;
            for (var i = 1; i < k; i++)
            {
                m1[i] = t * (l1 * m2[i - 1] + y1) + m1[i - 1] / (1 - t * b1);
                m2[i] = t * (l2 * m2[i - 1] + y2) + m2[i - 1] / (1 - t * b2);
            }

            return new[] {m1, m2};
        }
    }
}