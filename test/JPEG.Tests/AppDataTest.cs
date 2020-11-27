using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class AppDataTest

    {
        /*static string path = "../../../JPEG_example_down.jpg";
        FileStream s = new FileStream(path, FileMode.Open);*/
        [TestMethod]
        public void TestAppData_Print()
        {
            byte[] expected = new byte[10]{74, 70, 73, 70, 0, 1, 2, 1, 0, 72};
            byte[] actual = new byte[10];
            string path = @"E:\SWSU\КПО\jpeg-cs-92_1\test\test_jpeg\JPEG_example_down.jpg";
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
            string path = @"E:\SWSU\КПО\jpeg-cs-92_1\test\test_jpeg\JPEG_example_down.jpg";
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
}
