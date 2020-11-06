using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JPEG_CLASS_LIB
{
    class HuffmanTable : JPEGData
    {
        byte[] codeLength = new byte[16];
        byte[] values;
        byte Tc;
        byte Th;
        //byte all_length_values;
        //Stream stream;
        /// <summary>
        /// Конструктор класса HuffmanTable. Создает таблицу Хаффмена на основе данных из потока 
        /// </summary>
        /// <param name="s">Поток на основе данных из которого создается таблица Хаффмена</param>
        public HuffmanTable(Stream s) : base (s, MarkerType.DefineHuffmanTables)
        {
            Stream stream = s;
            
            byte value = (byte)stream.ReadByte();
            Tc = (byte)(value >> 4);
            Th = (byte)(value & 0x0F);
            byte all_length_values = 0;
            for (int i = 0; i<16; i++)
            {
                codeLength[i] = (byte)stream.ReadByte();
                all_length_values += codeLength[i];
            }
            values = new byte[all_length_values];
            for (int i = 0; i<all_length_values; i++)
            {
                values[i] = (byte)stream.ReadByte();
            }

            
        }
        /// <summary>
        /// Записывает в поток данные из таблицы Хаффмена
        /// </summary>
        /// <param name="s">Поток, в который происходит запись</param>
        public void Write(ref Stream s)
        {
            //s = stream;
            s.WriteByte((byte)((Tc << 4) + Th));
            foreach(byte i in codeLength)
            {
                s.WriteByte(i);
            }
            foreach(byte i in values)
            {
                s.WriteByte(i);
            }
            
        }
        /// <summary>
        /// Выводит в консоль данные из таблицы Хаффмена
        /// </summary>
        override public void Print()
        {
            base.Print();
            Console.Write("Tc: " + Tc + " Th: " + Th + " ");
            Console.WriteLine("Длинны кодов(codeLength): ");
            foreach(byte i in codeLength)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("Значения(values): ");
            foreach (byte i in values)
            {
                Console.Write(i + " ");
            }
        }
    }
}
