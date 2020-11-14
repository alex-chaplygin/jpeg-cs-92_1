using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс для декодирование энтропийно закодированных данных.
    /// </summary>
    public class Decoding
    {
        /// <summary>
        /// Поток данных.
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
        /// Конструктор класса, сохраняющий поток данных.
        /// </summary>
        /// <param name="s">Поток данных.</param>
        public Decoding(Stream s)
        {
            MainStream = s;
        }

        /// <summary>
        /// NEXTBIT считывает следующий бит сжатых данных и передает его процедурам более высокого уровня.
        /// </summary>
        /// <returns>Следующий бит сжатых данных.</returns>
        //См. F.2.2.5 в документации. 
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
    }
}
