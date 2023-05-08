namespace CadwiseATMEmulator
{
    /// <summary>
    /// Контейнер с деньгами внутри банкомата
    /// </summary>
    public class Tank
    {
        public int Denomination { get; set; }

        public int Volume { get; set; } = 1000;

        public int Count { get; set; } = 0;
    }
}
