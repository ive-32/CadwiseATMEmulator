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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl CurrentContentVM { get; set; }

        public ATM AtmEmulator = new ATM();

        // для отображения содержимого банкомата
        // увы ничего лучше не придумал чем Event и ObservableCollection здесь
        ObservableCollection<string> AtmTanks = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            CommandBinding bind = new CommandBinding(ATMCommands.PutMoney);
            bind.Executed += PutMoney_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.GetMoney);
            bind.Executed += GetMoney_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowAskMoneyScreen);
            bind.Executed += ShowAskMoneyScreen_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowPutMoneyScreen);
            bind.Executed += ShowPutMoneyScreen_Executed;
            this.CommandBindings.Add(bind);

            bind = new CommandBinding(ATMCommands.ShowMainScreen);
            bind.Executed += ShowMainScreen_Executed;
            bind.CanExecute += ShowMainScreen_CanExecute;
            this.CommandBindings.Add(bind);

            CurrentContentVM = new MainPage();

            AtmEmulator.OnChange += AtmEmulator_OnChange;
            DebugListBox.ItemsSource = AtmTanks;
            AtmEmulator_OnChange();
        }

        private void ShowAskMoneyScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            AtmEmulator.ChargeBox.OperationDescription = "Укажите купюры, которые выдать";
            AtmEmulator.ChargeBox.OperationName = "Выдать";
            AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.GetMoney;
            AtmEmulator.SetLimitsForGetMoney();
            CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            OnPropertyChanged("CurrentContentVM");
        }

        private void ShowPutMoneyScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AtmEmulator.ChargeBox.OperationDescription = "Положите деньги в приемник";
            AtmEmulator.ChargeBox.OperationName = "Внести";
            AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.PutMoney;
            AtmEmulator.SetLimitsForPutMoney();
            CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            OnPropertyChanged("CurrentContentVM");
        }

        private void PutMoney_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var result = AtmEmulator.PutMoney((CurrentContentVM as ChargeBoxScreen).ChargeBox);

            if (result.Result == TransactionResultType.Success)
                CurrentContentVM = new MainPage();

            if (result.Result == TransactionResultType.MoneyReturned)
            {
                AtmEmulator.ChargeBox.OperationDescription = result.ResultMessage + " Заберите возврат";
                AtmEmulator.ChargeBox.OperationName = "Готово";
                AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.ShowMainScreen;
                CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            };

            OnPropertyChanged("CurrentContentVM");
        }

        private void GetMoney_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var result = AtmEmulator.GetMoney();

            (CurrentContentVM as ChargeBoxScreen).ChargeBox.OperationDescription = result.ResultMessage + " Заберите деньги из приемника";
            (CurrentContentVM as ChargeBoxScreen).ChargeBox.OperationName = "Готово";
            (CurrentContentVM as ChargeBoxScreen).ChargeBox.RoutedCommand = ATMCommands.ShowMainScreen;
            CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            OnPropertyChanged("CurrentContentVM");
        }


        private void ShowMainScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //очищаю приемник купюр здесь в качестве эмуляции.
            //По идее он очищается руками человека
            //но у нас в эмуляторе это происходит по кнопке - "вернутся на главный экран" т.е. здесь
            //можно дописать возврат оставшихся в приемнике купюр в ящики 
            AtmEmulator.ChargeBox.ClearChargeBox(); 

            CurrentContentVM = new MainPage();
            OnPropertyChanged("CurrentContentVM");
        }

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void ShowMainScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = !(CurrentContentVM is MainPage);

        private void AtmEmulator_OnChange()
        {
            AtmTanks.Clear();
            AtmTanks.Add("Состояние ящиков банкомата");
            foreach (var tank in AtmEmulator.Tanks)
                AtmTanks.Add($"{tank.Denomination} : {tank.Count} of {tank.Volume}");
        }
    }
}
