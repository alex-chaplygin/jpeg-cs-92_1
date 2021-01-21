using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using JPEG_CLASS_LIB;

namespace JPEG.Tests
{
    [TestClass]
    public class TestJpegFile
    {
        [TestMethod]
        public void TestDecodeScan()
        {
            using (FileStream s = File.Open("../../../test_jpeg/test.jpg", FileMode.Open))
            {
                JPEGFile JPEG = new JPEGFile(s);
                List<short[]> MixedBlocks = JPEG.DecodeScan();
                ushort Data = (ushort)(s.ReadByte());  
                Data = (ushort)(Data << 8);            
                Data += (ushort)(s.ReadByte());
                MarkerType Marker = (MarkerType)Data;
                if (Marker != MarkerType.EndOfImage) throw new Exception();
            }
        }
    }
}
