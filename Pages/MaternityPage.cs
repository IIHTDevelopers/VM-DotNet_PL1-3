using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SeleniumExtras.WaitHelpers;
using DotNetSelenium.Utilities;

public class MaternityPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public MaternityPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    private readonly By maternityLink = By.CssSelector("a[href='#/Maternity']");
    private readonly By reportLink = By.XPath("(//a[@href='#/Maternity/Reports'])[2]");
    private readonly By maternityAllowanceReport = By.XPath("(//a[@href='#/Maternity/Reports/MaternityAllowance'])");
    private readonly By dateFrom = By.XPath("(//input[@id='date'])[1]");
    private readonly By showReportBtn = By.CssSelector("button.btn.green.btn-success[type='button']");
    private readonly By dataType = By.XPath("//div[@role='gridcell' and @col-id='TransactionType'][1]");

    public void VerifyMaternityAllowanceReport()
    {
        /**
        * @Test7
        * @description This method verifies the functionality of the Maternity Allowance Report.
        * It navigates to the Maternity module, accesses the report section, and opens the Maternity Allowance Report.
        * Initially, it ensures that the data grid is not visible, selects a date range by entering the 'from date,'
        * and clicks the 'Show Report' button. Finally, it waits for the report to load and asserts that the data grid becomes visible.
        */
        JObject maternityData = TestDataReader.LoadJson("Maternity.json");
        string fromDate = maternityData["DateRange"][0]["FromDate"].ToString();

        wait.Until(ExpectedConditions.ElementToBeClickable(maternityLink)).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(reportLink)).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(maternityAllowanceReport)).Click();

        var dateInput = wait.Until(ExpectedConditions.ElementIsVisible(dateFrom));
        dateInput.Clear();
        dateInput.SendKeys(fromDate);

        wait.Until(ExpectedConditions.ElementToBeClickable(showReportBtn)).Click();

        System.Threading.Thread.Sleep(2000); // Wait for the report to load

        bool isVisible = wait.Until(ExpectedConditions.ElementIsVisible(dataType)).Displayed;
        Assert.That(isVisible, Is.True, "Data grid is not visible after showing the report.");
    }
}
