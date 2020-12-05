﻿using System;
using System.IO;
using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JPEG.Tests
{
    [TestClass]
    public class HuffmanTableTest
    {

        [TestMethod]
        public void TestHuffmanTable_HuffmanTable()
        {

            Random random = new Random();
            ushort Length = 1;
            byte TcTh = 0x00;
            byte[] CodesArray;

            byte[] CodeLengthForLuminance = { 2, 3, 3, 3, 3, 3, 4, 5, 6, 7, 8, 9 , // CodeLength
            0, 0, 0 ,0}; // Оставшиеся значения, чтобы массив был длиной [16]
            int AllCodeLengthForLuminance = 0;
            foreach (var a in CodeLengthForLuminance) AllCodeLengthForLuminance += a;
            byte[] ValuesForLuminance = new byte[AllCodeLengthForLuminance];
            random.NextBytes(ValuesForLuminance);

            CodesArray = new byte[3 + CodeLengthForLuminance.Length + AllCodeLengthForLuminance];
            CodesArray[0] = (byte)(Length >> 8);
            CodesArray[1] = (byte)(Length & 0xFF);
            CodesArray[3] = TcTh;
            Array.Copy(CodeLengthForLuminance, 0, CodesArray, 3, CodeLengthForLuminance.Length);
            Array.Copy(ValuesForLuminance, 0, CodesArray, 3 + CodeLengthForLuminance.Length, AllCodeLengthForLuminance);

            MemoryStream M = new MemoryStream(CodesArray, true);
            HuffmanTable huffmanTable = new HuffmanTable(M);
            Console.WriteLine("HuffmanTable for Luminance:");
            huffmanTable.Print();
            //M.Dispose();

            short[] expectedCodeWordForLuminance =
                { 0, 2, 3, 4, 5, 6, 14, 30, 62, 126, 254, 510};
            CollectionAssert.AreEqual(expectedCodeWordForLuminance, huffmanTable.HUFFCODE);

            byte[] CodeLengthForChrominance = { 2, 2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, // CodeLength
            0, 0, 0, 0}; // Оставшиеся значения, чтобы массив был длиной [16]
            int AllCodeLengthForChrominance = 0;
            foreach (var a in CodeLengthForLuminance) AllCodeLengthForChrominance += a;
            byte[] ValuesForChrominance = new byte[AllCodeLengthForChrominance];
            random.NextBytes(ValuesForChrominance);

            CodesArray = new byte[3 + CodeLengthForChrominance.Length + AllCodeLengthForChrominance];
            CodesArray[0] = (byte)(Length >> 8);
            CodesArray[1] = (byte)(Length & 0xFF);
            CodesArray[3] = TcTh;
            Array.Copy(CodeLengthForChrominance, 0, CodesArray, 3, CodeLengthForChrominance.Length);
            Array.Copy(CodeLengthForChrominance, 0, CodesArray, 3 + CodeLengthForChrominance.Length, AllCodeLengthForChrominance);
            M = new MemoryStream(CodesArray, true);
            huffmanTable = new HuffmanTable(M);
            Console.WriteLine("HuffmanTable for Chrominance:");
            huffmanTable.Print();

            short[] expectedCodeWordForChrominance =
                { 0, 1, 2, 6, 14, 30, 62, 126, 254, 512, 1022, 2046};
            CollectionAssert.AreEqual(expectedCodeWordForChrominance, huffmanTable.HUFFCODE);

            M.Dispose();
        }
    }
}
