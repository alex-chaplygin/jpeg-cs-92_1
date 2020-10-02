using System;
using System.Collections.Generic;
using System.Text;

namespace JPEG_CLASS_LIB
{
    public class DCT
    {
        //Возвращает матрицу, каждый компонент которой приведен из диапазона 0..255 в диапазон -128..127
        public static short[,] Shift(byte[,] matrix)
        {
            short[,] mat = new short[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++) mat[i,j] = Convert.ToInt16(matrix[i,j]-128);
            return mat;
        }
        //Возвращает матрицу, каждый компонент которой приведен из диапазона -128..127 в диапазон 0..255
        public static byte[,] ReverseShift(short[,] matrix)
        {
            byte[,] mat = new byte[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++) mat[i, j] = Convert.ToByte(matrix[i, j]+128);
            return mat;
        }
    }
}
