using System;

namespace CadwiseATMEmulator.Helpers
{
    /// <summary>
    /// Класс вычисления сторон прямоугольника для размещения N объектов в сетке
    /// </summary>
    internal class GridSizeCalculator
    {
        public static (int, int) GetGridSize(int countOfCells)
        {
            if (countOfCells <= 0) 
                return (0, 0);

            var width = (int)Math.Round(Math.Sqrt(countOfCells));
            var height = countOfCells / width;

            if (width * height < countOfCells)
                width++;

            return (Math.Max(width, height), Math.Min(width,height));
        }
    }
}
