using System;
using System.IO;

/// <summary>
/// ���������� ������������� ��� ������ � ���������� ����������� � ������� JPEG.
/// </summary>
public class JPEG_CS
{
    /// <summary>
    /// ������ �����������.
    /// </summary>
	int width;
    /// <summary>
    /// ������ �����������.
    /// </summary>
	int height;
	
    /// <summary>
    /// ������� ������ �� ������.
    /// </summary>
    /// <param name="name">����� ��� �������� �������.</param>
	public JPEG_CS(Stream name)
	{
		
	}
	
    /// <summary>
    /// ������������� ���������� JPEG � ���������� �����������.
    /// </summary>
    /// <returns></returns>
	public Point[,] UnPack()
	{
		return new Point[100,100];
	}
	
    /// <summary>
    /// ������� ����������� � ���������� ��� � �����.
    /// </summary>
    /// <param name="picture">�����������.</param>
	public void Compress(Point[,] picture)
	{
		
	}
	
    /// <summary>
    /// ������������� ��������� ������ JPEG.
    /// </summary>
    /// <param name="parameters">�������� ������ JPEG.</param>
	public void SetArguments(int parameters)
	{
		
	}
}

/// <summary>
/// ��������� ������ JPEG.
/// </summary>
public enum Arguments
{
	HIGH_QUALITY = 1 << 0,
	AVERAGE_QUALITY = 1 << 1,
	LOW_QUALITY = 1 << 2,
	ENTROPY_QUALITY = 1 << 3
}

/// <summary>
/// �����.
/// </summary>
public struct Point
{
    /// <summary>
    /// ������� byte.
    /// </summary>
    public byte r;
    /// <summary>
    /// ������� byte.
    /// </summary>
    public byte g;
    /// <summary>
    /// ����� byte.
    /// </summary>
    public byte b;
}