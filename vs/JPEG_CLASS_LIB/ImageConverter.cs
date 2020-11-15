using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace JPEG_CLASS_LIB
{
    public class ImageConverter
    {
        public HSV[,] ImageHSV;
        public ImageConverter()
        {

        }
        private HSV pHSV(Point Pix)
        {
            HSV HSVp = new HSV();
            int[] mm = new int[] { Pix.r, Pix.g, Pix.b };
            int max = mm.Max();
            int min = mm.Min();
            double H = 0;
            if (max == Pix.b)
            {
                H = (60 * ((Pix.r * 1.0 - Pix.g) / (max - min)) + 240);
            }
            else if (max == Pix.g)
            {
                H = (60 * ((Pix.b * 1.0 - Pix.r) / (max - min)) + 120);
            }
            else if (max == Pix.r && Pix.g < Pix.b)
            {
                H = (60 * ((Pix.g * 1.0 - Pix.b) / (max - min)) + 360);
            }
            else if (max == Pix.r && Pix.g >= Pix.b)
            {
                H = (60 * ((Pix.g * 1.0 - Pix.b) / (max - min)) + 0);
            }
            HSVp.h = Convert.ToInt16(H);
            if (max == 0)
            {
                HSVp.s = 0;
            }
            else
            {
                HSVp.s = Math.Round(1 - (min * 1.0 / max), 2);
            }
            HSVp.v = Math.Round(max / 255.0, 2);
            return HSVp;

        }

        public void RGBtoHSV(Point[,] Image) 
        {
            int width = Image.GetLength(0);
            int height = Image.GetLength(1);
            HSV[,] img = new HSV[width, height];
            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                {
                    img[i, j] = pHSV(Image[i, j]);
                }
            ImageHSV = img;
        }

        public struct HSV 
        {
            public short h;
            public double s;
            public double v;
        }

        /// <summary>
        /// Конвертирует точку RGB модели в точку YUV модели. 
        /// </summary>
        /// <param name="pRGB">Точка RGB модели.</param>
        /// <returns>Точка YUV модели.</returns>
        private static YUV RGBToYUV(Point pRGB)
        {
            // Множители определены рекомендацией T-REC-T.871.
            YUV pYUV = new YUV();

            pYUV.Y = 0.299 * pRGB.r + 0.587 * pRGB.g + 0.114 * pRGB.b;
            pYUV.Cb = 0.5 * (pRGB.b - pYUV.Y) / (1 - 0.114) + 128;
            pYUV.Cr = 0.5 * (pRGB.r - pYUV.Y) / (1 - 0.299) + 128;

            return pYUV;
        }

        /// <summary>
        /// Конвертирует двумерный массив точек RGB модели в массив точек YUV модели, 
        /// сохраняя его в поле ImageYUV.
        /// </summary>
        /// <param name="imgRGB">Массив точек RGB модели.</param>
        /// <returns>Массив точек YUV модели.</returns>
        public static YUV[,] RGBToYUV(Point[,] imgRGB)
        {
            int width = imgRGB.GetLength(0);
            int height = imgRGB.GetLength(1);
            YUV[,] imgYUV = new YUV[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    imgYUV[i, j] = RGBToYUV(imgRGB[i, j]);
                }
            return imgYUV;
        }

        /// <summary>
        /// Конвертирует точку YUV модели в точку RGB модели.
        /// </summary>
        /// <param name="pYUV">Точка YUV модели.</param>
        /// <returns>Точка RGB модели.</returns>
        private static Point YUVToRGB(YUV pYUV)
        {
            // Множители определены рекомендацией T-REC-T.871.
            double temp;
            Point pRGB = new Point();

            temp = pYUV.Y + 1.402 * (pYUV.Cr - 128);
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.r = (byte)temp;

            temp = pYUV.Y - (0.114 * 1.772 * (pYUV.Cb - 128) +
                0.299 * 1.402 * (pYUV.Cr - 128)) / 0.587;
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.g = (byte)temp;

            temp = pYUV.Y + 1.772 * (pYUV.Cb - 128);
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.b = (byte)temp;

            return pRGB;
        }

        /// <summary>
        /// Точка цветовой модели YUV.
        /// </summary>
        public struct YUV
        {
            /// <summary>
            /// Компонента яркости.
            /// </summary>
            public double Y;

            /// <summary>
            /// Первая компонента цветности.
            /// </summary>
            public double Cb;

            /// <summary>
            /// Вторая компонента цветности.
            /// </summary>
            public double Cr;
        }
    }
}
