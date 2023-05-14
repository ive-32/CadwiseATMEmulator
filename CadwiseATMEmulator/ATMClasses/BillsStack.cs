namespace CadwiseATMEmulator
{
    /// <summary>
    /// Пачка купюр 
    /// </summary>
    public class BillsStack
    {
        public int MaxValue { get; set; } 

        public int Denomination { get; } //номинал купюры

        public int Count { get; set; }

        public BillsStack(int denomination, int maxValue = 20, int count = 0)
        {
            MaxValue = maxValue;
            Denomination = denomination;
            Count = count;
        }

    }
}
