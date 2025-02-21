using System;
using System.IO;
using DotNetSelenium.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace YakshaTests
{
    public class DoctorPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public DoctorPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        private By doctorsLink = By.CssSelector("a[href='#/Doctors']");
        private By inpatientDepartmentTab = By.XPath("(//a[@href='#/Doctors/InPatientDepartment'])[2]");
        private By searchBar = By.XPath("(//input[@placeholder='search'])[3]");
        private By orderDropdown = By.XPath("//select");
        private By imagingActionButton = By.XPath("//a[@danphe-grid-action='imaging']");
        private By searchOrderItem = By.XPath("//input[@placeholder='search order items']");
        private By proceedButton = By.XPath("//button[text()=' Proceed ']");
        private By signButton = By.XPath("//button[text()='Sign']");
        private By successMessage = By.XPath("//p[contains(text(),'success')]/../p[text()='Imaging and lab order add successfully']");

        public void PerformInpatientImagingOrder()
        {
            /**
            * @Test8
            * @description This method verifies the process of placing an imaging order for an inpatient.
            * It navigates to the Inpatient Department, searches for a specific patient, selects an imaging action,
            * chooses an order type, specifies the order item, and completes the process by signing the order.
            * The method confirms the successful placement of the order by verifying the success message.
            */
            JObject doctorData = TestDataReader.LoadJson("Doctor.json");
            string patient = doctorData["patientName"][0]["patient"].ToString();
            string option = doctorData["Dropdown"][0]["Option"].ToString();
            string searchOrderItemText = doctorData["Dropdown"][1]["searchOrderItem"].ToString();

            // Navigate to the Doctors page
            wait.Until(ExpectedConditions.ElementToBeClickable(doctorsLink)).Click();

            // Click on the Inpatient Department tab
            wait.Until(ExpectedConditions.ElementToBeClickable(inpatientDepartmentTab)).Click();

            System.Threading.Thread.Sleep(2000); // Wait for the page to load

            // Search for the patient
            var searchInput = wait.Until(ExpectedConditions.ElementIsVisible(searchBar));
            searchInput.Clear();
            searchInput.SendKeys(patient + Keys.Enter);

            // Click on the Imaging action button
            wait.Until(ExpectedConditions.ElementToBeClickable(imagingActionButton)).Click();

            System.Threading.Thread.Sleep(2000); // Wait for action to complete

            // Select the order type from the dropdown
            var dropdown = wait.Until(ExpectedConditions.ElementToBeClickable(orderDropdown));
            dropdown.Click();
            dropdown.SendKeys(option + Keys.Enter);

            System.Threading.Thread.Sleep(2000); // Wait for dropdown to update

            // Search for the order item
            var searchOrderInput = wait.Until(ExpectedConditions.ElementIsVisible(searchOrderItem));
            searchOrderInput.Clear();
            searchOrderInput.SendKeys(searchOrderItemText + Keys.Enter);

            // Click on the Proceed button
            wait.Until(ExpectedConditions.ElementToBeClickable(proceedButton)).Click();

            // Click on the Sign button
            wait.Until(ExpectedConditions.ElementToBeClickable(signButton)).Click();

            // Verify the success message is visible
            bool isSuccessVisible = wait.Until(ExpectedConditions.ElementIsVisible(successMessage)).Displayed;
            Assert.That(isSuccessVisible, Is.True, "Success message is not visible.");
        }
    }
}
