using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace CadwiseATMEmulator
{
    internal class DecompositionResult 
    {
        public int Amount = 0;
        public int RemainderAmount = 0;
        public readonly List<BillsStack> BillsStacks = new List<BillsStack>();
        public bool IsSuccess = false;
    }
    
    public class BillsSelectorVM : INotifyPropertyChanged
    {
        private int _amount = 0;
        private readonly BillsSelector _billsSelector;
        
        public string Amount
        {
            get => _amount.ToString();
            set
            {
                if (!Regex.IsMatch(value, @"(^\d+$)|(^$)"))
                    return;
                
                int.TryParse(value, out var amountValue);
                
                var res = DecomposeAmountToBillsStack(amountValue, _billsSelector.BillsCounters.Select(s => s.BillsStack).ToList());
                
                foreach (var billCounter in _billsSelector.BillsCounters)
                {
                    var billsStack = res.BillsStacks
                        .FirstOrDefault(s => s.Denomination == billCounter.BillsStack.Denomination);

                    if (billsStack != null)
                    {
                        if (billCounter.CountText != billsStack.Count.ToString())
                            billCounter.CountText = billsStack.Count.ToString();
                    }
                    else
                    {
                        if (billCounter.CountText != "0")
                            billCounter.CountText = "0";
                    }
                }
                
                _amount = res.IsSuccess ? amountValue : res.Amount;
                OnPropertyChanged("Amount");
                OnPropertyChanged("TotalString");
            }
        }

        public string TotalString
        {
            get
            {
                string result;
                var error = false;
                
                foreach (var billsCounter in _billsSelector.BillsCounters)
                {
                    int.TryParse(billsCounter.AmountText, out var amountInBillsCounter);
                    error = error || amountInBillsCounter != billsCounter.BillsStack.Count * billsCounter.BillsStack.Denomination;
                }

                var totalAmountByBillCounters = _billsSelector.BillsCounters
                    .Sum(bs => bs.BillsStack.Count * bs.BillsStack.Denomination);
                int.TryParse(Amount, out var totalAmount);
                
                error = error || totalAmount != totalAmountByBillCounters;

                result = error 
                    ? $"Невозможно набрать указанную сумму. Сумма итого: {totalAmountByBillCounters}" 
                    : $"Итого {totalAmountByBillCounters}";

                return result;
            }
        }

        public BillsSelectorVM(BillsSelector billsSelector)
        {
            _billsSelector = billsSelector;
        }

        private DecompositionResult DecomposeAmountToBillsStack(int targetAmount, List<BillsStack> billsStack)
        {
            var result = new DecompositionResult();
            
            if (targetAmount <= 0) 
                return result;
                
            foreach (var billStack in billsStack.OrderByDescending(s => s.Denomination))
            {
                if (billStack.Denomination > targetAmount) continue;
                
                var count = targetAmount / billStack.Denomination;
                if (count > billStack.MaxValue) count = billStack.MaxValue;
                    
                result.BillsStacks.Add(new BillsStack(billStack.Denomination, count, count));
                targetAmount -= count * billStack.Denomination;
            }
            
            result.Amount = result.BillsStacks.Sum(s => s.Denomination * s.Count);
            result.RemainderAmount = targetAmount;
            result.IsSuccess = targetAmount != 0;
                
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void BillsCounter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _amount = _billsSelector.BillsCounters.Sum(bs => bs.BillsStack.Count * bs.BillsStack.Denomination);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalString"));
        }
        
        public void TotalAmountLostFocus(object sender, RoutedEventArgs e)
        {
            _amount = _billsSelector.BillsCounters.Sum(bs => bs.BillsStack.Count * bs.BillsStack.Denomination);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalString"));
        }

        
    }
}