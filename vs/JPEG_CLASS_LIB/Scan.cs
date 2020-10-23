using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    class Scan : JPEGData
    {
        /// <summary>
        /// Читает скан из потока, начиная с поля "число компонентов в скане"
        /// </summary>
        /// <param name="s"></param>
        public Scan(Stream s) : base(s)
        {
            Ns = (byte)MainStream.ReadByte(); // Читаем количество компонентов изображения в скане.

            components = new Component[Ns];
            for (byte j = 0; j < Ns; j++)
            {
                byte Cs = (byte)MainStream.ReadByte();
                byte Td, Ta;
                Read4(out Td, out Ta);
                components[j] = new Component(Cs, Td, Ta);
            }

            Ss = (byte)MainStream.ReadByte();
            Se = (byte)MainStream.ReadByte();

            byte Ah, Al;
            Read4(out Ah, out Al);
        }

        /// <summary>
        /// Пишет скан в поток, начиная с поля "число компонентов в скане"
        /// </summary>
        /// <param name="s"></param>
        public void Write(Stream s)
        {
            MainStream.WriteByte(Ns);
            for (byte j = 0; j < Ns; j++)
            {
                MainStream.WriteByte(components[j].Cs);
                Write4(components[j].Td, components[j].Ta);
            }
            MainStream.WriteByte(Ss);
            MainStream.WriteByte(Se);
            Write4(Ah, Al);
        }


        /// <summary>
        /// Количество компонентов изображения в скане.
        /// </summary>
        public byte Ns { get; private set; }

        public Component[] components { get; private set; }

        /// <summary>
        /// Start of spectral or predictor selection.
        /// </summary>
        public byte Ss { get; private set; }

        /// <summary>
        /// End of spectral selection.
        /// </summary>
        public byte Se { get; private set; }

        /// <summary>
        /// Successive approximation bit position high.
        /// </summary>
        public byte Ah { get; private set; }

        /// <summary>
        /// Succesive approximation bit position low or point transform.
        /// </summary>
        public byte Al { get; private set; }
    }

    struct Component
    {
        public byte Cs { get; private set; }
        public byte Td { get; private set; }
        public byte Ta { get; private set; }

        public Component(byte Cs, byte Td, byte Ta)
        {
            this.Cs = Cs;
            this.Td = Td;
            this.Ta = Ta;
        }
    }
}
