using System;
using System.IO;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Содержит параметры изображения.
    /// </summary>
    public class Frame : JPEGData
    {
        /// <summary>
        /// Число колонок в изображении.
        /// </summary>
        ushort height;

        /// <summary>
        /// Число столбцов в изображении.
        /// </summary>
        ushort width;

        /// <summary>
        /// Точность представления в битах.
        /// </summary>
        byte numBit;

        /// <summary>
        /// Число каналов в изображении.
        /// </summary>
        byte numComponent;

        /// <summary>
        /// Массив компонентов.
        /// </summary>
        Component[] Components;

        /// <summary>
        /// Конструктор класса Frame.
        /// </summary>
        /// <param name="s">Поток с изображением.</param>
        public Frame(Stream s, MarkerType markerType) : base(s, markerType)
        {
            numBit = (byte)(s.ReadByte());
            height = Read16();
            width = Read16();
            numComponent = (byte)(s.ReadByte());
            Components = new Component[numComponent];
            for (int i = 0; i < (int)numComponent; i++)
            {
                Components[i].Number = (byte)(s.ReadByte());
                Read4(out Components[i].H, out Components[i].V);
                Components[i].QuantizationTableNumber = (byte)(s.ReadByte());
            }
        }

        /// <summary>
        /// Конструктор класса Frame.
        /// </summary>
        /// <param name="s">Поток с изображением</param>
        /// <param name="markerType">Маркер Frame.</param>
        /// <param name="height">Число колонок в изображении.</param>
        /// <param name="width">Число столбцов в изображении.</param>
        /// <param name="numBit">Точность представления в битах.</param>
        /// <param name="Components">Массив компонентов.</param>
        public Frame(Stream s, MarkerType markerType, ushort height, ushort width, byte numBit, Component[] Components) : base(s, markerType)
        {
            this.numBit = numBit;
            this.height = height;
            this.width = width;
            this.Components = Components;
            numComponent = (byte)this.Components.Length;
            Length = (ushort)(8 + 3 * numComponent);
        }

        /// <summary>
        /// Записывает Frame в текущий поток.
        /// </summary>
        public override void Write()
        {
            base.Write();
            MainStream.WriteByte(numBit);
            Write16(height);
            Write16(width);
            MainStream.WriteByte(numComponent);
            for (byte i = 0; i < numComponent; i++)
            {
                MainStream.WriteByte(Components[i].Number);
                Write4(Components[i].H, Components[i].V);
                MainStream.WriteByte(Components[i].QuantizationTableNumber);
            }
        }

        /// <summary>
        /// Чтение числа бит.
        /// </summary>
        public Int16 NumberIfBits
        {
            get { return numBit; }
        }

        /// <summary>
        /// Чтение высоты.
        /// </summary>
        public Int16 Height
        {
            get { return (short)height; }
        }

        /// <summary>
        /// Чтение ширины.
        /// </summary>
        public Int16 Width
        {
            get { return (short)width; }
        }

        /// <summary>
        /// Чтение числа компонент.
        /// </summary>
        public Int16 NumberOfComponent
        {
            get { return numComponent; }
        }

        /// <summary>
        /// Параметры канала.
        /// </summary>
        public struct Component
        {
            /// <summary>
            /// Номер компонента.
            /// </summary>
            public byte Number;

            /// <summary>
            /// Компонент H. 
            /// Коэффициент горизонтальной выборки.
            /// </summary>
            public byte H;

            /// <summary>
            /// Компонент V. 
            /// Коэффициент вертикальной выборки.
            /// </summary>
            public byte V;

            /// <summary>
            /// Номер таблицы квантования.
            /// </summary>
            public byte QuantizationTableNumber;
        }

        /// <summary>
        /// Выводит в консоль параметры изображения.
        /// </summary>
        override public void Print()
        {
            base.Print();
            Console.WriteLine();
            Console.WriteLine("******Параметры изображения");
            Console.WriteLine("Число бит: " + NumberIfBits + " Высота: " + Height + " Ширина: " + Width + " Число компонент: " + NumberOfComponent);
            for (int i = 0; i < numComponent; i++)
            {
                Console.WriteLine($"Компонент номер:{i}");
                Console.WriteLine($"Компонент.C{i} = " + Components[i].Number);
                Console.WriteLine($"Компонент.H{i} = " + Components[i].H);
                Console.WriteLine($"Компонент.V{i} = " + Components[i].V);
                Console.WriteLine($"Компонент.Номер таблицы квантования{i} = " + Components[i].QuantizationTableNumber);
                Console.WriteLine();
            }
        }
    }
}
