using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс кодирования минимальных блоков данных MCU, состоящих из блоков 8x8, последовательностью бит.
    /// </summary>
    public class Encoding
    {
        /// <summary>
        /// Вычисляет категории для каждой разницы DC коэффициентов для генерации таблиц Хаффмана. (F.1.2.1.2)
        /// </summary>
        /// <param name="data">Список блоков 8x8 после DCT и обхода зигзагом.</param>
        /// <returns>Массив категорий для всех DC</returns>
        public static byte[] EncodeDC(List<short[]> data)
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
        public static byte[] EncodeAC(List<short[]> data)
        {
            byte zerosCounter = 0;
            List<byte> ACCodes = new List<byte>();
            
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (data[i][j] == 0) { zerosCounter++; continue; }

                    int counter = 0;
                    while (counter <= zerosCounter)
                    {
                        if (zerosCounter > 15) { ACCodes.Add(0xF0); zerosCounter -= 16; }
                        else
                        {
                            byte temp = (byte)(zerosCounter << 4);
                            temp += ComputeACCategory(data[i][j]);
                            ACCodes.Add(temp);
                            zerosCounter = 0;
                            temp = 0;
                        }
                        counter++;
                    }
                }
            }
            if (zerosCounter > 0) ACCodes.Add(0x00);

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
    }
}
