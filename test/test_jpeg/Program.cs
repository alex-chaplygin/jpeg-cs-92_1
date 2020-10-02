using System;
using System.IO;
using JPEG_CLASS_LIB;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            _TestPack();
            _TestUnpack();
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
            JPEG_CS j = new JPEG_CS(File.Open("test.jpg", FileMode.Create));
            Point[,] изображение = j.UnPack();
            изображение[0, 0].r = 255;

            BlockTest();
        }

        static void BlockTest()
        {

            byte[,] matrix = new byte[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8  },
                { 9, 10,11,12 },
                { 13,14,15,16 }
            };

            Block block1 = new Block(matrix);
            WriteMatrix(block1.GetMatrix());
            Console.WriteLine();

            Block block2 = new Block(block1.Scaling(1, 2, 1, 2));
            WriteMatrix(block2.GetMatrix());
            Console.WriteLine();

            Block block3 = new Block(block2.Scaling(3, 1, 3, 1));
            WriteMatrix(block3.GetMatrix());
            Console.WriteLine();
        }

        static void WriteMatrix(byte[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    Console.Write(matrix[i,j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
