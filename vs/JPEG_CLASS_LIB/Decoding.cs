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
        /// Создает объект, сохраняет поток в классе.
        /// </summary>
        /// <param name="s">Поток сжатых данных.</param>
        public Decoding(Stream s)
        {
            MainStream = s;
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
    }
}
