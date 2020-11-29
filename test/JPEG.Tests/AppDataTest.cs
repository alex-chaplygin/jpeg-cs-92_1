using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class AppDataTest

    {
        [TestMethod]
        public void TestAppData_Print()
        {
            byte[] expected = new byte[10]{74, 70, 73, 70, 0, 1, 2, 1, 0, 72};
            byte[] actual = new byte[10];
            string path = "../../../test_jpeg/JPEG_example_down.jpg";
            FileStream s = new FileStream(path, FileMode.Open);
            s.Seek(0x4, SeekOrigin.Begin);
            AppData appData_Test_Print = new AppData(s);
            appData_Test_Print.Print();
            Array.Copy(appData_Test_Print.data, actual, 10);
            CollectionAssert.AreEqual(expected, actual);
            s.Dispose();
        }
        [TestMethod]
        public void TestAppData_Write()
        {
            string path = "../../../test_jpeg/JPEG_example_down.jpg";
            byte[] Testrray = new byte[18];
            byte[] ExpectedArraytedArray = new byte[] { 0xFF, 0xE0, 00 ,16, 74, 70, 73, 70, 0, 1, 2, 1, 0, 72, 0, 72, 0, 0 };
            MemoryStream M = new MemoryStream(Testrray, true);            
            FileStream s = new FileStream(path, FileMode.Open);
            s.Seek(0x4, SeekOrigin.Begin);
            AppData appData_Test_Write = new AppData(s);
            appData_Test_Write.Write(M);
            M.Position = 0;
            for (int i = 0; i < 17; i++) Testrray[i] = (byte)M.ReadByte();
            CollectionAssert.AreEqual(Testrray, ExpectedArraytedArray);
        }
    }

    [TestClass]
    public class DCTTest
    {
        [TestMethod]
        public void TestDCTDifference()
        {
            var jpeg = new JPEG_CS(null);
            var passDifference = new [] {42, 73, 148};
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
                WriteMatrix(split[0]);
                var fdct = jpeg.FDCT(split);
                var idct = jpeg.IDCT(fdct);
                Console.WriteLine("Матрица после DCT:");
                WriteMatrix(idct[0]);
                Console.WriteLine("Разностная матрица:");
                var absDifference = 0;
                for (int y = 0; y < split[0].GetLength(1); y++)
                {
                    for (int x = 0; x < split[0].GetLength(0); x++)
                    {
                        var tmpDiff = split[0][x, y] - idct[0][x, y];
                        Assert.IsTrue(tmpDiff < passDifference[parameters], $"Разница ({tmpDiff}) превышает допустимые показатели для {parametersName[parameters]}");
                        absDifference += Math.Abs(tmpDiff);
                        Console.Write(Convert.ToString(tmpDiff).PadRight(3, ' ')+"| ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"Абсолютная разность: {absDifference}");
                Console.WriteLine();
            }
        }
                
        static void WriteMatrix(byte[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    Console.Write(matrix[x, y].ToString("X2") + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
