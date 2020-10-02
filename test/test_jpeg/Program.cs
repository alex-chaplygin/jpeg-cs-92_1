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
            _TestQuantization();
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
        static void _TestQuantization()
        {
            Random random = new Random();
            short[,] matrixC = new short[4, 4];
            short[,] matrixQ = new short[4, 4] { {1,2,4,1 },
                                                {2,2,2,2 },
                                                {3,3,3,30 },
                                                {4,4,40,50 } };
            
            for (int i = 0; i< 16; i++)
            {
                matrixC[i/4,i%4] = Convert.ToByte(random.Next(0,16));
            }


            //до
            for (int i = 0; i < 16; i++)
            {
                Console.Write($"{matrixC[i / 4, i % 4]}\t");
                if (i % 4 == 3) { Console.Write("\n"); }
            }
            Console.WriteLine();
            //после квантования
            short[,] qtest = Quantization.QuantizationDirect(matrixC, matrixQ);
            for (int i = 0; i<16; i++)
            {
                Console.Write($"{qtest[i/4,i%4]}\t");
                if (i%4 == 3) {Console.Write("\n"); }
            }
            //после обратного
            Console.WriteLine();
            qtest = Quantization.QuantizationReverse(matrixC, matrixQ);
            for (int i = 0; i < 16; i++)
            {
                Console.Write($"{qtest[i / 4, i % 4]}\t");
                if (i % 4 == 3) { Console.Write("\n"); }
            }
        }
    }
}
