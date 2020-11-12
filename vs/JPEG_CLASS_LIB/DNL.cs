using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс сегмента DNL.
    /// </summary>
    public class DNL:JPEGData
    {
        /// <summary>
        /// Количество строк.
        /// </summary>
        public ushort NL;

        /// <summary>
        /// Создаёт экземпляр класса Define Number of Lines, считывает количество строк.
        /// </summary>
        /// <param name="s">Поток, на основе которого создается экземпляр класса.</param>
        public DNL(Stream s): base (s, MarkerType.DefineNumberOfLines)
        {
            NL = Read16();
        }

        /// <summary>Выводит в консоль поля класса.</summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine("Количество строк в frame: " + NL);
        }
    }
}
