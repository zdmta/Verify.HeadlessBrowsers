﻿using OpenQA.Selenium;

namespace VerifyTests;

public static class VerifySelenium
{
    public static void Enable()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter<WebDriver>(DriverToImage);
        VerifierSettings.RegisterFileConverter<IWebElement>(ElementToImage);
    }

    static ConversionResult ElementToImage(IWebElement target, IReadOnlyDictionary<string, object> context)
    {
        var element = (WebElement)target;
        var driver = (WebDriver)element.WrappedDriver;
        driver.WaitForIsReady();
        var bytes = element.GetScreenshot().AsByteArray;
        return new(
            null,
            new List<Target>
            {
                new("html", element.GetSource()),
                new("png", new MemoryStream(bytes))
            }
        );
    }

    static ConversionResult DriverToImage(WebDriver driver, IReadOnlyDictionary<string, object> context)
    {
        driver.WaitForIsReady();
        var bytes = driver.GetScreenshot().AsByteArray;
        return new(
            null,
            new List<Target>
            {
                new("html", driver.PageSource),
                new("png", new MemoryStream(bytes))
            }
        );
    }
}