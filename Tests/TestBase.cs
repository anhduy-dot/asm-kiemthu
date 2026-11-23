using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SauceDemoTests.Tests;

/// <summary>
/// Lop cha dung chung cho cac lop test: mo Chrome truoc moi test, dong sau moi test
/// va cung cap cac buoc dung lai nhieu lan (dang nhap, cho URL).
/// </summary>
public abstract class TestBase
{
    protected const string BaseUrl = "https://www.saucedemo.com/";
    protected const string MatKhauHopLe = "secret_sauce";

    protected IWebDriver Driver { get; private set; } = null!;

    [SetUp]
    public void KhoiTaoTrinhDuyet()
    {
        var options = new ChromeOptions();
        options.AddArgument("--window-size=1366,900");

        // secret_sauce nam trong danh sach mat khau bi lo -> Chrome bat popup "doi mat khau"
        // che mat giao dien. Tat trinh quan ly mat khau de test khong bi chan.
        options.AddUserProfilePreference("credentials_enable_service", false);
        options.AddUserProfilePreference("profile.password_manager_enabled", false);
        options.AddUserProfilePreference("profile.password_manager_leak_detection", false);

        // Chay an trinh duyet khi dat bien moi truong HEADLESS=true (dung cho CI sau nay)
        if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
        {
            options.AddArgument("--headless=new");
        }

        Driver = new ChromeDriver(options);
        Driver.Navigate().GoToUrl(BaseUrl);
    }

    [TearDown]
    public void DongTrinhDuyet()
    {
        ChupAnhTrangCuoiNeuCan();
        Driver?.Quit();
        Driver?.Dispose();
    }

    /// <summary>Nhap tai khoan, mat khau roi bam nut Login.</summary>
    protected void DangNhap(string username, string password)
    {
        Driver.FindElement(By.Id("user-name")).SendKeys(username);
        Driver.FindElement(By.Id("password")).SendKeys(password);
        Driver.FindElement(By.Id("login-button")).Click();
    }

    /// <summary>Cho (tuong minh) toi khi URL chua doan chi dinh - khong dung Thread.Sleep.</summary>
    protected void ChoUrlChua(string doanUrl, int soGiay = 10)
    {
        new WebDriverWait(Driver, TimeSpan.FromSeconds(soGiay))
            .Until(d => d.Url.Contains(doanUrl));
    }

    /// <summary>
    /// Cho (tuong minh) toi khi phan tu co mat VA dang hien thi, roi tra ve phan tu do.
    /// Can thiet vi saucedemo la trang SPA: URL doi truoc khi DOM cua trang moi ve xong.
    /// </summary>
    protected IWebElement ChoPhanTuHienThi(By locator, int soGiay = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(soGiay));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        return wait.Until(d =>
        {
            var phanTu = d.FindElement(locator);
            return phanTu.Displayed ? phanTu : null;
        })!;
    }

    /// <summary>
    /// Khi dat bien moi truong SCREENSHOT_DIR, luu anh trang web o cuoi moi test
    /// (lam minh chung trang thai trinh duyet). Khong dat bien -> bo qua.
    /// </summary>
    private void ChupAnhTrangCuoiNeuCan()
    {
        var thuMuc = Environment.GetEnvironmentVariable("SCREENSHOT_DIR");
        if (string.IsNullOrWhiteSpace(thuMuc) || Driver is not ITakesScreenshot mayChup)
        {
            return;
        }

        Directory.CreateDirectory(thuMuc);
        var tenTep = Regex.Replace(TestContext.CurrentContext.Test.Name, @"[^A-Za-z0-9_]+", "_").Trim('_');
        var duongDan = Path.Combine(thuMuc, $"{TestContext.CurrentContext.Test.ClassName?.Split('.').Last()}__{tenTep}.png");
        mayChup.GetScreenshot().SaveAsFile(duongDan);
        TestContext.Out.WriteLine($"Da luu anh: {duongDan}");
    }
}
