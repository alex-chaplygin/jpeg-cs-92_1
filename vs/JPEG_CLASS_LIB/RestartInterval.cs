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
        /// Записывает RestartInterval в поток.
        /// </summary>
        public override void Write()
        {
            base.Write();
            Write16(restartInterval);
        }
    }
}
