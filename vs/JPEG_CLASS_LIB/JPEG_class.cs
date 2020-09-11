using System;
using System.IO;

public class JPEG_CS
{
	int width;
	int height;
	
	public JPEG_CS(Stream name)
	{
		
	}
	
	public Point[,] UnPack()
	{
		return new Point[1,1];
	}
	
	public void Compress(Point[,] picture)
	{
		
	}
	
	public void SetArguments(int parameters)
	{
		
	}
}

public enum Arguments
{
	HIGH_QUALITY = 1 << 0,
	AVERAGE_QUALITY = 1 << 1,
	LOW_QUALITY = 1 << 2,
	ENTROPY_QUALITY = 1 << 3
}

public struct Point
	{
		public byte r;
		public byte g;
		public byte b;
	}