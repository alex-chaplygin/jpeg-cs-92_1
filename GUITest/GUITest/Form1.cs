﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JPEG_CLASS_LIB;


namespace GUITest
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            JPEG_CS Test = new JPEG_CS(File.Open("testjpeg.jpg", FileMode.Open));
            Point[,] CurrentJPEG = Test.UnPack();
            int width = CurrentJPEG.GetLength(0);
            int lenth = CurrentJPEG.GetLength(1);
            Bitmap BM = new Bitmap(width, lenth);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < lenth; j++) BM.SetPixel(i, j, Color.FromArgb(CurrentJPEG[i, j].r, CurrentJPEG[i, j].g, CurrentJPEG[i, j].b));            
            InitializeComponent();
            pictureBox1.Size = BM.Size;
            pictureBox1.Image = BM;         
        }
    }
}
