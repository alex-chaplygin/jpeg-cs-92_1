using System;
using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JPEG.Tests
{
    [TestClass]
    public class ImageConverterTest
    {
        [TestMethod]
        public void TestImageConverter()
        {
            Random random = new Random();

            int widgth = random.Next(2, 30);
            int height = random.Next(2, 30);
            // Создаем матрицу пикселей RGB [widgth, height] со случайными байтовыми значениями.
            Point[,] imgRGB = new Point[widgth, height];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < widgth; j++)
                    imgRGB[j, i] = new Point()
                    {
                        r = (byte)random.Next(0, 255),
                        g = (byte)random.Next(0, 255),
                        b = (byte)random.Next(0, 255),
                    };

            // Выводим исходный массив RGB пикселей.
            Console.WriteLine("Исходный массив RGB пикселей");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++)
                    Console.Write($"RGB=({imgRGB[j, i].r:d3};{imgRGB[j, i].g:d3};{imgRGB[j, i].b:d3}) ");
                Console.WriteLine();
            }

            // Конвертируем массив RGB пикселей в массив YUV пикселей и выводим его.
            byte[,] matrixY, matrixCb, matrixCr;
            ImageConverter.RGBToYUV(imgRGB, out matrixY, out matrixCb, out matrixCr);
            Console.WriteLine("Конвертированный из RGB в YUV массив пикселей");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++)
                    Console.Write($"YUV=({matrixY[j, i]:d3};{matrixCb[j, i]:d3};{matrixCr[j, i]:d3}) ");
                Console.WriteLine();
            }

            // Конвертируем массив YUV пикселей обратно в массив RGB пикселей и выводим его.
            Point[,] newImgRGB = ImageConverter.YUVToRGB(matrixY, matrixCb, matrixCr);
            Console.WriteLine("Конвертированный из YUV обратно в RGB массив пикселей");
            var maxDifference = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < widgth; j++) {
                    Console.Write($"RGB=({newImgRGB[j, i].r:d3};{newImgRGB[j, i].g:d3};{newImgRGB[j, i].b:d3}) ");
                    Assert.IsTrue(Math.Abs(newImgRGB[j, i].r-imgRGB[j, i].r)<=maxDifference && Math.Abs(newImgRGB[j, i].g-imgRGB[j, i].g)<=maxDifference && Math.Abs(newImgRGB[j, i].b-imgRGB[j, i].b)<=maxDifference, "Разница значений до и после конвертации превышает допустимый предел");
                }
                Console.WriteLine();
            }
        }

    }
}