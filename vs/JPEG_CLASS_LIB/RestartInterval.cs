using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
	public class RestartInterval : JPEGData
	{
		public ushort restartInterval;
		public RestartInterval(Stream s) : base(s)
		{
			restartInterval = Read16();
		
		}
		public void Write()
		{
			Write16(restartInterval);
		}
	}
}
