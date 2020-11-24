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
	    /*    _TestSplit();
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
            _TestImageConverter();*/
            _TestJPEGFile();
            //Console.ReadKey();
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
                Console.Write(Convert.ToString(curByte, 2).PadLeft(8, '0')+" ");
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
                    matrix[x, y] = (byte)(c*4);
                }
            }

            Channel channel1 = new Channel(matrix, 2, 2);
            channel1.Sample(2,2);
            WriteMatrix(channel1.GetMatrix());
            Console.WriteLine();
            
            Channel channel2 = new Channel(matrix, 2, 1);
            channel2.Sample(2,2);
            WriteMatrix(channel2.GetMatrix());
            Console.WriteLine(); 
            
            Channel channel3 = new Channel(matrix, 1, 1);
            channel3.Sample(2,2);
            WriteMatrix(channel3.GetMatrix());
            Console.WriteLine();

            
            // Channel channel4 = new Channel(matrix, 1, 1);
            // channel4.Sample(2,2);
            // WriteMatrix(channel4.GetMatrix());
            // Console.WriteLine();
            


            var channels = new[] {channel1, channel2, channel3};

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

            channels = new[] {channel1};

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
            Point[,] points = new Point[10,10];
            for (int i = 0; i< 10; i++)
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


        static void _TestChannel()
        {

            byte[,] matrix = new byte[4, 4];
            for (int y = 0, c = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++, c++)
                {
                    matrix[x, y] = (byte)(c*4);
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
                    Console.Write(matrix[x, y].ToString("X2")+ " ");
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
            short[,] qtest = Quantization.QuantizationDirect(matrixC, matrixQ);
            for (int i = 0; i<16; i++)
            {
                Console.Write($"{qtest[i/4,i%4]}\t");
                if (i%4 == 3) {Console.Write("\n"); }
            }
            //после обратного
            Console.WriteLine();
            qtest = Quantization.QuantizationReverse(matrixC, matrixQ);
            for (int i = 0; i < 16; i++)
            {
                Console.Write($"{qtest[i / 4, i % 4]}\t");
                if (i % 4 == 3) { Console.Write("\n"); }
            }
        }
        */
        static void _TestDCT()
        {
            short[,]matrix= new short[8, 8];
            //short a = 0;
            Random r = new Random();

            for (int i = 0; i<8;i++)
            {
                for (int j=0; j<8;j++)
                {
                    matrix[i, j] = Convert.ToInt16(Convert.ToInt16(r.Next(-128,128)));
                    //if (i==0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (j == 0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (i == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    //if (j == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));

                    Console.Write(matrix[i, j]+" ");
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
            short[,]matrix2= DCT.IDCT(matrix3);
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
            for (var blockIndex = 0; blockIndex<blocks.Count; blockIndex++)
            {
                var block = blocks[blockIndex];
                if (block.Length != BLOCK_SIZE * BLOCK_SIZE) throw new Exception("Incorrect block size!");
                Console.WriteLine("Block #"+blockIndex);
                var oneDArr = new byte[BLOCK_SIZE*BLOCK_SIZE];
                Buffer.BlockCopy(block, 0, oneDArr, 0, block.Length);
                for (int i = 1; i <= oneDArr.Length; i++)
                {
                    Console.Write(oneDArr[i-1].ToString("X2")+ " ");
                    if (i!=0 && i%BLOCK_SIZE==0) Console.WriteLine();
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
                    if (testMatrix[j, i]!=channel.GetMatrix()[j, i]) throw new Exception("Test failed - matrix must be equal!");
                }
            }
            
            // if (!Enumerable.SequenceEqual(testMatrix, channel.GetMatrix())) throw new Exception("Matrix must be equal!");
        }
        /*
        static void _TestCalculatingDC()
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

            JPEG_CS.DCCalculating(blocks);

            //вывод после
            Console.WriteLine($"\n\nПосле");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Console.Write($"{blocks[j][i / 4, i % 4]}\t");
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
                    Console.Write(D.NextBit()+" ");
                    i++;
                    if (i == 16)
                    {
                        Console.WriteLine();
                        i = 0;
                    }
                }
                while (true);
            }
            catch(Exception ex)
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
                    imgRGB[j,i] = new Point(){
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
    }
}
