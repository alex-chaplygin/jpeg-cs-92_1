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

    /// <summary>
    /// ��������� ����������� ������� ������������� � ������� �������� ������� �����������.
    /// </summary>
    /// <param name="MatrixCoefficient">�������� ������� �������������</param>
    /// <param name="MatrixQuantization">������� �����������</param>
    /// <returns>��������� ������� �������������</returns>
    static short[,] QuantizationDirect(short[,] MatrixCoefficient, short[,] MatrixQuantization)
    {
        for (int i = 0; i < MatrixCoefficient.GetLength(0);i++)
        {
            for (int j = 0; j < MatrixCoefficient.GetLength(1); j++)
            {
                MatrixCoefficient[i, j] /= MatrixQuantization[i, j];
            }
        }
        return MatrixCoefficient;
    }
    /// <summary>
    /// ��������� �������� ����������� ������� ������������� � ������� �������� ������� �����������
    /// </summary>
    /// <param name="MatrixCoefficient">������� �������������, ��������� �����������</param>
    /// <param name="MatrixQuantization">������� �����������</param>
    /// <returns>�������� ������� �������������</returns>
    static short[,] QuantizationReverse(short[,] MatrixCoefficient, short[,] MatrixQuantization)
    {
        for (int i = 0; i < MatrixCoefficient.GetLength(0); i++)
        {
            for (int j = 0; j < MatrixCoefficient.GetLength(1); j++)
            {
                MatrixCoefficient[i, j] *= MatrixQuantization[i, j];
            }
        }
        return MatrixCoefficient;
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