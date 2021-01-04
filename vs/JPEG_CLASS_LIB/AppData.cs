using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс AppData - содержит данные, методы их чтения из потока и записи в поток.
    /// </summary>
    public class AppData : JPEGData
    {
        /// <summary>
        /// Массив данных
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Конструктор класса, считывает данные из потока
        /// </summary>
        /// <param name="s"></param>
        public AppData(Stream s):base(s, MarkerType.ReservedForApplicationSegments)
        {
	    data = new byte[Length - 2];
            s.Read(data,0, Length - 2);
        }

        /// <summary>
        /// Метод записи массива данных в поток.
        /// </summary>
        /// <param name="s"></param>
        public override void Write()
        {
            base.Write();
            MainStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Метод вывода данных в консоль
        /// </summary>
        public override void Print()
        {
            base.Print();
            for (int i = 0, j = 1; i < data.Length; i++, j++)
            {
                Console.Write($"{data[i]}\t");
                if (j == 8)
                {
                    j = 0;
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
