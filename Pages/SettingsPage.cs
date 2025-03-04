using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using DotNetSelenium.Utilities;
using SeleniumExtras.WaitHelpers;

public class SettingsPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private By settingsLink = By.CssSelector("a[href='#/Settings']");
    private By radiologySubmodule = By.XPath("//a[@href='#/Settings/RadiologyManage' and contains(text(),'Radiology')]");
    private By addImagingTypeButton = By.XPath("//a[text()='Add Imaging Type']");
    private By imagingItemNameField = By.CssSelector("input#ImagingTypeName");
    private By addButton = By.XPath("//input[@id='Add']");
    private By searchBar = By.CssSelector("input#quickFilterInput");
    private By dynamicTemplates = By.XPath("(//a[@href='#/Settings/DynamicTemplates'])[2]");
    private By addTemplateButton = By.XPath("//a[@id='id_btn_template_newTemplate']");
    private By templateNameField = By.XPath("//input[@placeholder='template name']");
    private By templateType = By.XPath("//select[@id='TemplateTypeId']");
    private By templateCodeLoc = By.XPath("//input[@placeholder='enter template code']");
    private By textField = By.XPath("//div[@id='cke_1_contents']");
    private By typeOption = By.XPath("//span[text()='Discharge Summary']");

    public SettingsPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// This method verifies the creation of dynamic templates in the Settings module.
    /// It navigates to the Dynamic Templates submodule, fills out the template details including
    /// template type, name, code, and text field, and ensures the template is added successfully.
    /// </summary>
    public void VerifyDynamicTemplates()
    {
        JObject settings = TestDataReader.LoadJson("settings.json");
        string textFieldValue = settings["Templates"][0]["TextField"].ToString();
        string templateName = settings["Templates"][1]["TemplateName"].ToString();
        string templateCode = settings["Templates"][2]["TemplateCode"].ToString();
        string templateTypeValue = settings["Templates"][3]["TemplateType"].ToString();

        wait.Until(ExpectedConditions.ElementToBeClickable(settingsLink)).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(dynamicTemplates)).Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(addTemplateButton)).Click();

        wait.Until(ExpectedConditions.ElementIsVisible(templateType)).Click();
        new SelectElement(driver.FindElement(templateType)).SelectByText(templateTypeValue);

        wait.Until(ExpectedConditions.ElementIsVisible(templateNameField)).SendKeys(templateName);
        wait.Until(ExpectedConditions.ElementIsVisible(templateCodeLoc)).SendKeys(templateCode);

        IWebElement iframe = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("iframe[title='Rich Text Editor, editor1']")));
        driver.SwitchTo().Frame(iframe);

        IWebElement textArea = driver.FindElement(By.CssSelector("html[dir='ltr'] body"));
        textArea.Click();
        textArea.SendKeys(textFieldValue);

        driver.SwitchTo().DefaultContent();
        driver.FindElement(By.Id("Add")).Click();
    }
}