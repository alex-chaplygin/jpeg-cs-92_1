using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class FrameTest
    {
        [TestMethod]
        public void FrameWriteTest()
        {
            MemoryStream TestStream = new MemoryStream();
            Frame.Component[] Comp = new Frame.Component[3];
            Comp[0].Number = 1;
            Comp[0].H = 2;
            Comp[0].V = 2;
            Comp[0].QuantizationTableNumber = 0;
            Comp[1].Number = 2;
            Comp[1].H = 1;
            Comp[1].V = 1;
            Comp[1].QuantizationTableNumber = 1;
            Comp[2].Number = 3;
            Comp[2].H = 1;
            Comp[2].V = 1;
            Comp[2].QuantizationTableNumber = 1;
            Frame TestFrame = new Frame(TestStream, MarkerType.BaseLineDCT, 160, 120, 8, Comp);
            TestFrame.Write();
            byte[] TestArr = TestStream.ToArray();
            byte[] exArr = new byte[] { 0xFF, 0xC0, 0x00, 0x11, 0x08, 0x00, 0x78, 0x00, 0xA0, 0x03, 0x01, 0x22, 0x00, 0x02, 0x11, 0x01, 0x03, 0x11, 0x01 };
            //Сделано по Frame из JPEG_example_down, позиция 0x204
            CollectionAssert.AreEqual(TestArr, exArr);
        }
    }
}
