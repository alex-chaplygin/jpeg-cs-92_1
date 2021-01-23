using System;
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
        public Frame frame;

        /// <summary>
        /// Все таблицы квантования
        /// </summary>
        List<QuantizationTable> quantizationTables = new List<QuantizationTable> { };

        /// <summary>
        /// Все таблицы Хаффмана
        /// </summary>
        public List<HuffmanTable> huffmanTables = new List<HuffmanTable> { };

        /// <summary>
        /// Заголовок кодированных данных
        /// </summary>
        Scan scan;

        /// <summary>
        /// Класс кодирования данных.
        /// </summary>
        public Encoding encoding;

        /// <summary>
        /// Класс декодирования энтропийных данных.
        /// </summary>
        Decoding decoding;

        Stream MainStream;

        /// <summary>
        /// Интервал повтора - количество MCU - минимальных кодированных блоков. 
        /// </summary>
        RestartInterval restartInterval;

        /// <summary>
        /// Предыдущее значение DC коэффициента.
        /// </summary>
        short prediction = 0;

        /// <summary>
        /// количества вызовов EncodeRestartInterval.
        /// </summary>
        int restart_interval_num;

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
            encoding = new Encoding(s, null, null);
        }

        /// <summary>
        /// Кодирует интервал повтора
        /// </summary>
        /// <param name="list">Блоки MCU</param>
        public void EncodeRestartInterval(List<short[]> list)
        {
            int i = GetRestartInterval();
            while (i != 0)
            {
                encoding.EncodeBlock(list[i]);
                i--;
            }
            switch (restart_interval_num % 8)
			{
                case 0:
                    JPEGData jpgD0 = new JPEGData(MarkerType.RestartWithModEightCount0);
                    break;
                case 1:
                    JPEGData jpgD1 = new JPEGData(MarkerType.RestartWithModEightCount1);
                    break;
                case 2:
                    JPEGData jpgD2 = new JPEGData(MarkerType.RestartWithModEightCount2);
                    break;
                case 3:
                    JPEGData jpgD3 = new JPEGData(MarkerType.RestartWithModEightCount3);
                    break;
                case 4:
                    JPEGData jpgD4 = new JPEGData(MarkerType.RestartWithModEightCount4);
                    break;
                case 5:
                    JPEGData jpgD5 = new JPEGData(MarkerType.RestartWithModEightCount5);
                    break;
                case 6:
                    JPEGData jpgD6 = new JPEGData(MarkerType.RestartWithModEightCount6);
                    break;
                case 7:
                    JPEGData jpgD7 = new JPEGData(MarkerType.RestartWithModEightCount7);
                    break;
			}

            restart_interval_num++;
        }

        /// <summary>
        /// Кодирует минимальный блок кодирования.
        /// </summary>
        /// <param name="list">Блоки MCU</param>
        public void EncodeMCU(List<short[]> list)
        {
            for (byte i = 0; i < frame.NumberOfComponent; i++)
            {
                encoding.huffDC = GetHuffmanTable(0, (byte)(i == 0 ? 0 : 1));
                encoding.huffAC = GetHuffmanTable(1, (byte)(i == 0 ? 0 : 1));
                byte NumBlocks = (byte)(frame.Components[i].H * frame.Components[i].V);
                while (NumBlocks != 0)
                {
                    list[0][0] -= prediction;
                    prediction = list[0][0];
                    encoding.EncodeBlock(list[0]);
                    list.RemoveAt(0);
                    NumBlocks--;
                }
            }
        }

        /// <summary>
        /// Декодирование минимального блока кодирования.
        /// </summary>
        /// <returns>MCU</returns>
        public List<short[]> DecodeMCU()
        {
            List<short[]> result = new List<short[]>();
            for (byte i = 0; i < frame.NumberOfComponent; i++)
            {
                decoding.huffDC = GetHuffmanTable(0, scan.components[i].TableDC);
                decoding.huffAC = GetHuffmanTable(1, scan.components[i].TableAC);
                byte NumBlocks = (byte)(frame.Components[i].H * frame.Components[i].V);
                while (NumBlocks != 0)
                {
                    result.Add(decoding.DecodeBlock());
                    NumBlocks--;
                }
            }
            return result;
        }
        /// <summary>
        /// Декодирование скана
        /// </summary>
        /// <returns>Список перемешанных блоков</returns>
        public List<short[]> DecodeScan()
        {
            List<short[]> MixedBlocks = new List<short[]>();
            int Hmax = 0;
            int Vmax = 0;
            
            foreach(var i in frame.Components)
            {
                if (i.H > Hmax) Hmax = i.H;
                if (i.V > Vmax) Vmax = i.V;
            }

            int width = frame.Width;
            int height = frame.Height;

            while (width/Hmax%8!=0)
            {
                width++;
            }
            while (height/Vmax%8!=0)
            {
                height++;
            }

            int iterations = width * height/8/8 / (Hmax * Vmax);

            int iii = 0;
            
                while (iii < iterations)
                {
                    if (GetRestartInterval() == 0)
                    {
                        MixedBlocks.AddRange(DecodeMCU());
                        iii++;
                    }
                    else
                    {
                        List<short[]> RestartI = DecodeRestartInterval();
                        if (RestartI == null) break;
                        MixedBlocks.AddRange(RestartI);
                        iii += GetRestartInterval();

                    }
                }
            
            
            return (MixedBlocks);
        }
        List<short[]> DecodeRestartInterval()
            {
            List<short[]> list = new List<short[]>();
            for (int i =0; i<6; i++)
            {
                list.Add(new short[64] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, });
            }
            return (list);
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
            for (int k = 0; k < quantizationTables.Count; k++)
                if (quantizationTables[k].Tq == num)
                {
                    short[,] result = new short[8, 8];
                    short[] actual = new short[64];
                    for (int i = 0; i < 64; i++)
                        actual[i] = quantizationTables[k].QuantizationTableMain[i];
                    result = DCT.ReZigzag(actual);
                    return result;
                }
            return null;
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
        /// Конструктор для создания JPEG файла.
        /// </summary>
        public JPEGFile()
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
