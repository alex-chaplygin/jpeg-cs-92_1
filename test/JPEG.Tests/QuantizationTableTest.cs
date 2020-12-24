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
				QuantizationTable QT = new QuantizationTable(S, LQT, 3, 3, (ushort)LQT.Length);
				
				QT.Print();
				byte[] aaa = S.ToArray();
				Console.WriteLine("Значения потока после записи в него\n");
				Console.Write(Convert.ToString(((aaa[0]<<8)+aaa[1]), 16)+ " "+ Convert.ToString((aaa[2] << 8) + aaa[3], 16)+" ");
				for (int i=4; i<aaa.Length; i++)
                {
					Console.Write(aaa[i] + " ");
                }

			}
			
		}

	}
}
