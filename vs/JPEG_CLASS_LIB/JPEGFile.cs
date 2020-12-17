﻿using System.Collections.Generic;
using System.IO;

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
        List<JPEGData> Data = new List<JPEGData> { };

        /// <summary>
        /// Кадр
        /// </summary>
        Frame frame;

        /// <summary>
        /// Все таблицы квантования
        /// </summary>
        List<QuantizationTable> quantizationTables = new List<QuantizationTable> { };

        /// <summary>
        /// Все таблицы Хаффмана
        /// </summary>
        List<HuffmanTable> huffmanTables = new List<HuffmanTable> { };

        /// <summary>
        /// Заголовок кодированных данных
        /// </summary>
        Scan scan;

        /// <summary>
        /// Класс декодирования энтропийных данных.
        /// </summary>
        Decoding decoding;

        /// <summary>
        /// Интервал повтора - количество MCU - минимальных кодированных блоков. 
        /// </summary>
        RestartInterval restartInterval;

        /// <summary>
        /// Конструктор JPEGFile. Считывает все структуры JPEGData и записывает их в Data.
        /// </summary>
        /// <param name="s">Поток с изображением.</param>
        public JPEGFile(Stream s)
        {
            do
            {
                long position = s.Position;
                var temp = JPEGData.GetData(s);
                s.Position = position + temp.Length + 2;
                if (temp.Marker == MarkerType.DefineHuffmanTables) huffmanTables.Add((HuffmanTable)temp);
                else if (temp.Marker >= MarkerType.BaseLineDCT && temp.Marker <= MarkerType.DifferentialLoslessArithmetic) frame = (Frame)temp;
                else if (temp.Marker == MarkerType.DefineQuantizationTables) quantizationTables.Add((QuantizationTable)temp);
                else if (temp.Marker == MarkerType.StartOfScan) scan = (Scan)temp;
                else if (temp.Marker == MarkerType.DefineRestartInterval) restartInterval = (RestartInterval)temp;
                Data.Add(temp);
            }
            while (Data[Data.Count - 1].Marker != MarkerType.StartOfScan);
            decoding = new Decoding(s, null, null);
        }

        /// <summary>
        /// Ищет таблицу Хаффмана с заданными параметрами
        /// </summary>
        /// <param name="cl">Класс таблицы (0 - DC, 1 - AC)</param>
        /// <param name="num">Номер таблицы Хаффмана</param>
        /// <returns>Структура фрейма</returns>
        public HuffmanTable GetHuffmanTable(byte cl, byte num)
        {
            HuffmanTable result = null;
            for (int i = 0; i < huffmanTables.Count; i++)
                if (cl == huffmanTables[i].Tc && num == huffmanTables[i].Th)
                    result = huffmanTables[i];
            return result;
        }

        /// <summary>
        /// Находит структуру таблицы квантования
        /// </summary>
        /// <param name="num">Номер таблицы квантования</param>
        /// <returns>матрица квантования</returns>
        public short[,] GetQuantizationTable(int num)
        {
            short[,] result = new short[8, 8];
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    result[y, x] = quantizationTables[num].QuantizationTableMain[x + 8 * y];
            return result;
        }

        /// <summary>
        /// Возращает интервал повтора (количество MCU - минимальных кодированных блоков)
        /// </summary>
        /// <returns>интервал повтора</returns>
        public int GetRestartInterval()
        {
            if (restartInterval != null)
                return restartInterval.restartInterval;
            else
                return 0;
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
            foreach (JPEGData d in Data)
            {
                d.Print();
            }
        }

        public void PrintData()
        {
            frame.Print();
            foreach (JPEGData d in quantizationTables) d.Print();
            foreach (JPEGData d in huffmanTables) d.Print();
            scan.Print();
        }
    }
}
