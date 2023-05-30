using MathNet.Numerics.LinearAlgebra;

namespace CourseFEM
{
    public class FEMSolver
    {
        // Input data (hardcoded constants)
        private static readonly double sigma = 5000000.0;
        private static readonly double ro = 4700.0;
        private static readonly double c = 203000000000.0;
        private static readonly double e = -2.53;
        private static readonly double g = 43.6;
        private static readonly double D = 0.0;
        private static readonly int N = 256;
        private static readonly double L = 0.01;
        private static double[] P = new double[N + 1];
        private static double[] U = new double[N + 1];
        private static double h = 0.01 / N;
        private static double[,] Fi = new double[N + 1, N + 1];
        // fi n+1 n+1
        private static double[] Xi = new double[N];

        private static double[] LandR = new double[2 * N + 2];
        // 2*n +2 for landr
        private static double[,] DEEG = new double[2 * N + 2, 2 * N + 2];
        // 2*n+2 2*n+2
        private static double[,] C = new double[N + 1, N + 1];
        private static double[,] G = new double[N + 1, N + 1];
        private static double[,] E = new double[N + 1, N + 1];
        // n+1 for matrixes

        public static void Configure()
        {
            LandR[N] = sigma;
            FillArrayWithStep(Xi, 1, N);
            CountFi();
            MultiplyMatrixByValue(C, c);
            MultiplyMatrixByValue(G, -g);

            MultiplyMatrixByValue(E, e);
            //C[0, 1] = 0;
            //E[0, 1] = 0;
            //C[0, 0] = 0;
            //G[0, 1] = 0;
            //G[0, 0] = Math.Pow(10, 20);

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
        public static void CombineMatrices()
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
        private static void CombineMatricesNotMy()
        {
            double[,] m = new double[2 * N + 2, 2 * N + 2];

            for (int i = 0; i < 2 * N; i++)
            {
                for (int k = 0; k < 2 * N; k++)
                {
                    m[i, k] = 0;

                }

            }

            //формуємо матрицю
            for (int i = 0; i <= 2 * N + 1; i += 2)
            {
                if (i >= 2)
                {
                    m[i, i - 2] = C[i / 2, 0];
                    m[i, i - 1] = E[i / 2, 0];

                    m[i + 1, i - 2] = E[i / 2, 0];
                    m[i + 1, i - 1] = G[i / 2, 0];
                }

                m[i, i] = C[i / 2, 1];
                m[i, i + 1] = E[i / 2, 1];

                m[i + 1, i] = E[i / 2, 1];
                m[i + 1, i + 1] = G[i / 2, 1];


                if (i != 2 * N)
                {
                    m[i, i + 2] = C[i / 2, 2];
                    m[i, i + 3] = E[i / 2, 2];

                    m[i + 1, i + 2] = E[i / 2, 2];
                    m[i + 1, i + 3] = G[i / 2, 2];

                }

            }

        }


        private static void CountFi()
        {

            // 3 options
            // j = i - 1
            // j = i
            // j = i + 1
            // else = 0
            /*
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {

                    if (j == i)
                    {
                        Fi[i, j] = (2 / (h * h));
                    }
                    else if (j == i - 1)
                    {
                        Fi[i, j] = (-1 / (h * h));
                    }
                    else if (j == i + 1)
                    {
                        Fi[i, j] = (-1 / (h * h));
                    }
                    else
                    {
                        Fi[i, j] = 0;
                    }



                }
            }
            */

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
        private static double[,] TransformMatrix(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            double[,] transformedMatrix = new double[n, m + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    transformedMatrix[i, j] = matrix[i, j];
                }

                transformedMatrix[i, m] = vector[i];
            }

            return transformedMatrix;
        }

        public static double[] SolveLinearEquations(double[,] A, double[] B)
        {
            int n = B.Length;
            var matrixA = Matrix<double>.Build.DenseOfArray(A);
            var vectorB = Vector<double>.Build.Dense(B);

            var result = matrixA.Solve(vectorB);

            return result.ToArray();
        }


        public static void SplitVector(out double[] vector1, out double[] vector2)
        {
            CombineMatrices();
            DEEG[0, 0] = Math.Pow(10, 20);
            DEEG[N + 1, N + 1] = Math.Pow(10, 20);
            //var esr = SolveByTridiagonalMatrixAlgorithm();
            var mathnetVector = SolveLinearEquations(DEEG, LandR);


            // var res = DoSomething();

            //double[] combinedVector = Solve();
            //var result = SolveLinearSystem();
            //int N = combinedVector.Length / 2;
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
