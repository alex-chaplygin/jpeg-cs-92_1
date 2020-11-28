using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Декодирование энтропийно закодированных данных.
    /// </summary>
    public class Decoding
    {
        /// <summary>
        /// Сжатые данные.
        /// </summary>
        Stream MainStream;

        /// <summary>
        /// Текущий байт в сжатых данных.
        /// </summary>
        byte B;

        /// <summary>
        /// Следующий байт в сжатых данных, когда B = 0xFF. 
        /// </summary>
        byte B2;

        /// <summary>
        /// Счетчик битов.
        /// </summary>
        byte CNT = 0;
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
        /// Таблица Хаффмена
        /// </summary>
        HuffmanTable huff;
        /// <summary>
        /// Создает объект, сохраняет поток в классе.
        /// </summary>
        /// <param name="s">Поток сжатых данных.</param>
        public Decoding(Stream s, HuffmanTable huff)
        {
            MainStream = s;
            this.huff = huff;
            GenerateTables();
        }

        /// <summary>
        /// Читает следующий бит из сжатых данных, распознает маркеры и stuff-байты.
        /// </summary>
        /// <returns>Следующий бит сжатых данных.</returns> 
        public byte NextBit()
        {
            if (CNT == 0)
            {
                B = (byte)MainStream.ReadByte();
                CNT = 8;
                if (B == 0xFF)
                {
                    B2 = (byte)MainStream.ReadByte();
                    if (B2 != 0)
                    {
                        if (B2 == 0xDC)
                            throw new Exception("Встречен DNL маркер.");
                        else throw new Exception("Непредвиденная ошибка. Возможно, декодирование не было завершено в конце последнего интервала перезапуска.");
                    }
                }
            }
            byte bit = (byte)(B >> 7);
            CNT--;
            B = (byte)(B << 1);
            return bit;
        }

        /// <summary>
        /// Получает биты из потока.
        /// </summary>
        /// <param name="ssss">Число бит, которое нужно получить.</param>
        /// <returns>Разница (DIFF) DC коэффициента.</returns>
        public ushort Receive(byte ssss)
        {
            byte i = 0;
            ushort v = 0;
            do
            {
                i++;
                v = (ushort)((v << 1) + NextBit());
            }
            while (i != ssss);

            return v;
        }

        /// <summary>
        /// Конвертирует частично декодированную разницу DC коэффициентов в 
        /// полный код (расширение знакового бита в коде).
        /// </summary>
        /// <param name="diff">Частичный код разницы DC.</param>
        /// <param name="num_bits">Число бит для разницы.</param>
        /// <returns>Декодированная DC разница.</returns>
        public static short Extend(ushort diff, int num_bits)
        {
            var vt = (ushort)(1 << (num_bits - 1)); // 2^(num_bits-1)
            while (diff < vt)
            {
                vt = (ushort)((-1 << num_bits) + 1);
                diff += (ushort)vt;
            }

            return (short)diff;
	}
	
        /// <summary>
        /// Генериует вспомогательные таблицы для декодирования
        /// </summary>
        void GenerateTables()
        {
            byte[] BITS = huff.BITS;
            byte[] HUFFCODE = huff.HUFFCODE;
            
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
        /// Декодирует 8-ми битное значение, которое для DC коэффициента является категорией, а для AC - число нулей или категория для ненулевого коэффициента
        /// </summary>
        /// <returns>Декодированное значение</returns>
        public byte Decode()
        {
            int i = 0;//1
            byte CODE = NextBit();
            while (CODE>MaxCode[i])
            {
                i++;
                CODE = (Byte)((CODE << 1) + NextBit());

            }
            int j = VALPTR[i];
            j = j + CODE - MinCode[i];
            byte Value = huff.HUFFVAL[j];
            return Value;
        }

        /// <summary>
        /// Декодирует разницу для DC коэффициента
        /// возвращает DIFF - разница между предыдущим и текущим DC коэффициентом
        /// </summary>
        public short DecodeDC()
        {
            short diff = 0;
            ushort diff2 = 0;
            byte t = Decode();
            if (t == 0) return 0;
            diff2 = Receive(t);
            diff = Extend(diff2, t);
            return diff;
        }

    }
}
