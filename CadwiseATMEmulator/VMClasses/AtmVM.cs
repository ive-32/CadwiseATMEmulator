﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System;

namespace CadwiseATMEmulator
{
    public class AtmVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl CurrentContentVM { get; set; }

        public IATM AtmEmulator = new ATM();

        // для отображения содержимого банкомата
        // увы ничего лучше не придумал чем Event и ObservableCollection здесь
        public ObservableCollection<string> AtmTanks = new ObservableCollection<string>();

        public AtmVM() 
        {
            AtmEmulator.OnChange += AtmEmulator_OnChange;
            AtmEmulator_OnChange();
        }


        public void ShowAskMoneyScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            AtmEmulator.ChargeBox.OperationDescription = "Укажите купюры, которые выдать";
            AtmEmulator.ChargeBox.OperationName = "Выдать";
            AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.GetMoney;
            AtmEmulator.SetLimitsForGetMoney();
            CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            OnPropertyChanged("CurrentContentVM");
        }

        public void ShowPutMoneyScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AtmEmulator.ChargeBox.OperationDescription = "Положите деньги в приемник";
            AtmEmulator.ChargeBox.OperationName = "Внести";
            AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.PutMoney;
            AtmEmulator.SetLimitsForPutMoney();
            CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            OnPropertyChanged("CurrentContentVM");
        }

        public async void PutMoney_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentContentVM = new AwaitingScreen();
            OnPropertyChanged("CurrentContentVM");
            
            var result = await AtmEmulator.PutMoney();

            if (result.Result == TransactionResultType.Success)
            {
                CurrentContentVM = new SuccessScreen(result.ResultMessage);
            }

            if (result.Result == TransactionResultType.MoneyReturned)
            {
                CurrentContentVM = new SuccessScreen(result.ResultMessage + 
                    Environment.NewLine +
                    Environment.NewLine + 
                    AtmEmulator.ChargeBox.ToString());

                //для варианта когда возврат отображается в виде селектора купюр
                /*AtmEmulator.ChargeBox.OperationDescription = result.ResultMessage + " Заберите возврат";
                AtmEmulator.ChargeBox.OperationName = "Готово";
                AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.ShowMainScreen;
                CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);*/
            };

            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged("CurrentContentVM");
        }

        public async void GetMoney_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentContentVM = new AwaitingScreen();
            OnPropertyChanged("CurrentContentVM");

            var result = await AtmEmulator.GetMoney();

            CurrentContentVM = new SuccessScreen(result.ResultMessage +
                    Environment.NewLine +
                    Environment.NewLine +
                    AtmEmulator.ChargeBox.ToString());

            //для варианта когда выдача отображается в виде селектора купюр
            /*CurrentContentVM = new ChargeBoxScreen(AtmEmulator.ChargeBox);
            AtmEmulator.ChargeBox.OperationDescription = result.ResultMessage + " Заберите деньги из приемника";
            AtmEmulator.ChargeBox.OperationName = "Готово";
            AtmEmulator.ChargeBox.RoutedCommand = ATMCommands.ShowMainScreen;*/

            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged("CurrentContentVM");
        }


        public void ShowMainScreen_Executed(object sender, ExecutedRoutedEventArgs e)
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

        public void ShowMainScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = !(CurrentContentVM is MainPage) && !(CurrentContentVM is AwaitingScreen);

        public void PutMoney_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = AtmEmulator.ChargeBox.BillsStacks.Any(bs => bs.Count > 0);

        public void GetMoney_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = AtmEmulator.ChargeBox.BillsStacks.Any(bs => bs.Count > 0);

        public void AtmEmulator_OnChange()
        {
            AtmTanks.Clear();
            AtmTanks.Add("Состояние ящиков банкомата");
            foreach (var tank in AtmEmulator.Tanks)
                AtmTanks.Add($"{tank.Denomination} : {tank.Count} of {tank.Volume}");
        }
    }
}
