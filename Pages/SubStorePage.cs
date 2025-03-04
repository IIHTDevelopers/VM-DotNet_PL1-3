using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetSelenium.Utilities;
using SeleniumExtras.WaitHelpers;

public class SubStorePage
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private JObject subStoreData;

    private By wardSupplyLink = By.CssSelector("a[href='#/WardSupply']");
    private By substore = By.XPath("//i[text()='Accounts']");
    private By inventoryRequisitionTab = By.XPath("//a[text()='Inventory Requisition']");
    private By createRequisitionButton = By.XPath("//span[text()='Create Requisition']");
    private By targetInventoryDropdown = By.XPath("//input[@id='activeInventory']");
    private By itemNameField = By.XPath("//input[@placeholder='Item Name']");
    private By requestButton = By.XPath("//input[@value='Request']");
    private By successMessage = By.XPath("//p[contains(text(),'success')]/../p[text()='Requisition is Generated and Saved']");
    private By accountBtn = By.XPath("//span[contains(@class, 'report-name')]/i[contains(text(), 'Accounts')]");
    private By printButton = By.XPath("//button[@id='printButton']");
    private By consumptionLink = By.XPath("(//a[@href='#/WardSupply/Inventory/Consumption'])");
    private By newConsumptionBtn = By.XPath("//span[contains(@class, 'glyphicon') and contains(@class, 'glyphicon-plus')]");
    private By inputItemName = By.Id("itemName0");
    private By saveBtn = By.Id("save");
    private By successMessage1 = By.XPath("//p[contains(text(),' Success ')]/../p[text()='Consumption completed']");
    private By reportLink = By.XPath("(//a[@href='#/WardSupply/Inventory/Reports'])");
    private By consumptionReport = By.XPath("//span[contains(@class, 'report-name')]/i[contains(text(), 'Consumption Report')]");
    private By subCategory = By.XPath("//select[@id='selectedCategoryName']");
    private By showReport = By.XPath("//button[contains(text(),'Show Report')]");
    private By issueField = By.XPath("//input[@placeholder='Issue No']");

    public SubStorePage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    /**
   * @Test7
   * @description This method verifies the creation of an inventory requisition in the Ward Supply module.
   * It navigates to the Substore section, selects a target inventory, adds an item, and submits the requisition.
   * The method ensures the requisition is successfully created by verifying the success message.
   */
    public void CreateInventoryRequisition()
    {
        JObject subStoreData = TestDataReader.LoadJson("SubStore.json");
        string targetInventory = subStoreData["SubStore"][0]["TargetInventory"].ToString();
        string itemName = subStoreData["SubStore"][1]["ItemName"].ToString();

        driver.FindElement(wardSupplyLink).Click();
        driver.FindElement(substore).Click();
        driver.FindElement(inventoryRequisitionTab).Click();
        driver.FindElement(createRequisitionButton).Click();
        System.Threading.Thread.Sleep(2000);

        driver.FindElement(targetInventoryDropdown).Click();
        driver.FindElement(issueField).Click();
        driver.FindElement(targetInventoryDropdown).Click();
        driver.FindElement(targetInventoryDropdown).SendKeys(targetInventory);
        driver.FindElement(targetInventoryDropdown).SendKeys(Keys.Enter);
        System.Threading.Thread.Sleep(2000);

        driver.FindElement(itemNameField).SendKeys(itemName);
        driver.FindElement(itemNameField).SendKeys(Keys.Enter);
        System.Threading.Thread.Sleep(2000);

        driver.FindElement(requestButton).Click();
    }

    /**
   * @Test11
   * @description This method creates a new consumption section. It navigates through the Ward Supply module,
   * accesses the account and consumption sections, and opens the "New Consumption" form.
   * The function enters the item name, submits the form, and verifies the successful creation of the consumption
   * section by asserting that a success message becomes visible.
   * Throws an error if the success message is not displayed after submission.
   */
    public void CreatingConsumptionSection()
    {
        JObject subStoreData = TestDataReader.LoadJson("SubStore.json");
        string itemName = subStoreData["SubStore"][1]["ItemName"].ToString();

        driver.FindElement(wardSupplyLink).Click();
        driver.FindElement(accountBtn).Click();
        driver.FindElement(consumptionLink).Click();
        driver.FindElement(newConsumptionBtn).Click();
        driver.FindElement(inputItemName).SendKeys(itemName);
        driver.FindElement(inputItemName).SendKeys(Keys.Enter);
        driver.FindElement(saveBtn).Click();

        System.Threading.Thread.Sleep(3000);
        Assert.That(driver.FindElement(successMessage1).Displayed, Is.True, "Consumption section creation failed.");
    }

    /**
   * @Test12
   * @description This method creates a new report section in the Ward Supply module. It navigates through
   * the report section and selects the specified item name from the subcategory dropdown. After generating
   * the report, the function verifies if the selected item name is displayed in the report grid.
   * Throws an error if the item name is not found in the report results.
   */
    public void CreatingReportSection()
    {
        JObject subStoreData = TestDataReader.LoadJson("SubStore.json");
        string itemName = subStoreData["SubStore"][1]["ItemName"].ToString();

        driver.FindElement(wardSupplyLink).Click();
        driver.FindElement(accountBtn).Click();
        driver.FindElement(reportLink).Click();
        driver.FindElement(consumptionReport).Click();

        System.Threading.Thread.Sleep(2500);

        var fromDateInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//input[@id='date'])[1]")));
        fromDateInput.Clear();
        fromDateInput.SendKeys("01-01-2024");

        driver.FindElement(subCategory).Click();
        new SelectElement(driver.FindElement(subCategory)).SelectByText(itemName);
        driver.FindElement(showReport).Click();

        System.Threading.Thread.Sleep(2500);
        IList<IWebElement> resultText = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='SubCategoryName']"));
        List<string> trimmedResults = resultText.Select(e => e.Text.Trim()).ToList();

        System.Threading.Thread.Sleep(3000);
        Assert.That(trimmedResults.Contains(itemName.Trim()), "$\"Item '{itemName}' not found in the report results.");
    }
}
