﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        struct Component
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