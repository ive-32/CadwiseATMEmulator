using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Класс банкомата
    /// </summary>
    public class ATM : IATM
    {
        public static int[] BanknotesTypes = { 10, 50, 100, 200, 500, 1000, 2000, 5000 };

        public delegate void AtmTanksChanged();
        public event AtmTanksChanged OnChange;

        public IChargeBox ChargeBox { get; set; } = new ChargeBox();


        public List<Tank> Tanks { get; set; } = new List<Tank>();

        public ATM()
        {
            // для тестирования заполним по 5 банкнот в каждый ящик 
            // емкость ящика сделаем 10 - чтобы проще переполнить
            foreach (var banknoteType in BanknotesTypes)
                Tanks.Add(new Tank() { Denomination = banknoteType, Volume = 10, Count = 5 });
        }

        public async Task<ATMTransactionResult> PutMoney(IChargeBox chargeBox = null)
        {
            // вносим банкноты в ящики - игнорим неверные номиналы,
            // TODO здесь же добавляем проверку на подлинность купюр
            // все, что не влезло или не распознано - возвращаем

            // метод будет выполняться асинхронно т.к. будет ждать ответа оборудования
            // здес эмулируем задержку
            await Task.Delay(600); 

            // chargeBox ??= ChargeBox will appear only in c#8
            if (chargeBox == null)
                chargeBox = ChargeBox;

            var totalAmount = 0;
            foreach (var tank in Tanks)
            {
                var billsstack = chargeBox.BillsStacks
                    .FirstOrDefault(m => m.Denomination == tank.Denomination);

                if (billsstack is BillsStack)
                {
                    var allowedCount = Math.Min(tank.Volume - tank.Count, billsstack.Count);

                    tank.Count += allowedCount;
                    billsstack.Count -= allowedCount;
                    totalAmount += allowedCount * tank.Denomination;
                }
            }

            OnChange?.Invoke();

            if (chargeBox.BillsStacks.Any(m => m.Count > 0))
            {
                SetLimitsMax(); // для сохранения интерфейса - в реале это возврат и лимиты устанавливать не нужно
                return new ATMTransactionResult()
                {
                    Result = TransactionResultType.MoneyReturned,
                    ChargeBox = chargeBox,
                    ResultMessage = $"Принято {totalAmount}, заберите непринятые купюры."
                };
            }

            return new ATMTransactionResult()
            {
                Result = TransactionResultType.Success,
                ChargeBox = chargeBox,
                ResultMessage = $"Принято {totalAmount}"
            };
        }

        public async Task<ATMTransactionResult> GetMoney(IChargeBox chargeBox = null)
        {
            // метод будет выполняться асинхронно т.к. будет ждать ответа оборудования
            // здес эмулируем задержку
            await Task.Delay(600);

            // chargeBox ??= ChargeBox will appear only in c#8
            if (chargeBox == null) 
                chargeBox = ChargeBox;

            var totalAmount = 0;

            foreach (var billstack in chargeBox.BillsStacks)
            {
                var tank = Tanks.FirstOrDefault(t => t.Denomination == billstack.Denomination);
                if (!(tank is Tank)) continue;

                if (billstack.Count >= tank.Count)
                {
                    billstack.Count = tank.Count;
                    tank.Count = 0;
                }
                else
                    tank.Count -= billstack.Count;

                billstack.MaxValue = billstack.Count;
                totalAmount += billstack.Count * billstack.Denomination;
            }

            OnChange?.Invoke();

            return new ATMTransactionResult()
            {
                Result = TransactionResultType.Success,
                ChargeBox = ChargeBox,
                ResultMessage = $"Выдано {totalAmount}"
            };
        }

        public void SetLimitsForGetMoney()
        {
            foreach (var tank in Tanks)
            {
                var billsStack = ChargeBox.BillsStacks
                    .FirstOrDefault(bs => bs.Denomination == tank.Denomination);

                if (!(billsStack is BillsStack)) continue;

                billsStack.MaxValue = tank.Count;
            }
        }

        public void SetLimitsForPutMoney()
        {
            foreach (var billsStack in ChargeBox.BillsStacks)
            {
                var tank = Tanks.FirstOrDefault(t => t.Denomination == billsStack.Denomination);
                billsStack.MaxValue = tank.Volume - tank.Count;

                //в строке ниже ставим 20 чтобы можно было проверить что напихали больше купюр
                //чем может принять банкомат
                billsStack.MaxValue = 20;
            }

        }

        public void SetLimitsMax()
        {
            foreach (var billsStack in ChargeBox.BillsStacks)
                billsStack.MaxValue = 20;
        }
    }
}
