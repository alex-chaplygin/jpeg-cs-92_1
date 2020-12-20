using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class ScanTest
    {
        [TestMethod]
        public void ScanTest_Write()
        {
            Random rnd = new Random(0);
            byte NumberOfImageComponent = (byte)rnd.Next(256);
            //byte NumberOfImageComponent = (byte)3;
            ushort Length = (ushort)(6 + NumberOfImageComponent * 2); 
            Component[] components = new Component[NumberOfImageComponent];
            for (byte i = 0; i < NumberOfImageComponent; i++)
                components[i] = new Component((byte)rnd.Next(256),
                    (byte)rnd.Next(16), 
                    (byte)rnd.Next(16));
            byte Ah = (byte)rnd.Next(16); 
            byte Al = (byte)rnd.Next(16);

            MemoryStream s = new MemoryStream();
            Scan scan1 = new Scan(s, NumberOfImageComponent, components, Ah, Al);
            scan1.Write();

            byte[] expected = s.ToArray();

            // Устанавливаем указатель на параметр Lenght, с которого начинается чтение в конструкторе Scan.
            s.Seek(2, SeekOrigin.Begin); 

            Scan scan2 = new Scan(s); 
            s.Seek(0, SeekOrigin.Begin);

            scan2.Write();

            byte[] actual = s.ToArray();
            s.Dispose();

            CollectionAssert.AreEqual(expected, actual);

            ushort[] scan1Params = new ushort[4 + NumberOfImageComponent * 3];
            scan1Params[0] = Length;
            scan1Params[1] = NumberOfImageComponent;
            scan1Params[scan1Params.Length - 2] = Ah;
            scan1Params[scan1Params.Length - 1] = Al;
            for (byte i = 0; i < NumberOfImageComponent; i++)
            {
                scan1Params[2 + i * 3 + 0] = components[i].ComponentSelector;
                scan1Params[2 + i * 3 + 1] = components[i].TableDC;
                scan1Params[2 + i * 3 + 2] = components[i].TableAC;
            }

            ushort[] scan2Params = new ushort[4 + scan2.NumberOfImageComponent * 3];
            scan2Params[0] = scan2.Length;
            scan2Params[1] = scan2.NumberOfImageComponent;
            scan2Params[scan2Params.Length - 2] = scan2.ApproximationHigh;
            scan2Params[scan2Params.Length - 1] = scan2.ApproximationLow;
            for (byte i = 0; i < scan2.NumberOfImageComponent; i++)
            {
                scan2Params[2 + i * 3 + 0] = scan2.components[i].ComponentSelector;
                scan2Params[2 + i * 3 + 1] = scan2.components[i].TableDC;
                scan2Params[2 + i * 3 + 2] = scan2.components[i].TableAC;
            }

            Console.WriteLine("Изначальный скан");
            scan1.Print();
            Console.WriteLine("Скан после записи");
            scan2.Print();

            CollectionAssert.AreEqual(scan1Params, scan2Params);
        }
    }
}
