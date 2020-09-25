using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            JPEG_CS j = new JPEG_CS(File.Open("test.jpg", FileMode.Create));
            Point[,] изображение = j.UnPack();
            изображение[0, 0].r = 255;
        }
    }
}
