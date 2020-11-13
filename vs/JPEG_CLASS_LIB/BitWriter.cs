using System;
using System.Collections.Generic;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Позволяет записывать битовые значения в массив байт
    /// </summary>
    public class BitWriter
    {
        /// <summary>
        /// Список, в который происходит запись данных
        /// </summary>
        private List<byte> data = new List<byte> {0};

        /// <summary>
        /// Текущий номер байта
        /// </summary>
        private int bytePos = 0;

        /// <summary>
        /// Номер бита в текущем байте, откуда происходит чтение (7 бит - старший, 0 - младший)
        /// </summary>
        private int bitPos = 7;

        /// <summary>
        /// Создает объект для записи бит
        /// </summary>
        public BitWriter()
        {
            
        }

        /// <summary>
        /// Записывает заданное количество бит (максимум 16) в текущую позицию в массиве и сдвигает позиции. Биты пишутся начиная со старшего и по направлению к младшему.
        /// </summary>
        /// <param name="n">Число бит, которое нужно записать</param>
        /// <param name="data">Значение для записи</param>
        public void Write(int n, ushort data)
        {
            if (n<1 || n>16) throw new Exception("Значение n должно быть в диапазоне [1; 16]");
            while (n>0)
            {
                if (bitPos == -1)
                {
                    bitPos = 7;
                    bytePos++;
                    this.data.Add(0); 
                }

                if ((data & (1 << (n - 1))) != 0)
                {
                    this.data[bytePos] = Convert.ToByte(this.data[bytePos] | (1 << bitPos));
                }
                
                bitPos--;
                n--;
            }
        }

        /// <summary>
        /// Возвращает записанный массив байт
        /// </summary>
        /// <returns>Записанный массив байт</returns>
        public byte[] Get()
        {
            return data.ToArray();
        }


    }
}