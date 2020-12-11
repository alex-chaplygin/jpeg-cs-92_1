using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class ArithmeticConditioningTest
    {
        [TestMethod]
        public void ArithmeticConditioning()
        {
            Multiple[] multiples = {
                new Multiple((byte)0x0, (byte)0x1, (byte)0x05), 
                new Multiple((byte)0x6, (byte)0xA, (byte)0xF3),
                new Multiple((byte)0xB, (byte)0xF, (byte)0xFF)};

            ushort Length = (ushort)(2 + multiples.Length * 2);
            byte[] CodesArray = new byte[2 + 2 + multiples.Length * 2];

            CodesArray[0] = (byte)(Length >> 8);
            CodesArray[1] = (byte)(Length & 0xFF);

            for (int i = 0; i < multiples.Length; i++)
            {
                CodesArray[2 + i * 2] = (byte)((multiples[i].Tc << 4) + multiples[i].Tb);
                CodesArray[2 + i*2 + 1] = multiples[i].Cs;
            }

            MemoryStream M = new MemoryStream(CodesArray, true);
            ArithmeticConditioning arithmeticConditioning = new ArithmeticConditioning(M);
            Console.WriteLine("Результат чтения arithmeticConditioning:");
            arithmeticConditioning.Print();

            M.Dispose();
        }
    }
}
