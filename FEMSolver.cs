using MathNet.Numerics.LinearAlgebra;

namespace CourseFEM
{
    public class FEMSolver
    {
        private readonly double sigma;
        private readonly double ro;
        private readonly double c;
        private readonly double e;
        private readonly double g;
        private readonly double D;
        private readonly int N;
        private readonly double L;

        private static double h;
        private static double[,] Fi;
        private static double[] LandR;
        private static double[,] DEEG;
        private static double[,] C;
        private static double[,] G;
        private static double[,] E;

        public FEMSolver(double _sigma, double _ro, double _c, double _e, double _g, double _d, int _n, double _l)
        {
            sigma = _sigma;
            ro = _ro;
            c = _c;
            e = _e;
            g = _g;
            D = _d;
            N = _n;
            L = _l;
            h = L / N;
            Fi = new double[N + 1, N + 1];
            LandR = new double[2 * N + 2];
            DEEG = new double[2 * N + 2, 2 * N + 2];
            C = new double[N + 1, N + 1];
            E = new double[N + 1, N + 1];
            G = new double[N + 1, N + 1];
        }
        public void Configure()
        {
            LandR[N] = sigma;
            CountFi();
            MultiplyMatrixByValue(C, c);
            MultiplyMatrixByValue(G, -g);
            MultiplyMatrixByValue(E, e);
        }
        public static void FillArrayWithStep(double[] array, double L, int N)
        {
            double h = L / N;
            for (int i = 0; i < N; i++)
            {
                array[i] = i * h;
            }
        }
        private static void MultiplyMatrixByValue(double[,] matrix, double value)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = Fi[i, j] * value;
                }
            }
        }
        public void CombineMatrices()
        {
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < N + 1; j++)
                {
                    DEEG[i, j] = C[i, j];
                    DEEG[i, j + N + 1] = E[i, j];
                    DEEG[i + N + 1, j] = E[i, j];
                    DEEG[i + N + 1, j + N + 1] = G[i, j];
                }
            }
        }
        private void CountFi()
        {
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < N + 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        Fi[i, j] = 1 / h;
                    }
                    else
                    if (i == N && j == N)
                    {
                        Fi[i, j] = 1 / h;
                    }
                    else
                    {
                        if (j == i)
                        {
                            Fi[i, j] = (2) / (h);
                        }
                        else if (j == i - 1)
                        {
                            Fi[i, j] = -1 / h;
                        }
                        else if (j == i + 1)
                        {
                            Fi[i, j] = -1 / h;
                        }
                        else
                        {
                            Fi[i, j] = 0;
                        }
                    }
                }
            }
        }
        public static double[] SolveLinearEquations(double[,] A, double[] B)
        {
            int n = B.Length;
            var matrixA = Matrix<double>.Build.DenseOfArray(A);
            var vectorB = Vector<double>.Build.Dense(B);

            var result = matrixA.Solve(vectorB);

            return result.ToArray();
        }
        public void SplitVector(out double[] vector1, out double[] vector2)
        {
            CombineMatrices();
            DEEG[0, 0] = Math.Pow(10, 20);
            DEEG[N + 1, N + 1] = Math.Pow(10, 20);

            var mathnetVector = SolveLinearEquations(DEEG, LandR);

            vector1 = new double[N + 1];
            vector2 = new double[N + 1];

            for (int i = 0; i < N + 1; i++)
            {
                vector1[i] = mathnetVector[i];
                vector2[i] = mathnetVector[i + N + 1];
            }
        }
    }
}
