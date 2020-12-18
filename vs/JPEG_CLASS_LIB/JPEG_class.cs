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
	public short[,] LQT = new short[8, 8];

	/// <summary>
	/// Таблица квантования цветности.
	/// </summary>
	public short[,] CQT = new short[8, 8];

	/// <summary>
	/// Поток JPEG файла
	/// </summary>
	private Stream MainStream;

	/// <summary>
	/// Создает объект из потока.
	/// </summary>
	/// <param name="name">Поток для создания объекта.</param>
	public JPEG_CS(Stream name)
	{
		MainStream = name;
	}

	/// <summary>
	/// Распаковывает содержимое JPEG файла, преобразует цвет кадра из YUV в RGB.
	/// </summary>
	/// <returns>Массив точек изображения в RGB</returns>
	public Point[,] UnPack()
	{
		JPEGFile JF = new JPEGFile(MainStream);
		Channel[] channeles = JF.DecodeFrame();
		Point[,] result = ImageConverter.YUVToRGB(channeles[0].GetMatrix(), channeles[1].GetMatrix(), channeles[2].GetMatrix());
		return result;
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
		short[,] LQT =
		{
			{16, 12, 14, 14, 18, 24, 49, 72},
			{11, 12, 13, 17, 22, 35, 64, 92},
			{10, 14, 16, 22, 37, 55, 78, 95},
			{16, 19, 24, 29, 56, 64, 87, 98},
			{24, 26, 40, 51, 68, 81, 103, 112},
			{40, 58, 57, 87, 109, 104, 121, 100},
			{51, 60, 69, 80, 103, 113, 120, 103},
			{61, 55, 56, 62, 77, 92, 101, 99}
		};
		short[,] CQT =
		{
			{17, 18, 24, 47, 99, 99, 99, 99},
			{18, 21, 26, 66, 99, 99, 99, 99},
			{24, 26, 56, 99, 99, 99, 99, 99},
			{47, 66, 99, 99, 99, 99, 99, 99},
			{99, 99, 99, 99, 99, 99, 99, 99},
			{99, 99, 99, 99, 99, 99, 99, 99},
			{99, 99, 99, 99, 99, 99, 99, 99},
			{99, 99, 99, 99, 99, 99, 99, 99}
		};
		if ((param & (int)Parameters.HIGH_QUALITY) != 0)
		{
			for (int y = 0; y < LQT.GetLength(1); y++)
			{
				for (int x = 0; x < LQT.GetLength(0); x++)
				{
					this.LQT[x, y] = (short)(LQT[x, y] >> 1);
					this.CQT[x, y] = (short)(CQT[x, y] >> 1);
				}
			}
		}

		else if ((param & (int)Parameters.MEDIUM_QUALITY) != 0)
		{
			this.LQT = LQT;
			this.CQT = CQT;
		}
		else if ((param & (int)Parameters.LOW_QUALITY) != 0)
		{
			for (int y = 0; y < LQT.GetLength(1); y++)
			{
				for (int x = 0; x < LQT.GetLength(0); x++)
				{
					this.LQT[x, y] = (short)(LQT[x, y] << 1);
					this.CQT[x, y] = (short)(CQT[x, y] << 1);
				}
			}
		}
	}

	/// <summary>
	/// Осуществляет все необходимые DCT преобразования для списка блоков
	/// </summary>
	/// <param name="blocks">Список блоков одного канала после разбиения на блоки</param>
	/// <param name="quantizationMatrix">Матрица квантования</param>
	/// <returns>Список коэффициентов блоков одного канала</returns>
	public List<short[]> FDCT(List<byte[,]> blocks, short[,] quantizationMatrix)
	{
		List<short[]> Result = new List<short[]> { };
		List<short[,]> Temp = new List<short[,]> { };
		for (int i = 0; i < blocks.Count; i++)
		{
			Temp.Add(DCT.Shift(blocks[i]));
			Temp[i] = DCT.FDCT(Temp[i]);
			Temp[i] = DCT.QuantizationDirect(Temp[i], quantizationMatrix);
		}
		Temp = DCT.DCCalculating(Temp);
		for (int i = 0; i < Temp.Count; i++)
			Result.Add(DCT.Zigzag(Temp[i]));
		return Result;
	}

	/// <summary>
	/// Осуществляет все необходимые обратные DCT преобразования для списка блоков
	/// </summary>
	/// <param name="data">Список коэффициентов блоков одного канала</param>
	/// <param name="quantizationMatrix">Матрица квантования</param>
	/// <returns>Список блоков одного канала для сборки</returns>
	public List<byte[,]> IDCT(List<short[]> data, short[,] quantizationMatrix)
	{
		List<byte[,]> Result = new List<byte[,]> { };
		List<short[,]> Temp = new List<short[,]> { };
		for (int i = 0; i < data.Count; i++)
			Temp.Add(DCT.ReZigzag(data[i]));
		for (int i = 0; i < Temp.Count; i++)
		{
			Temp[i] = DCT.QuantizationReverse(Temp[i], quantizationMatrix);
			Temp[i] = DCT.IDCT(Temp[i]);
			Result.Add(DCT.ReverseShift(Temp[i]));			
		}		
		return Result;
	}
    
	/// <summary>
	/// Собирает списки каждого канала, далее к ним применяет IDCT преобразование, затем собирает блоки 8x8 в каналы и выполняет обратное масштабирование матриц каналов. Если канал один, то все блоки записываются в канал слева-направо, сверху вниз.
	/// </summary>
	/// <param name="channels">Каналы с пустыми матрицами, но с корректными шириной, высотой и значениями H и V</param>
	/// <param name="blocks">Список short[] перемешанных блоков</param>
    public void Collect(Channel[] channels, List<short[]> blocks)
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
		    
		    var tmpArray = new short[(correctedWidth*correctedHeight)/BLOCK_SIZE/BLOCK_SIZE][];

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
		    curChannel.Collect(IDCT(tmpArray.ToList(), LQT));
	    }
    }

	/// <summary>
	/// Выполняется разбиение каналов на блоки 8x8 их FDCT преобразование и перемешивание блоков в зависимости от значений факторов H и V в каналах (см. раздел A.2.3 стандарта). Если канал один, то перемешивания не происходит. Подразумевается, что каналы уже были уменьшены (для всех каналов был вызван метод Sample с рассчитанными Hmax и Vmax).
	/// </summary>
	/// <param name="channels">Массив классов Channel</param>
	/// <param name="quantizationMatrixes">Список матриц квантования для каждого из каналов</param>
	/// <returns>Cписок short[] блоков всех каналов в необходимом порядке.</returns>
	public List<short[]> Interleave(Channel[] channels, List<short[,]> quantizationMatrixes)
    {
		var BLOCK_SIZE = 8;
	    var returnList = new List<short[]>();
	    var spliitedChannels = new List<List<short[]>>();

	    for (var i = 0; i < channels.Length; i++)
	    {
		    spliitedChannels.Add(FDCT(channels[i].Split(), quantizationMatrixes[i]));
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
	MEDIUM_QUALITY = 1 << 1,
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
