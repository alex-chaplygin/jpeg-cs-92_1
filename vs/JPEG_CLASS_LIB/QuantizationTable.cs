﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        byte[] QuantizationTableMain = new byte[64];
        /// <summary>
        /// Pq - точность элемента таблицы квантования.
        /// Tq - идентификатор назначения таблицы квантования.
        /// </summary>
        byte Pq, Tq;
        /// <summary>
        /// Конктруктор класс, записывает значения Pq и Tq, значения таблицы квантования в массив из потока.
        /// </summary>
        /// <param name="s">Входной поток</param>
        public QuantizationTable(Stream s):base(s)
        {
            Read4(out Pq, out Tq);
            s.Read(QuantizationTableMain,0 , QuantizationTableMain.Length);
        }
        /// <summary>
        /// Метод, записывающий значения таблицы квантования в поток
        /// </summary>
        /// <param name="s">Выходной поток</param>
        public void Write(Stream s)
        {
            s.Write(new byte[2] {Pq,Tq}, 0, 2);
            s.Write(QuantizationTableMain, 0, QuantizationTableMain.Length);
        }
    }
}