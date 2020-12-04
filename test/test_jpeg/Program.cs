using JPEG_CLASS_LIB;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //_TestUnpack();
            //_TestInterleave();            
            //_TestJPEGData();
            //_TestBitReader();
            //_TestBitWriter();            
            //_TestEncoding();
            //_TestImageConverter();
            //_TestJPEGFile();
            //_TestBitWriterTwo();
            //_TestBitWriterError();
            //_TestDecodingExtend();
            //_TestImageConverter();
            //_TestHuffmanTable();
            //_TestDCTcoding();
            //_TestEncodingWriteBits();
            //_TestDecodingDCAC();
            Console.ReadKey();
        }

        static void _TestDecodingDCAC()
        {
            short[] Block = new short[64];
            FileStream S = File.Open("../../../test.jpg", FileMode.Open);
            S.Seek(0x1f7, SeekOrigin.Begin);
            HuffmanTable huffDC = new HuffmanTable(S);
            S.Seek(0x218, SeekOrigin.Begin);
            HuffmanTable huffAC = new HuffmanTable(S);
            S.Seek(0x3b3, SeekOrigin.Begin);
            Decoding decoding = new Decoding(S, huffDC, huffAC);
            for (int k = 0; k < 10; k++)
            {
                decoding.huffDC = huffDC;
                decoding.huffDC.GenerateTables();
                Block[0] = decoding.DecodeDC();
                decoding.huffAC = huffAC;
                decoding.huffAC.GenerateTables();
                decoding.DecodeAC(Block);
                Console.WriteLine("Декодирование DC и AC");
                for (int i = 0, j = 1; i < 64; i++, j++)
                {
                    string s = Block[i].ToString();
                    while (s.Length < 5) s = " " + s;
                    Console.Write(s);
                    if (j == 8)
                    {
                        Console.WriteLine();
                        j = 0;
                    }
                }
            }
            Console.WriteLine();
            S.Dispose();
        }

        private static void _TestHuffmanTable()
        {
            // var width = 32;
            // var height = 32;
            // Console.WriteLine($"{width}*{height}");
            // var testMatrix = new byte[width, height];
            // var r = new Random();
            // for (var i = 0; i < height; i++)
            // {
            //     for (var j = 0; j < width; j++)
            //     {
            //         testMatrix[j, i] = (byte) r.Next(0, 255);
            //     }
            // }
            // short[,] CQT =
            // {
            //     {17, 18, 24, 47, 99, 99, 99, 99},
            //     {18, 21, 26, 66, 99, 99, 99, 99},
            //     {24, 26, 56, 99, 99, 99, 99, 99},
            //     {47, 66, 99, 99, 99, 99, 99, 99},
            //     {99, 99, 99, 99, 99, 99, 99, 99},
            //     {99, 99, 99, 99, 99, 99, 99, 99},
            //     {99, 99, 99, 99, 99, 99, 99, 99},
            //     {99, 99, 99, 99, 99, 99, 99, 99}
            // };
            // var channel = new Channel(testMatrix, 2, 2);
            // var blocks = channel.Split();
            // var data = new List<short[]>();
            // foreach (var block in blocks)
            // {
            //     data.Add(DCT.Zigzag(Quantization.QuantizationDirect(DCT.FDCT(DCT.Shift(block)), CQT)));
            // }
            // var result = new HuffmanTable(data, false, -1);

            var testData = new byte[10] { 1, 3, 4, 1, 1, 0, 4, 1, 2, 1 };
            // var r = new Random();
            // for (var i = 0; i < testData.Length; i++)
            // {
            //     testData[i] = (byte) r.Next(0, 255);
            // }

            var result = new HuffmanTable(testData, false, 0);
            result.Print();
        }

        private static void _TestBitWriter()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(2, 3);
            bitWriter.Write(4, 0);
            bitWriter.Write(2, 3);
            bitWriter.Write(12, 4095);
            bitWriter.Write(3, 0);
            bitWriter.Write(1, 1);

            foreach (var curByte in bitWriter.Get())
            {
                Console.Write(Convert.ToString(curByte, 2).PadLeft(8, '0') + " ");
            }
            Console.WriteLine();
        }

        private static void _TestBitWriterTwo()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(5, 200);
            bitWriter.Write(6, 300);
            bitWriter.Write(7, 810);
            bitWriter.Write(Convert.ToInt32(1.5), 570);//1.5=2
            bitWriter.Write(4, 60);
            bitWriter.Write(2, 2);

            foreach (var curByte in bitWriter.Get())
            {
                Console.Write(Convert.ToString(curByte, 2).PadLeft(8, '0') + " ");
            }
            Console.WriteLine();
        }

        private static void _TestBitWriterError()
        {
            var bitWriter = new BitWriter();
            bitWriter.Write(15, 200);
            bitWriter.Write(6, 300);
            bitWriter.Write(7, 810);
            bitWriter.Write(19, 570);
            bitWriter.Write(14, 60);
            bitWriter.Write(12, 2);

            foreach (var curByte in bitWriter.Get())
            {
                Console.Write(Convert.ToString(curByte, 2).PadLeft(8, '0') + " ");
            }
            Console.WriteLine();
        }

        private static void _TestBitReader()
        {
            var data = new byte[3] { 0x12, 0x34, 0x56 };
            var bitReader = new BitReader(data);
            Console.WriteLine(bitReader.Read(1));
            Console.WriteLine(bitReader.Read(2));
            Console.WriteLine(bitReader.Read(3));
            Console.WriteLine(bitReader.Read(4));
            Console.WriteLine(bitReader.Read(5).ToString("X"));
            Console.WriteLine(bitReader.Read(6).ToString("X"));
            Console.WriteLine(bitReader.Read(3));
            Console.WriteLine();
            bitReader = new BitReader(data);
            Console.WriteLine(bitReader.Read(8).ToString("X"));
            Console.WriteLine(bitReader.Read(16).ToString("X"));

        }

        private static void _TestInterleave()
        {
            byte[,] matrix = new byte[48, 32];
            for (int y = 0, c = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++, c++)
                {
                    matrix[x, y] = (byte)(c * 4);
                }
            }

            Channel channel1 = new Channel(matrix, 2, 2);
            channel1.Sample(2, 2);
            WriteMatrix(channel1.GetMatrix());
            Console.WriteLine();

            Channel channel2 = new Channel(matrix, 2, 1);
            channel2.Sample(2, 2);
            WriteMatrix(channel2.GetMatrix());
            Console.WriteLine();

            Channel channel3 = new Channel(matrix, 1, 1);
            channel3.Sample(2, 2);
            WriteMatrix(channel3.GetMatrix());
            Console.WriteLine();


            // Channel channel4 = new Channel(matrix, 1, 1);
            // channel4.Sample(2,2);
            // WriteMatrix(channel4.GetMatrix());
            // Console.WriteLine();



            var channels = new[] { channel1, channel2, channel3 };

            var library = new JPEG_CS(null);
            library.SetParameters(3);
            Console.WriteLine("Тест для трех каналов:");


            var blocks = library.Interleave(channels);

            Console.WriteLine("Сборка блоков");
            Console.WriteLine();

            library.Collect(channels, blocks);

            foreach (var channel in channels)
            {
                WriteMatrix(channel.GetMatrix());
                Console.WriteLine();
            }

            Console.WriteLine("Тест для одного канала:");
            Console.WriteLine();

            WriteMatrix(channels[0].GetMatrix());
            Console.WriteLine();

            channels = new[] { channel1 };

            blocks = library.Interleave(channels);

            Console.WriteLine("Сборка блоков");
            Console.WriteLine();

            library.Collect(channels, blocks);

            WriteMatrix(channels[0].GetMatrix());
            Console.WriteLine();




        }
        static void _TestUnpack()
        {
            var up = new JPEG_CS(File.Open("testUP.jpg", FileMode.Create));
            Point[,] изображение = up.UnPack();
        }
        static void _TestPack()
        {
            JPEG_CS p = new JPEG_CS(File.Open("testP.jpg", FileMode.Create));
            Point[,] points = new Point[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    points[i, j].r = 0;
                    points[i, j].g = 255;
                    points[i, j].b = 0;
                }
            }
            p.Compress(points);
            JPEG_CS jj = new JPEG_CS(File.Open("test.jpg", FileMode.Create));
            Point[,] изображение = jj.UnPack();

            изображение[0, 0].r = 255;
        }

        static void _TestDCTcoding()
        {
            Random r = new Random();
            JPEG_CS TestJCS = new JPEG_CS(new MemoryStream());
            TestJCS.SetParameters(2);
            byte[,] TestMatrix = new byte[16, 16];
            for (int y = 0, c = 0; y < TestMatrix.GetLength(1); y++)
                for (int x = 0; x < TestMatrix.GetLength(0); x++, c++)
                    TestMatrix[x, y] = Convert.ToByte(r.Next(0, 255));
            Channel TestChannel = new Channel(TestMatrix, 2, 2);
            List<byte[,]> list1 = TestChannel.Split();
            Console.WriteLine("Изначальная матрица");
            for (int i = 0; i < TestMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < TestMatrix.GetLength(1); j++)
                {
                    string S = TestMatrix[i, j].ToString();
                    while (S.Length < 3) S = "0" + S;
                    Console.Write(S + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            List<short[]> list2 = TestJCS.FDCT(list1);
            for (int k = 0; k < list2.Count; k++)
            {
                Console.WriteLine($"FDCT {k + 1}");
                for (int i = 0; i < list2[k].Length; i++)
                    Console.Write(list2[k][i] + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            List<byte[,]> list3 = TestJCS.IDCT(list2);
            for (int k = 0; k < list3.Count; k++)
            {
                Console.WriteLine($"IDCT {k + 1}");
                for (int i = 0; i < list3[k].GetLength(0); i++)
                {
                    for (int j = 0; j < list3[k].GetLength(0); j++)
                    {
                        string S = list3[k][j, i].ToString();
                        while (S.Length < 3) S = "0" + S;
                        Console.Write(S + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            TestChannel.Collect(list3);
            TestMatrix = TestChannel.GetMatrix();
            Console.WriteLine("Конечная матрица");
            for (int i = 0; i < TestMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < TestMatrix.GetLength(1); j++)
                {
                    string S = TestMatrix[i, j].ToString();
                    while (S.Length < 3) S = "0" + S;
                    Console.Write(S + " ");
                }
                Console.WriteLine();
            }
        }
        static void WriteMatrix(byte[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    Console.Write(matrix[x, y].ToString("X2") + " ");
                }
                Console.WriteLine();
            }
        }    

        static void _TestJPEGData()
        {
            FileStream s = File.Open("../../../JPEG_example_down.jpg", FileMode.Open);
            s.Seek(0x21d, SeekOrigin.Begin);
            JPEGData d = JPEGData.GetData(s);
            d.Print();

            s.Seek(862, SeekOrigin.Begin); // Чтение скана.
            d = JPEGData.GetData(s);
            d.Print();

            s.Seek(0x48D0, SeekOrigin.Begin);
            d = JPEGData.GetData(s);
            d.Print();

            s.Dispose();
        }

        static void _NextBitTest(Decoding D)
        {
            //Console.WriteLine("\r\nПозиция в потоке: " + s.Position.ToString("X"));
            try
            {
                int i = 0;
                do
                {
                    Console.Write(D.NextBit() + " ");
                    i++;
                    if (i == 16)
                    {
                        Console.WriteLine();
                        i = 0;
                    }
                }
                while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void _TestJPEGFile()
        {
            FileStream s = File.Open("../../../JPEG_example_down.jpg", FileMode.Open);
            JPEGFile f = new JPEGFile(s);
            f.Print();
            s.Seek(0x48eb, SeekOrigin.Begin);
            HuffmanTable huff = new HuffmanTable(s);
            huff.Print();
            // Тестирование метода Receive класса Decoding
            s.Seek(0x4a9b, SeekOrigin.Begin);
            /*Decoding decoding = new Decoding(s, huff);
            Console.WriteLine("\nТаблица MaxCode");
            foreach (int i in decoding.MaxCode)
            {
                Console.Write(Convert.ToString(i, 2) + " ");
            }
            Console.WriteLine("\nТаблица MinCode");
            foreach (int i in decoding.MinCode)
            {
                Console.Write(Convert.ToString(i, 2) + " ");
            }
            Console.WriteLine("\nТаблица VALPTR");
            foreach (byte i in decoding.VALPTR)
            {
                Console.Write(Convert.ToString(i, 2) + " ");
            }
            Console.WriteLine();
            Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());
            Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());
            Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());

            //s.Seek(0x360, SeekOrigin.Begin);
            //Console.WriteLine($"Тестирование метода Receive класса Decoding от позиции {s.Position:x4}");
            //for (byte i = 1; i <= 16; i++)
            //   Console.WriteLine($"Результат чтения следующих {i:d2} бит из потока: {Convert.ToString(decoding.Receive(i), 2)}");
            s.Dispose();*/
        }


        private static void _TestEncoding()
        {
            int numberOfBlock = 3;
            // Создаем numberOfBlock количество блоков [64] со случайными байтовыми значениями.
            Random random = new Random();
            List<short[]> listOfBlocks = new List<short[]>();
            for (int k = 0; k < numberOfBlock; k++)
            {
                listOfBlocks.Add(new short[64]);
                for (int i = 0; i < 64; i++)
                    listOfBlocks[k][i] = (short)random.Next(0, 255);
            }

            // Выводим содержимое блоков до применения метода Encoding.EncodeDC
            Console.WriteLine("Содержимое блоков до применения метода Encoding.EncodeDC");
            for (int k = 0; k < numberOfBlock; k++)
                Console.WriteLine($"Значения {k} блока: " + string.Join(" ", listOfBlocks[k]));

            // Применяем метод Encoding.EncodeDC
            Encoding.EncodeDC(listOfBlocks);

            // Выводим содержимое блоков после применения Encoding.EncodeDC
            Console.WriteLine("Содержимое блоков после применения Encoding.EncodeDC");
            for (int k = 0; k < numberOfBlock; k++)
                Console.WriteLine($"Значения {k} блока: " + string.Join(" ", listOfBlocks[k]));
        }
        
        private static void _TestDecodingExtend()
        {
            ushort diff = 10;
            int num_bits = 4;
            // Тестирование метода Receive класса Decoding.
            short result = Decoding.Extend(diff, num_bits);
            // Вывод результатов.
            Console.WriteLine("Тестирование метода Receive класса Decoding");
            Console.WriteLine($"Частичный код разницы DC: {diff} {Convert.ToString(diff, 2)}");
            Console.WriteLine($"Число бит для разницы: {num_bits}");
            Console.WriteLine($"Полный код: {result} {Convert.ToString(result, 2)}");
        }

        private static void _TestEncodingDCAC()
        {
            short[,] LQT = new short[,] {{ 16, 12, 14, 14, 18, 24, 49, 72 },
                                         { 11, 12, 13, 17, 22, 35, 64, 92},
                                         { 10, 14, 16, 22, 37, 55, 78, 95},
                                         { 16, 19, 24, 29, 56, 64, 87, 98},
                                         { 24, 26, 40, 51, 68, 81, 103, 112},
                                         { 40, 58, 57, 87, 109, 104, 121, 100},
                                         { 51, 60, 69, 80, 103, 113, 120, 103},
                                         { 61, 55, 56, 62, 77, 92, 101, 99} };
            Random random = new Random();
            List<byte[,]> tempB = new List<byte[,]>();
            List<short[,]> tempS = new List<short[,]>();
            List<short[]> data = new List<short[]>();
            byte[] result;

            //заполнение
            for (int i = 0; i < 3; i++)
            {
                byte[,] a = new byte[8, 8];
                for (int j = 0; j < 64; j++)
                {
                    a[j / 8, j % 8] = (byte)random.Next(0, 256);
                }
                tempB.Add(a);
            }

            for (int i = 0; i < 3; i++)
            {
                tempS.Add(JPEG_CLASS_LIB.DCT.Shift(tempB[i]));
                tempS[i] = JPEG_CLASS_LIB.DCT.FDCT(tempS[i]);
                tempS[i] = JPEG_CLASS_LIB.DCT.QuantizationDirect(tempS[i], LQT);
                for (int j = 0; j < 64; j++)
                {
                    tempB[i][j / 8, j % 8] = (byte)tempS[i][j / 8, j % 8];
                }
            }
            tempS = DCT.DCCalculating(tempS);
            for (int i = 0; i < 3; i++)
            {
                /*for (int j = 0; j < 64; j++)
                {
                    tempS[i][j / 8, j % 8] = (short)tempB[i][j / 8, j % 8];
                }*/
                data.Add(JPEG_CLASS_LIB.DCT.Zigzag(tempS[i]));
            }

            //до кодирования
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    //if (j != 0 && j % 7 == 0) Console.WriteLine();
                    if (j > 50) data[i][j] = 0;
                    if (j > 10 && j < 35) data[i][j] = 0;
                    Console.Write($"{data[i][j]} ");
                }
                Console.Write("\n\n");
            }

            result = JPEG_CLASS_LIB.Encoding.EncodeAC(data);

            //string values = BitConverter.ToString(result).Replace("-"," ");
            //string[] resultStr = values.Split();
            Console.Write("\n\n");
            for (int i = 0; i < result.Length; i++)
            {
                //if (i != 0 && i % 7 == 0) Console.Write("\n\n");
                Console.Write(Convert.ToString(result[i], 16) + " ");
            }
            Console.WriteLine();
        }

        private static void _TestEncodingWriteBits()
        {
            ushort[] bits =
            {
                0xa6ff, //0b1010_0110_1111_1111
                0x002e, //0b0000_0000_0010_1110,
                0x000b, //0b0000_0000_0000_1011,
                0x0206,//0b0000_0010_0000_0110,
                0x0015,//0b0000_0000_0001_0101,
            };
            int[] num =
            {
                16,
                6,
                4,
                10,
                5,
            };
            // Запись в поток с помощью метода Encoding.WriteBits
            FileStream s = File.Create("../../../testEncodingWriteBits");
            Encoding encoding = new Encoding(s);
            Console.WriteLine($"Использование метода Encoding.WriteBits");
            for (int i = 0; i < bits.Length; i++)
            {
                Console.Write($"Двоичное представление числа {bits[i]}: ");
                Console.WriteLine(Convert.ToString(bits[i], 2));
                encoding.WriteBits(bits[i], num[i]);
            }
            encoding.FinishBits();

            // Чтение записанных в поток данных
            s.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("Записанные в поток байты:");
            byte b;
            for (int i = 0; i < s.Length; i++)
            {
                b = (byte)s.ReadByte();
                Console.WriteLine($"{b:X2} {Convert.ToString(b, 2)}");
            }

            s.Dispose();
            File.Delete("../../../testEncodingWriteBits");
        }
    }
}
