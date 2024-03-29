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
        public Stream MainStream;

        /// <summary>Маркер сегмента.</summary>
        public MarkerType Marker;

        /// <summary>Длина сегмента.</summary>
        public ushort Length;

        /// <summary>
        /// Создаёт экземпляр JPEGData, считывает маркер и длину сегмента, если она есть.
        /// </summary>
        /// <param name="s">Поток, на основе которого создается экземпляр класса.</param>
        /// <param name="type">Маркер, сегмента.</param>
        public JPEGData(Stream s, MarkerType type)
        {
            MainStream = s;
            Marker = type;
            if (!(Marker >= MarkerType.RestartWithModEightCount0 && Marker <= MarkerType.EndOfImage)) Length = Read16();
        }

        /// <summary>
        /// Конструктор вызывается при создании новой структуры данных JPEG для записи в файл
        /// </summary>
        /// <param name="type">Маркер сегмента</param>
        public JPEGData(MarkerType type)
        {
            Marker = type;
        }

        /// <summary>
        /// Конструктор, который устанавливает тип маркера и длину(для последующей записи)
        /// </summary>
        /// <param name="s">Поток сегмента</param>
        /// <param name="type">Маркер сегмента</param>
        /// <param name="length">Длина сегмента</param>
        public JPEGData(Stream s, MarkerType type, ushort length)
        {
            MainStream = s;
            Marker = type;
            Length = length;
        }

        /// <summary>
        /// Записывает маркер и длину в текущий поток.
        /// </summary>
        public virtual void Write()
        {
            Write16((ushort)Marker);
            if (!(Marker >= MarkerType.RestartWithModEightCount0 && Marker <= MarkerType.EndOfImage)) Write16(Length) ;
        }

        /// <summary>
        /// Записывает маркер и длину в поток.
        /// </summary>
        /// <param name="s">Поток для записи.</param>
        public virtual void Write(Stream s)
        {
            Write16(s, (ushort)Marker);
            if (!(Marker >= MarkerType.RestartWithModEightCount0 && Marker <= MarkerType.EndOfImage)) Write16(s, Length);
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

        /// <summary>
        /// Получает на вход два байта с заполненными младшими четырьмя битами, 
        /// объединяет их в один байт и записывает в поток.
        /// </summary>
        /// <param name="v1">Байт, младшие 4 бита которого буду записаны как старшие биты байта в поток.</param>
        /// <param name="v2">Байт, младшие 4 бита которого буду записаны как младшие биты байта в поток.</param>
        /// <param name="s">Поток для записи.</param>
        protected void Write4(Stream s, byte v1, byte v2)
        {
            s.WriteByte((byte)((v1 << 4) + v2));
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

        /// <summary>Читает 2 байта из потока и возвращает их в виде ushort.</summary>
        /// <param name="s">Поток, из которого будет производиться чтение.</param>
        /// <returns>2 байта в виде ushort.</returns>
        protected static ushort Read16(Stream s)
        {
            ushort Data = (ushort)(s.ReadByte());  //Чтение первого байта
            Data = (ushort)(Data << 8);            //Побитовый сдвиг первого байта
            Data += (ushort)(s.ReadByte()); //Чтение второго байта
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

        /// <summary>Получает на вход ushort, разделяет его на 2 байта и записывает в поток.</summary>
        /// <param name="data">ushort, который будет записан в поток.</param>
        /// <param name="s">Поток для записи.</param>
        protected void Write16(Stream s, ushort data)
        {
            byte Lbyte = (byte)(data);      //Чтение младшего байта из ushort
            byte Hbyte = (byte)(data >> 8); //Чтение старшего байта из ushort
            s.WriteByte(Hbyte);    //Запись старшего байта в поток
            s.WriteByte(Lbyte);    //Запись младшего байта в поток
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

        /// <summary>Читает маркер и в зависимости от типа маркера создает нужную структуру данных JPEG</summary>
        /// <returns>Cтруктура данных JPEG.</returns>
        public static JPEGData GetData(Stream s)
        {
            MarkerType Marker = (MarkerType)JPEGData.Read16(s);
            if (Marker == MarkerType.DefineHuffmanTables) return new HuffmanTable(s);
            else if (Marker == MarkerType.StartOfScan) return new Scan(s);
            else if (Marker == MarkerType.Comment) return new Comment(s);
            else if (Marker >= MarkerType.ReservedForApplicationSegments && Marker < MarkerType.ReservedForJPEGExtentions) return new AppData(s, Marker);
            else if (Marker == MarkerType.DefineNumberOfLines) return new DNL(s);
            else if (Marker >= MarkerType.BaseLineDCT && Marker <= MarkerType.DifferentialLoslessArithmetic) return new Frame(s, Marker);
            else if (Marker == MarkerType.DefineQuantizationTables) return new QuantizationTable(s);
            else if (Marker == MarkerType.DefineArithmeticCodingConditionings) return new ArithmeticConditioning(s);
            else if (Marker == MarkerType.DefineRestartInterval) return new RestartInterval(s);
            else return new JPEGData(s, Marker);
        }

        /// <summary>Выводит в консоль поля класса.</summary>
        public virtual void Print()
        {
            Console.WriteLine($"Маркер сегмента: {(int)Marker:X4} {Marker}");
            Console.WriteLine($"Длина сегмента: {Length} {Length:X2}");
        }
    }
}
