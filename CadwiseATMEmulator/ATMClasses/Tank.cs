namespace CadwiseATMEmulator
{
    /// <summary>
    /// Контейнер с деньгами внутри банкомата
    /// </summary>
    public class Tank
    {
        public int Denomination { get; }

        public int Volume { get; }

        public int Count { get; set; } 

        public Tank(int denomination, int volume = 1000, int count = 0) 
        {
            Denomination = denomination;
            Volume = volume;
            Count = count;
        }
    }
    
    
}
