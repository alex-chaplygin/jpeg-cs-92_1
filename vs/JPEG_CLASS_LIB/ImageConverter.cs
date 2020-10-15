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
    }
}
