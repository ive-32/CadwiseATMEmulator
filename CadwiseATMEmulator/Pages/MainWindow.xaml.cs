using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

            CommandBinding bind = new CommandBinding(ATMCommands.PutMoney);
            bind.Executed += AtmVM.PutMoney_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.GetMoney);
            bind.Executed += AtmVM.GetMoney_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowAskMoneyScreen);
            bind.Executed += AtmVM.ShowAskMoneyScreen_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowPutMoneyScreen);
            bind.Executed += AtmVM.ShowPutMoneyScreen_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowMainScreen);
            bind.Executed += AtmVM.ShowMainScreen_Executed;
            bind.CanExecute += AtmVM.ShowMainScreen_CanExecute;
            this.CommandBindings.Add(bind);

            AtmVM.CurrentContentVM = new MainPage();

            DebugListBox.ItemsSource = AtmVM.AtmTanks;

        }

    }
}
