using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
	/// <summary>
	/// class for интервал повтора - число MCU - minimum coding unit
	/// </summary>
	public class RestartInterval : JPEGData
	{
		public ushort restartInterval;
		//читает интервал повтора из потока
		public RestartInterval(Stream s) : base(s, MarkerType.DefineRestartInterval)
		{
			//use method Read16 from JPEGData
			restartInterval = Read16();
		}
		//пишет интервал повтора из потока
		public override void Write()
		{
			base.Write();
			//use method Write16 from JPEGData
			Write16(restartInterval);
		}
	}
}
