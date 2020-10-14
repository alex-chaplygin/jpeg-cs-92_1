using JPEG_CLASS_LIB;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Библиотека предназначена для сжатия и распаковки изображений в формате JPEG.
/// </summary>
public class JPEG_CS
{
    /// <summary>
    /// Ширина изображения.
    /// </summary>
	int width;
    /// <summary>
    /// Высота изображения.
    /// </summary>
	int height;
	
    /// <summary>
    /// Создает объект из потока.
    /// </summary>
    /// <param name="name">Поток для создания объекта.</param>
	public JPEG_CS(Stream name)
	{

	}
	
    /// <summary>
    /// Распаковывает содержимое JPEG и возвращает изображение.
    /// </summary>
    /// <returns></returns>
	public Point[,] UnPack()
	{
		return new Point[100,100];
	}
	
    /// <summary>
    /// Сжимает изображение и записывает его в поток.
    /// </summary>
    /// <param name="picture">Изображение.</param>
	public void Compress(Point[,] picture)
	{
		
	}
	
    /// <summary>
    /// Устанавливает параметры сжатия JPEG.
    /// </summary>
    /// <param name="parameters">Параметр сжатия JPEG.</param>
	public void SetArguments(int parameters)
	{
		
	}

    /// <summary>
    /// Заменяет первое значение в каждом блоке из списа на DC коэффициент
    /// </summary>
    /// <param name="blocks">Список блоков</param>
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
/// Параметры сжатия JPEG.
/// </summary>
public enum Arguments
{
	HIGH_QUALITY = 1 << 0,
	AVERAGE_QUALITY = 1 << 1,
	LOW_QUALITY = 1 << 2,
	ENTROPY_QUALITY = 1 << 3
}

/// <summary>
/// Точка.
/// </summary>
public struct Point
{
    /// <summary>
    /// Красный byte.
    /// </summary>
    public byte r;
    /// <summary>
    /// Зеленый byte.
    /// </summary>
    public byte g;
    /// <summary>
    /// Синий byte.
    /// </summary>
    public byte b;
}