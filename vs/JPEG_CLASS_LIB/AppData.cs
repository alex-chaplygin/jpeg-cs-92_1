using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    class AppData : JPEGData
    {
        public byte[] data;

        public AppData(Stream s):base(s)
        {
            s.Read(data,0,Lenght-2);
        }

        public void Write(Stream s)
        {
            s.Write(data, 0, data.Length);
        }
    }
}
