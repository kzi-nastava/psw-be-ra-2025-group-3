using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Tests;

namespace Explorer.Payments.Tests
{
    public class BasePaymentsIntegrationTest: BaseWebIntegrationTest<PaymentsTestFactory>
    {
        public BasePaymentsIntegrationTest(PaymentsTestFactory factory) : base(factory) { }
    }
}
