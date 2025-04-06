using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections.Generic;
using NLog;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework.Internal;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.DevTools;


namespace Tests
{
    [TestFixture("chrome", "134.0", "Windows 10")]
    [TestFixture("edge", "134.0", "macOS Ventura")]

    [Parallelizable(ParallelScope.Children)]
    public class AppleWebSiteTestsNunit
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "your username";
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "your accessKey";
        public static string seleniumUri = "https://hub.lambdatest.com:443/wd/hub";
        String hub = "@hub.lambdatest.com/wd/hub";
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public static int timeout = 30;
        private static IWebDriver? driver;
        private WebDriverWait wait;
        private String browser;
        private String version;
        private String os;


        public AppleWebSiteTestsNunit(String browser, String version, String os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [SetUp]
        public void Init()
        {
            logger.Info("Initializing WebDriver for {Browser} {Version} on {OS}", browser, version, os);
/*            Dictionary<string, object> ltOptions = new Dictionary<string, object>();
            ltOptions.Add("username", LT_USERNAME);
            ltOptions.Add("accessKey", LT_ACCESS_KEY);
            ltOptions.Add("geoLocation", "ZA");
            ltOptions.Add("timezone", "Johannesburg");
            ltOptions.Add("resolution", "1920x1080");
            ltOptions.Add("selenium_version", "4.29.0");
            ltOptions.Add("headless", false);
            ltOptions.Add("seCdp", true);
            ltOptions.Add("project", "LambdaTest Assessment");
            ltOptions.Add("build", "LambdaTest Assessment");
            ltOptions.Add("console", "error");
            ltOptions.Add("video", true);
            ltOptions.Add("network", true);
            ltOptions.Add("screenshots", true);
            ltOptions.Add("w3c", true);
            ltOptions.Add("plugin", "c#-c#");
            ltOptions.Add("acceptInsecureCerts", true);
            ltOptions.Add("acceptSslCerts", true);
            ltOptions.Add("platformName", os);
            ltOptions.Add("browserName", browser);
            ltOptions.Add("name", $"{TestContext.CurrentContext.Test.ClassName}:{TestContext.CurrentContext.Test.MethodName}");
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.BrowserVersion = version;
            chromeOptions.AddAdditionalOption("LT:Options", ltOptions);
            driver = new RemoteWebDriver(new Uri("https://" + LT_USERNAME + ":" + LT_ACCESS_KEY + hub), chromeOptions.ToCapabilities(), TimeSpan.FromSeconds(600));*/
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
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(timeout));
            driver.Manage().Timeouts().ImplicitWait.Add(System.TimeSpan.FromSeconds(timeout));
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
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

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
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
        [OneTimeTearDown]
        public void AfterTest()
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
