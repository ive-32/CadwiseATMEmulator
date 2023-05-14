using System.Linq;
using Xunit;
using CadwiseATMEmulator;

namespace CadwiseATMTests
{
    public class Tests
    {

        [Fact]
        public void CheckMoneyAvailable()
        {
            ATM atmEmulator = new ATM(0, 0);
            Assert.False(atmEmulator.ATMCanGiveMoney);
            
            atmEmulator = new ATM(5, 5);
            Assert.False(atmEmulator.ATMCanTakeMoney);
        }

        [Theory]
        [InlineData(50, 5, TransactionResultType.Success)]
        [InlineData(50, 10, TransactionResultType.MoneyReturned)]
        public async void PutMoneyTestTheory(int denomination, int count, TransactionResultType resultType)
        {
            ATM atmEmulator = new ATM();
            atmEmulator.ChargeBox.BillsStacks.First(s => s.Denomination == denomination).Count = count;
            var res = await atmEmulator.PutMoney();
            Assert.True(res.Result  == resultType);
            if (resultType == TransactionResultType.Success)
                Assert.Equal(res.ResultMessage, $"Принято {denomination * count}");
        }

        [Theory]
        [InlineData(50, 5, TransactionResultType.Success)]
        [InlineData(50, 10, TransactionResultType.MoneyReturned)]
        public async void GetMoneyTestTheory(int denomination, int count, TransactionResultType resultType)
        {
            ATM atmEmulator = new ATM();
            atmEmulator.ChargeBox.BillsStacks.First(s => s.Denomination == denomination).Count = count;
            var tankCount = atmEmulator.ATMTanks.First(s => s.Denomination == denomination).Count;
            ATMTransactionResult res;
            var i = 10;
            do
            {
                res = await atmEmulator.GetMoney();
            } while (res.Result != TransactionResultType.Success && --i>0);

            Assert.True(res.Result == TransactionResultType.Success 
                        || (res.Result == TransactionResultType.Error 
                            && res.ResultMessage.Contains("Эта ошибка сгенерирована искусственно")));
            if (res.Result == TransactionResultType.Success)
                Assert.True(res.ResultMessage.Contains($"Выдано {denomination * (tankCount > count ? count : tankCount)}"));
        }

    }
}