using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс JPEGFile.
    /// </summary>
    public class JPEGFile
    {
        static readonly short[,] LQT =
{
            {16, 12, 14, 14, 18, 24, 49, 72},
            {11, 12, 13, 17, 22, 35, 64, 92},
            {10, 14, 16, 22, 37, 55, 78, 95},
            {16, 19, 24, 29, 56, 64, 87, 98},
            {24, 26, 40, 51, 68, 81, 103, 112},
            {40, 58, 57, 87, 109, 104, 121, 100},
            {51, 60, 69, 80, 103, 113, 120, 103},
            {61, 55, 56, 62, 77, 92, 101, 99}
        };

        /// <summary>
        /// Максимальынй фактор разбиения по ширине.
        /// </summary>
        private int Hmax;
        /// <summary>
        /// Максимальный фактор разбиения по высоте.
        /// </summary>
        private int Vmax;

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

            Hmax = 0;
            Vmax = 0;
            foreach (var c in frame.Components)
            {
                if (c.H > Hmax) Hmax = c.H;
                if (c.V > Vmax) Vmax = c.V;
            }
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

            while (width / Hmax % 8 != 0)
            {
                width++;
            }
            while (height / Vmax % 8 != 0)
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
            var decodeScan = DecodeScan();

            for (int i = 0; i < 3; i++)
            {
                byte[,] tempMatrix = new byte[frame.Width, frame.Height];
                channeles[i] = new Channel(tempMatrix, frame.Components[i].H, frame.Components[i].V);
            }
            Collect(channeles, decodeScan);
            return channeles;
        }

        /// <summary>
        /// Собирает списки каждого канала, далее к ним применяет IDCT преобразование, затем собирает блоки 8x8 в каналы и выполняет обратное масштабирование матриц каналов. Если канал один, то все блоки записываются в канал слева-направо, сверху вниз.
        /// </summary>
        /// <param name="channels">Каналы с пустыми матрицами, но с корректными шириной, высотой и значениями H и V</param>
        /// <param name="blocks">Список short[] перемешанных блоков</param>
        public void Collect(Channel[] channels, List<short[]> blocks)
        {
            var BLOCK_SIZE = 8;
            var macroBlockCount = 0;
            foreach (var channel in channels)
            {
                macroBlockCount += channel.GetH * channel.GetV;
            }

            for (var channelIndex = 0; channelIndex < channels.Length; channelIndex++)
            {
                var curChannel = channels[channelIndex];

                var realWidth = curChannel.GetMatrix().GetLength(0);
                var realHeight = curChannel.GetMatrix().GetLength(1);
                var correctedWidth = realWidth % BLOCK_SIZE == 0 ? realWidth : BLOCK_SIZE * (realWidth / BLOCK_SIZE + 1);
                var correctedHeight = realHeight % BLOCK_SIZE == 0 ? realHeight : BLOCK_SIZE * (realHeight / BLOCK_SIZE + 1);

                var tmpArray = new short[(correctedWidth * correctedHeight) / BLOCK_SIZE / BLOCK_SIZE][];

                var otherChannelOffset = 0;
                for (var offsetChannelIndex = 0; offsetChannelIndex < channelIndex; offsetChannelIndex++)
                {
                    otherChannelOffset += channels[offsetChannelIndex].GetH * channels[offsetChannelIndex].GetV;
                }

                for (var blockIndex = 0; blockIndex < correctedWidth * correctedHeight / BLOCK_SIZE / BLOCK_SIZE / (curChannel.GetH * curChannel.GetV); blockIndex++)
                {
                    var channelBlockInRow = correctedWidth / BLOCK_SIZE / curChannel.GetH;
                    var startIndex = (blockIndex / channelBlockInRow * curChannel.GetV) * (correctedWidth / BLOCK_SIZE) + ((blockIndex % channelBlockInRow) * curChannel.GetH);

                    var innerBlocksGroup =
                        blocks.GetRange(macroBlockCount * blockIndex + otherChannelOffset, curChannel.GetH * curChannel.GetV);
                    for (var lineIndex = 0; lineIndex < curChannel.GetV; lineIndex += 1)
                    {
                        for (var rowIndex = 0; rowIndex < curChannel.GetH; rowIndex++)
                        {
                            tmpArray[startIndex + lineIndex * (correctedWidth / BLOCK_SIZE) + rowIndex] =
                                innerBlocksGroup[0];
                            innerBlocksGroup.RemoveAt(0);
                        }
                    }
                }
                curChannel.Collect(IDCT(tmpArray.ToList(), channelIndex), Hmax, Vmax);
            }
        }

        /// <summary>
        /// Осуществляет все необходимые обратные DCT преобразования для списка блоков
        /// </summary>
        /// <param name="data">Список коэффициентов блоков одного канала</param>
        /// <param name="Index">Индекс матрицы квантованияя</param>
        /// <returns>Список блоков одного канала для сборки</returns>
        public List<byte[,]> IDCT(List<short[]> data, int Index) //, short[,] quantizationMatrix
        {
            short[,] quantizationMatrix = GetQuantizationTable(frame.Components[Index].QuantizationTableNumber);
            List<byte[,]> Result = new List<byte[,]> { };
            List<short[,]> Temp = new List<short[,]> { };
            for (int i = 0; i < data.Count; i++)
                Temp.Add(DCT.ReZigzag(data[i]));
            for (int i = 0; i < Temp.Count; i++)
            {
                Temp[i] = DCT.QuantizationReverse(Temp[i], quantizationMatrix);
                Temp[i] = DCT.IDCT(Temp[i]);
                Result.Add(DCT.ReverseShift(Temp[i]));
            }
            return Result;
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
