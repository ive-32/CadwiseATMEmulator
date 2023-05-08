using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Interaction logic for BillsCounter.xaml
    /// </summary>
    public partial class BillsCounter : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BillsStack BillsStack { get; set; }

        public string CountText
        {
            get => BillsStack.Count == 0 ? "" : BillsStack.Count.ToString();
            set
            {
                if (!Regex.IsMatch(value, @"(^\-?\d+$)|(^$)"))
                    return;

                int.TryParse(value, out var res);

                if (res > BillsStack.MaxValue) 
                    res = BillsStack.MaxValue;
                
                if (res < 0) 
                    res = 0;

                BillsStack.Count = res;
                amount = (res * BillsStack.Denomination).ToString();
                RaiseUpdateFields();
            }
        }

        public string AmountText
        {
            get => amount == "0" ? "" : amount;
            set
            {
                if (!Regex.IsMatch(value, @"(^\-?\d+$)|(^$)"))
                    return;

                int.TryParse(value, out var res);

                if (res > BillsStack.MaxValue * BillsStack.Denomination) 
                    res = BillsStack.MaxValue * BillsStack.Denomination;
                
                if (res < 0) 
                    res = 0;

                amount = res.ToString();
                BillsStack.Count = (int)Math.Round((float)res / BillsStack.Denomination);
                RaiseUpdateFields();
            }
        }

        public string DenominationText { get => $"Номинал {BillsStack.Denomination}"; }

        private string amount = "0";

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BillsCounter(BillsStack billsStack)
        {
            InitializeComponent();
            AmountTextBox.DataContext = this;
            CountTextBox.DataContext = this;
            DenominationLabel.DataContext = this;
            BillsStack = billsStack;
            amount = (billsStack.Count * billsStack.Denomination).ToString();
        }

        private void AmountTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            AmountText = (BillsStack.Count * BillsStack.Denomination).ToString();
            RaiseUpdateFields();
        }

        private void IncCount(object sender, RoutedEventArgs e)
        {
            CountText = (++BillsStack.Count).ToString();
            RaiseUpdateFields();
        }

        private void DecCount(object sender, RoutedEventArgs e)
        {
            CountText = (--BillsStack.Count).ToString();
            RaiseUpdateFields();
        }

        private void RaiseUpdateFields()
        {
            OnPropertyChanged("CountText");
            OnPropertyChanged("AmountText");
            OnPropertyChanged("TotalString");
        }
    }
}
