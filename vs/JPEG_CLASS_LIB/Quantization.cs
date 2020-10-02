using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс Quantization - Реализует прямое и обратное квантование марицы.
    /// </summary>1
    public class Quantization
    {
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
                    MatrixCoefficient[i, j] /= MatrixQuantization[i, j];
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
    }
}
