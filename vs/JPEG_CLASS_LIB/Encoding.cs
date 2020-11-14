﻿using System;
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
        /// Предваряет каждый DC коэффициент номером категорией, 
        /// добавляя в одномерный массив перед DC коэффициентом значение номера категории.
        /// </summary>
        /// <param name="data">Список блоков 8x8 после DCT, рассчета DC и обхода зигзагом.</param>
        public static void EncodeDC(List<short[]> data)
        {
            short pred = 0; // DC коэффициент из последнего рассмотренного блока.
            short[] newBlock; // Увеличенный до размера [65] блок, хранящий номер категории.
            for (int i = 0; i < data.Count; i++)
            {
                newBlock = new short[65];
                Array.Copy(data[i], 0, newBlock, 1, 64);
                newBlock[0] = ComputeDCCategory((short)(newBlock[1] - pred));
                pred = newBlock[1];
                data[i] = newBlock;
            }
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
    }
}