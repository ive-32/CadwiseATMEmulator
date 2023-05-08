using System.Windows.Input;

namespace CadwiseATMEmulator
{
    public class ATMCommands
    {
        static ATMCommands()
        {
            PutMoney = new RoutedCommand("PutMoney", typeof(MainWindow));
            GetMoney = new RoutedCommand("GetMoney", typeof(MainWindow));
            ShowPutMoneyScreen = new RoutedCommand("ShowPutMoneyScreen", typeof(MainWindow));
            ShowAskMoneyScreen = new RoutedCommand("ShowAskMoneyScreen", typeof(MainWindow));
            ShowGetChargeScreen = new RoutedCommand("ShowGetChargeScreen", typeof(MainWindow));
            ShowMainScreen = new RoutedCommand("ShowMainScreen", typeof(MainWindow));
        }

        public static RoutedCommand PutMoney { get; set; }

        public static RoutedCommand ShowPutMoneyScreen { get; set; }

        public static RoutedCommand ShowAskMoneyScreen { get; set; }

        public static RoutedCommand ShowGetChargeScreen { get; set; }

        public static RoutedCommand GetMoney { get; set; }

        public static RoutedCommand ShowMainScreen { get; set; }

    }
}
