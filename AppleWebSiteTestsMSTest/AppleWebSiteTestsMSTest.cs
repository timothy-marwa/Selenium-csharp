using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Keys;
using SeleniumExtras.WaitHelpers;
using System;

namespace AppleWebSiteTestsMSTest
{
    [TestClass]
    [Parallelizable(ParallelScope.Children)]
    public class AppleWebSiteTestsMSTest
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "your username";
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "your accessKey";
        public static string seleniumUri = "https://hub.lambdatest.com:443/wd/hub";
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static int timeout = 30;
        private static IWebDriver driver;
        private WebDriverWait wait;
        private string browser;
        private string version;
        private string os;

        // Constructor to pass parameters to test case
        public AppleWebSiteTestsMSTest(string browser, string version, string os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [TestInitialize]
        public void Init()
        {
            logger.Info("Initializing WebDriver for {Browser} {Version} on {OS}", browser, version, os);

            // Enable for local testing
            if (browser.ToLower() == "chrome")
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                driver = new ChromeDriver(chromeOptions);
            }
            else if (browser.ToLower() == "edge")
            {
                EdgeOptions edgeOptions = new EdgeOptions();
                edgeOptions.AddArgument("--start-maximized");
                driver = new EdgeDriver(edgeOptions);
            }

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [TestMethod]
        public void AddItemsToCart()
        {
            // Step 1: Navigate to Apple website
            driver.Navigate().GoToUrl("https://www.apple.com");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@id='globalnav-menubutton-link-search']")));

            // Step 2: Click on the search icon and search for iPhone 16
            var searchButton = driver.FindElement(By.XPath("//a[@id='globalnav-menubutton-link-search']"));
            searchButton.Click();

            var searchInput = wait.Until(d => d.FindElement(By.XPath("//input[@placeholder='Search apple.com']")));
            searchInput.SendKeys("iPhone 16");
            searchInput.SendKeys(Keys.Enter);

            // Wait for search results and click on the first iPhone 16 result
            var iphoneLink = wait.Until(d => d.FindElement(By.XPath("(//a[@class='rf-serp-productname-link'])[1]")));
            iphoneLink.Click();

            // Step 3: Add iPhone 16 to the cart
            var buyButton = wait.Until(d => d.FindElement(By.XPath("//a[@class='detail-ctas-link button']")));
            buyButton.Click();
        }

        [TestCleanup]
        public void Cleanup()
        {
            bool passed = true; // MSTest does not have TestContext like NUnit, so you have to manually track status
            logger.Info("Test " + (passed ? "Passed" : "Failed"));

            try
            {
                // Logs the result to LambdaTest (if applicable)
                // ((IJavaScriptExecutor)driver).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                driver?.Quit();
                driver?.Dispose();
            }
        }

        [ClassCleanup]
        public static void AfterTest()
        {
            Dispose();
        }

        private static void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
