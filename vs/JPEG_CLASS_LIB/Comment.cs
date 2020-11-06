using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Закодированный комментарий.
    /// </summary>
    class Comment : JPEGData
    {
        /// <summary>
        /// Массив байт закодированного комментария.
        /// </summary>
        public byte[] CommentBytes { get; private set; }

        /// <summary>
        /// Длина сегмента.
        /// </summary>
        public ushort Lenght { get; private set; }

        /// <summary>
        /// Читает комментарий длинной 2 из поток.
        /// </summary>
        /// <param name="s"></param>
        public Comment (Stream s) : base(s)
        {
            Lenght = Read16();
            CommentBytes = new byte[Lenght - 2];
            for (int i = 0; i < Lenght - 2; i++)
            {
                CommentBytes[i] = (byte)MainStream.ReadByte();
            }
        }

        /// <summary>
        /// Записывает коммментарий длинной 2 в поток.
        /// </summary>
        /// <param name="s"></param>
        public void Write (Stream s)
        {
            Write16(Lenght);
            for (int i = 0; i < Lenght - 2; i++)
            {
                MainStream.WriteByte(CommentBytes[i]);
            }
        }
    }
}
