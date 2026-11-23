using OpenQA.Selenium;

namespace SauceDemoTests.Tests;

/// <summary>
/// Y2 (nang cao) - Luong thu hai: duyet danh sach san pham, them san pham vao gio va mo gio hang.
/// </summary>
[TestFixture]
public class CartTests : TestBase
{
    private const string TenBalo = "Sauce Labs Backpack";

    private static readonly By TheSanPham = By.CssSelector("[data-test='inventory-item']");
    private static readonly By TenSanPham = By.CssSelector("[data-test='inventory-item-name']");
    private static readonly By GiaSanPham = By.CssSelector("[data-test='inventory-item-price']");
    private static readonly By BadgeGioHang = By.CssSelector("[data-test='shopping-cart-badge']");

    [SetUp]
    public void DangNhapTruocMoiTest()
    {
        // NUnit chay [SetUp] cua TestBase truoc (mo trinh duyet), roi moi toi ham nay
        DangNhap("standard_user", MatKhauHopLe);
        ChoUrlChua("inventory.html");
        ChoPhanTuHienThi(By.CssSelector("[data-test='inventory-list']"));
    }

    [Test]
    public void DanhSachSanPham_Du6SanPham_TenVaGiaHopLe()
    {
        var danhSach = Driver.FindElements(TheSanPham)
            .Select(item => new
            {
                // Tim phan tu con BEN TRONG tung the san pham
                Ten = item.FindElement(TenSanPham).Text,
                Gia = item.FindElement(GiaSanPham).Text,
            })
            .ToList();

        danhSach.ForEach(sp => TestContext.Out.WriteLine($"{sp.Ten} - {sp.Gia}"));

        Assert.Multiple(() =>
        {
            Assert.That(danhSach, Has.Count.EqualTo(6));
            Assert.That(danhSach.Select(sp => sp.Ten), Has.All.Not.Empty);
            Assert.That(danhSach.Select(sp => sp.Gia), Has.All.Match(@"^\$\d+\.\d{2}$"));
            Assert.That(danhSach.Select(sp => sp.Ten), Does.Contain(TenBalo));
        });
    }

    [Test]
    public void ThemSanPhamVaoGio_MoGioHang_HienDungSanPham()
    {
        // Ghi lai gia tren trang danh sach de doi chieu voi gio hang
        var theBalo = Driver.FindElements(TheSanPham)
            .Single(item => item.FindElement(TenSanPham).Text == TenBalo);
        var giaTrenDanhSach = theBalo.FindElement(GiaSanPham).Text;

        Driver.FindElement(By.CssSelector("[data-test='add-to-cart-sauce-labs-backpack']")).Click();
        Assert.That(ChoPhanTuHienThi(BadgeGioHang).Text, Is.EqualTo("1"));

        Driver.FindElement(By.CssSelector("[data-test='shopping-cart-link']")).Click();
        ChoUrlChua("cart.html");
        // URL doi truoc khi trang gio hang ve xong -> cho danh sach gio hang hien thi roi moi doc
        ChoPhanTuHienThi(By.CssSelector("[data-test='cart-list']"));

        var cacDongTrongGio = Driver.FindElements(TheSanPham);
        Assert.Multiple(() =>
        {
            Assert.That(Driver.FindElement(By.CssSelector("[data-test='title']")).Text, Is.EqualTo("Your Cart"));
            Assert.That(cacDongTrongGio, Has.Count.EqualTo(1));
            Assert.That(cacDongTrongGio[0].FindElement(TenSanPham).Text, Is.EqualTo(TenBalo));
            Assert.That(cacDongTrongGio[0].FindElement(GiaSanPham).Text, Is.EqualTo(giaTrenDanhSach));
            Assert.That(cacDongTrongGio[0].FindElement(By.CssSelector("[data-test='item-quantity']")).Text, Is.EqualTo("1"));
        });
    }

    [Test]
    public void ThemHaiSanPham_BadgeHienThi2_NutDoiThanhRemove()
    {
        Driver.FindElement(By.CssSelector("[data-test='add-to-cart-sauce-labs-backpack']")).Click();
        Driver.FindElement(By.CssSelector("[data-test='add-to-cart-sauce-labs-bike-light']")).Click();

        Assert.Multiple(() =>
        {
            Assert.That(ChoPhanTuHienThi(BadgeGioHang).Text, Is.EqualTo("2"));
            Assert.That(ChoPhanTuHienThi(By.CssSelector("[data-test='remove-sauce-labs-backpack']")).Text, Is.EqualTo("Remove"));
            Assert.That(ChoPhanTuHienThi(By.CssSelector("[data-test='remove-sauce-labs-bike-light']")).Text, Is.EqualTo("Remove"));
        });
    }
}
