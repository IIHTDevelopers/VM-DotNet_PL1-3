using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using YakshaTests;

namespace YakshaCSharpPL2.Tests
{
    [TestFixture]
    public class YakshaTests
    {
        private IWebDriver? _driver;
        private WebDriverWait wait;
        private PharmacyPage? pharmacyPage;
        private MedicalRecordsPage medicalRecordsPage;
        private SettingsPage settingsPage;
        private SubStorePage? subStorePage;
        private RadiologyPage radiologyPage;
        private DispensaryPage dispensaryPage;
        private LoginPage loginPage;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("https://healthapp.yaksha.com/");
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            // Initialize the pages
            pharmacyPage = new PharmacyPage(_driver);
            medicalRecordsPage = new MedicalRecordsPage(_driver);
            settingsPage = new SettingsPage(_driver);
            subStorePage = new SubStorePage(_driver);
            radiologyPage = new RadiologyPage(_driver);
            dispensaryPage = new DispensaryPage(_driver);
            loginPage = new LoginPage(_driver);

            PerformLogin();
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        private void PerformLogin()
        {
            LoginPage loginPage = new LoginPage(_driver);
            loginPage.PerformLogin();
            Thread.Sleep(5000);
        }

        [Test, Order(1)]
        public void TestVerificationModule()
        {
            pharmacyPage.HandlingAlertOnPharmacy();
            VerifyUserIsOnCorrectUrl("/Pharmacy/Order/GoodsReceiptList");
        }

        [Test, Order(2)]
        public void TestVerifyPrintReceipt()
        {
            pharmacyPage.VerifyPrintReceipt();
            VerifyUserIsOnCorrectUrl("/Pharmacy/Order/GoodsReceiptList");
        }

        [Test, Order(3)]
        public void TestKeywordMatching()
        {
            medicalRecordsPage.KeywordMatching();
            VerifyUserIsOnCorrectUrl("/Medical-records/OutpatientList");
        }

        [Test, Order(4)]
        public void TestPresenceOfDoctorFilter()
        {
            medicalRecordsPage.PresenceOfDoctorFilter();
            VerifyUserIsOnCorrectUrl("/Medical-records/OutpatientList");
        }

        [Test, Order(5)]
        public void TestVerifyDynamicTemplates()
        {
            settingsPage.VerifyDynamicTemplates();
            VerifyUserIsOnCorrectUrl("/Settings/DynamicTemplates/Templates");
        }

        [Test, Order(6)]
        public void TestCreateInventoryInventoryRequisition()
        {
            subStorePage.CreateInventoryRequisition();
            VerifyUserIsOnCorrectUrl("/WardSupply/Inventory/InventoryRequisitionItem");
        }

        [Test, Order(7)]
        public void TestVerifyMaternityAllowanceReport()
        {
            MaternityPage maternityPage = new MaternityPage(_driver);
            maternityPage.VerifyMaternityAllowanceReport();
            VerifyUserIsOnCorrectUrl("/Maternity/Reports/MaternityAllowance");
        }

        [Test, Order(8)]
        public void TestPerformInpatientImagingOrder()
        {
            DoctorPage doctorPage = new DoctorPage(_driver);
            doctorPage.PerformInpatientImagingOrder();
            VerifyActiveOrderIsPresent();
        }

        [Test, Order(9)]
        public void TestVerifyLabResults()
        {
            radiologyPage.VerifyDataWithinLastThreeMonths();
            VerifyUserIsOnCorrectUrl("/Radiology/ImagingRequisitionList");
        }

        [Test, Order(10)]
        public void TestVerifyExportUserCollectionReport()
        {
            dispensaryPage.VerifyExportUserCollectionReport();
            VerifyIfRecordsArePresent();
        }

        [Test, Order(11)]
        public void TestCreatingConsumptionSection()
        {
            subStorePage.CreatingConsumptionSection();
            VerifyUserIsOnCorrectUrl("/WardSupply/Inventory/Consumption/ConsumptionList");
        }

        [Test, Order(12)]
        public void TestCreatingReportSection()
        {
            subStorePage.CreatingReportSection();
            VerifyUserIsOnCorrectUrl("/WardSupply/Inventory/Reports/ConsumptionReport");
        }

        [Test, Order(13)]
        public void TestVerifyPresenceOfSupplierName()
        {
            pharmacyPage.VerifyPresenceOfSupplierName();
            VerifyUserIsOnCorrectUrl("/Pharmacy/Order/GoodsReceiptList");
        }

        [Test, Order(14)]
        public void TestFilterListRequestsByDateAndType()
        {
            RadiologyPage radiologyPage = new RadiologyPage(_driver);
            radiologyPage.FilterListRequestsByDateAndType();
            VerifyUserIsOnCorrectUrl("/Radiology/ImagingRequisitionList");
        }

        [Test, Order(15)]
        public void TestPerformLoginWithInvalidCredentials()
        {
            loginPage.PerformLoginWithInvalidCredentials();
            VerifyUserIsNotLoggedIn();
        }

        // These are the helper methods

        private void VerifyUserIsOnCorrectUrl(string expectedUrl)
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Url.Contains(expectedUrl));
            Assert.That(_driver.Url.Contains(expectedUrl), $"Expected URL to contain '{expectedUrl}', but got '{_driver.Url}'");
        }

        public void VerifyActiveOrderIsPresent()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            By activeOrderLocator = By.XPath("//span[text()=' Active Orders']");

            try
            {
                bool isVisible = wait.Until(ExpectedConditions.ElementIsVisible(activeOrderLocator)).Displayed;
                Assert.That(isVisible, Is.True);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Active Orders element was not found within the timeout period.");
            }
        }

        public void VerifyUserIsNotLoggedIn()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var invalidCredentialsElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(text(),'Invalid credentials !')]")));
            Assert.That(invalidCredentialsElement.Displayed, Is.True, "Invalid credentials message is not visible.");
        }

        public void VerifyIfRecordsArePresent()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var records = wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div[col-id='PatientName']")));
            Assert.That(records.Count(), Is.GreaterThan(1), $"Expected more than 1 record, but found: {records.Count()}");
        }
    }
}
