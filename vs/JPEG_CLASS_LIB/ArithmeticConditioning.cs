using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Спецификации таблицы арифметического кодирования.
    /// По документации B.2.4.3
    /// </summary>
    public class ArithmeticConditioning : JPEGData
    {
        /// <summary>
        /// Множество компонентов для кодирования.
        /// </summary>
        public Multiple[] Multiples { get; private set; }

        /// <summary>
        /// Читает спецификацию из потока, начиная с поля "число компонентов в скане"
        /// </summary>
        /// <param name="s">Поток, из которого создается cпецификация.</param>
        public ArithmeticConditioning(Stream s)
            : base(s, MarkerType.DefineArithmeticCodingConditionings)
        {
            int n = (Length - 2) / 2;
            Multiples = new Multiple[n];
            for (int t = 0; t < n; t++)
            {
                byte Tc, Tb;
                Read4(out Tc, out Tb);
                byte Cs = (byte)s.ReadByte();
                Multiples[t] = new Multiple(Tc, Tb, Cs);
            }
        }

        /// <summary>
        /// Пишет спецификации таблицы арифметического кодирования в поток.
        /// </summary>
        /// <param name="s">Поток для записи.</param>
        public void Write(Stream s)
        {
            base.Write();
            int n = (Length - 2) / 2;
            for (int j = 0; j < n; j++)
            {
                Write4(Multiples[j].Tc, Multiples[j].Tb);
                MainStream.WriteByte(Multiples[j].Cs);
            }
        }

        /// <summary>
        /// Выводит в консоль информацию о классе.
        /// </summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine("Описание множеств.");
            int n = (Length - 2) / 2;
            for (int t = 0; t < n; t++)
            {
                Console.WriteLine($"    Tc: {Multiples[t].Tc:X}; " +
                    $"Tb: {Multiples[t].Tb:X}; " +
                    $"Cs: {Multiples[t].Cs:X}; ");
            }
        }
    }

    /// <summary>
    /// Множество компонентов для кодирования.
    /// </summary>
    public struct Multiple
    {
        /// <summary>
        /// Тип таблицы: 0 = DC, 1 = AC.
        /// </summary>
        public byte Tc { get; private set; }

        /// <summary>
        /// Определяет одну из четырех позиций, в которую должен быть установлен декодер таблицы.
        /// </summary>
        public byte Tb { get; private set; }

        /// <summary>
        /// Значение таблицы.
        /// </summary>
        public byte Cs { get; private set; }

        public Multiple(byte Tc, byte Tb, byte Cs)
        {
            this.Tc = Tc;
            this.Tb = Tb;
            this.Cs = Cs;
        }
    }
}
