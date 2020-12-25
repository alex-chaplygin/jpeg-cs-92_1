using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Таблица Квантования - класс, считывающий из потока таблицу квантования, точность её элементов и её идентификатор назначения.
    /// Также записывает те же данные в поток.я
    /// </summary>
    public class QuantizationTable : JPEGData
    {
        /// <summary>
        /// Таблица квантования содержащаяся в этом классе
        /// </summary>
        public byte[] QuantizationTableMain = new byte[64];

        /// <summary>
        /// Pq - точность элемента таблицы квантования.
        /// </summary>
        byte Pq;

        /// <summary>
        /// Tq - номер таблицы.
        /// </summary>
        public byte Tq;

        /// <summary>
        /// Конктруктор класс, записывает значения Pq и Tq, значения таблицы квантования в массив из потока.
        /// </summary>
        /// <param name="s">Входной поток</param>
        public QuantizationTable(Stream s):base(s, MarkerType.DefineQuantizationTables)
        {
            Read4(out Pq, out Tq);
            s.Read(QuantizationTableMain,0 , QuantizationTableMain.Length);
        }
        /// <summary>
        /// Конструктор для записи
        /// </summary>
        /// <param name="s">Поток в который будет идти запись</param>
        /// <param name="table">Таблица квантования</param>
        /// <param name="Pq">Точность эллемента таблицы квантования</param>
        /// <param name="Tq">Номер таблицы</param>
        /// <param name="length">Длина таблица квантования</param>
        public QuantizationTable(Stream s, short[,]table, byte Pq, byte Tq) : base(s, MarkerType.DefineQuantizationTables, (ushort)table.Length)
        {
            QuantizationTableMain = (DCT.Zigzag(table)).Select(x => Convert.ToByte(x)).ToArray();
            this.Pq = Pq;
            this.Tq = Tq;
            Write(s);
        }

        /// <summary>
        /// Метод, записывающий значения таблицы квантования в поток
        /// </summary>
        /// <param name="s">Выходной поток</param>
        public void Write(Stream s)
        {
            base.Write();
            s.Write(new byte[2] {Pq,Tq}, 0, 2);
            s.Write(QuantizationTableMain, 0, QuantizationTableMain.Length);
        }

        /// <summary>
        /// Метод вывода значений класса таблицы квантования
        /// </summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine($"Значения Pq - {Pq}, и Tq - {Tq}\nТаблица квантования");
            for (int i = 0; i < QuantizationTableMain.Length; i++)
            {
                Console.Write($"{QuantizationTableMain[i]}\t");
            }
            Console.WriteLine();
        }
    }
}
