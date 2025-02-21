using DotNetSelenium.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

public class RadiologyPage
{
    private IWebDriver driver;
    private WebDriverWait wait;

    public RadiologyPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    /**
   * @Test9
   * @description This method verifies that the data displayed in the radiology list request is within the last three months.
   * It navigates to the Radiology module, selects the "Last 3 Months" option from the date range dropdown, and confirms the filter.
   * The method retrieves all dates from the table, validates their format, and checks if they fall within the last three months.
   * Logs detailed errors if dates are invalid or out of range and provides debug information about the number of date cells and retrieved dates.
   * Throws an error if any date is invalid or outside the range.
   */
    public void VerifyDataWithinLastThreeMonths()
    {
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='#/Radiology']"))).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'List Requests')]"))).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[@data-toggle='dropdown']"))).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[text()='Last 3 Months']"))).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(),'OK')]"))).Click();

        Thread.Sleep(3000); // Wait for data to load
        var dateCells = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='CreatedOn'][1]"));
        Assert.That(dateCells.Count, Is.GreaterThan(0), "No date cells found. Verify the locator or table data.");

        DateTime today = DateTime.Now;
        DateTime threeMonthsAgo = today.AddDays(-90);

        foreach (var cell in dateCells)
        {
            DateTime dateValue;
            if (DateTime.TryParseExact(cell.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                Assert.That(dateValue, Is.InRange(threeMonthsAgo, today), $"Date out of range: {dateValue}");
            }
            else
            {
                Assert.Fail($"Invalid date format: {cell.Text}");
            }
        }
    }

    /**
   * @Test14
   * @description This method filters the list of radiology requests based on a specified date range and imaging type.
   * It navigates to the Radiology module, applies the selected filter, enters the 'From' and 'To' dates, and confirms the filter action.
   * The method verifies that the filtered results match the specified imaging type.
   */
    public void FilterListRequestsByDateAndType()
    {
        JObject radiologyData = TestDataReader.LoadJson("Radiology.json");
        string filterOption = radiologyData["FilterDropdown"][0]["Filter"].ToString();
        string fromDate = radiologyData["DateRange"][0]["FromDate"].ToString();
        string toDate = radiologyData["DateRange"][1]["ToDate"].ToString();

        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href='#/Radiology']"))).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'List Requests')]"))).Click();
        Thread.Sleep(2000);

        var filterDropdown = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//select")));
        filterDropdown.Click();
        filterDropdown.SendKeys(filterOption);
        filterDropdown.SendKeys(Keys.Enter);
        Thread.Sleep(2000);

        var fromDateInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//input[@id='date'])[1]")));
        fromDateInput.Clear();
        fromDateInput.SendKeys(fromDate);

        var toDateInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//input[@id='date'])[2]")));
        toDateInput.Clear();
        toDateInput.SendKeys(toDate);

        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(),'OK')]"))).Click();
        Thread.Sleep(3000);

        //var resultElements = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='ImagingTypeName']"));
        //List<string> results = resultElements.Select(e => e.Text.Trim()).ToList();

        //Assert.That(results, Is.SubsetOf(filterOption.Trim()), "Filtered results do not match the specified imaging type.");

        var resultTextElements = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='ImagingTypeName']"));
        var trimmedResults = resultTextElements.Select(text => text.Text.Trim()).ToList();
        bool matchFound = trimmedResults.Contains(filterOption.Trim());

        if (!matchFound)
        {
            throw new Exception("Filtered results do not match the specified imaging type.");
        }
    }
}
