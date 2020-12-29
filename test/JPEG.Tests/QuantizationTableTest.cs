using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace JPEG.Tests
{
    [TestClass]
    public class QuantizationTableTest
    {
		[TestMethod]
		public void QuantizationTableWriteTest()
		{
			using (MemoryStream S = new MemoryStream())
			{
				short[,] LQT =
				{
			{16, 12, 14, 14, 18, 24, 49, 72},
			{11, 12, 13, 17, 22, 35, 64, 92},
			{10, 14, 16, 22, 37, 55, 78, 95},
			{16, 19, 24, 29, 56, 64, 87, 98},
			{24, 26, 40, 51, 68, 81, 103, 112},
			{40, 58, 57, 87, 109, 104, 121, 100},
			{51, 60, 69, 80, 103, 113, 120, 103},
			{61, 55, 56, 62, 77, 92, 101, 99}
				};
				byte[] etalon = { 255, 219, 0, 68, 3, 3, 16, 12, 11, 10, 12, 14, 14, 13, 14, 16, 24, 19, 16, 17, 18, 24, 22, 22, 24, 26, 40, 51, 58, 40, 29, 37, 35, 49, 72, 64, 55, 56, 51, 57, 60, 61, 55, 69, 87, 68, 64, 78, 92, 95, 87, 81, 109, 80,
					56, 62, 103, 104, 103, 98, 112, 121, 113, 77, 92, 120, 100, 103, 101, 99 };
				
				QuantizationTable QT = new QuantizationTable(S, LQT, 3, 3);
				byte[] a = S.ToArray();
				
				CollectionAssert.AreEqual(etalon, a);

			}
			
		}

	}
}
