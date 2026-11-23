using OpenQA.Selenium;

namespace SauceDemoTests.Tests;

/// <summary>
/// Y2 - Tu dong hoa luong dang nhap: luong thuan (dang nhap thanh cong)
/// va luong nghich (tai khoan bi khoa, sai mat khau, bo trong username).
/// Locator chi dung id / data-test, khong dung XPath.
/// </summary>
[TestFixture]
public class LoginTests : TestBase
{
    [Test]
    public void TrangDangNhap_CacLocatorDeuDuyNhat()
    {
        // Moi locator phai khop dung 1 phan tu (tuong duong "1 of 1" khi Ctrl+F trong DevTools)
        string[] cssLocators =
        [
            "#user-name",
            "#password",
            "#login-button",
            "[data-test='username']",
            "[data-test='password']",
            "[data-test='login-button']",
        ];

        Assert.Multiple(() =>
        {
            foreach (var css in cssLocators)
            {
                Assert.That(Driver.FindElements(By.CssSelector(css)), Has.Count.EqualTo(1), $"Locator {css}");
            }
        });
    }

    [Test]
    public void DangNhap_TaiKhoanHopLe_VaoTrangSanPham()
    {
        DangNhap("standard_user", MatKhauHopLe);

        ChoUrlChua("inventory.html");
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.EndWith("/inventory.html"));
            Assert.That(Driver.FindElement(By.CssSelector("[data-test='title']")).Text, Is.EqualTo("Products"));
            Assert.That(Driver.FindElements(By.CssSelector("[data-test='inventory-item']")), Has.Count.EqualTo(6));
        });
    }

    [TestCase("locked_out_user", MatKhauHopLe, "Epic sadface: Sorry, this user has been locked out.")]
    [TestCase("standard_user", "sai_mat_khau", "Epic sadface: Username and password do not match any user in this service")]
    [TestCase("", MatKhauHopLe, "Epic sadface: Username is required")]
    public void DangNhap_ThongTinKhongHopLe_HienThongBaoLoi(string username, string password, string thongBaoMongDoi)
    {
        DangNhap(username, password);

        var thongBaoLoi = Driver.FindElement(By.CssSelector("[data-test='error']"));
        Assert.Multiple(() =>
        {
            Assert.That(thongBaoLoi.Text, Is.EqualTo(thongBaoMongDoi));
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
        });
    }
}
