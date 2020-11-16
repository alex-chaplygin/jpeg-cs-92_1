using JPEG_CLASS_LIB;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
	/// Таблица квантования яркости.
	/// </summary>
	byte[] LQT = new byte[64];

	/// <summary>
	/// Таблица квантования цветности.
	/// </summary>
	byte[] CQT = new byte[64];

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
	/// <param name="param">Параметр сжатия JPEG.</param>
	public void SetParameters(int param)
	{
		byte[] LQT = new byte[] { 16, 11, 10, 16, 124, 140, 151, 161, 12, 12, 14, 19, 126, 158, 160, 155, 14, 13, 16, 24, 140, 157, 169, 156, 14, 17, 22, 29, 151, 187, 180, 162, 18, 22, 37, 56, 168, 109, 103, 177, 24, 35, 55, 64, 181, 104, 113, 192, 49, 64, 78, 87, 103, 121, 120, 101, 72, 92, 95, 98, 112, 100, 103, 199 };
		byte[] CQT = new byte[] { 17, 18, 24, 47, 99, 99, 99, 99, 18, 21, 26, 66, 99, 99, 99, 99, 24, 26, 56, 99, 99, 99, 99, 99, 47, 66, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 };
		if ((param & 0x1) == (int)(Parameters.HIGH_QUALITY))
		{
			for (int i = 0; i < LQT.Length; i++)
			{
				this.LQT[i] = (byte)(LQT[i] >> 1);
				this.CQT[i] = (byte)(CQT[i] >> 1);
			}
		}
		else if ((param & 0x2) == (int)(Parameters.AVERAGE_QUALITY))
		{
			this.LQT = LQT;
			this.CQT = CQT;
		}
		else if ((param & 0x4) == (int)(Parameters.LOW_QUALITY))
		{
			for (int i = 0; i < LQT.Length; i++)
			{
				this.LQT[i] = (byte)(LQT[i] << 1);
				this.CQT[i] = (byte)(CQT[i] << 1);
			}
		}
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
	/// Собирает блоки 8x8 в каналы и выполняет обратное масштабирование матриц каналов. Если канал один, то все блоки записываются в канал слева-направо, сверху вниз.
	/// </summary>
	/// <param name="channels">Каналы с пустыми матрицами, но с корректными шириной, высотой и значениями H и V</param>
	/// <param name="blocks">Список перемешанных блоков</param>
    public void Collect(Channel[] channels, List<byte[,]> blocks)
    {
	    var BLOCK_SIZE = 8;
	    var macroBlockCount = 0;
	    foreach (var channel in channels)
	    {
		    macroBlockCount += channel.GetH * channel.GetV;
	    }

	    for (var channelIndex = 0; channelIndex < channels.Length; channelIndex++)
	    {
		    var curChannel = channels[channelIndex];
		    
		    var realWidth = curChannel.GetMatrix().GetLength(0);
		    var realHeight = curChannel.GetMatrix().GetLength(1);
		    var correctedWidth = realWidth % BLOCK_SIZE == 0 ? realWidth : BLOCK_SIZE*(realWidth / BLOCK_SIZE + 1);
		    var correctedHeight = realHeight % BLOCK_SIZE == 0 ? realHeight : BLOCK_SIZE*(realHeight / BLOCK_SIZE + 1);
		    
		    var tmpArray = new byte[(correctedWidth*correctedHeight)/BLOCK_SIZE/BLOCK_SIZE][,];

		    var otherChannelOffset = 0;
		    for (var offsetChannelIndex = 0; offsetChannelIndex < channelIndex; offsetChannelIndex++)
		    {
			    otherChannelOffset += channels[offsetChannelIndex].GetH * channels[offsetChannelIndex].GetV;
		    }
		    
		    for (var blockIndex = 0; blockIndex < correctedWidth * correctedHeight / BLOCK_SIZE / BLOCK_SIZE / (curChannel.GetH * curChannel.GetV); blockIndex++)
		    {
			    var channelBlockInRow = correctedWidth / BLOCK_SIZE / curChannel.GetH;
			    var startIndex = (blockIndex/channelBlockInRow*curChannel.GetV)*(correctedWidth/BLOCK_SIZE) + ((blockIndex % channelBlockInRow) * curChannel.GetH);
			    
			    var innerBlocksGroup =
				    blocks.GetRange(macroBlockCount*blockIndex+otherChannelOffset, curChannel.GetH * curChannel.GetV);
			    for (var lineIndex = 0; lineIndex < curChannel.GetV; lineIndex+=1)
			    {
				    for (var rowIndex = 0; rowIndex < curChannel.GetH; rowIndex++)
				    {
					    tmpArray[startIndex + lineIndex * (correctedWidth / BLOCK_SIZE) + rowIndex] =
						    innerBlocksGroup[0];
					    innerBlocksGroup.RemoveAt(0);
				    }
			    }
		    }
		    curChannel.Collect(tmpArray.ToList());
	    }
    }
    
    /// <summary>
    /// Выполняется разбиение каналов на блоки 8x8 и перемешивание блоков в зависимости от значений факторов H и V в каналах (см. раздел A.2.3 стандарта). Если канал один, то перемешивания не происходит. Подразумевается, что каналы уже были уменьшены (для всех каналов был вызван метод Sample с рассчитанными Hmax и Vmax).
    /// </summary>
    /// <param name="channels">Массив классов Channel</param>
    /// <returns>Cписок блоков всех каналов в необходимом порядке.</returns>
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
public enum Parameters
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