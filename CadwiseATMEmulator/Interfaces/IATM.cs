using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadwiseATMEmulator
{
    public interface IATM
    {
        event ATM.AtmTanksChanged OnChange;

        IChargeBox ChargeBox { get; set; }

        Task<ATMTransactionResult> GetMoney(IChargeBox chargeBox = null);

        Task<ATMTransactionResult> PutMoney(IChargeBox chargeBox = null);

        void SetLimitsForGetMoney();

        void SetLimitsForPutMoney();

        bool ATMCanGiveMoney { get; }
        
        bool ATMCanTakeMoney { get; }
        
        IEnumerable<string> ATMTanksState { get; }
    }
}