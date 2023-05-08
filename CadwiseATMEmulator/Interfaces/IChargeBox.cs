using System.Collections.Generic;
using System.Windows.Input;

namespace CadwiseATMEmulator
{
    public interface IChargeBox
    {
        List<BillsStack> BillsStacks { get; set; }

        string OperationDescription { get; set; }
        string OperationName { get; set; }
        RoutedCommand RoutedCommand { get; set; }

        void ClearChargeBox();
    }
}