using System.Collections.Generic;
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
        public List<JPEGData> Data = new List<JPEGData> { };

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

        Stream MainStream;

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
                var temp = JPEGData.GetData(s);
                s.Position = position + temp.Length + 2;
                if (temp.Marker == MarkerType.DefineHuffmanTables) huffmanTables.Add((HuffmanTable)temp);
                else if (temp.Marker >= MarkerType.BaseLineDCT && temp.Marker <= MarkerType.DifferentialLoslessArithmetic) frame = (Frame)temp;
                else if (temp.Marker == MarkerType.DefineQuantizationTables) quantizationTables.Add((QuantizationTable)temp);
                else if (temp.Marker == MarkerType.StartOfScan) scan = (Scan)temp;
                Data.Add(temp);
            }
            while (Data[Data.Count - 1].Marker != MarkerType.StartOfScan);
            decoding = new Decoding(s, null, null);
        }

        /// <summary>
        /// Конструктор для создания JPEG файла.
        /// </summary>
        JPEGFile()
        {

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
        /// Запоминает поток. Записывает все структуры из списка Data в поток.
        /// </summary>
        /// <param name="s">Поток, в который записываются структуры из списка Data.</param>
        public void WriteHeaders(Stream s)
        {
            MainStream = s;
            for (int i = 0; i < Data.Count; i++)
            {
                Data[i].MainStream = MainStream;
                Data[i].Write();
            }
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
