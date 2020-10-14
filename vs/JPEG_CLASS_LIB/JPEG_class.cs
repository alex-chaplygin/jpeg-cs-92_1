using JPEG_CLASS_LIB;
using System;
using System.IO;
using System.Collections.Generic;

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

    /// <summary>
    /// �������� ������ �������� � ������ ����� �� ����� �� DC �����������
    /// </summary>
    /// <param name="blocks">������ ������</param>
    static public void CalculatingDC(ref List<short[,]> blocks)
    {
        short[] NativeValues = new short[blocks.Count];
        NativeValues[0] = blocks[0][0, 0];
        for (int i = 1; i < blocks.Count ; i++)
        {
            NativeValues[i] = blocks[i][0, 0];
            blocks[i][0, 0] -= blocks[i-1][0, 0];
        }
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