using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace SeleniumTests
{
    public class WebFormTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string Url = "https://www.selenium.dev/selenium/web/web-form.html";

        public WebFormTests()
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Navigate().GoToUrl(Url);
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        // Сценарій 1: Заповнити текстове поле і textarea, натиснути Submit
        // Перевірити що URL змінився і з'явилося повідомлення про успіх
        [Fact]
        public void SubmitForm_WithTextAndTextarea_ShowsConfirmationPage()
        {
            var textInput = _driver.FindElement(By.Name("my-text"));
            textInput.SendKeys("Hello Selenium");

            var textarea = _driver.FindElement(By.Name("my-textarea"));
            textarea.SendKeys("This is a test message");

            var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.Url.Contains("submitted-form"));

            Assert.Contains("submitted-form", _driver.Url);

            var message = _driver.FindElement(By.Id("message"));
            Assert.Equal("Received!", message.Text);
        }

        // Сценарій 2: Вибрати значення з Dropdown, поставити Checkbox
        // Перевірити що вибране значення і стан чекбоксу збереглися
        [Fact]
        public void SelectDropdown_AndCheckCheckbox_ValuesAreCorrect()
        {
            var js = (IJavaScriptExecutor)_driver;

            var dropdown = new SelectElement(_driver.FindElement(By.Name("my-select")));
            dropdown.SelectByText("Two");

            var checkbox = _driver.FindElement(By.Id("my-check-2"));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", checkbox);
            if (!checkbox.Selected)
                js.ExecuteScript("arguments[0].click();", checkbox);

            Assert.Equal("Two", dropdown.SelectedOption.Text);
            Assert.True(checkbox.Selected);
            Assert.Equal("2", dropdown.SelectedOption.GetAttribute("value"));
        }

        // Сценарій 3: Ввести пароль і вибрати значення з Datalist
        // Перевірити що поля містять введені значення
        [Fact]
        public void FillPassword_AndDatalist_FieldsContainCorrectValues()
        {
            var passwordInput = _driver.FindElement(By.Name("my-password"));
            passwordInput.SendKeys("SecurePass123");

            var datalistInput = _driver.FindElement(By.Name("my-datalist"));
            datalistInput.Clear();
            datalistInput.SendKeys("New York");

            Assert.Equal("SecurePass123", passwordInput.GetAttribute("value"));
            Assert.Equal("New York", datalistInput.GetAttribute("value"));
            Assert.Equal("password", passwordInput.GetAttribute("type"));
        }
    }
}
