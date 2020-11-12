﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс JPEGFile.
    /// </summary>
    public class JPEGFile
    {
        /// <summary>
        /// Список всех структур JPEG до StartOfScan.
        /// </summary>
        List<JPEGData> Data = new List<JPEGData>{};

        /// <summary>
        /// Конструктор JPEGFile. Считывает все структуры JPEGData и записывает их в Data.
        /// </summary>
        /// <param name="s">Поток с изображением.</param>
        public JPEGFile(Stream s) 
        {
            do
            {
                JPEGData temp = JPEGData.GetData(s);
                Data.Add(temp);
                s.Position += temp.Length;
            }
            while (Data[Data.Count - 1].Marker != MarkerType.StartOfScan);
        }

        /// <summary>
        /// Выводит в консоль все JPEGData
        /// </summary>
        public void Print()
        {
            foreach(JPEGData d in Data)
            {
                d.Print();
            }
        }
    }
}
