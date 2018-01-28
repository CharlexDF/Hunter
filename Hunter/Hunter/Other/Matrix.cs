using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class Matrix
    {
        public static double[,] Copy(double[,] a)
        {
            int i, j;
            double[,] ret = new double[a.GetLength(0), a.GetLength(1)];
            for (i = 0; i < a.GetLength(0); i++)
            {
                for (j = 0; j < a.GetLength(1); j++)
                {
                    ret[i, j] = a[i, j];
                }
            }
            return ret;
        }

        public static bool Add(double[,] a, double[,] b, ref double[,] ret)
        {
            Debug.Assert(a.GetLength(0) == b.GetLength(0));
            Debug.Assert(a.GetLength(1) == b.GetLength(1));
            Debug.Assert(a.GetLength(0) == ret.GetLength(0));
            Debug.Assert(a.GetLength(0) == ret.GetLength(1));
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    ret[i, j] = a[i, j] + b[i, j];
                }
            }
            return true;
        }

        public static bool Subtract(double[,] a, double[,] b, ref double[,] ret)
        {
            Debug.Assert(a.GetLength(0) == b.GetLength(0));
            Debug.Assert(a.GetLength(1) == b.GetLength(1));
            Debug.Assert(a.GetLength(0) == ret.GetLength(0));
            Debug.Assert(a.GetLength(0) == ret.GetLength(1));
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    ret[i, j] = a[i, j] - b[i, j];
                }
            }
            return true;
        }

        public static bool Multiply(double[,] a, double[,] b, ref double[,] ret)
        {
            Debug.Assert(a.GetLength(1) == b.GetLength(0));
            Debug.Assert(a.GetLength(0) == ret.GetLength(0));
            Debug.Assert(b.GetLength(1) == ret.GetLength(1));
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    ret[i, j] = 0;
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        ret[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return true;
        }

        public static bool Ttransport(double[,] a, ref double[,] ret)
        {
            if (a.GetLength(0) != ret.GetLength(1) || a.GetLength(1) != ret.GetLength(0))
                return false;
            for (int i = 0; i < a.GetLength(1); i++)
                for (int j = 0; j < a.GetLength(0); j++)
                    ret[i, j] = a[j, i];

            return true;
        }

        public static double CalcDeterminant(double[,] a)
        {
            Debug.Assert(a.GetLength(0) == a.GetLength(1));
            int i, j, k, m;
            double sum, factor;
            double[,] x = Copy(a);

            sum = 1;
            for (i = 0; i < x.GetLength(0); i++)
            {
                if (x[i, i] == 0)
                {
                    k = 0;
                    for (m = i + 1; m < x.GetLength(0); m++)
                    {
                        if (x[m, i] != 0)
                        {
                            k = m;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        return 0;
                    }
                    for (j = 0; j < x.GetLength(1); j++)
                    {
                        Utility.Swap(ref x[i, j], ref x[k, j]);
                    }
                    sum *= -1;
                }

                sum *= x[i, i];
                factor = 1 / x[i, i];
                if (factor != 1)
                {
                    for (j = i; j < x.GetLength(1); j++)
                    {
                        x[i, j] *= factor;
                    }
                }

                m = i + 1;
                for (; m < x.GetLength(0); m++)
                {
                    factor = x[m, i];
                    if (factor == 0)
                    {
                        continue;
                    }
                    for (j = i; j < x.GetLength(1); j++)
                    {
                        x[m, j] = x[m, j] - x[i, j] * factor;
                    }
                }
            }

            Utility.Log("sum = " + sum);

            return sum;
        }

        public static bool Inverse(double[,] a, ref double[,] b)
        {
            double det = CalcDeterminant(a);
            if (det == 0) return false;
            det = 1 / det;

            double[,] B = new double[a.GetLength(0), a.GetLength(1)];
            double[,] SP = new double[a.GetLength(0), a.GetLength(1)];
            double[,] AB = new double[a.GetLength(0), a.GetLength(1)];

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    for (int m = 0; m < a.GetLength(0); m++)
                        for (int n = 0; n < a.GetLength(1); n++)
                            B[m, n] = a[m, n];

                    for (int x = 0; x < a.GetLength(1); x++)
                        B[i, x] = 0;
                    for (int y = 0; y < a.GetLength(0); y++)
                        B[y, j] = 0;
                    B[i, j] = 1;
                    SP[i, j] = CalcDeterminant(B);
                    AB[i, j] = det * SP[i, j];
                }
            }
            Inverse(AB, ref b);

            return true;
        }

        public static void Print(double[,] mtx)
        {
            int i, j;
            string line = "";
            Utility.Log("Print Matrix...");
            for (i = 0; i < mtx.GetLength(0); i++)
            {
                line = "";
                for (j = 0; j < mtx.GetLength(1); j++)
                {
                    if (j == 0)
                    {
                        line = line + mtx[i, j].ToString();
                    }
                    else
                    {
                        line = line + "," + mtx[i, j].ToString();
                    }
                }
                Utility.Log(line);
            }
        }

        public static void test()
        {
            //double[,] mtx = new double[3,3];
            double[] x = { 1, 2, 3, 4, 5 };
            double[,] Mat_Y = { { 10 }, { 20 }, { 30 }, { 40 }, { 50 } };
            double[,] Mat_X = { { 1, 1, 1 }, { 1, 2, 4 }, { 1, 3, 9 }, { 1, 4, 16 }, { 1, 5, 25 } };
            double[,] Mat_TX = new double[3, Mat_X.GetLength(0)];
            double[,] Max_Pro = new double[3, 3];
            double[,] Mat_IX = new double[3, 3];
            double[,] Max_Ret = new double[Mat_X.GetLength(0), 1];

            //double[,] test = { { 1, 0, 0, -1 }, { 0, 2, 2, 0 }, { 0, -3, 3, 0 }, { 4, 0, 0, 4 } };
            double[,] test = { { 1, 1,1,1 }, { 1,2,3,4 }, { 1,3,6,10 }, { 1,4,10,20 } };
            //double[,] test = { { 1, 1, 1, 0 }, { 1, 1, 0, 1 }, { 1, 0, 1, 1 }, { 0, 1, 1, 1 } };
            //double[,] test = { { 2, 1, -1 }, { 4, -1, 1 }, { 201, 102, -99 } };
            Matrix.CalcDeterminant(test);

            //Mat_Y n 1
            //Mat_X n 3
            //Utility.Log("Mat_Y.GetLength(0) = " + Mat_Y.GetLength(0) + " Mat_Y.GetLength(1) = " + Mat_Y.GetLength(1));
            //Utility.Log("Mat_X.GetLength(0) = " + Mat_X.GetLength(0) + " Mat_X.GetLength(1) = " + Mat_X.GetLength(1));

            //Matrix.Multiply(Mat_X, Mat_Y, ref Max_Ret);
            //Matrix.Print(Max_Ret);

            //Matrix.Ttransport(Mat_X, ref Mat_TX);

            //Utility.Log("Mat_TX.GetLength(0) = " + Mat_TX.GetLength(0) + " Mat_TX.GetLength(1) = " + Mat_TX.GetLength(1));
            //Utility.Log("Mat_X.GetLength(0) = " + Mat_X.GetLength(0) + " Mat_X.GetLength(1) = " + Mat_X.GetLength(1));
            //Utility.Log("Max_Pro.GetLength(0) = " + Max_Pro.GetLength(0) + " Max_Pro.GetLength(1) = " + Max_Pro.GetLength(1));

            //Matrix.Multiply(Mat_TX, Mat_X, ref Max_Pro);

            //Matrix.Inverse(Max_Pro, ref Mat_IX);

            //Matrix.Print(Matrix.Copy(Mat_X));


            //Matrix.Print(Mat_X);

            //Matrix.Multiply(Mat_IX, Mat_TX, ref Max_Pro);

            //Matrix.Multiply(Max_Pro, Mat_Y, ref Max_Ret);

            //Matrix.Print(Max_Ret);
        }








        public static List<int[]> plist = new List<int[]>();
        public static void Permutate(int[] arr, int idx = 0)
        {
            if (idx == arr.Length - 1)
            {
                plist.Add(arr);
                return;
            }

            int i;
            //int[] sub_arr = new int[arr.Length];
            for (i = idx; i < arr.Length; i++)
            {
                int[] sub_arr = new int[arr.Length];
                Array.Copy(arr, sub_arr, arr.Length);
                //sub_arr.CopyTo(arr, 0);
                Utility.Swap(ref sub_arr[idx], ref sub_arr[i]);
                Permutate(sub_arr, idx + 1);
            }
        }
        public static void TestPermutate()
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            Permutate(arr);

            int i, j;
            string line = "";
            int[] sub_arr = new int[arr.Length];
            for (i = 0; i < plist.Count; i++)
            {
                sub_arr = plist[i];
                line = "";
                for (j = 0; j < sub_arr.Length; j++)
                {
                    if (j == 0)
                    {
                        line = sub_arr[j].ToString();
                    }
                    else
                    {
                        line += ", " + sub_arr[j].ToString();
                    }
                }
                Utility.Log(line);
            }
        }

    }

}
