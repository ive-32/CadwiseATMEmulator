using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for SuccessPutMoney.xaml
    /// </summary>
    public partial class SuccessScreen : UserControl
    {
        public string ResultString { get; set; } = "Операция выполнена";

        public SuccessScreen(string message)
        {
            ResultString = message;
            InitializeComponent();
            DataContext = this;
        }
    }
}
