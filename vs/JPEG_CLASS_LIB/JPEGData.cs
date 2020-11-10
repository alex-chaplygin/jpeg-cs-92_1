using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    public class JPEGData
    {
        protected Stream MainStream;
        public ushort Marker;

        /// <summary>
        /// Длина сегмента.
        /// </summary>
        protected ushort Length { get; protected private set; }

        public JPEGData(Stream s)
        {
            MainStream = s;
        }

        /// <summary>
        /// Читает  1 байт, разбивает его по 4 бита, которые записывает в младшие разряды двух байтов.
        /// </summary>
        /// <param name="v1">Байт, в который будут записаны 4 старших бита из считываемого байта.</param>
        /// <param name="v2">Байт, в который будут записаны 4 младших бита из считываемого байта.</param>
        protected void Read4(out byte v1, out byte v2)
        {
            byte value = (byte)MainStream.ReadByte(); // Чтение байта, который будет разбит по 4 бита.
            v1 = (byte)(value >> 4);   // В v1 записываем старшие четыре бита из байта.
            v2 = (byte)(value & 0x0F); // В v2 записываем младшие четыре бита из байта.
        }
        /// <summary>
        /// Получает на вход два байта с заполненными младшими четырьмя битами, 
        /// объединяет их в один байт и записывает в поток.
        /// </summary>
        /// <param name="v1">Байт, младшие 4 бита которого буду записаны как старшие биты байта в поток.</param>
        /// <param name="v2">Байт, младшие 4 бита которого буду записаны как младшие биты байта в поток.</param>
        protected void Write4(byte v1, byte v2)
        {
            MainStream.WriteByte((byte)((v1 << 4) + v2));
        }

        /// <summary>Читает 2 байта и возвращает их в виде ushort</summary>
        /// <returns>2 байта в виде ushort</returns>
        protected ushort Read16()
        {
            Marker = (ushort)(MainStream.ReadByte());  //Чтение первого байта
            Marker = (ushort)(Marker << 8);            //Побитовый сдвиг первого байта
            Marker += (ushort)(MainStream.ReadByte()); //Чтение второго байта
            return Marker;
        }
        /// <summary>Получает на вход ushort, разделяет его на 2 байта и записывает в поток</summary>
        protected void Write16(ushort data)
        {
            byte Lbyte = (byte)(data);      //Чтение младшего байта из ushort
            byte Hbyte = (byte)(data >> 8); //Чтение старшего байта из ushort
            MainStream.WriteByte(Hbyte);    //Запись старшего байта в поток
            MainStream.WriteByte(Lbyte);    //Запись младшего байта в поток
        }

        /// <summary>Читает 4 байта и возвращает их в виде uint.</summary>
        /// <returns>4 байта в виде uint.</returns>
        protected uint Read32()
        {
            uint Marker = 0;
            Marker += (uint)(MainStream.ReadByte());  // Чтение первого байта.
            Marker = (Marker << 8);                   // Побитовый сдвиг первого байта.
            Marker += (uint)(MainStream.ReadByte());  // Чтение второго байта.
            Marker = (Marker << 8);                   // Побитовый сдвиг второго байта.
            Marker += (uint)(MainStream.ReadByte());  // Чтение третьего байта.
            Marker = (Marker << 8);                   // Побитовый сдвиг третьего байта.
            Marker += (uint)(MainStream.ReadByte());  // Чтение четвертого байта.
            return Marker;
        }
        /// <summary>Получает на вход uint, разделяет его на 4 байта и записывает в поток.</summary>
        protected void Write32(uint data)
        {
            MainStream.WriteByte((byte)(data >> 24)); // Запись первого байта из uint в поток.
            MainStream.WriteByte((byte)(data >> 16)); // Запись второго байта из uint в поток.
            MainStream.WriteByte((byte)(data >> 8));  // Запись третьего байта из uint в поток.
            MainStream.WriteByte((byte)data);         // Запись четвертого байта из uint в поток.

        }
    }
}
