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
        /// Высота блока.
        /// </summary>
        private int h;
        /// <summary>
        /// Ширина блока.
        /// </summary>
        private int v;

        /// <summary>
        /// Создает блок на основе заданной матрицы.
        /// </summary>
        /// <param name="matrix">Заданная матрица для создания блока.</param>
        /// <param name="H">Ширина блока.</param>
        /// <param name="V">Высота блока.</param>
        public Block(byte[,] matrix, int H, int V)
        {
            this.matrix = matrix;
            this.h = H;
            this.v = V;
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
        /// Возвращает ширину блока.
        /// </summary>
        public int GetH => h;
        /// <summary>
        /// Возвращает высоту блока.
        /// </summary>
        public int GetV => v;

        public void Sample(int Hmax, int Vmax)
        {
            float hProportion = (float)h / Hmax;
            float vProportion = (float)v / Vmax;

            int width = (int)(matrix.GetLength(0) / hProportion);
            if (width == 0) width = 1;
            int height = (int)(matrix.GetLength(1) / vProportion);
            if (height == 0) height = 1;


            // Масштабирование по ширине.
            byte[,] tempMatrix = new byte[width, matrix.GetLength(1)];
            if (hProportion == 1)
            {
                tempMatrix = matrix;
            }
            else //Кусочно-постоянная интерполяция.
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < tempMatrix.GetLength(1); y++)
                    {
                        tempMatrix[x, y] = matrix[(int)(x * hProportion), y];
                    }
                }
            }

            // Масштабирование по высоте.
            byte[,] scaledMatrix = new byte[width, height];
            if (vProportion == 1)
            {
                scaledMatrix = tempMatrix;
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        scaledMatrix[x, y] = tempMatrix[x, (int)(y * vProportion)];
                    }
                }
            }
            this.matrix = scaledMatrix;
        }

        public void Resample(int Hmax, int Vmax)
        {
            float hProportion = Hmax / (float)h;
            float vProportion = Vmax / (float)v;

            int width = (int)(matrix.GetLength(0) / hProportion);
            if (width == 0) width = 1;
            int height = (int)(matrix.GetLength(1) / vProportion);
            if (height == 0) height = 1;


            // Масштабирование по ширине.
            byte[,] tempMatrix = new byte[width, matrix.GetLength(1)];
            if (hProportion == 1)
            {
                tempMatrix = matrix;
            }
            else //Кусочно-постоянная интерполяция.
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < tempMatrix.GetLength(1); y++)
                    {
                        tempMatrix[x, y] = matrix[(int)(x * hProportion), y];
                    }
                }
            }

            // Масштабирование по высоте.
            byte[,] scaledMatrix = new byte[width, height];
            if (vProportion == 1)
            {
                scaledMatrix = tempMatrix;
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        scaledMatrix[x, y] = tempMatrix[x, (int)(y * vProportion)];
                    }
                }
            }
            this.matrix = scaledMatrix;
        }
    }
}
