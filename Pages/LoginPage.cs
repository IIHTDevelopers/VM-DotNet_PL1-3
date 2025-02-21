using System;
using System.IO;
using DotNetSelenium.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class LoginPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By usernameInput = By.CssSelector("#username_id");
    private readonly By passwordInput = By.CssSelector("#password");
    private readonly By loginButton = By.CssSelector("#login");
    private readonly By loginErrorMessage = By.XPath("//div[contains(text(),'Invalid credentials !')]");
    private readonly By admin = By.XPath("//li[@class='dropdown dropdown-user']");
    private readonly By logOut = By.XPath("//a[text()=' Log Out ']");

    // Store login data
    private string validUsername;
    private string validPassword;

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    /*
   * @description This method performs the login operation using the provided valid credentials. It highlights the input
   *              fields for better visibility during interaction and fills the username and password fields. After submitting
   *              the login form by clicking the login button, it validates the success of the login process. The login is
   *              considered successful if there are no errors.
   */
    public void PerformLogin()
    {
        // Fetch username and password dynamically from "LoginData.json"
        JObject testData = TestDataReader.LoadJson("ValidLogin.json");
        validUsername = testData["ValidLogin"][0]["ValidUserName"]?.ToString();
        validPassword = testData["ValidLogin"][1]["ValidPassword"]?.ToString();

        IWebElement usernameField = wait.Until(ExpectedConditions.ElementIsVisible(usernameInput));
        usernameField.Clear();
        usernameField.SendKeys(validUsername);

        IWebElement passwordField = wait.Until(ExpectedConditions.ElementIsVisible(passwordInput));
        passwordField.Clear();
        passwordField.SendKeys(validPassword);

        IWebElement loginBtn = wait.Until(ExpectedConditions.ElementToBeClickable(loginButton));
        loginBtn.Click();

        IWebElement adminElement = wait.Until(ExpectedConditions.ElementIsVisible(admin));
        if (!adminElement.Displayed)
        {
            throw new Exception("Admin element is not visible after login.");
        }
    }

    /**
   * @Test15 This method attempts login with invalid credentials and retrieves the resulting error message.
   * @description Tries logging in with incorrect credentials to verify the login error message display.
   *              Highlights each input field and the login button during interaction. Captures and returns
   *              the error message displayed upon failed login attempt.
   */
    public void PerformLoginWithInvalidCredentials()
    {
        // Fetch username and password dynamically from "LoginData.json"
        JObject testData = TestDataReader.LoadJson("ValidLogin.json");
        string invalidUsername = testData["InvalidLogin"][0]["InvalidUserName"].ToString();
        string invalidPassword = testData["InvalidLogin"][1]["InvalidPassword"].ToString();

        System.Threading.Thread.Sleep(2000);

        try
        {
            if (wait.Until(ExpectedConditions.ElementIsVisible(admin)).Displayed)
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(admin)).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(logOut)).Click();
            }
        }
        catch (NoSuchElementException) { }

        IWebElement usernameField = wait.Until(ExpectedConditions.ElementIsVisible(usernameInput));
        usernameField.Clear();
        usernameField.SendKeys(invalidUsername);

        IWebElement passwordField = wait.Until(ExpectedConditions.ElementIsVisible(passwordInput));
        passwordField.Clear();
        passwordField.SendKeys(invalidPassword);

        wait.Until(ExpectedConditions.ElementToBeClickable(loginButton)).Click();

        bool errorVisible = wait.Until(ExpectedConditions.ElementIsVisible(loginErrorMessage)).Displayed;
        if (!errorVisible)
        {
            throw new Exception("Login error message is not visible.");
        }
    }
}
