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
    /// Выполняется разбиение каналов на блоки 8x8 и перемешивание блоков в зависимости от значений факторов H и V в каналах (см. раздел A.2.3 стандарта). Если канал один, то перемешивания не происходит. Подразумевается, что каналы уже были уменьшены (для всех каналов был вызван метод Sample с рассчитанными Hmax и Vmax).
    /// </summary>
    /// <param name="channels">Массив классов Channel</param>
    /// <returns>Возвращается список блоков всех каналов в необходимом порядке.</returns>
    public List<byte[,]> Interleave(Channel[] channels)
    {
	    var BLOCK_SIZE = 8;
	    var returnList = new List<byte[,]>();
	    var spliitedChannels = new List<List<byte[,]>>();

	    for (var i = 0; i < channels.Length; i++)
	    {
		    spliitedChannels.Add(channels[i].Split());
	    }
	    
	    for (var blockIndex = 0; blockIndex < spliitedChannels[0].Count/(channels[0].GetH*channels[0].GetV); blockIndex++)
	    {
		    for (var channelIndex = 0; channelIndex < channels.Length; channelIndex++)
		    {
			    var curChannel = channels[channelIndex];
			    var realWidth = curChannel.GetMatrix().GetLength(0);
			    var correctedWidth = realWidth % BLOCK_SIZE == 0 ? realWidth : BLOCK_SIZE*(realWidth / BLOCK_SIZE + 1);
			    var channelBlockInRow = correctedWidth / BLOCK_SIZE / curChannel.GetH;
			    var startIndex = (blockIndex/channelBlockInRow*curChannel.GetV)*(correctedWidth/BLOCK_SIZE) + ((blockIndex % channelBlockInRow) * curChannel.GetH);

			    for (var lineIndex = 0; lineIndex < curChannel.GetV; lineIndex+=1)
			    {
				    for (var rowIndex = 0; rowIndex < curChannel.GetH; rowIndex++)
				    {
					    Console.WriteLine($"block[{blockIndex}]: [{channelIndex}][{startIndex+lineIndex*(correctedWidth / BLOCK_SIZE)+rowIndex}]");
						returnList.Add(spliitedChannels[channelIndex][startIndex+lineIndex*(correctedWidth / BLOCK_SIZE)+rowIndex]);
				    }
			    }
		    }
	    }
	    return returnList;
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