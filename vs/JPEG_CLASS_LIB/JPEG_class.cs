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
    /// <param name="blocks">Список исходных блоков</param>
    /// <returns>Список изменённых блоков</returns>
    static public List<byte[,]> DCCalculating(List<byte[,]> blocks)
    {
        for (int i = blocks.Count -1; i > 0 ; i--)
        {
            blocks[i][0, 0] -= blocks[i - 1][0, 0];
        }
        return blocks;
    }
    /// <summary>
    /// Заменяет первое значение (DC-коэффициент) в каждом блоке на исходное значение
    /// </summary>
    /// <param name="blocks">Список блоков</param>
    /// <returns>Список исходных блоков</returns>
    static public List<byte[,]> DCRestore(List<byte[,]> blocks)
    {
        for (int i = 1; i < blocks.Count; i++)
        {
            blocks[i][0, 0] += blocks[i-1][0,0];
        }
        return blocks;
    }
    
    /// <summary>
    ////Разбивает исходную матрицу на блоки
    /// </summary>
    /// <param name="matrix">Исходная матрица</param>
    /// <returns>Список блоков</returns>
    public static List<byte[,]> Split(byte[,] matrix)
    {
    	    
	    var BLOCK_SIZE = 8;

	    var height = matrix.GetLength(0);
	    var width = matrix.GetLength(1);

	    var correctedWidth = width % BLOCK_SIZE == 0 ? width : BLOCK_SIZE*(width / BLOCK_SIZE + 1);
	    var correctedHeight = height % BLOCK_SIZE == 0 ? height : BLOCK_SIZE*(height / BLOCK_SIZE + 1);

	    var correctedMatrix = new byte[correctedHeight, correctedWidth];

	    for (var i = 0; i < height; i++)
	    {
		    for (var j = 0; j < width; j++)
		    {
			    correctedMatrix[i, j] = matrix[i, j];
		    }
	    }

	    var arr = new byte[correctedWidth * correctedHeight];
	    
	    for (var i = 0; i < correctedHeight; i++)
	    {
		    for (var j = 0; j < correctedWidth; j++)
		    {
			    arr[i*correctedWidth+j] = correctedMatrix[i, j];
		    }
	    }
	    
	    var splitResultList = new List<byte[,]>();
    
	    var blockCount = arr.Length / (BLOCK_SIZE*BLOCK_SIZE);
	    var blockInRow = correctedWidth / BLOCK_SIZE;
	    for (var blockIndex = 0; blockIndex < blockCount; blockIndex++)
	    {
		    splitResultList.Add(new byte[BLOCK_SIZE,BLOCK_SIZE]);
		    var startIndex = (blockIndex/blockInRow)*blockInRow*BLOCK_SIZE*BLOCK_SIZE+(blockIndex%blockInRow)*BLOCK_SIZE;
		    var innerIndex = 0;
		    for (var row = 0; row < BLOCK_SIZE; row++)
		    {
			    for (var column = 0; column<BLOCK_SIZE; column++) {
				    var realIndex = startIndex + row * correctedWidth + column;
				    splitResultList[blockIndex][innerIndex/BLOCK_SIZE,innerIndex%BLOCK_SIZE] = arr[realIndex];
				    innerIndex++;
			    }
		    }
	    }
	    return splitResultList;
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