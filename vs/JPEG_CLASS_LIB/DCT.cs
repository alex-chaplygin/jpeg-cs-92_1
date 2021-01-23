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
        public static short[,] FDCT(short[,] inp_matrix)
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
                    if (u == 0) Cu = 1 / Math.Sqrt(2);
                    if (v == 0) Cv = 1 / Math.Sqrt(2);
                    matrix2[v, u] = Convert.ToInt16(S * Cu * Cv / 4.0);
                }
            return (matrix2);
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
                            if (u == 0) Cu = 1 / Math.Sqrt(2);
                            if (v == 0) Cv = 1 / Math.Sqrt(2);
                            s += Cu * Cv * (matrix[v, u] * Math.Cos((2 * x + 1) * u * Math.PI / 16.0) * Math.Cos((2 * y + 1) * v * Math.PI / 16.0));
                        }
                    matrix2[y, x] = Convert.ToInt16(s / 4.0);
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
            int zz = 1;
            int x = 0;
            int y = 0;

            //int a = 1;
            short[] mass = new short[64];
            //mass[0] = (matrix[0, 0]);

            short[,] matrix_temp = new short[8, 8];
            int m = 0;
            int n = 1;
            matrix_temp[0, 0] = matrix[0, 0];

            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0)
                {
                    x += k1;
                    y += k2;
                }
                else
                {
                    y += k3;
                    x += k4;
                }
                //mass[a] = (matrix[y, x]);
                //a++;
                matrix_temp[n, m] = matrix[x, y];

                if (n == 7)
                {
                    n = 0;
                    m++;
                }
                else n++;

                for (int j = 0; j < zz; j++)
                {
                    if (i % 2 == 0)
                    {
                        x--;
                        y++;
                        //mass[a] = (matrix[y, x]);
                        matrix_temp[n, m] = matrix[x, y];
                    }
                    else
                    {
                        x++;
                        y--;
                        //mass[a] = (matrix[y, x]);
                        matrix_temp[n, m] = matrix[x, y];
                    }
                    //a++;

                    //n++;
                    if (n == 7)
                    {
                        n = 0;
                        m++;
                    }
                    else n++;
                }
                if (zz == 7)
                {
                    k = -1;
                    k1 = 0;
                    k2 = 1;
                    k3 = 0;
                    k4 = 1;
                }
                zz += k;
            }
            //mass[63] = (matrix[7, 7]);

            matrix_temp[7, 7] = matrix[7, 7];
            int a = 0;
            for (int w = 0; w < 8; w++)
            {
                for (int v = 0; v < 8; v++)
                {
                    mass[a] = matrix_temp[v, w];
                    a++;
                }
            }
            /*int k = 1;
            int k1 = 1;
            int k2 = 0;
            int k3 = 1;
            int k4 = 0;
            int zz = 1;
            int x = 0;
            int y = 0;

            short[] mass = new short[64];
            short[,] matrix_temp = new short[8, 8];
            int m = 0;
            int n = 1;
            matrix_temp[0, 0] = matrix[0, 0];

            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0)
                {
                    x += k1;
                    y += k2;
                }
                else
                {
                    y += k3;
                    x += k4;
                }
                matrix_temp[n,m] = matrix[x,y];
                if (n == 7)
                {
                    n = 0;
                    m++;
                }
                else n++;
                for (int j = 0; j < zz; j++)
                {
                    if (i % 2 == 0)
                    {
                        x--;
                        y++;
                        matrix_temp[n,m] = matrix[x, y];
                    }
                    else
                    {
                        x++;
                        y--;
                        matrix_temp[n,m] = matrix[x, y];
                    }
                    if (n == 7)
                    {
                        n = 0;
                        m++;
                    }
                    else n++;
                }
                if (zz == 7)
                {
                    k = -1;
                    k1 = 0;
                    k2 = 1;
                    k3 = 0;
                    k4 = 1;
                }
                zz += k;
            }
            matrix_temp[7, 7] = matrix[7, 7];
            
            int a = 0;
            for (int w = 0; w < 8; w++)
            {
                for (int v = 0; v < 8; v++)
                {
                    mass[a] = matrix_temp[v, w];
                    a++;
                }
            }*/
            //--------
            /*int k = 1;
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
            mass[63] = (matrix[7, 7]);*/
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
            short[,] matrix_temp = new short[8, 8];
            matrix_temp[0, 0] = mass[0];
            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0) { x += k1; y += k2; }
                else { y += k3; x += k4; }
                (matrix_temp[y, x]) = mass[a];
                a++;
                for (int j = 0; j < zz; j++)
                {
                    if (i % 2 == 0)
                    {
                        x--; y++; matrix_temp[y, x] = mass[a];
                    }
                    else { x++; y--; matrix_temp[y, x] = mass[a]; }
                    a++;
                }
                if (zz == 7) { k = -1; k1 = 0; k2 = 1; k3 = 0; k4 = 1; }
                zz += k;
            }
            matrix_temp[7, 7] = mass[63];
            for (int w = 0; w < 8; w++)
            {
                for (int v = 0; v < 8; v++)
                {
                    matrix[v, w] = matrix_temp[v, w];
                }
            }
            return (matrix);
        }

        /// <summary>
        /// Возвращает матрицу, каждый компонент которой приведен из диапазона 0..255 в диапазон -128..127
        /// </summary>
        public static short[,] Shift(byte[,] matrix)
        {
            short[,] mat = new short[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    short temp = (short)(matrix[i, j] - 128);
                    if (temp < -128) temp = -128;
                    else if (temp > 127) temp = 127;
                    mat[i, j] = temp;
                }
            return mat;
        }

        /// <summary>
        /// Возвращает матрицу, каждый компонент которой приведен из диапазона -128..127 в диапазон 0..255
        /// </summary>
        public static byte[,] ReverseShift(short[,] matrix)
        {
            byte[,] mat = new byte[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    short temp = (short)(matrix[i, j] + 128);
                    if (temp < 0) temp = 0;
                    else if (temp > 255) temp = 255;
                    mat[i, j] = (byte)temp;
                }
            return mat;
        }

        /// <summary>
        /// Реализует квантование матрицы коэффициентов с помощью заданной матрицы квантования.
        /// </summary>
        /// <param name="MatrixCoefficient">Исходная матрица коэффициентов</param>
        /// <param name="MatrixQuantization">Матрица квантования</param>
        /// <returns>Изменённая матрица коэффициентов</returns>
        public static short[,] QuantizationDirect(short[,] MatrixCoefficient, short[,] MatrixQuantization)
        {
            for (int i = 0; i < MatrixCoefficient.GetLength(0); i++)
            {
                for (int j = 0; j < MatrixCoefficient.GetLength(1); j++)
                {
                    double temp = MatrixCoefficient[i, j];
                    temp /= Convert.ToDouble(MatrixQuantization[i, j]);
                    MatrixCoefficient[i, j] = Convert.ToInt16(Math.Round(temp));
                }
            }
            return MatrixCoefficient;
        }

        /// <summary>
        /// Реализует обратное квантование матрицы коэффициентов с помощью заданной матрицы квантования
        /// </summary>
        /// <param name="MatrixCoefficient">Матрица коэффициентов, прошедшая квантование</param>
        /// <param name="MatrixQuantization">Матрица квантования</param>
        /// <returns>Исходная матрица коэффициентов</returns>
        public static short[,] QuantizationReverse(short[,] MatrixCoefficient, short[,] MatrixQuantization)
        {
            for (int i = 0; i < MatrixCoefficient.GetLength(0); i++)
            {
                for (int j = 0; j < MatrixCoefficient.GetLength(1); j++)
                {
                    MatrixCoefficient[i, j] *= MatrixQuantization[i, j];
                }
            }
            return MatrixCoefficient;
        }

        /// <summary>
        /// Заменяет первое значение в каждом блоке из списа на DC коэффициент
        /// </summary>
        /// <param name="blocks">Список исходных блоков</param>
        /// <returns>Список изменённых блоков</returns>
        static public List<short[,]> DCCalculating(List<short[,]> blocks)
        {
            for (int i = blocks.Count - 1; i > 0; i--)
            {
                blocks[i][0, 0] -= blocks[i - 1][0, 0];
            }
            return blocks;
        }

        /// <summary>
        /// Заменяет первое значение (DC-коэффициент) в каждом блоке на исходное значение
        /// </summary>
        /// <param name="blocks">Список блоков</param>
        /// <returns>Список исходных блоков</returns>
        static public List<short[,]> DCRestore(List<short[,]> blocks)
        {
            for (int i = 1; i < blocks.Count; i++)
            {
                blocks[i][0, 0] += blocks[i - 1][0, 0];
            }
            return blocks;
        }
    }
}