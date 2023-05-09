using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for PutMoney.xaml
    /// </summary>
    public partial class ChargeBoxScreen : UserControl
    {
        public IChargeBox ChargeBox { get; set; }

        public ChargeBoxScreen(IChargeBox chargeBox)
        {
            InitializeComponent();
            DataContext = chargeBox;
            ChargeBox = chargeBox ?? new ChargeBox();

            var billsSelector = new BillsSelector(chargeBox);
            MainGrid.Children.Add(billsSelector);

            billsSelector.SetValue(Grid.RowProperty, 1);
        }
    }
}
