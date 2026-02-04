using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum WalletTransactionType
    {
        AdminTopUp = 1,
        CheckoutPurchase = 2,
        WelcomeBonusAc = 3,
        RankRewardAc = 4,
        TourRewardAc = 5,
        KeyPointDetailUnlock = 6
    }
}
