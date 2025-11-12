using Explorer.BuildingBlocks.Tests;

namespace Explorer.Stakeholders.Tests;

public class BaseStakeholdersIntegrationTest : BaseWebIntegrationTest<StakeholdersTestFactory>
{
    public BaseStakeholdersIntegrationTest(StakeholdersTestFactory factory): base(factory) {}
}