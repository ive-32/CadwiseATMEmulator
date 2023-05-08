using System.Collections.Generic;

namespace CadwiseATMEmulator
{
    public interface IATM
    {
        event ATM.AtmTanksChanged OnChange;

        IChargeBox ChargeBox { get; set; }

        List<Tank> Tanks { get; set; } 

        ATMTransactionResult GetMoney(IChargeBox chargeBox = null);
        ATMTransactionResult PutMoney(IChargeBox chargeBox);
        void SetLimitsForGetMoney();
        void SetLimitsForPutMoney();
        void SetLimitsMax();
    }
}