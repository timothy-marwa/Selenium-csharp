using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using Xunit;
using System.Collections.Generic;
using NLog;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.DevTools;

namespace AppleWebSiteTestsXUnit
{
    public class AppleWebSiteTestsXunit : IClassFixture<BrowserFixture>, IDisposable
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "your username";
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "your accessKey";
        public static string seleniumUri = "https://hub.lambdatest.com:443/wd/hub";
        String hub = "@hub.lambdatest.com/wd/hub";
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public static int timeout = 30;
        private IWebDriver? driver;
        private WebDriverWait? wait;
        private String browser;
        private String version;
        private String os;

        public AppleWebSiteTestsXunit(BrowserFixture fixture)
        {
            this.browser = fixture.Browser;
            this.version = fixture.Version;
            this.os = fixture.OS;
            Init();
        }

        private void Init()
        {
            logger.Info("Initializing WebDriver for {Browser} {Version} on {OS}", browser, version, os);
            // Enable for local testing
            if (browser.ToLower() == "chrome")
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                driver = new ChromeDriver(chromeOptions);
                var devTools = ((ChromeDriver)driver).GetDevToolsSession();
            }
            else if (browser.ToLower() == "edge")
            {
                EdgeOptions edgeOptions = new EdgeOptions();
                edgeOptions.AddArgument("--start-maximized");
                driver = new EdgeDriver(edgeOptions);
                var devTools = ((EdgeDriver)driver).GetDevToolsSession();
            }
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Fact]
        public void AddItemsToCart()
        {
            // Step 1: Navigate to Apple website
            driver?.Navigate().GoToUrl("https://www.apple.com");
            wait?.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@id='globalnav-menubutton-link-search']")));
            // Step 2: Click on the search icon and search for iPhone 16
            var searchButton = driver?.FindElement(By.XPath("//a[@id='globalnav-menubutton-link-search']"));
            searchButton?.Click();

            var searchInput = wait?.Until(d => d.FindElement(By.XPath("//input[@placeholder='Search apple.com']")));
            searchInput?.SendKeys("iPhone 16");
            searchInput?.SendKeys(Keys.Enter);

            // Wait for search results and click on the first iPhone 16 result
            var iphoneLink = wait?.Until(d => d.FindElement(By.XPath("(//a[@class='rf-serp-productname-link'])[1]")));
            iphoneLink?.Click();

            // Step 3: Add iPhone 16 to the cart
            var buyButton = wait?.Until(d => d.FindElement(By.XPath("//a[@class='detail-ctas-link button']")));
            buyButton?.Click();
        }

        public void Dispose()
        {
            bool passed = true; // xUnit does not have a direct equivalent to NUnit's TestContext
            logger.Info("Test " + (passed ? "Passed" : "Failed"));

            try
            {
                // Logs the result to LambdaTest
                // ((IJavaScriptExecutor)driver).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                driver?.Quit();
                driver?.Dispose();
            }
        }
    }

    public class BrowserFixture
    {
        public String Browser { get; private set; }
        public String Version { get; private set; }
        public String OS { get; private set; }

        public BrowserFixture()
        {
            // Set default values or read from configuration
            Browser = "chrome";
            Version = "134.0";
            OS = "Windows 10";
        }
    }
}