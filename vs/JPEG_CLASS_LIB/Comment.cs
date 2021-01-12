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
        /// Читает комментарий из поток.
        /// </summary>
        /// <param name="s"></param>
        public Comment (Stream s) : base(s, MarkerType.Comment)
        {
            Length = Read16();
            CommentBytes = new byte[Length - 2];
            for (int i = 0; i < Length - 2; i++)
            {
                CommentBytes[i] = (byte)MainStream.ReadByte();
            }
        }

        /// <summary>
        /// Записывает комментарий в поток.
        /// </summary>
        /// <param name="s"></param>
        public override void Write ()
        {
            base.Write();
            Write16(Length);
            for (int i = 0; i < Length - 2; i++)
            {
                MainStream.WriteByte(CommentBytes[i]);
            }
        }

        /// <summary>
        /// Выводит в консоль информацию о классе.
        /// </summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine("Закодированные байты комментария:");
            for (int i = 0; i < Length - 2; i++)
            {
                Console.Write($"{CommentBytes[i]:X2} ");
            }
            Console.WriteLine();
        }
    }
}
