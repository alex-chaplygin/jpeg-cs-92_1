using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    class DecodingTest
    {
        [TestMethod]
        private static void _TestDecodingNextBit()
        {
            byte[] expected = new byte[5];
            byte[] actual = new byte[5];

            Random r = new Random();
            FileStream S = File.Open("../../../test.jpg", FileMode.Open);

            S.Seek(0x1f7, SeekOrigin.Begin);
            HuffmanTable huffDC = new HuffmanTable(S);
            S.Seek(0x218, SeekOrigin.Begin);
            HuffmanTable huffAC = new HuffmanTable(S);
            S.Seek(0x3a7, SeekOrigin.Begin); //начала скана

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
            S.Dispose();
            ms.Dispose();
        }

        [TestMethod]
        private static void _TestDecodingRecieve()
        {
            ushort[] expected = new ushort[5];
            ushort[] actual = new ushort[5];

            Random r = new Random();
            FileStream S = File.Open("../../../test.jpg", FileMode.Open);

            S.Seek(0x1f7, SeekOrigin.Begin);
            HuffmanTable huffDC = new HuffmanTable(S);
            S.Seek(0x218, SeekOrigin.Begin);
            HuffmanTable huffAC = new HuffmanTable(S);
            S.Seek(0x3a7, SeekOrigin.Begin); //начала скана

            MemoryStream ms = new MemoryStream(5);
            for (int i = 0; i < 5; i++)
            {
                expected[i] = (byte)r.Next(0, (int)Math.Pow(2,16));
                ms.Write(new byte[2] {(byte)(expected[i] >> 8),(byte)(expected[i] % 256) },0,2);
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            for (int i = 0; i < 5; i++)
            {
                actual[i] = d.Receive(16);
            }

            CollectionAssert.AreEqual(expected, actual);
            S.Dispose();
            ms.Dispose();
        }

        //[TestMethod]
        private static void _TestDecodingExtend()
        {
            Random r = new Random();
            FileStream S = File.Open("../../../test.jpg", FileMode.Open);

            S.Seek(0x1f7, SeekOrigin.Begin);
            HuffmanTable huffDC = new HuffmanTable(S);
            S.Seek(0x218, SeekOrigin.Begin);
            HuffmanTable huffAC = new HuffmanTable(S);
            S.Seek(0x3a7, SeekOrigin.Begin); //начала скана

            MemoryStream ms = new MemoryStream(3);
            for (int i = 0; i < 3; i++)
            {
                ms.WriteByte((byte)r.Next(0, 255));
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            ushort diff = d.Receive(16);
            diff = (ushort)Decoding.Extend(diff, 16);
            Console.Write($"Результат: {diff}");
        }
    }
}
