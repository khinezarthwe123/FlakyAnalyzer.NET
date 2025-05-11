using FlakyTestNUnit.src.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.DevTools.V132.Debugger;

namespace FlakyTestNUnit.src.Tests
{
    [TestFixture]
    public class FirefoxUIBehaviorTest : CsvLogger
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            stopwatch = Stopwatch.StartNew();
            driver = new FirefoxDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(CsvLogger.WaitingTime));
            browserVersion = ((FirefoxDriver)driver).Capabilities.GetCapability("browserVersion")?.ToString() ?? string.Empty;
            browserName = ((FirefoxDriver)driver).Capabilities.GetCapability("browserName")?.ToString() ?? string.Empty;
        }

        // Test Suite 1: Flaky test with delayed error
        [Test]
        public void LoadCustomerCards()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/Dashboard");
                var cards = wait.Until(d =>
                {
                    var elements = d.FindElements(By.CssSelector("#customerData .customer-card"));
                    return elements.Count >= 500 ? elements : null;
                });

                if (new Random().Next(0, 100) < 30) // ~20% chance of error
                {
                    throw new TimeoutException("Error");
                }

                Assert.That(cards.Count, Is.EqualTo(500));
                Assert.IsTrue(cards.First().Text.Contains("Email"));
            });
        }

        // Test Suite 2: Flaky test simulating Ajax error
        [Test]
        public void FormSubmission()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/UserRegistration");
                driver.FindElement(By.Id("name")).SendKeys("Emily");
                driver.FindElement(By.Id("email")).SendKeys("emily@example.com");
                driver.FindElement(By.Id("submitBtn")).Click();
                if (new Random().Next(0, 100) < 30) //  ~20% chance of error
                {
                    throw new TimeoutException("Error");
                }
                var successMsg = wait.Until(d => d.FindElement(By.Id("successMessage")));
                Assert.IsTrue(successMsg.Text.Contains("successfully"), $"[Run Submission failed.]");
            });
        }

        // Test Suite 3: Reliable check of static settings content
        [Test]
        public void SettingsDisplay()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/Setting");
                var boxes = wait.Until(d => d.FindElements(By.CssSelector(".setting-box")));
                Assert.That(boxes.Count, Is.EqualTo(20));
                Assert.IsTrue(boxes.First().Text.Contains("Default Tier"));
            });
        }

        // Test Suite 4: Reliable navigation to help page
        [Test]
        public void HelpPageNavigation()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com");
                driver.FindElement(By.LinkText("Help")).Click();
                Assert.IsTrue(driver.Url.Contains("Help"));
            });
        }

        // Test Suite 5: Reliable page title check
        [Test]
        public void StaticPageTitle()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/Setting");
                Assert.That(driver.Title, Is.EqualTo("Setting - FlakyTesting Application"));
            });
        }

        // Test Suite 6: Reliable form validation check
        [Test]
        public void FormEmptyFieldError()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/UserRegistration");
                driver.FindElement(By.Id("submitBtn")).Click();
                var error = wait.Until(d => d.FindElement(By.ClassName("validation-error")));
                Assert.IsTrue(error.Text.Contains("required"));
            });
        }

        // Test Suite 7: Reliable visibility of footer
        [Test]
        public void CheckFooterVisibility()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com");
                var footer = driver.FindElement(By.TagName("footer"));
                Assert.IsTrue(footer.Displayed);
            });
        }

        // Test Suite 8: Reliable feature card count
        [Test]
        public void FeatureCardCount()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/Feature");
                var cards = wait.Until(d => d.FindElements(By.ClassName("feature-card")));
                Assert.That(cards.Count, Is.EqualTo(10));                
            });
        }

        // Test Suite 9: Link navigation between pages
        [Test]
        public void NavigationLinks()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/Setting");
                driver.FindElement(By.LinkText("Feature")).Click();
                Assert.IsTrue(driver.Url.Contains("Feature"));                
            });
        }


        // Test Suite 10: Input retention after navigation
        [Test]
        public void InputPersistence()
        {
            ExecuteWithRetry(() =>
            {
                driver.Navigate().GoToUrl("http://flakytestapp.com/Home/UserRegistration");
                driver.FindElement(By.Id("name")).SendKeys("TestUser");
                driver.Navigate().Back();
                driver.Navigate().Forward();
                string? inputValue = driver.FindElement(By.Id("name")).GetAttribute("value");
                Assert.That(inputValue ?? string.Empty, Is.EqualTo("TestUser"));                
            });
        }


        [TearDown]
        public void Cleanup()
        {
            driver.Dispose();
        }
    }
}