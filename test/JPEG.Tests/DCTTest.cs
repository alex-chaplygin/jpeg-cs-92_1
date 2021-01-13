using System;
using System.Collections.Generic;
using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JPEG.Tests
{
    [TestClass]
    public class DCTTest
    {
        [TestMethod]
        public void TestDCTShift()
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

            CollectionAssert.AreEqual(testmassive, testbyte);
        }

        [TestMethod]
        public void TestZigzag()
        {
            /*short[,] matrix = new short[8, 8]{{0, 1, 5, 6, 14, 15, 27, 28},
                                                {2, 4, 7, 13, 16, 26, 29, 42},
                                                {3, 8, 12, 17, 25, 30, 41, 43},
                                                {9, 11, 18, 24, 31, 40, 44, 53},
                                                { 10, 19, 23, 32, 39, 45, 52, 54 },
                                                { 20, 22, 33, 38, 46, 51, 55, 60 },
                                                { 21, 34, 37, 47, 50, 56, 59, 61 },
                                                { 35, 36, 48, 49, 57, 58, 62, 63 } };*/
            short[,] matrix = new short[8, 8]{{ 0, 2, 3, 9, 10, 20, 21, 35 },
                                              { 1, 4, 8, 11, 19, 22, 34, 36 },
                                              { 5, 7, 12, 18, 23, 33, 37, 48 },
                                              { 6, 13, 17, 24, 32, 38, 47, 49 },
                                              { 14, 16, 25, 31, 39, 46, 50, 57 },
                                              { 15, 26, 30, 40, 45, 51, 56, 58 },
                                              { 27, 29, 41, 44, 52, 55, 59, 62 },
                                              { 28, 42, 43, 53, 54, 60, 61, 63 } };
            short[] mass3 = new short[64] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
            31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63};
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
            CollectionAssert.AreEqual(mass,mass3);
            CollectionAssert.AreEqual(matrix, matrix3);
        }

        [TestMethod]
        public void TestDCT()
        {
            short[,] matrix = new short[8, 8];
            Random r = new Random();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    matrix[i, j] = Convert.ToInt16(Convert.ToInt16(r.Next(-128, 127)));
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
                    Assert.IsTrue(Math.Abs(matrix2[i, j] - matrix[i, j]) <= 1, $"Разница ({Math.Abs(matrix2[i, j] - matrix[i, j])}) до и после DCT превышает допустимый уровень");
                    Console.Write(matrix2[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void TestCalculatingDC()
        {
            var blocks = new List<short[,]>();
            var initialBlocks = new List<short[,]>();

            Random random = new Random();
            for (int j = 0; j < 3; j++)
            {
                var s = new short[4, 4];
                var d = new short[4, 4];
                blocks.Add(s);
                initialBlocks.Add(d);
                for (int i = 0; i < 16; i++)
                {
                    var tmpValue = Convert.ToByte(random.Next(0, 16));
                    s[i / 4, i % 4] = tmpValue;
                    d[i / 4, i % 4] = tmpValue;
                    // s[i / 4, i % 4] = Convert.ToByte(random.Next(0, 16));
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

            DCT.DCRestore(blocks);

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


            for (int blockIndex = 0; blockIndex < 3; blockIndex++)
            {
                for (int y = 0; y < initialBlocks[blockIndex].GetLength(1); y++)
                {
                    for (int x = 0; x < initialBlocks[blockIndex].GetLength(0); x++)
                    {
                        Assert.IsTrue(initialBlocks[blockIndex][x, y] == blocks[blockIndex][x, y], $"Элементы не совпадают: initialBlocks[{blockIndex}][{x},{y}] != blocks[{blockIndex}][{x},{y}]");
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestDCTDifference()
        {
            var jpeg = new JPEG_CS(null);
            var passDifference = new [] {42, 73, 148};
            // var passDifference = new [] {5, 15, 15};
            var parametersName = new [] {"высокого качества", "среднего качества", "низкого качества"};
            for (var parameters = 0; parameters < 3; parameters++)
            {
                Console.WriteLine($"Параметры: {Convert.ToInt32(Math.Pow(2, parameters))}");
                jpeg.SetParameters(Convert.ToInt32(Math.Pow(2, parameters)));
                var width = 8;
                var height = 8;
                var testMatrix = new byte[width, height];
                var r = new Random();
                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < width; j++)
                    {
                        testMatrix[j, i] = (byte) r.Next(0, 255);
                    }
                }
                var channel = new Channel(testMatrix, 2, 2);
                var split = channel.Split();
                Console.WriteLine("Тестовая матрица:");
                WriteMatrix(split[0], 2);
                var fdct = jpeg.FDCT(split, jpeg.LQT);
                var idct = jpeg.IDCT(fdct, jpeg.LQT);
                Console.WriteLine("Матрица после DCT:");
                WriteMatrix(idct[0], 2);
                Console.WriteLine("Разностная матрица:");
                for (int y = 0; y < split[0].GetLength(1); y++)
                {
                    for (int x = 0; x < split[0].GetLength(0); x++)
                    {
                        var tmpDiff = split[0][x, y] - idct[0][x, y];
                        Assert.IsTrue(tmpDiff < passDifference[parameters], $"Разница ({tmpDiff}) превышает допустимые показатели для {parametersName[parameters]}");
                        Console.Write(Convert.ToString(tmpDiff).PadRight(3, ' ') + "| ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
                
        static void WriteMatrix(byte[,] matrix, short baseInfo)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    string tmpOut;
                    if (baseInfo==16)
                        tmpOut = matrix[x, y].ToString("X2");
                    else
                    {
                        tmpOut = matrix[x, y].ToString();
                        while (tmpOut.Length < 3) tmpOut = "0" + tmpOut;
                    }

                    Console.Write(tmpOut + " ");
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void TestQuantization()
        {
            short[,] matrix = new short[8, 8];
            Random r = new Random();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    matrix[i, j] = Convert.ToInt16(Convert.ToInt16(r.Next(-128, 127)));
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,] LQT =
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

            var LQT_HIGH = new short[8,8];
            for (int y = 0; y < LQT.GetLength(1); y++)
            {
                for (int x = 0; x < LQT.GetLength(0); x++)
                {
                    LQT_HIGH[x, y] = (short)(LQT[x, y] >> 1);
                }
            }

            short[,] matrix3 = DCT.QuantizationDirect(DCT.FDCT(matrix), LQT_HIGH);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    Console.Write(matrix3[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,] matrix2 = DCT.IDCT(DCT.QuantizationReverse(matrix3, LQT_HIGH));
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Assert.IsTrue(Math.Abs(matrix2[i, j] - matrix[i, j]) <= 42, $"Разница ({Math.Abs(matrix2[i, j] - matrix[i, j])}) до и после квантования превышает допустимый уровень");
                    Console.Write(matrix2[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}