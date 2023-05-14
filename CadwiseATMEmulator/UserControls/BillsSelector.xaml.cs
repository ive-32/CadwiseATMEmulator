using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows;

namespace CadwiseATMEmulator
{
    public partial class BillsSelector : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly List<BillsCounter> BillsCounters = new List<BillsCounter>();

        private BillsSelectorVM BillsSelectorVM { get; set; }

        private IChargeBox ChargeBox { get; set; }

        public BillsSelector(IChargeBox chargeBox)
        {
            ChargeBox = chargeBox ?? new ChargeBox();
            InitializeComponent();

            BillsSelectorVM = new BillsSelectorVM(this);
            TotalAmount.DataContext = BillsSelectorVM;
            Total.DataContext = BillsSelectorVM;

            var (width, height) = Helpers.GridSizeCalculator.GetGridSize(ATM.BanknotesTypes.Length);
            
            // Вычисляем и строим таблицу со счетчиками купюр для каждого номинала 
            for (var i = 0; i < height; i++)
                BillsGrid.RowDefinitions.Add(new RowDefinition { 
                    Height = new System.Windows.GridLength(
                        1.0f / height, 
                        System.Windows.GridUnitType.Star)});

            for (var i = 0; i < width; i++)
                BillsGrid.ColumnDefinitions.Add(new ColumnDefinition { 
                    Width = new System.Windows.GridLength(
                        1.0f / width, 
                        System.Windows.GridUnitType.Star) });
            

            for (var i = 0; i < ATM.BanknotesTypes.Length; i++)
            {
                var billStack = ChargeBox.BillsStacks.FirstOrDefault(bs => bs.Denomination == ATM.BanknotesTypes[i]) 
                                ?? new BillsStack(ATM.BanknotesTypes[i]);

                var billsCounter = new BillsCounter(billStack);

                billsCounter.SetValue(Grid.RowProperty, i / width);
                billsCounter.SetValue(Grid.ColumnProperty, i % width);
                
                BillsCounters.Add(billsCounter);

                BillsGrid.Children.Add(billsCounter);

                billsCounter.PropertyChanged += BillsSelectorVM.BillsCounter_PropertyChanged;
            }
        }
        
        private void TotalAmountLostFocus(object sender, RoutedEventArgs e)
            => BillsSelectorVM.TotalAmountLostFocus(sender, e);
    }
}
