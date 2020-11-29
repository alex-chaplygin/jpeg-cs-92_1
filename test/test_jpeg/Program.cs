using System;
using System.IO;
using JPEG_CLASS_LIB;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*_TestSplit();
            _TestCalculatingDC();
            _TestPack();
            _TestUnpack();
            _TestQuantization();
            _TestDCT();
            _TestDCTShift();
            _TestChannel();
            _TestInterleave();
            _TestZigzad();
            _TestJPEGData();
            _TestBitReader();
            _TestBitWriter();            
            _TestEncoding();
            _TestImageConverter();
            _TestJPEGFile();
            _TestBitWriterTwo();
            _TestBitWriterError();
            _TestDecodingExtend();
            _TestImageConverter();*/
            //_TestJPEGFile();
            //_TestHuffmanTable();
            // _TestDCTcoding();
            //_TestChannelV2();
            _TestEncodingWriteBits();
            Console.ReadKey();
        }

        static void _TestChannelV2()
        {
            const int TEST_COUNT = 10;
            var r = new Random();
            for (var i = 0; i < TEST_COUNT; i++)
            {
                var width = r.Next(4, 1024);
                var height = r.Next(4, 1024);
                byte[,] matrix = new byte[width, height];
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        matrix[x, y] = (byte) r.Next(0, 255);

                    }
                }
                Console.WriteLine($"Тестовая матрица (W*H): {width}*{height}");
                WriteMatrix(matrix);

                var H = Convert.ToInt32(Math.Pow(2, r.Next(0, 4)));
                var V = Convert.ToInt32(Math.Pow(2, r.Next(0, 4)));
                Console.WriteLine($"H={H}, V={V}");

                var channel = new Channel(matrix, H, V);
                channel.Sample(H, V);
                var blocks = channel.Split();
                channel.Collect(blocks);
                channel.Resample(H, V);
                
                Console.WriteLine($"Результирующая матрица (W*H): {channel.GetMatrix().GetLength(0)}*{channel.GetMatrix().GetLength(1)}");
                WriteMatrix(channel.GetMatrix());
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (matrix[x, y]!=channel.GetMatrix()[x, y]) throw new Exception("Ошибка при тестировании: элементы тестовой и результирующей матрицы не совпадают!");
                    }
                }
                
            }
            
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

            var testData = new byte[10] {1, 3, 4, 1, 1, 0, 4, 1, 2, 1};
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

        static void _TestChannel()
        {

            byte[,] matrix = new byte[4, 4];
            for (int y = 0, c = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++, c++)
                {
                    matrix[x, y] = (byte)(c * 4);
                }
            }

            Console.WriteLine("Block(matrix, 4, 4)");
            Channel channel1 = new Channel(matrix, 4, 4);
            WriteMatrix(channel1.GetMatrix());

            Console.WriteLine("\nResample(16, 8)");
            channel1.Resample(16, 8);
            WriteMatrix(channel1.GetMatrix());

            Console.WriteLine("\nSample(16, 8)");
            channel1.Sample(16, 8);
            WriteMatrix(channel1.GetMatrix());
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
        /*
        static void _TestQuantization()
        {
            Random random = new Random();
            short[,] matrixC = new short[4, 4];
            short[,] matrixQ = new short[4, 4] { {1,2,4,1 },
                                                 {2,2,2,2 },
                                                 {3,3,3,30 },
                                                 {4,4,40,50 } };
            
            for (int i = 0; i< 16; i++)
            {
                matrixC[i/4,i%4] = Convert.ToByte(random.Next(0,16));
            }


            //до
            for (int i = 0; i < 16; i++)
            {
                Console.Write($"{matrixC[i / 4, i % 4]}\t");
                if (i % 4 == 3) { Console.Write("\n"); }
            }
            Console.WriteLine();
            //после квантования
            short[,] qtest = DCT.QuantizationDirect(matrixC, matrixQ);
            for (int i = 0; i<16; i++)
            {
                Console.Write($"{qtest[i/4,i%4]}\t");
                if (i%4 == 3) {Console.Write("\n"); }
            }
            //после обратного
            Console.WriteLine();
            qtest = DCT.QuantizationReverse(matrixC, matrixQ);
            for (int i = 0; i < 16; i++)
            {
                Console.Write($"{qtest[i / 4, i % 4]}\t");
                if (i % 4 == 3) { Console.Write("\n"); }
            }
        }
        */

        static void _TestDCT()
        {
            short[,] matrix = new short[8, 8];
            //short a = 0;
            Random r = new Random();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    matrix[i, j] = Convert.ToInt16(Convert.ToInt16(r.Next(-128, 128)));
                    //if (i==0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (j == 0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (i == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (j == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));

                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,] matrix3 = DCT.FDCT(matrix);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    Console.Write(matrix3[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,] matrix2 = DCT.IDCT(matrix3);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    Console.Write(matrix2[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        static void _TestZigzad()
        {
            short[,] matrix = new short[8, 8]{{0, 1, 5, 6, 14, 15, 27, 28},
                                                    {2, 4, 7, 13, 16, 26, 29, 42},
                                                    {3, 8, 12, 17, 25, 30, 41, 43},
                                                    {9, 11, 18, 24, 31, 40, 44, 53},
                                                    { 10, 19, 23, 32, 39, 45, 52, 54 },
                                                    { 20, 22, 33, 38, 46, 51, 55, 60 },
                                                    { 21, 34, 37, 47, 50, 56, 59, 61 },
                                                    { 35, 36, 48, 49, 57, 58, 62, 63 } };
            short[] mass = DCT.Zigzag(matrix);//fff
            for (int i = 0; i < 64; i++)
            {
                Console.Write(mass[i] + " ");
            }
            short[,] matrix3 = DCT.ReZigzag(mass);
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(matrix3[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        static void _TestDCTShift()
        {
            Random Random = new Random();
            byte[,] testmassive = new byte[5, 5];
            for (int i = 0; i < testmassive.GetLength(0); i++)
                for (int j = 0; j < testmassive.GetLength(1); j++) testmassive[i, j] = Convert.ToByte(Random.Next(0, 256));
            short[,] testshort = new short[,] { };
            byte[,] testbyte = new byte[,] { };
            testshort = DCT.Shift(testmassive);
            testbyte = DCT.ReverseShift(testshort);
            Console.WriteLine("Изначальная матрица");
            for (int i = 0; i < testmassive.GetLength(0); i++)
            {
                for (int j = 0; j < testmassive.GetLength(1); j++) Console.Write(testmassive[i, j].ToString() + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Матрица со сдвигом");
            for (int i = 0; i < testshort.GetLength(0); i++)
            {
                for (int j = 0; j < testshort.GetLength(1); j++) Console.Write(testshort[i, j].ToString() + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Матрица с обратным сдвигом");
            for (int i = 0; i < testbyte.GetLength(0); i++)
            {
                for (int j = 0; j < testbyte.GetLength(1); j++) Console.Write(testbyte[i, j].ToString() + " ");
                Console.WriteLine();
            }
        }
        static void _TestSplit()
        {
            var r = new Random();

            // var width = r.Next(2, 1080); //big test output
            // var height = r.Next(2, 1920);

            var width = r.Next(2, 25); //compact test output
            var height = r.Next(2, 25);

            Console.WriteLine($"{width}*{height}");

            var testMatrix = new byte[width, height];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var tmpByte = new byte[1];
                    r.NextBytes(tmpByte);
                    testMatrix[j, i] = tmpByte[0];
                }
            }

            WriteMatrix(testMatrix);

            var channel = new Channel(testMatrix, 0, 0);
            var blocks = channel.Split();

            Console.WriteLine();
            Console.WriteLine($"For given {width}*{height} there is {blocks.Count} block(s)");

            //for test print BLOCK_SIZE=8 - fixed!
            var BLOCK_SIZE = 8;
            for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
            {
                var block = blocks[blockIndex];
                if (block.Length != BLOCK_SIZE * BLOCK_SIZE) throw new Exception("Incorrect block size!");
                Console.WriteLine("Block #" + blockIndex);
                var oneDArr = new byte[BLOCK_SIZE * BLOCK_SIZE];
                Buffer.BlockCopy(block, 0, oneDArr, 0, block.Length);
                for (int i = 1; i <= oneDArr.Length; i++)
                {
                    Console.Write(oneDArr[i - 1].ToString("X2") + " ");
                    if (i != 0 && i % BLOCK_SIZE == 0) Console.WriteLine();
                }

                Console.WriteLine();
            }

            channel.Collect(blocks);

            Console.WriteLine();
            WriteMatrix(channel.GetMatrix());

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (testMatrix[j, i] != channel.GetMatrix()[j, i]) throw new Exception("Test failed - matrix must be equal!");
                }
            }

            // if (!Enumerable.SequenceEqual(testMatrix, channel.GetMatrix())) throw new Exception("Matrix must be equal!");
        }

        /*static void _TestCalculatingDC()
        {
            List<byte[,]> blocks = new List<byte[,]>();

            Random random = new Random();
            for (int j = 0; j < 3; j++)
            {
                byte[,] s = new byte[4, 4];
                blocks.Add(s);
                for (int i = 0; i < 16; i++)
                {
                    blocks[j][i / 4, i % 4] = Convert.ToByte(random.Next(0, 16));
                }
            }

            //вывод до
            Console.WriteLine("До вычисления DC");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Console.Write($"{blocks[j][i / 4, i % 4]}\t");
                    if (i % 4 == 3) { Console.Write("\n"); }
                }
                Console.WriteLine();
            }

            DCT.DCCalculating(blocks);

            //вывод после
            Console.WriteLine($"\n\nПосле");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Console.Write($"{blocks[j][i / 4, i % 4]}В\t");
                    if (i % 4 == 3) { Console.Write("\n"); }
                }
                Console.WriteLine();
            }

            JPEG_CS.DCRestore(blocks);

            //вывод после обратного
            Console.WriteLine($"\n\nПосле обратного");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Console.Write($"{blocks[j][i / 4, i % 4]}\t");
                    if (i % 4 == 3) { Console.Write("\n"); }
                }
                Console.WriteLine();
            }
        }
        */

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
            Decoding decoding = new Decoding(s, huff);
            Console.WriteLine("\nТаблица MaxCode");
            foreach (int i in decoding.MaxCode)
            {
                Console.Write(Convert.ToString(i, 2)+" ");
            }
            Console.WriteLine("\nТаблица MinCode");
            foreach (int i in decoding.MinCode)
            {
                Console.Write(Convert.ToString(i, 2)+" ");
            }
            Console.WriteLine("\nТаблица VALPTR");
            foreach (byte i in decoding.VALPTR)
            {
                Console.Write(Convert.ToString(i, 2)+" ");
            }
            Console.WriteLine();
	        Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());
            Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());
            Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode());

            //s.Seek(0x360, SeekOrigin.Begin);
            //Console.WriteLine($"Тестирование метода Receive класса Decoding от позиции {s.Position:x4}");
            //for (byte i = 1; i <= 16; i++)
            //   Console.WriteLine($"Результат чтения следующих {i:d2} бит из потока: {Convert.ToString(decoding.Receive(i), 2)}");
            s.Dispose();
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

        private static void _TestEncodingWriteBits()        
        {
            ushort[] bits = 
            {
                0b1010_0110_1111_1111,
                0b0000_0000_0010_1110,
                0b0000_0000_0000_1011,
                0b0000_0010_0000_0110,
                0b0000_0000_0001_0101,
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
