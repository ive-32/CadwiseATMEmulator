using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for SuccessPutMoney.xaml
    /// </summary>
    public partial class SuccessPutMoney : UserControl
    {
        public string ResultString { get; set; } = "Операция выполнена";

        public SuccessPutMoney(string message)
        {
            ResultString = message;
            InitializeComponent();
            DataContext = this;
        }
    }
}
