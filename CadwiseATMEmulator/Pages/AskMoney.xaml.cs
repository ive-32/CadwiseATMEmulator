using System.Windows.Controls;
using System.Windows.Media;

namespace CadwiseATMEmulator
{
    //<local:ChargeBoxScreen Grid.Row="1"/>
    public partial class AskMoney : UserControl
    {
        public IChargeBox ChargeBox { get; set; }
        public TextBox TotalAmount { get; set; }
        
        public AskMoney()
        {
            InitializeComponent();
            ChargeBoxScreen chargeBoxScreen = new ChargeBoxScreen(ChargeBox);
            chargeBoxScreen.SetValue(Grid.RowProperty, 0);
            chargeBoxScreen.MainGrid.RowDefinitions.Insert(1, new RowDefinition{
                Height = new System.Windows.GridLength(70, System.Windows.GridUnitType.Pixel)});

            chargeBoxScreen.BillsSelector.MainGrid.RowDefinitions[1].Height =
                new System.Windows.GridLength(70, System.Windows.GridUnitType.Pixel);
            chargeBoxScreen.BillsSelector.SetValue(Grid.RowProperty, 2);
            chargeBoxScreen.ButtonPutMoney.SetValue(Grid.RowProperty, 3);
            MainGrid.Children.Add(chargeBoxScreen);

            TotalAmount = new TextBox();
            TotalAmount.SetValue(BackgroundProperty, new SolidColorBrush(Colors.Beige));
            TotalAmount.SetValue(Grid.RowProperty, 1);
            chargeBoxScreen.MainGrid.Children.Add(TotalAmount);
        }
    }
}