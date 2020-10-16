using System;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Часть изображения, содержит один канал исходного изображения.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Массив, который хранит данные блока.
        /// </summary>
        private byte[,] matrix;

        /// <summary>
        /// Фактор разбиения по ширине.
        /// </summary>
        private int h;
        /// <summary>
        /// Фактор разбиения по высоте.
        /// </summary>
        private int v;

        /// <summary>
        /// Создает блок на основе заданной матрицы.
        /// </summary>
        /// <param name="matrix">Заданная матрица для создания канала.</param>
        /// <param name="H">Фактор разбиения по ширине.</param>
        /// <param name="V">Фактор разбиения по высоте.</param>
        public Channel(byte[,] matrix, int H, int V)
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
        /// Возвращает фактор разбиения по ширине.
        /// </summary>
        public int GetH => h;
        /// <summary>
        /// Возвращает фактор разбиения по высоте.
        /// </summary>
        public int GetV => v;

        public void Sample(int Hmax, int Vmax)
        {
            if (Hmax < h || Vmax < v) return;

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
            else
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
            if (Hmax < h || Vmax < v) return;
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
            else
            {
                for (int x = 0; x < width; x++)
                {
                    int x0 = (int)(x * hProportion);
                    int x1 = x0 + 1;
                    if (x1 > matrix.GetLength(0)) x1--;
                    else if (x1 == matrix.GetLength(0))
                    {
                        x1--; x0--;
                    }
                    for (int y = 0; y < tempMatrix.GetLength(1); y++)
                    {
                        tempMatrix[x, y] = Lerp(x,
                            (int)(x0 / hProportion), matrix[x0, y],
                            (int)(x1 / hProportion), matrix[x1, y]);
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
                    int y0 = (int)(y * vProportion);
                    int y1 = y0 + 1;
                    if (y1 > matrix.GetLength(1)) y1--;
                    else if (y1 == matrix.GetLength(1))
                    {
                        y1--; y0--;
                    }
                    for (int x = 0; x < width; x++)
                    {
                        scaledMatrix[x, y] = Lerp(y,
                            (int)(y0 / vProportion), tempMatrix[x, y0],
                            (int)(y1 / vProportion), tempMatrix[x, y1]);
                    }
                }
            }
            this.matrix = scaledMatrix;
        }

        /// <summary>
        /// Линейная интерполяция байтовых значений.
        /// </summary>
        static private byte Lerp(int x, int x0, byte y0, int x1, byte y1)
        {
            float res = y0 + (float)(y1 - y0) / (x1 - x0) * (x - x0);
            if (res > 255) return 255;
            else if (res < 0) return 0;
            else return (byte)res;
        }
    }
}
