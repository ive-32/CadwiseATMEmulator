using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CadwiseATMEmulator
{
    /// <summary>
    /// Класс банкомата
    /// </summary>
    public class ATM : IATM
    {
        public static readonly int[] BanknotesTypes = { 10, 50, 100, 200, 500, 1000, 2000, 5000 };
        private static readonly SemaphoreSlim TankOperationSemaphore = new SemaphoreSlim(1, 1);
        public delegate void AtmTanksChanged();
        public event AtmTanksChanged OnChange;

        public IChargeBox ChargeBox { get; set; } = new ChargeBox();

        private List<Tank> Tanks { get; set; } = new List<Tank>();

        public ATM(int count = 5, int volume = 10)
        {
            // для тестирования заполним по 5 банкнот в каждый ящик 
            // емкость ящика сделаем 10 - чтобы проще переполнить
            foreach (var banknoteType in BanknotesTypes)
                Tanks.Add(new Tank(denomination: banknoteType, volume: volume, count: count));
        }

        public async Task<ATMTransactionResult> PutMoney(IChargeBox chargeBox = null)
        {
            // вносим банкноты в ящики - игнорим неверные номиналы,
            // TODO здесь же добавляем проверку на подлинность купюр
            // все, что не влезло или не распознано - возвращаем
            var totalAmount = 0;

            var isSemaphoreAvailable = await TankOperationSemaphore.WaitAsync(1000);
            if (!isSemaphoreAvailable)
                return new ATMTransactionResult()
                {
                    Result = TransactionResultType.Error,
                    ChargeBox = ChargeBox,
                    ResultMessage = $"Оборудование недоступно"
                };
            try
            {
                // метод будет выполняться асинхронно т.к. будет ждать ответа оборудования
                // здесь эмулируем задержку
                await Task.Delay(600);

                // chargeBox ??= ChargeBox will appear only in c#8
                if (chargeBox == null)
                    chargeBox = ChargeBox;

                foreach (var tank in Tanks)
                {
                    var billsStack = chargeBox.BillsStacks
                        .FirstOrDefault(m => m.Denomination == tank.Denomination);

                    if (billsStack == null) continue;
                    
                    var allowedCount = Math.Min(tank.Volume - tank.Count, billsStack.Count);

                    tank.Count += allowedCount;
                    billsStack.Count -= allowedCount;
                    totalAmount += allowedCount * tank.Denomination;
                    
                    // для эмуляции возврата, через селектор купюр
                    // ограничиваем значеие ячейки селектора текущим значением, чтобы не было ощущения
                    // что можно вытащить больше чем там лежит   
                    billsStack.MaxValue = billsStack.Count;  
                }
                
            }
            catch (Exception e)
            {
                return new ATMTransactionResult()
                {
                    Result = TransactionResultType.Error,
                    ChargeBox = ChargeBox,
                    ResultMessage = $"Ошибка оборудования{Environment.NewLine}{e.Message}"
                };
            }
            finally
            {
                TankOperationSemaphore.Release();
            }

            OnChange?.Invoke();

            if (chargeBox.BillsStacks.Any(m => m.Count > 0))
            {
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
            var totalAmount = 0;
            var isSemaphoreAvailable = await TankOperationSemaphore.WaitAsync(1000);
            if (!isSemaphoreAvailable)
                return new ATMTransactionResult()
                {
                    Result = TransactionResultType.Error,
                    ChargeBox = ChargeBox,
                    ResultMessage = $"Оборудование недоступно"
                };
            try
            {
                // метод будет выполняться асинхронно т.к. будет ждать ответа оборудования
                // здесь эмулируем задержку
                if (DateTime.Now.Millisecond < 200) // для реалистичности сгененируем ошибку с вероятностью 20% 
                    throw new ExternalException($"Hardware error{Environment.NewLine}Эта ошибка сгенерирована искусственно, для отладки");
                await Task.Delay(600);
                // chargeBox ??= ChargeBox will appear only in c#8
                if (chargeBox == null)
                    chargeBox = ChargeBox;

                foreach (var billStack in chargeBox.BillsStacks)
                {
                    var tank = Tanks.FirstOrDefault(t => t.Denomination == billStack.Denomination);

                    if (tank == null) continue;

                    if (billStack.Count >= tank.Count)
                    {
                        billStack.Count = tank.Count;
                        tank.Count = 0;
                    }
                    else
                        tank.Count -= billStack.Count;

                    totalAmount += billStack.Count * billStack.Denomination;

                    // для эмуляции возврата, через селектор купюр
                    // ограничиваем значеие ячейки селектора текущим значением, чтобы не было ощущения
                    // что можно вытащить больше чем там лежит   
                    billStack.MaxValue = billStack.Count;
                }
            }
            catch (Exception e)
            {
                return new ATMTransactionResult()
                {
                    Result = TransactionResultType.Error,
                    ChargeBox = ChargeBox,
                    ResultMessage = $"Ошибка оборудования{Environment.NewLine}{e.Message}"
                };
            }
            finally
            {
                TankOperationSemaphore.Release();
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

                if (billsStack == null) continue;

                billsStack.MaxValue = tank.Count;
            }
        }

        public void SetLimitsForPutMoney()
        {
            foreach (var billsStack in ChargeBox.BillsStacks)
            {
                var tank = Tanks.FirstOrDefault(t => t.Denomination == billsStack.Denomination);
                
                if (tank == null) continue;
                
                // здесь ограничиваем внесение купюр реальной емкостью ящиков 
                // закомментировал, чтобы наглядее показывать как возвращаются непринятые купюры
                //billsStack.MaxValue = tank.Volume - tank.Count;

                //в строке ниже ставим 20 чтобы можно было проверить, что напихали больше купюр
                //чем может принять банкомат
                billsStack.MaxValue = 20;
            }
        }
        
        public bool ATMCanGiveMoney => Tanks.Any(t => t.Count > 0);
        
        public bool ATMCanTakeMoney => Tanks.Any(t => t.Volume - t.Count > 0);
        
        public IEnumerable<string> ATMTanksState
            => Tanks.Select(tank => $"{tank.Denomination} : {tank.Count} of {tank.Volume}");

        public IEnumerable<Tank> ATMTanks
            => Tanks;
    }
}
