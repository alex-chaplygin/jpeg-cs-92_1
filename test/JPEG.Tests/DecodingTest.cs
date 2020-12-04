using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.IO;


namespace JPEG.Tests
{
    [TestClass]
    public class DecodingTest
    {
        [TestMethod]
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
