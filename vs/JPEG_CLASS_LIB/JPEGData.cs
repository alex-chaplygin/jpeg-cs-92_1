using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    public class JPEGData
    {
        Stream MainStream;
        public ushort Marker;
        public JPEGData(Stream s)
        {
            MainStream = s;
        }
        /// <summary>Читает 2 байта и возвращает их в виде ushort</summary>
        /// <returns>2 байта в виде ushort</returns>
        public ushort Read16()
        {
            Marker = (ushort)(MainStream.ReadByte());  //Чтение первого байта
            Marker = (ushort)(Marker << 8);            //Побитовый сдвиг первого байта
            Marker += (ushort)(MainStream.ReadByte()); //Чтение второго байта
            return Marker;
        }
        /// <summary>Получает на вход ushort, разделяет его на 2 байта и записывает в поток</summary>
        public void Write16(ushort data)
        {
            byte Lbyte = (byte)(data);      //Чтение младшего байта из ushort
            byte Hbyte = (byte)(data >> 8); //Чтение старшего байта из ushort
            MainStream.WriteByte(Hbyte);    //Запись старшего байта в поток
            MainStream.WriteByte(Lbyte);    //Запись младшего байта в поток
        }
    }
}
