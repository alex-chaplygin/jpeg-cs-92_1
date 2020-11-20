using System;
using System.Collections.Generic;

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
        /// Разбивает внутреннюю матрицу канала на блоки 8x8
        /// </summary>
        /// <returns>Список блоков матрицы канала</returns>
        public List<byte[,]> Split()
        {
    	    
            var BLOCK_SIZE = 8;

            var width = matrix.GetLength(0);
            var height = matrix.GetLength(1);

            var correctedWidth = width % BLOCK_SIZE == 0 ? width : BLOCK_SIZE*(width / BLOCK_SIZE + 1);
            var correctedHeight = height % BLOCK_SIZE == 0 ? height : BLOCK_SIZE*(height / BLOCK_SIZE + 1);
	    
            var correctedMatrix = new byte[correctedWidth, correctedHeight];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    correctedMatrix[i, j] = matrix[i, j];
                }
            }

            var arr = new byte[correctedWidth * correctedHeight];
            
            for (var i = 0; i < correctedHeight; i++)
            {
                for (var j = 0; j < correctedWidth; j++)
                {
                    arr[i*correctedWidth+j] = correctedMatrix[j, i];
                }
            }
            
            var splitResultList = new List<byte[,]>();
    
            var blockCount = arr.Length / (BLOCK_SIZE*BLOCK_SIZE);
            var blockInRow = correctedWidth / BLOCK_SIZE;
            for (var blockIndex = 0; blockIndex < blockCount; blockIndex++)
            {
                splitResultList.Add(new byte[BLOCK_SIZE,BLOCK_SIZE]);
                var startIndex = (blockIndex/blockInRow)*blockInRow*BLOCK_SIZE*BLOCK_SIZE+(blockIndex%blockInRow)*BLOCK_SIZE;
                var innerIndex = 0;
                for (var row = 0; row < BLOCK_SIZE; row++)
                {
                    for (var column = 0; column<BLOCK_SIZE; column++) {
                        var realIndex = startIndex + row * correctedWidth + column;
                        splitResultList[blockIndex][innerIndex/BLOCK_SIZE,innerIndex%BLOCK_SIZE] = arr[realIndex];
                        innerIndex++;
                    }
                }
            }
            return splitResultList;
        }
        
        /// <summary>
        /// Соединяет список блоков 8x8 в матрицу канала. Заменяет матрицу канала. 
        /// </summary>
        /// <param name="list">Блоки матрицы канала</param>
        public void Collect(List<byte[,]> list)
        {
            if (list.Count==0) throw new Exception("Empty blocks!");
            var width = matrix.GetLength(0);
            var height = matrix.GetLength(1);
            
            matrix = new byte[width,height];

            var BLOCK_SIZE = 8;
            var correctedWidth = width % BLOCK_SIZE == 0 ? width : BLOCK_SIZE*(width / BLOCK_SIZE + 1);
            var blockInRow = correctedWidth / BLOCK_SIZE;

            for (var k = 0; k < list.Count; k++)
            {
                var initialX = (k % blockInRow)*BLOCK_SIZE;
                var initialY = k / blockInRow * BLOCK_SIZE;

                    for (var j = 0; j < BLOCK_SIZE; j++)
                    {
                        for (var i = 0; i < BLOCK_SIZE; i++)
                        {
                        if (initialX+i>=width || initialY + j>=height) continue;
                        matrix[initialX + i, initialY + j] = list[k][j, i];
                    }
                }
            }
        }


        /// <summary>
        /// Возвращает фактор разбиения по ширине.
        /// </summary>
        public int GetH => h;
        /// <summary>
        /// Возвращает фактор разбиения по высоте.
        /// </summary>
        public int GetV => v;

        /// <summary>
        /// Преобразует исходную матрицу в новую, изменяя ширину и высоту. Значения матрицы интерполируются. Новая ширина матрицы равна текущая ширина разделить на Hmax/H. Новая высота матрицы равна текущая высота разделить на Vmax/V. Например, если H = 4, Hmax=4, то ширина не изменяется. H = 2, Hmax = 4, ширина уменьшается в 2 раза.
        /// </summary>
        /// <param name="Hmax">Максимальное значение из всех значений H всех каналов</param>
        /// <param name="Vmax">Максимальное значение из всех значений V всех каналов</param>
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

        /// <summary>
        /// Преобразует исходную матрицу в новую, изменяя ширину и высоту.
        /// Значения матрицы линейно интерполируются.
        /// </summary>
        /// <param name="Hmax">Коэффициент Hmax</param>
        /// <param name="Vmax">Коэффициент Vmax</param>
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
