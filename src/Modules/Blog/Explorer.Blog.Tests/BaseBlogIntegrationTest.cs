using Explorer.BuildingBlocks.Tests;

namespace Explorer.Blog.Tests;

public class BaseBlogIntegrationTest : BaseWebIntegrationTest<BlogTestFactory>
{
    public BaseBlogIntegrationTest(BlogTestFactory factory) : base(factory) { }
}