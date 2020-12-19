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
            TestCompleteChannel();
            //_TestUnpack();
            // _TestInterleave();            
            //_TestJPEGData();
            //_TestBitReader();
            //_TestBitWriter();            
            //_TestJPEGFile();
            //_TestBitWriterTwo();
            //_TestBitWriterError();
            //_TestHuffmanTable();
            // _TestEncodingWriteBits();
            //DecodeBlockTest();
            Console.ReadKey();
        }

        private static void TestCompleteChannel()
        {
            var width = 8;
            var height = 8;
            var r = new Random();
            byte[,] testMatrix = new byte[width, height];
            for (int y = 0; y < testMatrix.GetLength(1); y++)
                for (int x = 0; x < testMatrix.GetLength(0); x++)
                    testMatrix[x, y] = Convert.ToByte(r.Next(0, 255));
            var channel = new Channel(testMatrix, 1, 1);
            var Hmax = 2;
            var Vmax = 2;
            
            Console.WriteLine($"Оригинальная матрица ({width}x{height}): ");

            WriteMatrix(channel.GetMatrix());
            Console.WriteLine();
            
            channel.Complete(Hmax, Vmax);
            Console.WriteLine($"После дополнения ({channel.GetMatrix().GetLength(0)}x{channel.GetMatrix().GetLength(1)}): ");

            var originalColor = Console.ForegroundColor;
            
            for (int y = 0; y < channel.GetMatrix().GetLength(1); y++)
            {
                for (int x = 0; x < channel.GetMatrix().GetLength(0); x++)
                {
                    Console.ForegroundColor = (x < width && y < height) ? originalColor : ConsoleColor.DarkGreen;
                    Console.Write(channel.GetMatrix()[x, y].ToString("X2") + " ");
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = originalColor;
            Console.WriteLine();
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
            List<short[,]> QM = new List<short[,]>();

            var library = new JPEG_CS(null);
            library.SetParameters(3);
            Console.WriteLine("Тест для трех каналов:");

            QM.Add(new short[,]{
            {16, 12, 14, 14, 18, 24, 49, 72},
            {11, 12, 13, 17, 22, 35, 64, 92},
            {10, 14, 16, 22, 37, 55, 78, 95},
            {16, 19, 24, 29, 56, 64, 87, 98},
            {24, 26, 40, 51, 68, 81, 103, 112},
            {40, 58, 57, 87, 109, 104, 121, 100},
            {51, 60, 69, 80, 103, 113, 120, 103},
            {61, 55, 56, 62, 77, 92, 101, 99}});
            QM.Add(new short[,]{
            {17, 18, 24, 47, 99, 99, 99, 99},
            {18, 21, 26, 66, 99, 99, 99, 99},
            {24, 26, 56, 99, 99, 99, 99, 99},
            {47, 66, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99}});
            QM.Add(new short[,]{
            {17, 18, 24, 47, 99, 99, 99, 99},
            {18, 21, 26, 66, 99, 99, 99, 99},
            {24, 26, 56, 99, 99, 99, 99, 99},
            {47, 66, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99}});

            var blocks = library.Interleave(channels, QM);

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
            QM = new List<short[,]>();
            QM.Add(new short[,]{
            {16, 12, 14, 14, 18, 24, 49, 72},
            {11, 12, 13, 17, 22, 35, 64, 92},
            {10, 14, 16, 22, 37, 55, 78, 95},
            {16, 19, 24, 29, 56, 64, 87, 98},
            {24, 26, 40, 51, 68, 81, 103, 112},
            {40, 58, 57, 87, 109, 104, 121, 100},
            {51, 60, 69, 80, 103, 113, 120, 103},
            {61, 55, 56, 62, 77, 92, 101, 99}});

            blocks = library.Interleave(channels, QM);

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
            }/*
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
            TestChannel.Collect(list3);*/
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
            //f.Print();
            f.PrintData();
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

        private static void _TestImageConverter()
        {
            int widgth = 4;
            int height = 2;
            // Создаем матрицу пикселей RGB [widgth, height] со случайными байтовыми значениями.
            Random random = new Random();
            Point[,] imgRGB = new Point[widgth, height];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < widgth; j++)
                    imgRGB[j, i] = new Point()
                    {
                        r = (byte)random.Next(0, 255),
                        g = (byte)random.Next(0, 255),
                        b = (byte)random.Next(0, 255),
                    };

            // Выводим исходный массив RGB пикселей.
            Console.WriteLine("Исходный массив RGB пикселей");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++)
                    Console.Write($"RGB=({imgRGB[j, i].r:d3};{imgRGB[j, i].g:d3};{imgRGB[j, i].b:d3}) ");
                Console.WriteLine();
            }

            // Конвертируем массив RGB пикселей в массив YUV пикселей и выводим его.
            byte[,] matrixY, matrixCb, matrixCr;
            ImageConverter.RGBToYUV(imgRGB, out matrixY, out matrixCb, out matrixCr);
            Console.WriteLine("Конвертированный из RGB в YUV массив пикселей");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++)
                    Console.Write($"YUV=({matrixY[j, i]:d3};{matrixCb[j, i]:d3};{matrixCr[j, i]:d3}) ");
                Console.WriteLine();
            }

            // Конвертируем массив YUV пикселей обратно в массив RGB пикселей и выводим его.
            Point[,] newImgRGB = ImageConverter.YUVToRGB(matrixY, matrixCb, matrixCr);
            Console.WriteLine("Конвертированный из YUV обратно в RGB массив пикселей");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++)
                    Console.Write($"RGB=({newImgRGB[j, i].r:d3};{newImgRGB[j, i].g:d3};{newImgRGB[j, i].b:d3}) ");
                Console.WriteLine();
            }
        }
        private static void DecodeBlockTest()
        {
            short[] Block = new short[64];
            using (FileStream S = File.Open("../../../test.jpg", FileMode.Open))
            {
                S.Seek(0x1f7, SeekOrigin.Begin);
                HuffmanTable huffDC = new HuffmanTable(S);
                S.Seek(0x218, SeekOrigin.Begin);
                HuffmanTable huffAC = new HuffmanTable(S);
                S.Seek(0x3b3, SeekOrigin.Begin);
                Decoding decoding = new Decoding(S, huffDC, huffAC);
                for (int k = 0; k < 10; k++)
                {
                    decoding.huffAC = huffAC;
                    decoding.huffAC.GenerateTables();
                    decoding.huffDC = huffDC;
                    decoding.huffDC.GenerateTables();
                    Block = decoding.DecodeBlock();
                    Console.WriteLine("\nДекодирование "+k+" блока");

                    for (int i = 0;  i < 64; i++)
                    {
                        string s = Block[i]+" ";
                        Console.Write(s);
                        
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
