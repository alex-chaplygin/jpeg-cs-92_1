using JPEG_CLASS_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JPEG.Tests
{
    [TestClass]
    public class ChannelTest
    {
        Random r = new Random();
        [TestMethod]
        public void GetMatrix()
        {
            byte[,] exM = new byte[5, 5];
            for (int y = 0, c = 0; y < exM.GetLength(1); y++)
                for (int x = 0; x < exM.GetLength(0); x++, c++)
                    exM[x, y] = (byte)r.Next(0, 129);
            Channel C = new Channel(exM, 2, 2);
            byte[,] resM = C.GetMatrix();
            CollectionAssert.AreEqual(exM, resM);
        }
        [TestMethod]
        public void Split()
        {
            byte[,] M = new byte[9, 9]
            {{0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8}};
            byte[,] exM0 = new byte[8, 8]
            {{0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {2,2,2,2,2,2,2,2},
            {3,3,3,3,3,3,3,3},
            {4,4,4,4,4,4,4,4},
            {5,5,5,5,5,5,5,5},
            {6,6,6,6,6,6,6,6},
            {7,7,7,7,7,7,7,7}};
            byte[,] exM1 = new byte[8, 8]
            {{0,0,0,0,0,0,0,0},
            {1,0,0,0,0,0,0,0},
            {2,0,0,0,0,0,0,0},
            {3,0,0,0,0,0,0,0},
            {4,0,0,0,0,0,0,0},
            {5,0,0,0,0,0,0,0},
            {6,0,0,0,0,0,0,0},
            {7,0,0,0,0,0,0,0},};
            byte[,] exM2 = new byte[8, 8]
            {{8,8,8,8,8,8,8,8},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},};
            byte[,] exM3 = new byte[8, 8]
            {{8,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},};
            Channel C = new Channel(M, 2, 2);
            List<byte[,]> resList = C.Split();
            CollectionAssert.AreEqual(exM0, resList[0]);
            CollectionAssert.AreEqual(exM1, resList[1]);
            CollectionAssert.AreEqual(exM2, resList[2]);
            CollectionAssert.AreEqual(exM3, resList[3]);
        }
        [TestMethod]
        public void Collect()
        {
            byte[,] exM = new byte[9, 9]
            {{0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8},
            {0,1,2,3,4,5,6,7,8}};
            byte[,] M0 = new byte[8, 8]
            {{0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {2,2,2,2,2,2,2,2},
            {3,3,3,3,3,3,3,3},
            {4,4,4,4,4,4,4,4},
            {5,5,5,5,5,5,5,5},
            {6,6,6,6,6,6,6,6},
            {7,7,7,7,7,7,7,7}};
            byte[,] M1 = new byte[8, 8]
            {{0,0,0,0,0,0,0,0},
            {1,0,0,0,0,0,0,0},
            {2,0,0,0,0,0,0,0},
            {3,0,0,0,0,0,0,0},
            {4,0,0,0,0,0,0,0},
            {5,0,0,0,0,0,0,0},
            {6,0,0,0,0,0,0,0},
            {7,0,0,0,0,0,0,0},};
            byte[,] M2 = new byte[8, 8]
            {{8,8,8,8,8,8,8,8},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},};
            byte[,] M3 = new byte[8, 8]
            {{8,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},};
            List<byte[,]> Blocks = new List<byte[,]> { };
            Blocks.Add(M0);
            Blocks.Add(M1);
            Blocks.Add(M2);
            Blocks.Add(M3);
            Channel C = new Channel(new byte[9, 9], 2, 2);
            C.Collect(Blocks);
            byte[,] resM = C.GetMatrix();
            CollectionAssert.AreEqual(exM, resM);
        }
        [TestMethod]
        public void Sample()
        {
            byte[,] M = new byte[,]
            {{11,09,00,00,09,11},
            {09,05,00,00,05,09},
            {00,00,00,00,00,00},
            {00,00,00,00,00,00},
            {09,05,00,00,05,09},
            {11,09,00,00,09,11}};
            byte[,] exM = new byte[,]
            {{11,9,0,9},
            {9,5,0,5},
            {0,0,0,0},
            {9,5,0,5}};
            Channel C = new Channel(M, 4, 4);
            C.Sample(6, 6);
            byte[,] resM = C.GetMatrix();
            CollectionAssert.AreEqual(exM, resM);
        }
        [TestMethod]
        public void Resample()
        {
            byte[,] M = new byte[,]
            {{9,0,9},
            {0,0,0},
            {9,0,9}};
            byte[,] exM = new byte[,]
            {{09,04,00,04,09,13},
            {04,02,00,02,04,06},
            {00,00,00,00,00,00},
            {04,02,00,02,04,06},
            {09,04,00,04,09,13},
            {13,06,00,06,13,19}};
            Channel C = new Channel(M, 5, 5);
            C.Resample(10, 10);
            byte[,] resM = C.GetMatrix();
            CollectionAssert.AreEqual(exM, resM);
        }
    }
}
