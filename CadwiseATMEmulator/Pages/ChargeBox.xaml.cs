using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for PutMoney.xaml
    /// </summary>
    public partial class ChargeBoxScreen : UserControl
    {
        public IChargeBox ChargeBox { get; set; }

        public BillsSelector BillsSelector { get; set; }

        
        
        public ChargeBoxScreen(IChargeBox chargeBox = null)
        {
            InitializeComponent();
            
            ChargeBox = chargeBox ?? new ChargeBox();
            DataContext = ChargeBox;
                
            BillsSelector = new BillsSelector(chargeBox);
            BillsSelector.SetValue(Grid.RowProperty, 1);
            MainGrid.Children.Add(BillsSelector);
            
           
        }
    }
}
