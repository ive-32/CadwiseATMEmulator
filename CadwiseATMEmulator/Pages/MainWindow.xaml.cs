using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        AtmVM AtmVM = new AtmVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = AtmVM;

            var bind = new CommandBinding(ATMCommands.PutMoney);
            bind.Executed += AtmVM.PutMoney_Executed;
            bind.CanExecute += AtmVM.PutMoney_CanExecute;
            CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.GetMoney);
            bind.Executed += AtmVM.GetMoney_Executed;
            bind.CanExecute += AtmVM.GetMoney_CanExecute;
            CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowAskMoneyScreen);
            bind.Executed += AtmVM.ShowAskMoneyScreen_Executed;
            bind.CanExecute += AtmVM.ShowAskMoneyScreen_CanExecute;
            CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowPutMoneyScreen);
            bind.Executed += AtmVM.ShowPutMoneyScreen_Executed;
            bind.CanExecute += AtmVM.ShowPutMoneyScreen_CanExecute;
            CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowMainScreen);
            bind.Executed += AtmVM.ShowMainScreen_Executed;
            bind.CanExecute += AtmVM.ShowMainScreen_CanExecute;
            CommandBindings.Add(bind);

            AtmVM.CurrentContentVM = new MainPage();
            DebugListBox.ItemsSource = AtmVM.AtmTanks;
        }

    }
}
