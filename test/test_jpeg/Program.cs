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
