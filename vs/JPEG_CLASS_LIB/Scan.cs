using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Закодированные компоненты изображения.
    /// </summary>
    class Scan : JPEGData
    {
        /// <summary>
        /// Читает скан из потока, начиная с поля "число компонентов в скане"
        /// </summary>
        /// <param name="s">Поток, из которого создается скан.</param>
        public Scan(Stream s) : base(s, MarkerType.StartOfScan)
        {
            NumberOfImageComponent = (byte)MainStream.ReadByte(); // Читаем количество компонентов изображения в скане.

            components = new Component[NumberOfImageComponent];
            for (int j = 0; j < NumberOfImageComponent; j++)
            {
                byte Cs = (byte)MainStream.ReadByte();
                byte Td, Ta;
                Read4(out Td, out Ta);
                components[j] = new Component(Cs, Td, Ta);
            }

            SelectionStart = (byte)MainStream.ReadByte();
            SelectionEnd = (byte)MainStream.ReadByte();

            byte Ah, Al;
            Read4(out Ah, out Al);
            ApproximationHigh = Ah;
            ApproximationLow = Al;
        }

        /// <summary>
        /// Пишет скан в поток, начиная с поля "число компонентов в скане"
        /// </summary>
        /// <param name="s">Поток для записи.</param>
        public void Write(Stream s)
        {
            base.Write();
            MainStream.WriteByte(NumberOfImageComponent);
            for (int j = 0; j < NumberOfImageComponent; j++)
            {
                MainStream.WriteByte(components[j].ComponentSelector);
                Write4(components[j].TableDC, components[j].TableAC);
            }
            MainStream.WriteByte(SelectionStart);
            MainStream.WriteByte(SelectionEnd);
            Write4(ApproximationHigh, ApproximationLow);
        }


        /// <summary>
        /// Количество компонентов изображения в скане.
        /// По документации Ns.
        /// </summary>
        public byte NumberOfImageComponent { get; private set; }

        /// <summary>
        /// Массив компонентов скана изображения.
        /// </summary>
        public Component[] components { get; private set; }

        /// <summary>
        /// Номер первого коэффициента DCT.
        /// По документации Ss.
        /// </summary>
        public byte SelectionStart { get; private set; }

        /// <summary>
        /// Номер последнего коэффициента DCT. 
        /// По документации Se.
        /// </summary>
        public byte SelectionEnd { get; private set; }

        /// <summary>
        /// Высокая позиция бита последовательного приближения.
        /// По документации Ah.
        /// </summary>
        public byte ApproximationHigh { get; private set; }

        /// <summary>
        /// Низкая позиция бита последовательного приближения.
        /// По документации Al.
        /// </summary>
        public byte ApproximationLow { get; private set; }


        /// <summary>
        /// Выводит в консоль информацию о классе.
        /// </summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine($"Кол-во компонентов изображения в скане: {NumberOfImageComponent:X2}");
            Console.WriteLine("Описание компонентов.");
            for (int j = 0; j < NumberOfImageComponent; j++)
            {
                Console.WriteLine($"    Номер комп.: {components[j].ComponentSelector:X2}; " +
                    $"ном. табл. Хаффмана для DC коэф.: {components[j].TableDC:X2}; " +
                    $"для AC коэф.: {components[j].TableAC:X2}; ");
            }
            Console.WriteLine($"Номер первого коэффициента DCT: {SelectionStart:X2}");
            Console.WriteLine($"Номер последнего коэффициента DCT: {SelectionEnd:X2}");
            Console.WriteLine($"Ah: {ApproximationHigh:X}; Al: {ApproximationLow:X}");
        }
    }

    /// <summary>
    /// Компонент JPEG изображения.
    /// </summary>
    public struct Component
    {
        /// <summary>
        /// Номер компонента.
        /// По документации Cs.
        /// </summary>
        public byte ComponentSelector { get; private set; }
        /// <summary>
        /// Номер таблицы Хаффмана для DC коэффициентов.
        /// По документации Td.
        /// </summary>
        public byte TableDC { get; private set; }
        /// <summary>
        /// Номер таблицы Хаффмана для AC коэффициентов.
        /// По документации Ta.
        /// </summary>
        public byte TableAC { get; private set; }

        public Component(byte Cs, byte Td, byte Ta)
        {
            this.ComponentSelector = Cs;
            this.TableDC = Td;
            this.TableAC = Ta;
        }
    }
}
