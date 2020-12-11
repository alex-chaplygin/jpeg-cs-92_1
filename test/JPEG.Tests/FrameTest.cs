using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JPEG_CLASS_LIB;
using System.IO;
namespace JPEG.Tests
{
	
	[TestClass]
	public class FrameTest
	{
		[TestMethod]
		public void TestFrame_Write()
		{
			byte[] strbyte = new byte[] { 0,0,0,0,0,0xc,0,0x50,0,0x3c,0x2a,2,2,0x44,8,1,0x33,9 };
			MarkerType type = new MarkerType();
			ushort width = 80;
			ushort height = 60;
			byte numBit = 0x2a;//42
			byte numComponent = 0x2;//2
			Frame.Component[] components = new Frame.Component[numComponent];
			components[0].Number = 2;
			components[0].H = 4;
			components[0].V = 4;
			components[0].QuantizationTableNumber = 8;
			components[1].Number = 1;
			components[1].H = 3;
			components[1].V = 3;
			components[1].QuantizationTableNumber = 9;
			MemoryStream stream = new MemoryStream();
			Frame frame_Test_Write = new Frame(stream, type,width,height,numBit,numComponent,components);
			frame_Test_Write.Write();
			byte[] newstrbyte = stream.ToArray();
			CollectionAssert.AreEqual(newstrbyte, strbyte);
			Assert.AreEqual(frame_Test_Write.Width, 80);
			Assert.AreEqual(frame_Test_Write.Height, 60);
			Assert.AreEqual(frame_Test_Write.NumberIfBits, 0x2a);
			Assert.AreEqual(frame_Test_Write.NumberOfComponent, 0x2);
		}
	}
}
