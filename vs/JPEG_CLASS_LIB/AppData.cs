using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс AppData - содержит данные, методы их чтения из потока и записи в поток.
    /// </summary>
    class AppData : JPEGData
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
            s.Read(data,0, Length - 2);
        }

        /// <summary>
        /// Метод записи массива данных в поток.
        /// </summary>
        /// <param name="s"></param>
        public void Write(Stream s)
        {
            base.Write();
            s.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Метод вывода данных в консоль
        /// </summary>
        public override void Print()
        {
            base.Print();
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write($"{data[i]}\t");
            }
            Console.WriteLine();
        }
    }
}
