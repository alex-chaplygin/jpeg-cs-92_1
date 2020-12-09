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
        /// Выводит в консоль все JPEGData
        /// </summary>
        public void Print()
        {
            foreach(JPEGData d in Data)
            {
                d.Print();
            }
        }

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
    }
}
