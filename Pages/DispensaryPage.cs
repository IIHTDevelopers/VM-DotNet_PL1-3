using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class DispensaryPage
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    private string expectedFileKeyword = "PharmacyUserwiseCollectionReport_2025";

    public DispensaryPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    private By dispensaryLink = By.XPath("//a[@href='#/Dispensary']");
    private By reportsTab = By.XPath("//a[contains(text(),'Reports')]");
    private By userCollectionReport = By.XPath("//i[text()='User Collection']");
    private By fromDate = By.XPath("(//input[@id='date'])[1]");
    private By showReportButton = By.XPath("//span[text()='Show Report']");
    private By exportButton = By.XPath("//button[@title='Export To Excel']");

    public void VerifyExportUserCollectionReport()
    {
        /**
        * @Test10
        * @description This test verifies the export functionality for the User Collection Report.
        * @expected The exported file should download with the name `PharmacyUserwiseCollectionReport_2025`.
        */

        // Navigate to Dispensary module
        wait.Until(ExpectedConditions.ElementToBeClickable(dispensaryLink)).Click();

        // Click Reports tab
        wait.Until(ExpectedConditions.ElementToBeClickable(reportsTab)).Click();

        // Select User Collection Report
        wait.Until(ExpectedConditions.ElementToBeClickable(userCollectionReport)).Click();

        // Enter From Date
        wait.Until(ExpectedConditions.ElementToBeClickable(fromDate)).SendKeys("01-01-2020");

        // Click Show Report
        wait.Until(ExpectedConditions.ElementToBeClickable(showReportButton)).Click();
        Thread.Sleep(2000); // Wait for the report to load

        // Click Export Button
        wait.Until(ExpectedConditions.ElementToBeClickable(exportButton)).Click();

        // Wait for file download
        bool fileDownloaded = WaitForFileDownload(expectedFileKeyword, 20);

        // Assert file download success
        Assert.That(fileDownloaded, Is.True, $"Expected file containing '{expectedFileKeyword}' not found in {downloadFolder}");
    }

    private bool WaitForFileDownload(string fileKeyword, int timeout)
    {
        while (timeout > 0)
        {
            var downloadedFiles = Directory.GetFiles(downloadFolder);
            if (downloadedFiles.Any(file => file.Contains(fileKeyword)))
            {
                Console.WriteLine($"File downloaded successfully: {downloadedFiles.First(file => file.Contains(fileKeyword))}");
                return true;
            }
            Thread.Sleep(1000);
            timeout--;
        }
        return false;
    }
}
