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
        /// <summary>
        /// Сжатые данные.
        /// </summary>
        Stream MainStream;

        /// <summary>
        /// Создает объект, сохраняет поток в классе.
        /// </summary>
        /// <param name="s">Поток сжатых данных.</param>
        public Encoding(Stream s)
        {
            MainStream = s;
        }

        /// <summary>
        /// Предваряет каждый DC коэффициент номером категорией, 
        /// добавляя в одномерный массив перед DC коэффициентом значение номера категории.
        /// </summary>
        /// <param name="data">Список блоков 8x8 после DCT и обхода зигзагом.</param>
        public static void EncodeDC(List<short[]> data)
        {
            short[] newBlock; // Увеличенный до размера [65] блок, хранящий номер категории.
            for (int i = data.Count - 1; i > 0; i--)
            {
                newBlock = new short[65];
                Array.Copy(data[i], 0, newBlock, 1, 64);
                newBlock[1] = (short)(newBlock[1] - data[i-1][0]); // Записываем разность в DC коэффициент.
                newBlock[0] = ComputeDCCategory((short)newBlock[1]); // По DC коэффициенту определяем категорию.
                data[i] = newBlock;
            }
            // Повторяем опрерацию для самого первого блока.
            newBlock = new short[65];
            Array.Copy(data[0], 0, newBlock, 1, 64);
            newBlock[0] = ComputeDCCategory((short)newBlock[1]);
            data[0] = newBlock;
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
        /// <param name="num">Число бит (от 1 до 16).</param>
        public void WriteBits(ushort bits, int num)
        {
            byte b; // Записываемый в поток байт

            b = (num > 8) ? (byte)(bits >> (num - 8)) : (byte)(bits << (8 - num));
            MainStream.WriteByte(b);
            if (b == 0xFF) MainStream.WriteByte(0x00);

            if (num <= 8) return;

            b = (byte)(bits << (16 - num));
            MainStream.WriteByte(b);
            if (b == 0xFF) MainStream.WriteByte(0x00);
        }
    }
}
