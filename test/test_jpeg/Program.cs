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
            _TestDCT();
            
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
        static void _TestDCT()
        {
            short[,]matrix= new short[8, 8];
            //short a = 0;
            Random r = new Random();

            for (int i = 0; i<8;i++)
            {
                for (int j=0; j<8;j++)
                {
                    matrix[i, j] = Convert.ToInt16(Convert.ToInt16(r.Next(-128,128)));
                    if (i==0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    if (j == 0) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    if (i == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));
                    if (j == 7) matrix[i, j] = Convert.ToInt16(Convert.ToInt16(0));

                    Console.Write(matrix[i, j]+" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,] matrix3 = DCT.FDCT(matrix);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    Console.Write(matrix3[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            short[,]matrix2= DCT.IDCT(matrix3);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    
                    Console.Write(matrix2[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
