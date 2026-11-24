namespace SauceDemoTests.Tests;

/// <summary>
/// Y1 - Test dau tien: mo trang saucedemo va kiem tra tieu de trang.
/// (Tu Y2: khoi tao/dong trinh duyet chuyen sang TestBase.)
/// </summary>
[TestFixture]
public class HomePageTests : TestBase
{
    [Test]
    public void MoTrangChu_TieuDeLaSwagLabs()
    {
        TestContext.Out.WriteLine($"Tieu de trang: {Driver.Title}");
        Assert.That(Driver.Title, Is.EqualTo("Swag Labs"));
    }
}
