using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using JPEG_CLASS_LIB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void _TestDecodingRecieve()
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
        public void _TestDecodingExtend()
        {
            short expected = 0;

            Random r = new Random();

            HuffmanTable huffDC = null;
            HuffmanTable huffAC = null;

            MemoryStream ms = new MemoryStream(3);
            for (int i = 0; i < 3; i++) 
            {
                if (i == 0)
                {
                    expected = (short)r.Next(0,255);
                    ms.WriteByte((byte)(expected%256));
                    continue;
                }
                ms.WriteByte((byte)(r.Next(0,255)));
            }
            ms.Seek(0, SeekOrigin.Begin);

            Decoding d = new Decoding(ms, huffDC, huffAC);

            ushort actual = d.Receive(4);
            actual = (ushort)Decoding.Extend(actual, 4);
            Console.Write($"Результат: {actual}");

            short vt = (1 << 3); 
            while (expected < vt)
            {
                vt = (-1<<4)+1;
                expected += vt;
            }

            Assert.AreEqual(expected, actual);
            ms.Dispose();
	}
	
	public void DecodeTest()
        {
            
            using (FileStream s = File.Open("../../../test_jpeg/test.jpg", FileMode.Open))
            {
                s.Seek(0x1f7, SeekOrigin.Begin);
                HuffmanTable huffDC = new HuffmanTable(s);
                s.Seek(0x218, SeekOrigin.Begin);
                HuffmanTable huffAC = new HuffmanTable(s);
                s.Seek(0x3b3, SeekOrigin.Begin);
                Decoding decoding = new Decoding(s, huffDC, huffAC);
                decoding.huffDC.GenerateTables();
                decoding.huffAC.GenerateTables();
                Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode(decoding.huffAC));
                Console.WriteLine("\nЗначение, которое вернула функция Decode:" + decoding.Decode(decoding.huffDC));
                Console.WriteLine();
            }
        }
        [TestMethod]
        public void DecodeDCTest()
        {
            short[] Block = new short[64];
            using (FileStream S = File.Open("../../../test_jpeg/test.jpg", FileMode.Open))
            {
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
                    Console.WriteLine("\nДекодирование DC"+" "+Block[0]);
                }
                Console.WriteLine();
            }
        }
                [TestMethod]
        public void DecodeACTest()
        {
            short[] Block = new short[64];
            using (FileStream S = File.Open("../../../test_jpeg/test.jpg", FileMode.Open))
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
                    decoding.DecodeAC(Block);
                    Console.WriteLine("\nДекодирование AC");
                    
                    for (int i = 1, j = 2; i < 64; i++, j++)
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
            }
        }
    }
}
