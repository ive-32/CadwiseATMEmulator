using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;

namespace CadwiseATMEmulator
{
    public partial class BillsSelector : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<BillsCounter> billsCounters = new List<BillsCounter>();

        public string TotalString { get
            {
                var result = 0;
                foreach (var billsCounter in billsCounters)
                    result += billsCounter.BillsStack.Count * billsCounter.BillsStack.Denomination;
                return $"Итого {result}";
            }
        }

        private IChargeBox ChargeBox { get; set; }

        public BillsSelector(IChargeBox chargeBox)
        {
            ChargeBox = chargeBox ?? new ChargeBox();
            InitializeComponent();
            DataContext = this;

            var (width, height) = Helpers.GridSizeCalculator.GetGridSize(ATM.BanknotesTypes.Length);
            
            // Вычисляем и строим таблицу со счетчиками купюр для каждого номинала 
            for (int i = 0; i < height; i++)
                BillsGrid.RowDefinitions.Add(new RowDefinition { 
                    Height = new System.Windows.GridLength(
                        1.0f / height, 
                        System.Windows.GridUnitType.Star)});

            for (int i = 0; i < width; i++)
                BillsGrid.ColumnDefinitions.Add(new ColumnDefinition { 
                    Width = new System.Windows.GridLength(
                        1.0f / width, 
                        System.Windows.GridUnitType.Star) });


            for (int i = 0; i < ATM.BanknotesTypes.Length; i++)
            {
                var billStack = ChargeBox.BillsStacks
                    .FirstOrDefault(bs => bs.Denomination == ATM.BanknotesTypes[i]);

                if (!(billStack is BillsStack)) 
                    billStack = new BillsStack(ATM.BanknotesTypes[i]);

                var billsCounter = new BillsCounter(billStack);

                billsCounter.SetValue(Grid.RowProperty, i / width);
                billsCounter.SetValue(Grid.ColumnProperty, i % width);
                
                billsCounters.Add(billsCounter);

                BillsGrid.Children.Add(billsCounter);

                billsCounter.PropertyChanged += BillsCounter_PropertyChanged;
            }
        }

        private void BillsCounter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalString"));
        }
    }
}
