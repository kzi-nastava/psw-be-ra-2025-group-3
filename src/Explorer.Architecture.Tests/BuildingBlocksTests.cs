using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Explorer.Architecture.Tests;

public class BuildingBlocksTests : BaseArchitecturalTests
{
    [Fact]
    public void Core_should_not_reference_other_projects()
    {
        var examinedTypes = GetExaminedTypes("Explorer.BuildingBlocks.Core");
        var forbiddenTypes = GetForbiddenTypes("Explorer.BuildingBlocks.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_should_not_reference_other_projects_apart_from_core()
    {
        var examinedTypes = GetExaminedTypes("Explorer.BuildingBlocks.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Explorer.BuildingBlocks.Infrastructure", "Explorer.BuildingBlocks.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }
}