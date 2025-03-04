using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using SeleniumExtras.WaitHelpers;
using DotNetSelenium.Utilities;

    public class MedicalRecordsPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        private By medicalRecordsLink = By.CssSelector("a[href='#/Medical-records']");
        private By mrOutpatientList = By.XPath("(//a[@href='#/Medical-records/OutpatientList'])[2]");
        private By okButton = By.XPath("//button[@class='btn green btn-success']");
        private By searchBar = By.Id("quickFilterInput");
        private By fromDate = By.XPath("(//input[@id='date'])[1]");
        private By doctorFilter = By.XPath("//input[@placeholder='Doctor Name']");

        public MedicalRecordsPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// This method verifies patient records in the Dispensary module by applying a date filter
        /// and searching for a specific patient by gender. It validates the search results by checking
        /// if the gender appears in the filtered records.
        /// </summary>
        public void KeywordMatching()
        {
            JObject testData = TestDataReader.LoadJson("MedicalRecord.json");
            string fromDateValue = testData["DateRange"][0]["FromDate"].ToString();
            string gender = testData["PatientGender"][0]["Gender"].ToString();

            wait.Until(ExpectedConditions.ElementToBeClickable(medicalRecordsLink)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(mrOutpatientList)).Click();
            Thread.Sleep(2000);

            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(fromDate));
            element.Clear();
            element.SendKeys(fromDateValue);
            wait.Until(ExpectedConditions.ElementToBeClickable(okButton)).Click();
            Thread.Sleep(2000);

            element = wait.Until(ExpectedConditions.ElementIsVisible(searchBar));
            element.Clear();
            element.SendKeys(gender);
            driver.FindElement(searchBar).SendKeys(Keys.Enter);
            Thread.Sleep(3000);

            IList<IWebElement> resultElements = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='Gender']"));
            foreach (IWebElement elem in resultElements)
            {
                Assert.That(gender.Trim(), Is.EqualTo(elem.Text.Trim()), $"Mismatch found! Expected: {gender}, Found: {elem.Text.Trim()}");
            }
            Console.WriteLine($"All records match the gender: {gender}");
        }

        /// <summary>
        /// This method verifies the presence of the doctor filter functionality in the medical records module.
        /// It applies the filter by selecting a specific doctor and a date range, and validates that the search results
        /// correctly display records associated with the selected doctor.
        /// </summary>
        public void PresenceOfDoctorFilter()
        {
            try
            {
                JObject testData = TestDataReader.LoadJson("MedicalRecord.json");
                string fromDateValue = testData["DateRange"][0]["FromDate"].ToString();
                string doctor = testData["DoctorName"][0]["Doctor"].ToString();

                wait.Until(ExpectedConditions.ElementToBeClickable(medicalRecordsLink)).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(mrOutpatientList)).Click();
                Thread.Sleep(2000);

                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(doctorFilter));
                element.Clear();
                element.SendKeys(doctor);
                element.SendKeys(Keys.Enter);

                element = wait.Until(ExpectedConditions.ElementIsVisible(fromDate));
                element.Clear();
                element.SendKeys(fromDateValue);

                wait.Until(ExpectedConditions.ElementToBeClickable(okButton)).Click();
                Thread.Sleep(3000);

                IList<IWebElement> resultElements = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='PerformerName']"));
                foreach (IWebElement elem in resultElements)
                {
                    Assert.That(doctor.Trim(), Is.EqualTo(elem.Text.Trim()), $"Mismatch found! Expected: {doctor}, Found: {elem.Text.Trim()}");
                }
                Console.WriteLine($"All records match the doctor: {doctor}");
            }
            catch (Exception e)
            {
                Assert.Fail($"Error verifying doctor filter: {e.Message}");
            }
        }
    }
