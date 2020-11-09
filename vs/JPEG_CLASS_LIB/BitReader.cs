using System;

namespace JPEG_CLASS_LIB
{
    public class BitReader
    {
        //Массив из которого происходит чтение бит
        private byte[] data;

        //Текущий номер байта, откуда происходит чтение
        private int bytePos = 0;

        //Номер бита в текущем байте, откуда происходит чтение (7 бит - старший, 0 - младший)
        private int bitPos = 7;

        /// <summary>
        /// Создает объект для чтения бит, сохраняет переданный массив байт
        /// </summary>
        /// <param name="data">Закодировнные данные</param>
        public BitReader(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Читает заданное количество бит (максимум 16) из текущей позиции в массиве и сдвигает позиции. Читаются биты начиная со старшего и по направлению к младшему.
        /// </summary>
        /// <param name="n">Число необходимых к прочтению бит</param>
        /// <returns>Значение, составленное из прочитанных бит</returns>
        public ushort Read(int n)
        {
            if (n<1 || n>16) throw new Exception("Значение n должно быть в диапазоне [1; 16]");
            if (bytePos*8+(7-bitPos)+n>data.Length*8) throw new Exception("Значение n выходит за пределы массива data");
            var retVal = (ushort) 0;
            while (n>0)
            {
                if (bitPos == -1)
                {
                    bitPos = 7;
                    bytePos++;
                }

                if ((data[bytePos] & (1 << bitPos)) != 0)
                {
                    retVal += Convert.ToUInt16(1 << n-1);
                }
                bitPos--;
                n--;
            }
            return retVal;
        }
    }
}