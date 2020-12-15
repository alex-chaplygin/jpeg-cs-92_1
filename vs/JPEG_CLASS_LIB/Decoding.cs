using System;
using System.IO;

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
        /// Таблица Хаффмана DC.
        /// </summary>
        public HuffmanTable huffDC;

        /// <summary>
        /// Таблица Хаффмана AC.
        /// </summary>
        public HuffmanTable huffAC;

        /// <summary>
        /// Создает объект, сохраняет поток в классе.
        /// </summary>
        /// <param name="s">Поток сжатых данных.</param>
        /// <param name="huffDC">Таблица Хаффмана DC.</param>
        /// <param name="huffAC">Таблица Хаффмана AC.</param>
        public Decoding(Stream s, HuffmanTable huffDC, HuffmanTable huffAC)
        {
            MainStream = s;
            this.huffDC = huffDC;
            this.huffAC = huffAC;
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
        /// Декодирует 8-ми битное значение, которое для DC коэффициента является категорией, а для AC - число нулей или категория для ненулевого коэффициента
        /// </summary>
        /// <returns>Декодированное значение</returns>
        public byte Decode(HuffmanTable H)
        {
            int i = 0;//1
            byte CODE = NextBit();
            while (i<H.MaxCode.Length && CODE > H.MaxCode[i])
            {
                i++;
                CODE = (Byte)((CODE << 1) + NextBit());
            }
            if (i >= 16) return 0;
            int j = H.VALPTR[i];
            j = j + CODE - H.MinCode[i];
            byte Value = H.HUFFVAL[j];
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
            byte t = Decode(huffDC);
            if (t == 0) return 0;
            diff2 = Receive(t);
            diff = Extend(diff2, t);
            Console.WriteLine($"Decoded DC: {diff}");
            return diff;
        }

        /// <summary>
        /// Декодирует AC коэффициенты из потока.
        /// </summary>
        /// <param name="block">Блок, куда сохраняются коэффициенты.</param>
        public void DecodeAC(short[] block)
        {
            byte K, RS, RRRR, R, SSSS;
            bool flag = true;
            K = 1;
            while (flag)
            {
                RS = Decode(huffAC);
                SSSS = (byte)(RS % 16);
                RRRR = (byte)(RS >> 4);
                R = RRRR;
                if (SSSS == 0)
                    if (R == 15) K += 16;
                    else flag = false;
                else
                {
                    K += R;
                    block[K] = (short)Receive(SSSS);
                    block[K] = Extend((ushort)block[K], SSSS);
                    if (K == 63) flag = false;
                    else K++;
                }
            }
        }
        /// <summary>
        /// Декодирует блок (DC и 63 коэффициента AC) из потока
        /// </summary>
        /// <returns>Массив коэффициентов</returns>
        public short[] DecodeBlock()
        {
            short[] block = new short[64];
            block[0] = DecodeDC();
            DecodeAC(block);
            return (block);
        }
    }
}
