using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс JPEGFile.
    /// </summary>
    public class JPEGFile
    {
        /// <summary>
        /// Список всех структур JPEG до StartOfScan.
        /// </summary>
        List<JPEGData> Data = new List<JPEGData>{};

        /// <summary>
        /// Конструктор JPEGFile. Считывает все структуры JPEGData и записывает их в Data.
        /// </summary>
        /// <param name="s">Поток с изображением.</param>
        public JPEGFile(Stream s)
        {
            long position = 0;
            do
            {
                position = s.Position;
                JPEGData temp = JPEGData.GetData(s);
                s.Position = position + temp.Length + 2;
                Data.Add(temp);
            }
            while (Data[Data.Count - 1].Marker != MarkerType.StartOfScan);
        }

        /// <summary>
        /// Декодирование кадра JPEG
        /// </summary>
        /// <returns>Массив масштабированных каналов изображения</returns>
        public Channel[] DecodeFrame()
        {
            Channel[] channeles = new Channel[3];
            for (int i = 0; i < 3; i++)
            {
                byte[,] matx = new byte[200, 100];
                //заполнение матрицы
                for (int a = 0; a < 200; a++)
                {
                    for (int b = 0; b < 100; b++)
                    {
                        if (a < 100 && b < 50) matx[a, b] = 50;
                        if (a > 100 && b < 50) matx[a, b] = 100;
                        if (a < 100 && b > 50) matx[a, b] = 150;
                        if (a > 100 && b > 50) matx[a, b] = 200;
                    }
                }

                Channel temp = new Channel(matx, 200, 100);
                channeles[i] = temp;
            }
            return channeles;
        }

        /// <summary>
        /// Выводит в консоль все JPEGData
        /// </summary>
        public void Print()
        {
            foreach(JPEGData d in Data)
            {
                d.Print();
            }
        }
    }
}
