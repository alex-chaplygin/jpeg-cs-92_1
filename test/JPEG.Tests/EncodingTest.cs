using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.Collections.Generic;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class EncodingTest
    {
        [TestMethod]
        public void TestEncodingAC()
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
            byte[] ex = new byte[] { 0x13, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0xf0, 0x34, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x0 };

            //заполнение
            for (int i = 0; i < 1; i++)
            {
                byte[,] a = new byte[8, 8];
                for (int j = 0; j < 64; j++)
                {
                    a[j / 8, j % 8] = (byte)random.Next(0, 256);
                }
                tempB.Add(a);
            }

            for (int i = 0; i < 1; i++)
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
            for (int i = 0; i < 1; i++)
            {
                data.Add(JPEG_CLASS_LIB.DCT.Zigzag(tempS[i]));
            }

            //до кодирования
            for (int i = 0; i < 1; i++)
            {
                data[0][0] = 0;
                Console.WriteLine("До кодирования");
                for (int j = 0; j < 64; j++)
                {
                    if (j > 0 && j <= 10) data[i][j] = 4;
                    if (j > 10 && j <= 28) data[i][j] = 0;
                    if (j > 28 && j <= 47) data[i][j] = 9;
                    if (j > 47 && j < 64) data[i][j] = 0;
                    Console.Write($"{data[i][j]} ");
                }
                Console.Write("\n\n");
            }

            result = Encoding.EncodeAC(data);

            Console.Write("\n\n");
            for (int i = 0; i < result.Length; i++)
            {
                Console.Write(Convert.ToString(result[i], 16) + " ");
            }
            Console.WriteLine();
            CollectionAssert.AreEqual(ex, result);
        }

        [TestMethod]
        public void TestEncodingDC()
        {
            int numberOfBlock = 12;
            // Создаем numberOfBlock количество блоков [64] со случайными байтовыми значениями.
            Random random = new Random();
            List<short[]> listOfBlocks = new List<short[]> { };
            byte[] resultt = new byte[] { };
            byte[] ex = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            for (int k = 0; k < numberOfBlock; k++)
            {
                listOfBlocks.Add(new short[64]);
                for (int i = 0; i < 64; i++)
                {
                    listOfBlocks[k][i] = (short)random.Next(0, 10);
                    if (k == 0) listOfBlocks[k][0] = 0;
                    if (k == 1) listOfBlocks[k][0] = -1;
                    if (k == 2) listOfBlocks[k][0] = 3;
                    if (k == 3) listOfBlocks[k][0] = -5;
                    if (k == 4) listOfBlocks[k][0] = 8;
                    if (k == 5) listOfBlocks[k][0] = -25;
                    if (k == 6) listOfBlocks[k][0] = 45;
                    if (k == 7) listOfBlocks[k][0] = -80;
                    if (k == 8) listOfBlocks[k][0] = 160;
                    if (k == 9) listOfBlocks[k][0] = -330;
                    if (k == 10) listOfBlocks[k][0] = 700;
                    if (k == 11) listOfBlocks[k][0] = -1500;
                }
            }

            // Выводим содержимое блоков до применения метода Encoding.EncodeDC
            Console.WriteLine("Содержимое блоков до применения метода Encoding.EncodeDC");
            for (int k = 0; k < numberOfBlock; k++)
            {
                Console.WriteLine($"Значения {k} блока: " + string.Join(" ", listOfBlocks[k]));
            }

            // Применяем метод Encoding.EncodeDC
            resultt = Encoding.EncodeDC(listOfBlocks);

            for (int i = 0; i < resultt.Length; i++)
            {
                Console.WriteLine("Категория: " + resultt[i]);
            }
            CollectionAssert.AreEqual(ex, resultt);
        }

        [TestMethod]
        public void TestEncoding_WriteBits()
        {
            byte[] Ex = new byte[] { 0xa6, 0xff, 0x00, 0xba, 0xe0, 0x6a, 0x80 };
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
            MemoryStream s = new MemoryStream();
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
            byte[] b = new byte[7];
            for (int i = 0; i < s.Length; i++)
            {
                b[i] = (byte)s.ReadByte();
                Console.WriteLine($"{b[i]:X2} {Convert.ToString(b[i], 2)}");
            }

            s.Dispose();
            File.Delete("../../../testEncodingWriteBits");
            CollectionAssert.AreEqual(Ex, b);
        }
    }
}
