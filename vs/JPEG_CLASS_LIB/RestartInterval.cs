using System;
using System.IO;

namespace JPEG_CLASS_LIB
{
    /// <summary>
    /// Класс для RestertInterval.
    /// </summary>
    public class RestartInterval : JPEGData
    {
        /// <summary>
        /// Число MCU.
        /// </summary>
        public ushort restartInterval;

        /// <summary>
        /// Конструктор класса RestartInterval, читает MCU.
        /// </summary>
        /// <param name="s">Поток с изображением.</param>
        public RestartInterval(Stream s) : base(s, MarkerType.DefineRestartInterval)
        {
            restartInterval = Read16();
        }
        
        /// <summary>
        /// Конструктор класса RestartInterval для записи
        /// </summary>
        /// <param name="s">Поток с изображением</param>
        /// <param name="restartInterval">Число MCU</param>
        public RestartInterval(Stream s, ushort restartInterval) : base(s, MarkerType.DefineRestartInterval, 4)
        {
            this.restartInterval = restartInterval;
        }

        /// <summary>
        /// Записывает RestartInterval в поток.
        /// </summary>
        public override void Write()
        {
            base.Write();
            Write16(restartInterval);
        }

        /// <summary>
        /// Выводит данные компонента в консоль.
        /// </summary>
        public override void Print()
        {
            base.Print();
            Console.WriteLine($"Число MCU: {restartInterval}");
        }
    }
}
