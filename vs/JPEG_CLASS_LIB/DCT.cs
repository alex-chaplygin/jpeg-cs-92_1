using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    public class DCT
    {
        /// <summary>
        /// На вход подается short матрица. Возвращает short матрицу полученную из входной по формуле FDCT
        /// </summary>
        public static short[,] FDCT(short[,]inp_matrix)
        {
            short[,] matrix = inp_matrix;
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            short[,] matrix2 = new short[n, m];
            double S = 0;
            double Cv = 1;
            double Cu = 1;
            for (int u = 0; u < n; u++)
                for (int v = 0; v < m; v++)
                {
                    S = 0;
                    for (int x = 0; x < m; x++)
                        for (int y = 0; y < n; y++)
                        {
                            S += matrix[y, x] * Math.Cos((2 * x + 1) * u * Math.PI / 16.0) * Math.Cos((2 * y + 1) * v * Math.PI / 16.0);
                        }
		    Cv = 1;
		    Cu = 1;
                    if (u == 0)  Cu = 1 / Math.Sqrt(2);
		    if (v == 0) Cv = 1 / Math.Sqrt(2); 
                    matrix2[v, u] = Convert.ToInt16(S*Cu * Cv/4.0);
                }
            return(matrix2);
        }

        /// <summary>
        /// На вход подается short матрица. Возвращает short матрицу полученную из входной по формуле IDCT
        /// </summary>
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
                for (int y = 0; y < n; y++)
                {
                    s = 0;
                    for (int u = 0; u < m; u++)
                        for (int v = 0; v < n; v++)
                        {
			    Cv = 1;
			    Cu = 1;
			    if (u == 0)  Cu = 1 / Math.Sqrt(2);
			    if (v == 0) Cv = 1 / Math.Sqrt(2); 
                            s += Cu*Cv*(matrix[v, u] * Math.Cos((2 * x + 1) * u * Math.PI / 16.0) * Math.Cos((2 * y + 1) * v * Math.PI / 16.0));
                        }
                    matrix2[y, x] = Convert.ToInt16(s/4.0);
                }
            return (matrix2);
	    }
        /// <summary>
        /// На вход подается матрица 8 на 8. Возвращает массив 64, который получен из матрицы при проходе её зигзагом
        /// </summary>
        public static short[] Zigzag(short[,] matrix)
        {
            int k = 1;
            int k1 = 1;
            int k2 = 0;
            int k3 = 1;
            int k4 = 0;
            int zz = 1;//fff
            int x = 0;
            int y = 0;
            int a = 1;
            short[] mass = new short[64];
            mass[0] = (matrix[0, 0]);
            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0) { x += k1; y += k2; }
                else { y += k3; x += k4; }
                mass[a] = (matrix[y, x]);
                a++;
                for (int j = 0; j < zz; j++)
                {
                    if (i % 2 == 0)
                    {
                        x--; y++; mass[a] = (matrix[y, x]);
                    }
                    else { x++; y--; mass[a] = (matrix[y, x]); }
                    a++;
                }
                if (zz == 7) { k = -1; k1 = 0; k2 = 1; k3 = 0; k4 = 1; }
                zz += k;
            }
            mass[63] = (matrix[7, 7]);
            return (mass);
        }
        /// <summary>
        /// На вход подается массив 64. Возвращает матрицу 8 на 8, которая получена из массива путем его записи в матрицу по зигзагу
        /// </summary>
        public static short[,] ReZigzag(short[] mass)
        {
            int k = 1;
            int k1 = 1;
            int k2 = 0;
            int k3 = 1;
            int k4 = 0;
            int zz = 1;//fff
            int x = 0;
            int y = 0;
            int a = 1;
            short[,] matrix = new short[8, 8];
            (matrix[0, 0]) = mass[0];
            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0) { x += k1; y += k2; }
                else { y += k3; x += k4; }
                (matrix[y, x]) = mass[a];
                a++;
                for (int j = 0; j < zz; j++)
                {
                    if (i % 2 == 0)
                    {
                        x--; y++; matrix[y, x] = mass[a];
                    }
                    else { x++; y--; matrix[y, x] = mass[a]; }
                    a++;
                }
                if (zz == 7) { k = -1; k1 = 0; k2 = 1; k3 = 0; k4 = 1; }
                zz += k;
            }
            matrix[7, 7] = mass[63];
            return (matrix);
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
