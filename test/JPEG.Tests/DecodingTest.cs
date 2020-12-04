using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using JPEG_CLASS_LIB;

namespace JPEG.Tests
{
    [TestClass]
    public class DecodingTest
    {
        [TestMethod]
        public void _TestDecodingNextBit()
        {
            byte[] expected = new byte[5];
            byte[] actual = new byte[5];

            Random r = new Random();
            HuffmanTable huffDC = null;
            HuffmanTable huffAC = null;

            MemoryStream ms = new MemoryStream(5);
            for (int i = 0; i < 5; i++)
            {
                expected[i] = (byte)r.Next(0, 255);
                ms.WriteByte(expected[i]);
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    actual[i] = (byte)(actual[i] << 1);
                    actual[i] += d.NextBit();
                }
                Console.WriteLine(actual[i]);
            }

            CollectionAssert.AreEqual(expected, actual);
            ms.Dispose();
        }

        [TestMethod]
        public static void _TestDecodingRecieve()
        {
            ushort[] expected = new ushort[5];
            ushort[] actual = new ushort[5];

            Random r = new Random();

            HuffmanTable huffDC = null;
            HuffmanTable huffAC = null;

            MemoryStream ms = new MemoryStream(5);
            for (int i = 0; i < 5; i++)
            {
                expected[i] = (byte)r.Next(0, (int)Math.Pow(2, 16));
                ms.Write(new byte[2] { (byte)(expected[i] >> 8), (byte)(expected[i] % 256) }, 0, 2);
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            for (int i = 0; i < 5; i++)
            {
                actual[i] = d.Receive(16);
            }

            CollectionAssert.AreEqual(expected, actual);
            ms.Dispose();
        }

        [TestMethod]
        public static void _TestDecodingExtend()
        {
            short expected = 0;

            Random r = new Random();

            HuffmanTable huffDC = null;
            HuffmanTable huffAC = null;

            MemoryStream ms = new MemoryStream(6);
            for (int i = 0; i < 6; i++)
            {
                byte temp1 = 0;
                byte temp2 = 0;
                if (i == 0)
                {
                    temp1 = (byte)r.Next(0, 255);
                    ms.WriteByte(temp1);
                    expected = (short)((int)(temp1) << 8);
                }
                else if (i == 1)
                {
                    temp2 = (byte)r.Next(0, 255);
                    ms.WriteByte(temp2);
                    expected += temp2;
                }
                ms.WriteByte((byte)r.Next(0, 255));
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            ushort actual = d.Receive(8);
            actual = (ushort)Decoding.Extend(actual, 8);
            Console.Write($"Результат: {actual}");

            ushort temp = (ushort)Math.Pow(2, 7);
            while (expected < temp)
            {
                temp = (ushort)((-1 * Math.Pow(2, 8)) + 1);
                expected += (short)temp;
            }

            Assert.AreEqual(expected, actual);
            ms.Dispose();
        }
    }
}
