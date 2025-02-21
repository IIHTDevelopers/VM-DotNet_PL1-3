using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SeleniumExtras.WaitHelpers;
using DotNetSelenium.Utilities;
using NUnit.Framework;

public class PharmacyPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public PharmacyPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    private By pharmacyModule = By.CssSelector("a[href='#/Pharmacy']");
    private By orderLink = By.XPath("//a[contains(text(),'Order')]");
    private By addNewGoodReceiptButton = By.XPath("//button[contains(text(),'Add New Good Receipt')]");
    private By goodReceiptModalTitle = By.XPath("//span[contains(text(),'Add Good Receipt')]");
    private By printReceiptButton = By.Id("saveGr");
    private By addNewItemButton = By.Id("btn_AddNew");
    private By itemNameField = By.XPath("//input[@placeholder='Select an Item']");
    private By batchNoField = By.Id("txt_BatchNo");
    private By itemQtyField = By.Id("ItemQTy");
    private By rateField = By.Id("GRItemPrice");
    private By saveButton = By.Id("btn_Save");
    private By supplierNameField = By.Id("SupplierName");
    private By invoiceField = By.Id("InvoiceId");
    private By successMessage = By.XPath("//p[contains(text(),'success')]/../p[text()='Goods Receipt is Generated and Saved.']");
    private By supplierName = By.XPath("//input[@placeholder='select supplier']");
    private By showDetails = By.XPath("//button[text()=' Show Details ']");

    /**
   * @Test1
   * @description This method navigates to the Pharmacy module, verifies the Good Receipt modal,
   * handles alerts during the Good Receipt print process, and ensures the modal is visible
   * before performing further actions.
   */
    public void HandlingAlertOnPharmacy()
    {
        try
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(pharmacyModule)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(orderLink)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewGoodReceiptButton)).Click();

            IWebElement modalVisible = wait.Until(ExpectedConditions.ElementIsVisible(goodReceiptModalTitle));
            if (!modalVisible.Displayed)
                throw new Exception("Good Receipt Modal is not displayed.");

            wait.Until(ExpectedConditions.ElementToBeClickable(printReceiptButton)).Click();
            HandleAlert("Please select supplier");
            HandleAlert("Please enter Invoice no.");
        }
        catch (Exception e)
        {
            throw new Exception($"Error performing Good Receipt print: {e.Message}");
        }
    }

    private void HandleAlert(string alertText)
    {
        try
        {
            IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
            string alertMessage = alert.Text;
            Console.WriteLine("Alert message: " + alertMessage);

            if (alertMessage.Contains(alertText))
                alert.Accept();
            else
                alert.Dismiss();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to handle alert: {e.Message}");
        }
    }

    /**
   * @Test2
   * @description This method verifies the process of adding a new Good Receipt in the Pharmacy module,
   * filling in item details such as item name, batch number, quantity, rate, supplier name,
   * and a randomly generated invoice number. It concludes by validating the successful printing of the receipt.
   */
    public void VerifyPrintReceipt()
    {
        try
        {
            JObject testData = TestDataReader.LoadJson("Pharmacy.json");
            string itemName = testData["Fields"][0]["ItemName"]?.ToString();
            string batchNo = testData["Fields"][1]["batchNoField"]?.ToString();
            string itemQty = testData["Fields"][2]["itemQtyField"]?.ToString();
            string rate = testData["Fields"][3]["rateField"]?.ToString();
            string supplierName = testData["Fields"][4]["supplierNameField"]?.ToString();

            wait.Until(ExpectedConditions.ElementToBeClickable(pharmacyModule)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(orderLink)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewGoodReceiptButton)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewItemButton)).Click();

            var itemField = wait.Until(ExpectedConditions.ElementIsVisible(itemNameField));
            itemField.SendKeys(itemName);
            System.Threading.Thread.Sleep(2000); // Wait for data to load
            itemField.SendKeys(Keys.Enter);
            wait.Until(ExpectedConditions.ElementIsVisible(batchNoField)).SendKeys(batchNo);
            wait.Until(ExpectedConditions.ElementIsVisible(itemQtyField)).SendKeys(itemQty);
            wait.Until(ExpectedConditions.ElementIsVisible(rateField)).SendKeys(rate);
            wait.Until(ExpectedConditions.ElementToBeClickable(saveButton)).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(supplierNameField)).SendKeys(supplierName + "\n");
            string randomInvoiceNo = new Random().Next(100, 999).ToString();
            wait.Until(ExpectedConditions.ElementIsVisible(invoiceField)).SendKeys(randomInvoiceNo);

            wait.Until(ExpectedConditions.ElementToBeClickable(printReceiptButton)).Click();
            System.Threading.Thread.Sleep(2000);

            if (wait.Until(ExpectedConditions.ElementIsVisible(successMessage)) == null)
                throw new Exception("Receipt was not generated successfully.");
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to verify Print Receipt: {e.Message}");
        }
    }

    /**
   * @Test13
   * @description This method verifies the presence of a supplier name in the order section of the Pharmacy module.
   * It navigates through the necessary elements to input the supplier name, triggers the search, and then checks if
   * the supplier name appears in the results grid. If the supplier name is not found, it throws an error.
   */
    public void VerifyPresenceOfSupplierName()
    {
        try
        {
            JObject testData = TestDataReader.LoadJson("Pharmacy.json");
            string supplier = testData["Fields"][4]["supplierNameField"].ToString();

            wait.Until(ExpectedConditions.ElementToBeClickable(pharmacyModule)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(orderLink)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(supplierName)).Click();

            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(supplierName));
            element.Clear();
            element.SendKeys(supplier);
            wait.Until(ExpectedConditions.ElementToBeClickable(showDetails)).Click();
            System.Threading.Thread.Sleep(3000);

            var resultElements = driver.FindElements(By.XPath("//div[@role='gridcell' and @col-id='SupplierName']"));
            List<string> resultTexts = new List<string>();
            foreach (var elem in resultElements)
            {
                resultTexts.Add(elem.Text.Trim());
            }

            if (!resultTexts.Contains(supplier.Trim()))
                throw new Exception($"Supplier name '{supplier}' not found in results.");
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to verify supplier name: {e.Message}");
        }
    }
}
