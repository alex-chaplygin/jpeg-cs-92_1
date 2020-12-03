using System;
using System.IO;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Коды Хаффмана для энторпийного кодирования и декодирования
    /// </summary>
    public class HuffmanTable : JPEGData
    {
        /// <summary>
        /// Массив длинн кодов таблицы Хаффмана
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
        /// Массив длин кодов таблицы Хаффмана для соответствия значениям
        /// </summary>
        byte[] HUFFSIZE;

        /// <summary>
        ///Массив длин кодов таблицы Хаффмана
        /// </summary>

        public byte[] BITS;
        /// <summary>
        /// Массив кодов таблицы Хаффмана для соответствия значениям
        /// </summary>

        public byte[] HUFFCODE;
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
        public byte[] HUFFVAL;

        /// <summary>
        /// Последний элемент в таблице кодов (число кодов)
        private int LASTK;

        /// <summary>
        /// Содержит самый большой код для длины I (индекс массива)
        /// </summary>
        public int[] MaxCode = new int[16];

        /// <summary>
        /// Содержит самый маленький код для длины I (индекс массива)
        /// </summary>
        public int[] MinCode = new int[16];

        /// <summary>
        /// Содержит индекс для начала списка значений в массиве HUFFVAL для декодирования кода длиной I (индекс массива)
        /// </summary>
        public byte[] VALPTR = new byte[16];


        /// <summary>
        /// Конструктор класса HuffmanTable. Создает таблицу Хаффмана на основе данных из потока 
        /// </summary>
        /// <param name="s">Поток на основе данных из которого создается таблица Хаффмана</param>
        public HuffmanTable(Stream s) : base(s, MarkerType.DefineHuffmanTables)
        {
            byte value = (byte)MainStream.ReadByte();
            Tc = (byte)(value >> 4);
            Th = (byte)(value & 0x0F);
            int all_length_values = 0;
            for (int i = 0; i < 16; i++)
            {
                codeLength[i] = (byte)MainStream.ReadByte();
                all_length_values += codeLength[i];
            }

            values = new byte[all_length_values];
            for (int i = 0; i < all_length_values; i++)
            {
                values[i] = (byte)MainStream.ReadByte();
            }

            Generate_size_table(codeLength, all_length_values);
            Generate_code_table(all_length_values);
            Order_codes();
	    GenerateTables();
        }

        /// <summary>
        /// Генериует вспомогательные таблицы для декодирования
        /// </summary>
        public void GenerateTables()
        {
            int i = -1;//0
            byte j = 0;
            MaxCode = new int[16];
            MinCode = new int[16];
            bool Done = false;
            do
            {
                i++;
                if (i >= 16) { Done = true; break; }//>
                while ((BITS[i] == 0) && (!Done))
                {
                    MaxCode[i] = -1;
                    i++;
                    if (i >= 16)//>
                    {
                        Done = true;
                        break;
                    }
                }
                if (Done) break;
                VALPTR[i] = j;
                MinCode[i] = HUFFCODE[j];
                j = Convert.ToByte(j + BITS[i] - 1);
                MaxCode[i] = HUFFCODE[j];
                j++;
            }
            while ((BITS[i] != 0));
        }

        /// <summary>
        /// Генерирует массив длин кодов таблицы Хаффмана, чтобы каждый элемент соответствовал значению
        /// </summary>
        /// <param name="codeLength">Массив длинн значений таблицы Хаффмана</param>
        /// <param name="all_length_values">Количество всех значений (values) в таблице Хаффмана</param>
        private void Generate_size_table(byte[] codeLength, int all_length_values)
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
            } while (!(I > 16));

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
                } while (HUFFSIZE[K] == SI);

                if (HUFFSIZE[K] == 0)
                {
                    break;
                }

                do
                {
                    CODE = (byte)(CODE << 1);
                    SI++;
                } while (HUFFSIZE[K] != SI);
            } while (HUFFSIZE[K] == SI);
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
            } while (K < LASTK);
        }

        /// <summary>
        /// Записывает в поток все данные из таблицы Хаффмана
        /// </summary>
        public override void Write()
        {
            base.Write();
            MainStream.WriteByte((byte)((Tc << 4) + Th));
            foreach (byte i in codeLength)
            {
                MainStream.WriteByte(i);
            }

            foreach (byte i in values)
            {
                MainStream.WriteByte(i);
            }
        }

        /// <summary>
        /// Выводит в консоль данные из таблицы Хаффмана
        /// </summary>
        override public void Print()
        {
            base.Print();
            Console.WriteLine("Tc: " + Tc + " Th: " + Th + " ");
            Console.WriteLine("Длины кодов(codeLength): ");
            foreach (byte i in codeLength)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine("\nЗначения(values): ");
            foreach (byte i in values)
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
                Console.Write(Convert.ToString(i, 2) + " ");
            }

            Console.WriteLine("\nEHUFCO:");
            foreach (byte i in EHUFCO)
            {
                Console.Write(Convert.ToString(i, 2) + " ");
            }

            Console.WriteLine("\nEHUFSI:");
            foreach (byte i in EHUFSI)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine();
        }


        /// <summary>
        /// Создает новую таблицу Хаффмана на основе исходных коэффициентов. Генерирует таблицу значений кодов и таблицу длин кодов согласно алгоритмам в разделе K.2
        /// </summary>
        /// <param name="data">Список коэффицентов блоков 8x8 после DCT(сдвиг уровней, DCT, квантование, рассчет DC) и обхода зигзагом</param>
        /// <param name="DC">
        ///true - генерация дерева Хаффмана для DC коэффициентов, 
        ///false - генерация дерева Хаффмана для AC коэффициентов
        /// </param>
        public HuffmanTable(byte[] data, bool DC, byte table_id) : base(MarkerType.DefineHuffmanTables)
        {
            if (DC)
            {
                Tc = 0;
            }
            else
            {
                Tc = 1;
            }

            Th = table_id;
            var freq = new int[257];
            freq[256] = 1;
            foreach (var curByte in data)
            {
                freq[curByte]++;
            }

            int[] codesize = new int[257];
            int[] others = new int[257];

            int i;
            for (i = 0; i < 257; i++)
            {
                others[i] = -1;
            }
            Code_size(freq, codesize, others);
            byte[] bits = new byte[33];
            Count_bits(bits, codesize);
            Adjust_bits(bits);
            int allSize = 0;
            for (i = 0; i < 16; i++)
                allSize += bits[i];
            HUFFVAL = new byte[allSize];
            Sort_input(codesize);

            codeLength = bits;
            values = HUFFVAL;

            Generate_size_table(codeLength, allSize);
            Generate_code_table(allSize);
            Order_codes();

            Length = (ushort)(19 + values.Length); //2 + 1 + 16 + values.Length
        }

        /// <summary>
        /// Сортирует входные значения по размеру кодов (Схема K.4 стандарта)
        /// </summary>
        /// <param name="codesize">Массив размеров кодов</param>
        private void Sort_input(int[] codesize)
        {
            var k = 0;
            for (var i = 1; i <= 32; i++)
            {
                for (var j = 0; j <= 255; j++)
                {
                    if (codesize[j] == i)
                    {
                        HUFFVAL[k] = (byte)j;
                        k++;
                    }
                }
            }
        }

        /// <summary>
        /// Оптимизирует массив bits так, чтобы ни один код не был длиннее 16 битов (Схема K.3 стандарта)
        /// </summary>
        /// <param name="bits">Массив количества кодов каждого размера</param>
        private void Adjust_bits(byte[] bits)
        {
            int i;
            for (i = 32; i > 16; i--)
            {
                while (bits[i] > 0)
                {
                    var j = i - 1;
                    while (bits[j] == 0)
                    {
                        j--;
                    }

                    bits[i] -= 2;
                    bits[i - 1]++;
                    bits[j + 1] += 2;
                    bits[j]--;
                }
            }

            while (bits[i] == 0)
            {
                i--;
            }

            bits[i]--;
        }

        /// <summary>
        /// Определяет количество кодов для каждой из длин (Схема K.2 стандарта)
        /// </summary>
        /// <param name="bits">Массив количества кодов каждого размера</param>
        /// <param name="codesize">Массив размеров кодов</param>
        private void Count_bits(byte[] bits, int[] codesize)
        {
            for (var i = 0; i <= 256; i++)
            {
                if (codesize[i] != 0)
                {
                    bits[codesize[i]]++;
                }
            }
        }

        /// <summary>
        /// Находит размеры кода Хаффмана (Схема K.1 стандарта)
        /// </summary>
        /// <param name="freq">Массив частот встречаемости</param>
        /// <param name="codesize">Массив размеров кодов</param>
        /// <param name="others">Массив индексов следующего символа в цепочке всех символов текущей ветви дерева кода</param>
        void Code_size(int[] freq, int[] codesize, int[] others)
        {
            while (true)
            {
                var v1 = -1;
                var v = Int32.MaxValue;
                for (var i = 0; i <= 256; i++)
                {
                    if (freq[i] != 0 && freq[i] <= v)
                    {
                        v = freq[i];
                        v1 = i;
                    }
                }
                var v2 = -1;
                v = Int32.MaxValue;
                for (var i = 0; i <= 256; i++)
                {
                    if (freq[i] != 0 && freq[i] <= v && i != v1)
                    {
                        v = freq[i];
                        v2 = i;
                    }
                }
                if (v2 < 0)
                {
                    break;
                }
                freq[v1] += freq[v2];
                freq[v2] = 0;
                codesize[v1]++;
                while (others[v1] != -1)
                {
                    v1 = others[v1];
                    codesize[v1]++;
                }
                others[v1] = v2;
                codesize[v2]++;
                while (others[v2] != -1)
                {
                    v2 = others[v2];
                    codesize[v2]++;
                }
            }
        }
    }
}
