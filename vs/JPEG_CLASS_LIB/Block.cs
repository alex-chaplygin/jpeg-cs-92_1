using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Часть изображения, содержит один канал исходного изображения.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Массив, который хранит данные блока.
        /// </summary>
        private byte[,] matrix;

        /// <summary>
        /// Создает блок на основе заданной матрицы.
        /// </summary>
        /// <param name="matrix">Заданная матрица для создания блока.</param>
        public Block(byte[,] matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        /// Возвращает текущую матрицу.
        /// </summary>
        /// <returns>Текущая матрица.</returns>
        public byte[,] GetMatrix()
        {
            return matrix;
        }

        /// <summary>
        /// Преобразует исходную матрицу в новую, изменяя ширину и высоту.
        /// Значения матрицы интерполируются.
        /// Ширина будет увеличина в (H/Hmax) раз.
        /// Высота будет увеличина в (V/Vmax) раз.
        /// </summary>
        /// <returns>Масштабированная матрица.</returns>
        public byte[,] Scaling(int H, int Hmax, int V, int Vmax)
        {
            float hProportion = (float)H / Hmax;
            float vProportion = (float)V / Vmax;

            int width = (int)(matrix.GetLength(1) * hProportion);
            if (width == 0) width = 1;
            int height = (int)(matrix.GetLength(0) * vProportion);
            if (height == 0) height = 1;

            // Масштабирование по ширине.
            byte[,] tempMatrix = new byte[matrix.GetLength(0), width];
            if (hProportion == 1)
            {
                tempMatrix = matrix;
            }
            else if (hProportion < 1) // Прореживание.
            {
                int hThinning = Hmax / H;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < tempMatrix.GetLength(0); y++)
                    {
                        tempMatrix[y, x] = matrix[y, x * hThinning];
                    }
                }
            }
            else //Кусочно-постоянная интерполяция.
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < tempMatrix.GetLength(0); y++)
                    {
                        tempMatrix[y, x] = matrix[y, (int)(x / hProportion)];
                    }
                }
            }

            // Масштабирование по высоте.
            byte[,] scaledMatrix = new byte[height, width];
            if (vProportion == 1)
            {
                scaledMatrix = tempMatrix;
            }
            else if (vProportion < 1) // Прореживание.
            {
                int vThinning = Vmax / V;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        scaledMatrix[y, x] = tempMatrix[y * vThinning, x];
                    }
                }
            }
            else //Кусочно-постоянная интерполяция.
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        scaledMatrix[y, x] = tempMatrix[(int)(y / vProportion), x];
                    }
                }
            }
            return scaledMatrix;
        }

    }
}
