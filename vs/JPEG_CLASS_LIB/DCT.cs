using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    public class DCT
    {
        public static short[,] FDCT(short[,]inp_matrix)
        {
            short[,] matrix = inp_matrix;
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            short[,] matrix2 = new short[n, m];
            double S = 0;
            double Cv = 1;
            double Cu = 1;
            for (int v = 0; v < n; v++)
            {
                //if (u == 0) Cu = 1 / Math.Sqrt(2);
                //else Cu = 1;
                for (int u = 0; u < m; u++)
                {
                    S = 0;
                    for (int x = 0; x < m; x++)
                    {
                        for (int y = 0; y < n; y++)
                        {
                            S += matrix[y, x] * Math.Cos((2 * x + 1) * u * Math.PI / 16.0) * Math.Cos((2 * y + 1) * v * Math.PI / 16.0);
                        }
                    }
                    if ((v == 0) && (u == 0)) { Cv = 1 / Math.Sqrt(2); Cu = 1 / Math.Sqrt(2); }
                    else { Cv = 1; Cu = 1; }
                    matrix2[v, u] = Convert.ToInt16(S*Cu * Cv/4.0);
                    //matrix2[u, v] = Convert.ToInt16(S);
                    //matrix2[u, v] = (S * Cu * Cv / 4);

                }
            }
            return(matrix2);
        }


        public static short[,] IDCT(short[,] inp_matrix)
        {
            short[,] matrix = inp_matrix;
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            short[,] matrix2 = new short[n, m];
            double s = 0;
            double Cv = 1;
            double Cu = 1;
            for (int x = 0; x < m; x++)
            {
                //if (u == 0) Cu = 1 / Math.Sqrt(2);
                //else Cu = 1;
                for (int y = 0; y < n; y++)
                {
                    s = 0;
                    for (int u = 0; u < m; u++)
                    {
                        for (int v = 0; v < n; v++)
                        {
                            if ((v == 0) && (u == 0)) { Cv = 1 / Math.Sqrt(2); Cu = 1 / Math.Sqrt(2); }
                            else { Cv = 1; Cu = 1; }
                            s += Cu*Cv*(matrix[v, u] * Math.Cos((2 * x + 1) * u * Math.PI / 16.0) * Math.Cos((2 * y + 1) * v * Math.PI / 16.0));
                        }
                    }
                    //if (v == 0) Cv = 1 / Math.Sqrt(2);
                    //else Cv = 1;
                    matrix2[y, x] = Convert.ToInt16(s/4.0);
                    //matrix2[y, x] = (s / 4);

                }
            }
            return (matrix2);
	}
        /// <summary>
        /// Возвращает матрицу, каждый компонент которой приведен из диапазона 0..255 в диапазон -128..127
        /// </summary>
        public static short[,] Shift(byte[,] matrix)
        {
            short[,] mat = new short[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++) mat[i,j] = Convert.ToInt16(matrix[i,j]-128);
            return mat;
        }
        /// <summary>
        /// Возвращает матрицу, каждый компонент которой приведен из диапазона -128..127 в диапазон 0..255
        /// </summary>
        public static byte[,] ReverseShift(short[,] matrix)
        {
            byte[,] mat = new byte[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++) mat[i, j] = Convert.ToByte(matrix[i, j]+128);
            return mat;
        }
    }
}
