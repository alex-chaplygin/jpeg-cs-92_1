using System;
using System.IO;
using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JPEG.Tests
{
    [TestClass]
    public class RestartIntervalTest
    {
        [TestMethod]
        public void RestartIntervalWriteTest()
        {
            var restartIntervalTest = (ushort) new Random().Next(0, 100);
            
            var s = new MemoryStream();
            var ri1 = new RestartInterval(s, restartIntervalTest);
            ri1.Write();

            byte[] expected = s.ToArray();

            s.Seek(2, SeekOrigin.Begin); 

            var ri2 = new RestartInterval(s); 
            s.Seek(0, SeekOrigin.Begin);

            ri2.Write();

            byte[] actual = s.ToArray();
            s.Dispose();

            Console.WriteLine("Изначальные значения:");
            ri1.Print();
            Console.WriteLine("Значения после записи");
            ri2.Print();

            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(restartIntervalTest, ri2.restartInterval);
        }

    }
}