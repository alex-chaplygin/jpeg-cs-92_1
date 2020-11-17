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
        /// <param name="Y">Выходная компонента яркости.</param>
        /// <param name="Cb">Выходная первая компонента цветности.</param>
        /// <param name="Cr">Выходная вторая компонента цветности.</param>
        private static void RGBToYUV(Point pRGB, out byte Y, out byte Cb, out byte Cr)
        {
            // Множители определены рекомендацией T-REC-T.871.
            double temp;

            temp = 0.299 * pRGB.r + 0.587 * pRGB.g + 0.114 * pRGB.b;
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            Y = Convert.ToByte(temp);

            temp = 0.5 * (pRGB.b - Y) / (1 - 0.114) + 128;
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            Cb = Convert.ToByte(temp);

            temp = 0.5 * (pRGB.r - Y) / (1 - 0.299) + 128;
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            Cr = Convert.ToByte(temp);
        }

        /// <summary>
        /// Конвертирует двумерный массив точек RGB модели в массив точек YUV модели и разделяет его компоненты на три матрицы.
        /// </summary>
        /// <param name="imgRGB">Массив точек RGB модели</param>
        /// <param name="matrixY">Выходная матрица компоненты яркости YUV модели.</param>
        /// <param name="matrixCb">Выходная матрица первой компоненты цветности YUV модели.</param>
        /// <param name="matrixCr">Выходная матрица второй компоненты цветности YUV модели.</param>
        public static void RGBToYUV(Point[,] imgRGB, out byte[,] matrixY, out byte[,] matrixCb, out byte[,] matrixCr)
        {
            int width = imgRGB.GetLength(0);
            int height = imgRGB.GetLength(1);

            matrixY = new byte[width, height];
            matrixCb = new byte[width, height];
            matrixCr = new byte[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    RGBToYUV(imgRGB[i, j], out matrixY[i, j], out matrixCb[i, j], out matrixCr[i, j]);
        }

        /// <summary>
        /// Конвертирует точку YUV модели в точку RGB модели.
        /// </summary>
        /// <param name="Y">Компонента яркости точки YUV модели.</param>
        /// <param name="Cb">Первая компонента цветности точки YUV модели.</param>
        /// <param name="Cr">Первая компонента цветности точки YUV модели.</param>
        /// <returns>Точка RGB модели.</returns>
        private static Point YUVToRGB(byte Y, byte Cb, byte Cr)
        {
            // Множители определены рекомендацией T-REC-T.871.
            double temp;
            Point pRGB = new Point();

            temp = Y + 1.402 * (Cr - 128);
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.r = Convert.ToByte(temp);

            temp = Y - (0.114 * 1.772 * (Cb - 128) +
                0.299 * 1.402 * (Cr - 128)) / 0.587;
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.g = Convert.ToByte(temp);

            temp = Y + 1.772 * (Cb - 128);
            if (temp > 255) temp = 255;
            else if (temp < 0) temp = 0;
            pRGB.b = Convert.ToByte(temp);

            return pRGB;
        }

        /// <summary>
        /// Конвертирует матрицы, соответствующие компонентам YUV точек, в массив точек RGB модели.
        /// </summary>
        /// <param name="matrixY"></param>
        /// <param name="matrixCb"></param>
        /// <param name="matrixCr"></param>
        /// <returns>Массив точек RGB модели.</returns>
        public static Point[,] YUVToRGB(byte[,] matrixY, byte[,] matrixCb, byte[,] matrixCr)
        {
            int width = matrixY.GetLength(0);
            int height = matrixY.GetLength(1);

            Point[,] imgRGB = new Point[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    imgRGB[i, j] = YUVToRGB(matrixY[i, j], matrixCb[i, j], matrixCr[i, j]);
            return imgRGB;
        }
    }
}
