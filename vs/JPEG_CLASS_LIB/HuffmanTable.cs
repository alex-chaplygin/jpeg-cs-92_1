using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Коды Хаффмана для энторпийного кодирования и декодирования
    /// </summary>
    class HuffmanTable : JPEGData
    {
        /// <summary>
        /// Массив длин кодов таблицы Хаффмана
        /// </summary>
        byte[] codeLength = new byte[16];
        /// <summary>
        /// Массив значений кодов таблицы Хаффмана
        /// </summary>
        byte[] values;
        /// <summary>
        /// Класс таблицы, 0 - DC, 1 - AC
        /// </summary>
        byte Tc;
        /// <summary>
        /// Номер таблицы
        /// </summary>
        byte Th;
        /// <summary>
        /// Массив длинн кодов таблицы Хаффмана
        /// </summary>
        byte[] HUFFSIZE;
        /// <summary>
        ///Массив длин значений таблицы Хаффмана
        /// </summary>
        byte[] BITS;
        /// <summary>
        /// Массив кодов таблицы Хаффмана для соответствия значениям
        /// </summary>
        byte[] HUFFCODE;
        /// <summary>
        /// Массив кодов, отсортированный по значениям
        /// </summary>
        byte[] EHUFCO;
        /// <summary>
        /// Массив длин кодов, отсортированный по значениям
        /// </summary>
        byte[] EHUFSI;
        /// <summary>
        /// Массив значений кодов таблицы Хаффмана
        /// </summary>
        byte[] HUFFVAL;
        /// <summary>
        /// Последний элемент в таблице кодов (число кодов)
        private int LASTK;
        
        /// <summary>
        /// Конструктор класса HuffmanTable. Создает таблицу Хаффмана на основе данных из потока 
        /// </summary>
        /// <param name="s">Поток на основе данных из которого создается таблица Хаффмана</param>
        public HuffmanTable(Stream s) : base(s, MarkerType.DefineHuffmanTables)
        {
            byte value = (byte)stream.ReadByte();
            Tc = (byte)(value >> 4);
            Th = (byte)(value & 0x0F);
            byte all_length_values = 0;
            for (int i = 0; i < 16; i++)
            {
                codeLength[i] = (byte)stream.ReadByte();
                all_length_values += codeLength[i];
            }
            values = new byte[all_length_values];
            for (int i = 0; i < all_length_values; i++)
            {
                values[i] = (byte)stream.ReadByte();
            }
            Generate_size_table(codeLength, all_length_values);
            Generate_code_table(all_length_values);
            Order_codes();
            
        }
        
        /// <summary>
        /// Генерирует массив длин кодов таблицы Хаффмана, чтобы каждый элемент соответствовал значению
        /// </summary>
        /// <param name="codeLength">Массив длинн значений таблицы Хаффмана</param>
        /// <param name="all_length_values">Количество всех значений (values) в таблице Хаффмана</param>
        private void Generate_size_table(byte[]codeLength, int all_length_values)
        {
            int K = 0;
            byte I = 1;
            int J = 1;
            BITS = codeLength;
            HUFFSIZE = new byte[all_length_values + 1];
            do
            {
                while (!(J > BITS[I - 1]))
                {
                    HUFFSIZE[K] = I;
                    K++;
                    J++;
                }
                I++;
                J = 1;
            }
            while (!(I > 16));
            HUFFSIZE[K] = 0;
            LASTK = K;
        }
        
        /// <summary>
        /// Генерирует массив кодов Хаффмана
        /// </summary>
        /// <param name="all_length_values">количество всех значений (values) в таблице Хаффмана</param>
        private void Generate_code_table(int all_length_values)
        {
            int K = 0;
            byte CODE = 0;
            byte SI = HUFFSIZE[0];
            HUFFCODE = new byte[all_length_values];
            do
            {
                do
                {
                    HUFFCODE[K] = CODE;
                    CODE++;
                    K++;
                }
                while (HUFFSIZE[K] == SI);
                if (HUFFSIZE[K] == 0)
                {
                    break;
                }
                do
                {
                    CODE = (byte)(CODE << 1);
                    SI++;
                }
                while (HUFFSIZE[K] != SI);
            }
            while (HUFFSIZE[K] == SI);
        }
        
        /// <summary>
        /// Создается ассоциативный массив (2 массива EHUFCO и EHUFSI размером 256 байт, но не все ячейки заполнены), для каждого значения запоминается битовый код и размер кода
        /// </summary>
        private void Order_codes()
        {
            byte I;
            int K = 0;
            EHUFCO = new byte[256];
            EHUFSI = new byte[256];
            HUFFVAL = values;
            do
            {
                I = HUFFVAL[K];
                EHUFCO[I] = HUFFCODE[K];
                EHUFSI[I] = HUFFSIZE[K];
                K++;
            }
            while (K < LASTK);
        }
        
        /// <summary>
        /// Записывает в поток все данные из таблицы Хаффмана
        /// </summary>
        /// <param name="s">Поток, в который происходит запись</param>
        public void Write(ref Stream s)
        {
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
        /// Выводит в консоль данные из таблицы Хаффмана
        /// </summary>
        override public void Print()
        {
            base.Print();
            Console.WriteLine("Tc: " + Tc + " Th: " + Th + " ");
            Console.WriteLine("Длинны кодов(codeLength): ");
            foreach(byte i in codeLength)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nЗначения(values): ");
            foreach (byte i in values)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nEHUFCO:");
            foreach (byte i in EHUFCO)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nEHUFSI:");
            foreach (byte i in EHUFSI)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nHUFFSIZE:");
            foreach (byte i in HUFFSIZE)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nHUFFCODE:");
            foreach (byte i in HUFFCODE)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nHUFFVAL:");
            foreach (byte i in HUFFVAL)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nBITS:");
            foreach (byte i in BITS)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();
        }
    }
}
