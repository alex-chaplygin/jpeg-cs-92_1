using System;
using System.IO;

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
    /// Реализует квантование матрицы коэффициентов с помощью заданной матрицы квантования.
    /// </summary>
    /// <param name="MatrixCoefficient">Исходная матрица коэффициентов</param>
    /// <param name="MatrixQuantization">Матрица квантования</param>
    /// <returns>Изменённая матрица коэффициентов</returns>
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
    /// Реализует обратное квантование матрицы коэффициентов с помощью заданной матрицы квантования
    /// </summary>
    /// <param name="MatrixCoefficient">Матрица коэффициентов, прошедшая квантование</param>
    /// <param name="MatrixQuantization">Матрица квантования</param>
    /// <returns>Исходная матрица коэффициентов</returns>
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