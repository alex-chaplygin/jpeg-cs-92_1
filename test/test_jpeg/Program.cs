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
            _TestSplit();
            _TestCalculatingDC();
            _TestPack();
            _TestUnpack();
            _TestQuantization();
            _TestDCT();
            _TestDCTShift();
            _TestChannel();
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
                    Console.Write($"{matrix[x, y], 3} ");
                }
                Console.WriteLine();
            }
        }

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

            // var width = r.Next(2, 1080); //big output
            // var height = r.Next(2, 1920);
            
            var width = r.Next(2, 25); //compact test output
            var height = r.Next(2, 25);
            
            Console.WriteLine($"{width}*{height}");
            
            var testMatrix = new byte[width, height];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var tmpByte = new byte[1];
                    r.NextBytes(tmpByte);
                    testMatrix[i, j] = tmpByte[0];
                    Console.Write(tmpByte[0].ToString("X2")+ " ");
                }
                Console.WriteLine();
            }
            
            var blocks = JPEG_CS.Split(testMatrix);
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
        }
        
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
    }
}
