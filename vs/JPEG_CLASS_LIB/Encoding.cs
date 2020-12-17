using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс кодирования минимальных блоков данных MCU, состоящих из блоков 8x8, последовательностью бит.
    /// </summary>
    public class Encoding
    {
        /// Сжатые данные.
        /// </summary>
        Stream MainStream;

        /// <summary>
        /// Текущий байт в сжатых данных.
        /// </summary>
        byte B;

        /// <summary>
        /// Счетчик битов.
        /// </summary>
        int CNT = 0;

        /// <summary>
        /// Счетчик избыточных битов
        /// </summary>
        int extraBits = 0;
        
        /// <summary>
        /// Таблица Хаффмана для DC-коэффициентов
        /// </summary>
        HuffmanTable huffDC;

        /// <summary>
        /// Таблица Хаффмана для AC-коэффициентов
        /// </summary>
        HuffmanTable huffAC;

        /// <summary>
        /// Создает объект, сохраняет поток в классе.
        /// </summary>
        /// <param name="s">Поток сжатых данных.</param>
        public Encoding(Stream s, HuffmanTable huffDC, HuffmanTable huffAC)
        {
            MainStream = s;
            B = 0;
            CNT = 0;
            this.huffDC = huffDC;
            this.huffAC = huffAC;
        }

        /// <summary>
        /// Выполняет кодирование блока коэффициентов в поток с помощью таблиц Хаффмана
        /// </summary>
        /// <param name="block">Коэффициенты блока 8x8 после обхода зигзагом</param>
        public void EncodeBlock(short[] block)
        {
            
            //F.1.1.5.1: At the beginning of the scan and at the beginning of each restart interval, the prediction for the DC coefficient prediction is initialized to 0
            //short pred = 0;  
            
            short diff = block[0];

            var ssss = ComputeDCCategory(diff);
            
            Console.WriteLine($"diff: {diff}, ssss: {ssss}");

            
            WriteBits(huffDC.EHUFCO[ssss], huffDC.EHUFSI[ssss]);

            if (ssss != 0)
            {
                WriteBits((ushort) (diff > 0 ? diff : diff-1), ssss);
            }
            
            EncodeAC(block);
        } 
        
        /// <summary>
        /// Кодирует AC-коэффициенты в поток (Figure F.2)
        /// </summary>
        /// <param name="block">Блок коэффициентов</param>
        public void EncodeAC(short[] block)
        {
            int k = 0;
            int r = 0;

            while (true)
            {
                k++;
                if (block[k] == 0)
                {
                    if (k == 63)
                    {
                        WriteBits(huffAC.EHUFCO[0], huffAC.EHUFSI[0]);
                        break;
                    }
                    r++;
                }
                else
                {
                    while (r > 15)
                    {
                        WriteBits(huffAC.EHUFCO[0xF0], huffAC.EHUFSI[0xF0]);
                        r -= 16;
                    }

                    encodeR(r, block[k]);
                    r = 0;
                    if (k==63) break;
                }
            }
        }

        /// <summary>
        /// Выполняет последовательное кодирование ненулевых AC-коэффициентов (Figure F.3)
        /// </summary>
        /// <param name="r">run-length коэффициент</param>
        /// <param name="cur">Коэффициент ZZ(k) из блока после обхода зигзагом</param>
        void encodeR(int r, short cur)
        {
            var ssss = ComputeACCategory(cur);
            var rs = 16 * r + ssss;
            WriteBits(huffAC.EHUFCO[rs], huffAC.EHUFSI[rs]);
            if (cur < 0)
            {
                cur--;
            }
            WriteBits((ushort) cur, ssss);
        }

        /// <summary>
        /// Вычисляет категории для каждой разницы DC коэффициентов для генерации таблиц Хаффмана. (F.1.2.1.2)
        /// </summary>
        /// <param name="data">Список блоков 8x8 после DCT и обхода зигзагом.</param>
        /// <returns>Массив категорий для всех DC</returns>
        public static byte[] GenerateDC(List<short[]> data)
        {
            byte[] DCCategories = new byte[data.Count];
            
            for (int i = 0; i < data.Count; i++)
            {
                DCCategories[i] = Convert.ToByte(ComputeDCCategory(data[i][0]));
            }

            return DCCategories;
        }

        /// <summary>
        /// Вычисляет коды для AC коэффициентов (F.1.2.2.1).
        /// Для каждого ненулевого AC генерируется составной байт, RRRRSSSS (2 по 4 бита).
        /// SSSS - категория для AC коэффициента, RRRR - смещение текущего коэффициента относительно предыдущего (число нулей между предыдущим и текущим ненулевыми AC).
        /// Если число нулей больше 15, генерируется код 0xF0 (15 нулей).
        /// Если все оставшиеся коэффициенты равны нулю, то генерируется код 0x00 (конец блока).
        /// Все сгенерированные коды идут последовательно для всех блоков.
        /// </summary>
        /// <param name="data">Список блоков 8x8 после DCT и обхода зигзагом.</param>
        /// <returns>Массив последовательных кодов для AC коэффициентов всех блоков</returns>
        public static byte[] GenerateAC(List<short[]> data)
        {
            byte zeroFinalCounter = 0;
            List<byte> ACCodes = new List<byte>();
            
            for (int i = 0; i < data.Count; i++)
            {
                byte zeroRipCounter = 0;
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (data[i][j] == 0)
                    {
                        zeroFinalCounter++;
                        zeroRipCounter++;
                    }
                    else
                    {
                        while (zeroRipCounter > 15) { ACCodes.Add(0xF0); zeroRipCounter -= 15; };
                        byte temp = (byte)(zeroRipCounter << 4);
                        temp += ComputeACCategory(data[i][j]);
                        ACCodes.Add(temp);
                        zeroRipCounter = 0;
                        zeroFinalCounter = 0;
                    }
                }
                if (zeroFinalCounter > 0) ACCodes.Add(0x00);
                zeroFinalCounter = 0;
            }

            byte[] a = ACCodes.ToArray();

            return a;
        }

        /// <summary>
        /// Список абсолютных значений разности DC коэффициентов (diff) для определения категории.
        /// </summary>
        private static readonly short[] DIFFValueList
            = new short[] { 0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047 };

        /// <summary>
        /// Определяет категорию для разности DC коэффициентов.
        /// </summary>
        /// <param name="diff">Разность значений для определения категории.</param>
        /// <returns>Номер категории.</returns>
        private static short ComputeDCCategory(short diff)
        {
            if (diff < -2047 || diff > 2047) throw new Exception("Значение diff должно быть в диапазоне [-2047; 2047]");

            short category = 0;
            diff = Abs(diff);
            while (diff > DIFFValueList[category]) category++;

            return category;
        }

        /// <summary>
        /// Вычисляет категорию АС коэффициента
        /// </summary>
        /// <param name="data">Входное значение из таблицы</param>
        /// <returns>Категория АС коэффициента</returns>
        private static byte ComputeACCategory(short value)
        {
            value = Math.Abs(value);
            if (value == 1) return 1;
            for (byte i = 2; i<= 10; i++)
            {
                if (value < Math.Pow(2, i)) return i;
            }
            throw new Exception("Значение для вычисления категории АС должно быть в диапазоне [-1023;1023 ]");
        }

        /// <summary>
        /// Опредеяет абсолютное значение числа.
        /// </summary>
        /// <param name="value">Число для нахождения абсолютного значения.</param>
        /// <returns>Абсолютное значение числа.</returns>
        private static short Abs(short value) =>
            (value < 0) ?  (short)-value : value;

        /// <summary>
        /// Записывает в поток последовательность бит, начиная со старшего (7) и по направлению к младшему (0).
        /// Если в текущем для записи байте все биты равны единице, то после него записывается байт 0x00 (stuff byte), 
        /// чтобы отличать битовый поток от маркеров.
        /// </summary>
        /// <param name="bits">Последовательность бит для записи.</param>
        /// <param name="num">Число бит (от 1 до 8).</param>
        public void WriteBits(byte bits, int num)
        {
            bits = (byte)(bits & ((1 << num) - 1));
            if (num + CNT > 8)
            {
                extraBits = num + CNT - 8;
                B += (byte)(bits >> extraBits);
                CNT = 8;
            }
            else
            {
                B += (byte)(bits << (8 - CNT - num));
                extraBits = 0;
                CNT += num;
            }

            if (CNT == 8)
            {
                MainStream.WriteByte(B);
                if (B == 0xFF) MainStream.WriteByte(0x00);
                B = 0;
                CNT = 0;
                if (extraBits > 0)
                    WriteBits((byte)(bits & ((1 << extraBits) - 1)), extraBits);
            }
        }

        /// <summary>
        /// Записывает в поток последовательность бит, начиная со старшего (15) и по направлению к младшему (0).
        /// Если в текущем для записи байте все биты равны единице, то после него записывается байт 0x00 (stuff byte), 
        /// чтобы отличать битовый поток от маркеров.
        /// </summary>
        /// <param name="bits">Последовательность бит для записи.</param>
        /// <param name="num">Число бит (от 1 до 16).</param>
        public void WriteBits(ushort bits, int num)
        {
            if (num > 8)
            {
                WriteBits((byte)(bits >> 8), num - 8);
                num = 8;
            }

            WriteBits((byte)(bits & 0xFF), num);
        }

        /// <summary>
        /// Записывает в поток последний незаполненный байт, дополнив его нужным числом нулевых бит.
        /// </summary>
        public void FinishBits() 
        {
            if (CNT > 0)
            {
                MainStream.WriteByte(B);
                B = 0;
                CNT = 0;
            }
        }
    }
}
