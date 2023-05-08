using System.Collections.Generic;
using System.Windows.Input;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Бокс для приема/выдачи купюр
    /// </summary>
    public class ChargeBox : IChargeBox
    {
        public List<BillsStack> BillsStacks { get; set; } = new List<BillsStack>();

        public string OperationDescription { get; set; }

        public string OperationName { get; set; }

        public RoutedCommand RoutedCommand { get; set; }

        public ChargeBox()
        {
            foreach (var denomination in ATM.BanknotesTypes)
                BillsStacks.Add(new BillsStack(denomination));
        }

        /// <summary>
        /// очистка ящика - имитация того что деньги забрал человек
        /// </summary>
        public void ClearChargeBox()
        {
            foreach (var billsStack in BillsStacks)
                billsStack.Count = 0;
        }
    }
}
