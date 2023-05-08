using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for PutMoney.xaml
    /// </summary>
    public partial class ChargeBoxScreen : UserControl
    {
        public ChargeBox ChargeBox { get; set; }

        public ChargeBoxScreen(ChargeBox chargeBox)
        {
            InitializeComponent();
            DataContext = chargeBox;
            ChargeBox = chargeBox;

            BillsSelector billsSelector = new BillsSelector(chargeBox);
            MainGrid.Children.Add(billsSelector);

            billsSelector.SetValue(Grid.RowProperty, 1);
        }
    }
}
