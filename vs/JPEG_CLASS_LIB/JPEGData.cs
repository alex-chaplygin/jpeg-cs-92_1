﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>Базовый класс данных JPEG.</summary>
    public class JPEGData
    {
        /// <summary>Поток с данными сегмента.</summary>
        protected Stream MainStream;
        /// <summary>Маркер сегмента.</summary>
        protected MarkerType Marker;
        /// <summary>Длина сегмента.</summary>
        protected ushort Length;
        /// <summary>
        /// Создаёт экземпляр JPEGData, считывает маркер и длину сегмента, если она есть.
        /// </summary>
        /// <param name="s">Поток, на основе которого создается экземпляр класса.</param>
        public JPEGData(Stream s)
        {
            MainStream = s;
            Marker = (MarkerType)Read16();
            if (!(Marker >= MarkerType.RestartWithModEightCount0 && Marker <= MarkerType.EndOfImage)) Length = Read16();
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

        /// <summary>Читает 2 байта из потока и возвращает их в виде ushort.</summary>
        /// <returns>2 байта в виде ushort.</returns>
        protected ushort Read16()
        {
            ushort Data = (ushort)(MainStream.ReadByte());  //Чтение первого байта
            Data = (ushort)(Data << 8);            //Побитовый сдвиг первого байта
            Data += (ushort)(MainStream.ReadByte()); //Чтение второго байта
            return Data;
        }
        /// <summary>Получает на вход ushort, разделяет его на 2 байта и записывает в поток.</summary>
        /// <param name="data">ushort, который будет записан в поток.</param>
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
        /// <summary>Читает маркер, если маркер SOI, EOI или RSTm, то возвращает самого себя, иначе возвращает null.</summary>
        /// <returns>JPEGData или null, в зависимости от маркера.</returns>
        public JPEGData GetData()
        {
            Marker = (MarkerType)Read16();
            if (Marker >= MarkerType.RestartWithModEightCount0 && Marker <= MarkerType.EndOfImage) return this;
            else return null;
        }
    }
}
