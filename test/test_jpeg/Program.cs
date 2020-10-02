using JPEG_CLASS_LIB;
using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            _TestPack();
            _TestUnpack();
            _TestDCTShift();
        }

        static void _TestUnpack()
        {
            var up = new JPEG_CS(File.Open("testUP.jpg", FileMode.Create));
            Point[,] изображение = up.UnPack();
        }

        static void _TestPack()
        {
            JPEG_CS p = new JPEG_CS(File.Open("testP.jpg", FileMode.Create));
            Point[,] points = new Point[10,10];
            for (int i = 0; i< 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    points[i, j].r = 0;
                    points[i, j].g = 255;
                    points[i, j].b = 0;
                }
            }
            p.Compress(points);
        }
        static void _TestDCTShift()
        {
            Random Random = new Random();
            byte[,] testmassive = new byte[5, 5];
            for (int i = 0; i < testmassive.GetLength(0); i++)
                for (int j = 0; j < testmassive.GetLength(1); j++) testmassive[i, j] = Convert.ToByte(Random.Next(0, 256));
            short[,] testshort = new short[,] { };
            byte[,] testbyte = new byte[,] { };
            testshort = DCT.Shift(testmassive);
            testbyte = DCT.ReverseShift(testshort);
            Console.WriteLine("Изначальная матрица");
            for (int i = 0; i < testmassive.GetLength(0); i++)
            {
                for (int j = 0; j < testmassive.GetLength(1); j++) Console.Write(testmassive[i, j].ToString() + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Матрица со сдвигом");
            for (int i = 0; i < testshort.GetLength(0); i++)
            {
                for (int j = 0; j < testshort.GetLength(1); j++) Console.Write(testshort[i, j].ToString() + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Матрица с обратным сдвигом");
            for (int i = 0; i < testbyte.GetLength(0); i++)
            {
                for (int j = 0; j < testbyte.GetLength(1); j++) Console.Write(testbyte[i, j].ToString() + " ");
                Console.WriteLine();
            }
        }
    }
}
